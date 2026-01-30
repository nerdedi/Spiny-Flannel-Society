using UnityEngine;
using SFS.Core;

namespace SFS.Audio
{
    /// <summary>
    /// Audio feedback for collectibles.
    /// "Affirming, not celebratory" per the design doc.
    /// </summary>
    public class CollectibleAudio : MonoBehaviour
    {
        [Header("Sounds")]
        public AudioClipReference pickupSound;

        [Header("Pitch Progression")]
        [Tooltip("Pitch increases slightly with each pickup for subtle satisfaction")]
        public bool progressivePitch = true;
        public float pitchIncrement = 0.02f;
        public float maxPitch = 1.3f;

        static float currentPitch = 1f;
        static int pickupCount = 0;

        void OnEnable()
        {
            GameEvents.OnCollectibleChanged += OnCollected;
        }

        void OnDisable()
        {
            GameEvents.OnCollectibleChanged -= OnCollected;
        }

        void OnCollected(int total)
        {
            pickupCount = total;

            if (progressivePitch)
            {
                currentPitch = Mathf.Min(1f + (pickupCount * pitchIncrement), maxPitch);
            }
        }

        /// <summary>
        /// Play the pickup sound at a position.
        /// </summary>
        public void PlayAt(Vector3 position)
        {
            if (!pickupSound) return;

            var clip = pickupSound.GetRandomClip();
            if (!clip) return;

            // Create temporary audio source
            var tempObj = new GameObject("PickupSound");
            tempObj.transform.position = position;

            var source = tempObj.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = pickupSound.GetVolume();
            source.pitch = progressivePitch ? currentPitch : pickupSound.GetPitch();
            source.spatialBlend = 0.5f; // Partial 3D
            source.Play();

            Destroy(tempObj, clip.length + 0.1f);
        }

        /// <summary>
        /// Reset pitch progression (e.g., on respawn or new level).
        /// </summary>
        public static void ResetProgression()
        {
            currentPitch = 1f;
            pickupCount = 0;
        }
    }
}
