using UnityEngine;

namespace SFS.Audio
{
    /// <summary>
    /// Configuration for ambient audio layers used by StoryBeatAudioController.
    /// Defines how each layer responds to story beats.
    /// </summary>
    [CreateAssetMenu(fileName = "AmbientConfig", menuName = "SFS/Audio/Ambient Layer Config")]
    public class AmbientLayerConfig : ScriptableObject
    {
        [System.Serializable]
        public class BeatVolumes
        {
            [Range(0f, 1f)] public float arrival = 0.5f;
            [Range(0f, 1f)] public float firstContact = 0.6f;
            [Range(0f, 1f)] public float compression = 0.4f;
            [Range(0f, 1f)] public float choosingRest = 0.7f;
            [Range(0f, 1f)] public float society = 0.6f;
            [Range(0f, 1f)] public float sharedDifficulty = 0.5f;
            [Range(0f, 1f)] public float belonging = 0.8f;
        }

        [Header("Base Ambient")]
        public AudioClip baseAmbientClip;
        public BeatVolumes baseVolumes = new BeatVolumes();

        [Header("Tension Layer")]
        [Tooltip("Activated during Compression beat")]
        public AudioClip tensionClip;
        [Range(0f, 1f)] public float tensionMaxVolume = 0.4f;

        [Header("Calm Layer")]
        [Tooltip("Activated during Rest and Belonging beats")]
        public AudioClip calmClip;
        [Range(0f, 1f)] public float calmMaxVolume = 0.6f;

        [Header("Society Layer")]
        [Tooltip("Warm, communal tone for Society and Belonging")]
        public AudioClip societyClip;
        [Range(0f, 1f)] public float societyMaxVolume = 0.5f;

        [Header("Transition")]
        public float crossfadeTime = 2f;

        public float GetBaseVolumeForBeat(Core.StoryBeat beat)
        {
            return beat switch
            {
                Core.StoryBeat.Arrival => baseVolumes.arrival,
                Core.StoryBeat.FirstContact => baseVolumes.firstContact,
                Core.StoryBeat.Compression => baseVolumes.compression,
                Core.StoryBeat.ChoosingRest => baseVolumes.choosingRest,
                Core.StoryBeat.TheSociety => baseVolumes.society,
                Core.StoryBeat.SharedDifficulty => baseVolumes.sharedDifficulty,
                Core.StoryBeat.QuietBelonging => baseVolumes.belonging,
                _ => 0.5f
            };
        }
    }
}
