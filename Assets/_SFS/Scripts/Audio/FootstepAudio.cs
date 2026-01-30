using UnityEngine;
using SFS.Core;

namespace SFS.Audio
{
    /// <summary>
    /// Handles footstep sounds with rhythm awareness for companion sync.
    /// Called from animation events or controller.
    /// </summary>
    public class FootstepAudio : MonoBehaviour
    {
        [Header("Audio")]
        public AudioClipReference footstepSounds;
        public AudioSource audioSource;

        [Header("Settings")]
        [Range(0f, 1f)]
        public float baseVolume = 0.5f;

        [Tooltip("Reduce volume in rest zones")]
        [Range(0f, 1f)]
        public float restZoneVolumeMultiplier = 0.3f;

        [Header("Surface Detection (Optional)")]
        public LayerMask groundLayers;
        public float rayDistance = 0.5f;

        bool inRestZone;
        bool lowSensory;

        void OnEnable()
        {
            StoryBeatEvents.OnRestZoneEntered += OnRestEnter;
            StoryBeatEvents.OnRestZoneExited += OnRestExit;
            GameEvents.OnSettingsChanged += OnSettingsChanged;
        }

        void OnDisable()
        {
            StoryBeatEvents.OnRestZoneEntered -= OnRestEnter;
            StoryBeatEvents.OnRestZoneExited -= OnRestExit;
            GameEvents.OnSettingsChanged -= OnSettingsChanged;
        }

        void Start()
        {
            if (!audioSource)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.spatialBlend = 1f; // 3D
                audioSource.playOnAwake = false;
            }

            OnSettingsChanged();
        }

        void OnRestEnter() => inRestZone = true;
        void OnRestExit() => inRestZone = false;

        void OnSettingsChanged()
        {
            if (SettingsManager.Instance)
            {
                lowSensory = SettingsManager.Instance.Data.lowSensory;
            }
        }

        /// <summary>
        /// Call from animation event on footstep frames.
        /// </summary>
        public void PlayFootstep()
        {
            if (lowSensory) return;
            if (!footstepSounds || !audioSource) return;

            var clip = footstepSounds.GetRandomClip();
            if (!clip) return;

            float volume = footstepSounds.GetVolume() * baseVolume;
            if (inRestZone) volume *= restZoneVolumeMultiplier;

            audioSource.pitch = footstepSounds.GetPitch();
            audioSource.PlayOneShot(clip, volume);
        }

        /// <summary>
        /// Alternative: Call with explicit timing for rhythm sync.
        /// </summary>
        public void PlayFootstepAtTime(float delay)
        {
            Invoke(nameof(PlayFootstep), delay);
        }
    }
}
