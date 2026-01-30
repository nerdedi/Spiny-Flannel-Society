using UnityEngine;

namespace SFS.Audio
{
    /// <summary>
    /// ScriptableObject for organizing audio clips by category.
    /// Create instances for each audio type in Assets/_SFS/ScriptableObjects/Audio/
    /// </summary>
    [CreateAssetMenu(fileName = "AudioClipSet", menuName = "SFS/Audio/Clip Set")]
    public class AudioClipReference : ScriptableObject
    {
        [Header("Identification")]
        public string setName;

        [Header("Clips")]
        [Tooltip("Multiple clips for variation")]
        public AudioClip[] clips;

        [Header("Playback Settings")]
        [Range(0f, 1f)]
        public float volume = 1f;

        [Range(0f, 0.3f)]
        public float volumeVariation = 0.1f;

        [Range(0.8f, 1.2f)]
        public float pitchMin = 0.95f;

        [Range(0.8f, 1.2f)]
        public float pitchMax = 1.05f;

        /// <summary>
        /// Get a random clip from the set.
        /// </summary>
        public AudioClip GetRandomClip()
        {
            if (clips == null || clips.Length == 0) return null;
            return clips[Random.Range(0, clips.Length)];
        }

        /// <summary>
        /// Get randomized volume.
        /// </summary>
        public float GetVolume()
        {
            return volume + Random.Range(-volumeVariation, volumeVariation);
        }

        /// <summary>
        /// Get randomized pitch.
        /// </summary>
        public float GetPitch()
        {
            return Random.Range(pitchMin, pitchMax);
        }
    }
}
