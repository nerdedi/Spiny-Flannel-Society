using UnityEngine;
using SFS.Core;

namespace SFS.Audio
{
    /// <summary>
    /// Controls ambient audio based on story beats.
    /// Handles the emotional audio language: presence, tension, relief.
    /// </summary>
    public class StoryBeatAudioController : MonoBehaviour
    {
        [Header("Ambient Layers")]
        public AudioSource baseAmbient;
        public AudioSource tensionLayer;
        public AudioSource calmLayer;
        public AudioSource societyLayer;

        [Header("Volume Targets")]
        [Range(0f, 1f)] public float baseVolume = 0.5f;
        [Range(0f, 1f)] public float tensionVolume = 0.4f;
        [Range(0f, 1f)] public float calmVolume = 0.6f;
        [Range(0f, 1f)] public float societyVolume = 0.5f;

        [Header("Transitions")]
        public float fadeSpeed = 1.5f;

        // Target volumes per beat
        float targetBase, targetTension, targetCalm, targetSociety;

        void Start()
        {
            // Initialize all at zero except base
            if (baseAmbient) baseAmbient.volume = baseVolume;
            if (tensionLayer) tensionLayer.volume = 0f;
            if (calmLayer) calmLayer.volume = 0f;
            if (societyLayer) societyLayer.volume = 0f;

            // Start playing all (we control via volume)
            if (baseAmbient && !baseAmbient.isPlaying) baseAmbient.Play();
            if (tensionLayer && !tensionLayer.isPlaying) tensionLayer.Play();
            if (calmLayer && !calmLayer.isPlaying) calmLayer.Play();
            if (societyLayer && !societyLayer.isPlaying) societyLayer.Play();

            // Set initial targets
            SetTargetsForBeat(StoryBeat.Arrival);
        }

        void OnEnable()
        {
            StoryBeatEvents.OnBeatChanged += OnBeatChanged;
        }

        void OnDisable()
        {
            StoryBeatEvents.OnBeatChanged -= OnBeatChanged;
        }

        void OnBeatChanged(StoryBeat previous, StoryBeat current)
        {
            SetTargetsForBeat(current);
        }

        void SetTargetsForBeat(StoryBeat beat)
        {
            // Reset all
            targetBase = baseVolume;
            targetTension = 0f;
            targetCalm = 0f;
            targetSociety = 0f;

            switch (beat)
            {
                case StoryBeat.Arrival:
                    // Ambient present but not comforting yet
                    targetBase = baseVolume * 0.7f;
                    break;

                case StoryBeat.FirstContact:
                    // Slightly warmer
                    targetBase = baseVolume;
                    targetCalm = calmVolume * 0.3f;
                    break;

                case StoryBeat.Compression:
                    // Sound layers stack
                    targetBase = baseVolume;
                    targetTension = tensionVolume;
                    break;

                case StoryBeat.ChoosingRest:
                    // Music softens or drops out
                    targetBase = baseVolume * 0.4f;
                    targetCalm = calmVolume;
                    break;

                case StoryBeat.TheSociety:
                    // Warm, communal
                    targetBase = baseVolume * 0.6f;
                    targetCalm = calmVolume * 0.5f;
                    targetSociety = societyVolume;
                    break;

                case StoryBeat.SharedDifficulty:
                    // Active but supported
                    targetBase = baseVolume * 0.8f;
                    targetCalm = calmVolume * 0.4f;
                    targetSociety = societyVolume * 0.3f;
                    break;

                case StoryBeat.QuietBelonging:
                    // World feels calmer than at the start
                    targetBase = baseVolume * 0.5f;
                    targetCalm = calmVolume;
                    targetSociety = societyVolume * 0.7f;
                    break;
            }
        }

        void Update()
        {
            float delta = fadeSpeed * Time.deltaTime;

            if (baseAmbient)
                baseAmbient.volume = Mathf.MoveTowards(baseAmbient.volume, targetBase, delta);
            if (tensionLayer)
                tensionLayer.volume = Mathf.MoveTowards(tensionLayer.volume, targetTension, delta);
            if (calmLayer)
                calmLayer.volume = Mathf.MoveTowards(calmLayer.volume, targetCalm, delta);
            if (societyLayer)
                societyLayer.volume = Mathf.MoveTowards(societyLayer.volume, targetSociety, delta);
        }
    }
}
