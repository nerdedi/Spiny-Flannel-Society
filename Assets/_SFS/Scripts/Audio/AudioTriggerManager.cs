using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SFS.Audio
{
    /// <summary>
    /// Central audio coordinator. Manages ambient layers, SFX, and reacts
    /// to DefaultsRegistry / GameState events.
    ///
    /// Ambient layers (wind, nature, cello) cross-fade based on drift intensity
    /// and sensory state. When audio_layering is rewritten, concurrent streams
    /// duck to reduce overload.
    ///
    /// Place on a persistent GameObject (same level as DefaultsRegistry).
    /// </summary>
    public class AudioTriggerManager : MonoBehaviour
    {
        public static AudioTriggerManager Instance { get; private set; }

        // ── Inspector config ────────────────────────────────────
        [Header("Mixer")]
        public AudioMixerGroup AmbientGroup;
        public AudioMixerGroup SFXGroup;
        public AudioMixerGroup MusicGroup;

        [Header("Ambient Layers")]
        public AudioClip WindLoop;
        public AudioClip NatureLoop;
        public AudioClip CelloLoop;

        [Header("SFX")]
        public AudioClip ReadRevealSFX;
        public AudioClip RewriteCommitSFX;
        public AudioClip WindprintActivateSFX;
        public AudioClip ChapterAdvanceSFX;
        public AudioClip CollectibleSFX;

        [Header("Settings")]
        [Range(0f, 1f)] public float MasterVolume = 1f;
        [Range(0f, 1f)] public float AmbientVolume = 0.6f;
        [Range(0f, 1f)] public float SFXVolume = 0.8f;
        public float CrossFadeDuration = 2f;
        public int MaxConcurrentStreams = 4;

        // ── State ───────────────────────────────────────────────
        readonly List<AmbientLayer> _layers = new();
        readonly Queue<AudioSource> _sfxPool = new();
        bool _audioLayeringRewritten;
        int _activeSFXCount;

        // ── Events ──────────────────────────────────────────────
        public static event Action<string> OnSFXPlayed;
        public static event Action<float> OnAmbientChanged;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(this); return; }
            Instance = this;

            InitSFXPool(8);
            InitAmbientLayers();
        }

        void Start()
        {
            // Subscribe to game events
            Core.DefaultsRegistry.OnDefaultRewritten += HandleDefaultRewritten;
            Core.SFSGameState.OnDriftChanged += HandleDriftChanged;
            Core.SFSGameState.OnChapterAdvanced += HandleChapterAdvanced;
            Core.GameEvents.OnCollectibleChanged += HandleCollectible;
        }

        void OnDestroy()
        {
            Core.DefaultsRegistry.OnDefaultRewritten -= HandleDefaultRewritten;
            Core.SFSGameState.OnDriftChanged -= HandleDriftChanged;
            Core.SFSGameState.OnChapterAdvanced -= HandleChapterAdvanced;
            Core.GameEvents.OnCollectibleChanged -= HandleCollectible;
        }

        // ═════════════════════════════════════════════════════════
        //  PUBLIC API
        // ═════════════════════════════════════════════════════════

        /// <summary>Play a one-shot SFX with optional spatial position.</summary>
        public void PlaySFX(AudioClip clip, Vector3? position = null, float pitch = 1f)
        {
            if (clip == null) return;

            // Respect concurrent stream limit when ducking
            int limit = _audioLayeringRewritten ? MaxConcurrentStreams / 2 : MaxConcurrentStreams;
            if (_activeSFXCount >= limit) return;

            AudioSource src = GetPooledSource();
            if (src == null) return;

            src.clip = clip;
            src.pitch = pitch;
            src.volume = SFXVolume * MasterVolume;
            src.spatialBlend = position.HasValue ? 1f : 0f;
            if (position.HasValue) src.transform.position = position.Value;
            src.outputAudioMixerGroup = SFXGroup;
            src.Play();

            _activeSFXCount++;
            StartCoroutine(ReturnToPoolWhenDone(src, clip.length / pitch));

            OnSFXPlayed?.Invoke(clip.name);
        }

        /// <summary>Play the Read reveal sound.</summary>
        public void PlayReadReveal() => PlaySFX(ReadRevealSFX, pitch: 1.1f);

        /// <summary>Play the Rewrite commit sound.</summary>
        public void PlayRewriteCommit() => PlaySFX(RewriteCommitSFX, pitch: 0.95f);

        /// <summary>Play the Windprint activation sound.</summary>
        public void PlayWindprintActivate(Vector3 pos) => PlaySFX(WindprintActivateSFX, pos);

        /// <summary>Set target ambient mix based on sensory state.</summary>
        public void SetSensoryState(SensoryState state)
        {
            switch (state)
            {
                case SensoryState.Calm:
                    SetAmbientTargets(wind: 0.3f, nature: 0.7f, cello: 0.5f);
                    break;
                case SensoryState.Active:
                    SetAmbientTargets(wind: 0.5f, nature: 0.4f, cello: 0.6f);
                    break;
                case SensoryState.Overload:
                    SetAmbientTargets(wind: 0.8f, nature: 0.1f, cello: 0.2f);
                    break;
            }
        }

        /// <summary>Update ambient levels directly from drift (0 = calm, 1 = heavy drift).</summary>
        public void UpdateFromDrift(float drift)
        {
            // Higher drift → more wind, less nature/cello
            float wind   = Mathf.Lerp(0.2f, 0.9f, drift);
            float nature = Mathf.Lerp(0.8f, 0.1f, drift);
            float cello  = Mathf.Lerp(0.6f, 0.15f, drift);
            SetAmbientTargets(wind, nature, cello);
        }

        // ═════════════════════════════════════════════════════════
        //  AMBIENT LAYER SYSTEM
        // ═════════════════════════════════════════════════════════

        void InitAmbientLayers()
        {
            _layers.Add(CreateLayer("Wind", WindLoop));
            _layers.Add(CreateLayer("Nature", NatureLoop));
            _layers.Add(CreateLayer("Cello", CelloLoop));
        }

        AmbientLayer CreateLayer(string name, AudioClip clip)
        {
            GameObject go = new($"Ambient_{name}");
            go.transform.SetParent(transform);
            AudioSource src = go.AddComponent<AudioSource>();
            src.clip = clip;
            src.loop = true;
            src.playOnAwake = false;
            src.volume = 0f;
            src.outputAudioMixerGroup = AmbientGroup;
            if (clip != null) src.Play();

            return new AmbientLayer
            {
                Name = name,
                Source = src,
                TargetVolume = 0f,
                CurrentVolume = 0f
            };
        }

        void SetAmbientTargets(float wind, float nature, float cello)
        {
            float scale = AmbientVolume * MasterVolume;
            if (_layers.Count >= 3)
            {
                _layers[0].TargetVolume = wind * scale;
                _layers[1].TargetVolume = nature * scale;
                _layers[2].TargetVolume = cello * scale;
            }
            OnAmbientChanged?.Invoke(Core.SFSGameState.Instance != null
                ? Core.SFSGameState.Instance.DriftIntensity : 1f);
        }

        void Update()
        {
            float dt = Time.deltaTime;
            foreach (var layer in _layers)
            {
                layer.CurrentVolume = Mathf.MoveTowards(
                    layer.CurrentVolume, layer.TargetVolume,
                    dt / Mathf.Max(CrossFadeDuration, 0.01f));
                if (layer.Source != null)
                    layer.Source.volume = layer.CurrentVolume;
            }
        }

        // ═════════════════════════════════════════════════════════
        //  SFX POOL
        // ═════════════════════════════════════════════════════════

        void InitSFXPool(int size)
        {
            for (int i = 0; i < size; i++)
            {
                GameObject go = new($"SFXSource_{i}");
                go.transform.SetParent(transform);
                AudioSource src = go.AddComponent<AudioSource>();
                src.playOnAwake = false;
                _sfxPool.Enqueue(src);
            }
        }

        AudioSource GetPooledSource()
        {
            return _sfxPool.Count > 0 ? _sfxPool.Dequeue() : null;
        }

        IEnumerator ReturnToPoolWhenDone(AudioSource src, float delay)
        {
            yield return new WaitForSeconds(delay + 0.05f);
            src.Stop();
            _activeSFXCount = Mathf.Max(0, _activeSFXCount - 1);
            _sfxPool.Enqueue(src);
        }

        // ═════════════════════════════════════════════════════════
        //  EVENT HANDLERS
        // ═════════════════════════════════════════════════════════

        void HandleDefaultRewritten(string key)
        {
            PlayRewriteCommit();

            if (key == "audio_layering")
            {
                _audioLayeringRewritten = true;
                MaxConcurrentStreams = 2; // Duck to reduce overload
                Debug.Log("[SFS Audio] Audio layering rewritten — concurrent streams reduced.");
            }

            // Each rewrite calms the soundscape slightly
            if (Core.SFSGameState.Instance != null)
                UpdateFromDrift(Core.SFSGameState.Instance.DriftIntensity);
        }

        void HandleDriftChanged(float drift)
        {
            UpdateFromDrift(drift);
        }

        void HandleChapterAdvanced(int chapter)
        {
            PlaySFX(ChapterAdvanceSFX, pitch: 1f + chapter * 0.02f);
        }

        void HandleCollectible(int current, int total)
        {
            PlaySFX(CollectibleSFX, pitch: 1f + (float)current / total * 0.3f);
        }

        // ── Inner types ─────────────────────────────────────────

        class AmbientLayer
        {
            public string Name;
            public AudioSource Source;
            public float TargetVolume;
            public float CurrentVolume;
        }
    }

    public enum SensoryState
    {
        Calm,
        Active,
        Overload
    }
}
