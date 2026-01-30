using UnityEngine;
using SFS.Core;

namespace SFS.World
{
    /// <summary>
    /// Beat 2: First Contact
    /// Special feedback for the first collectible that establishes safety.
    /// "The world responds when you reach out."
    /// </summary>
    public class FirstCollectibleFeedback : MonoBehaviour
    {
        [Header("Visual")]
        [Tooltip("Soft glow effect")]
        public Light glowLight;
        public float glowIntensity = 2f;
        public float glowPulseSpeed = 1.5f;

        [Tooltip("Subtle particle effect")]
        public ParticleSystem collectParticles;

        [Header("Audio")]
        [Tooltip("Affirming, not celebratory")]
        public AudioClip collectSound;
        [Range(0f, 1f)]
        public float soundVolume = 0.6f;

        [Header("UI")]
        [Tooltip("Optional first-time prompt")]
        public GameObject firstCollectPrompt;
        public float promptDuration = 2f;

        [Header("Story")]
        public bool transitionToFirstContact = true;

        float baseIntensity;
        bool collected;

        void Start()
        {
            if (glowLight)
            {
                baseIntensity = glowLight.intensity;
            }
        }

        void Update()
        {
            if (collected) return;

            // Gentle pulse to draw attention without urgency
            if (glowLight)
            {
                float pulse = Mathf.Sin(Time.time * glowPulseSpeed) * 0.5f + 0.5f;
                glowLight.intensity = baseIntensity + (glowIntensity - baseIntensity) * pulse;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (collected) return;
            if (!other.CompareTag("Player")) return;

            collected = true;

            // Add to tracker
            var tracker = other.GetComponentInParent<CollectibleTracker>();
            if (tracker) tracker.Add(1);

            // Play affirming sound
            if (collectSound)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position, soundVolume);
            }

            // Play particles
            if (collectParticles)
            {
                collectParticles.transform.SetParent(null);
                collectParticles.Play();
                Destroy(collectParticles.gameObject, collectParticles.main.duration + 1f);
            }

            // Show prompt
            if (firstCollectPrompt)
            {
                firstCollectPrompt.SetActive(true);
                Destroy(firstCollectPrompt, promptDuration);
            }

            // Transition story beat
            if (transitionToFirstContact && StoryBeatManager.Instance)
            {
                StoryBeatManager.Instance.TransitionTo(StoryBeat.FirstContact);
            }

            // Disable glow
            if (glowLight)
            {
                glowLight.enabled = false;
            }

            // Destroy collectible visual
            Destroy(gameObject, 0.1f);
        }
    }
}
