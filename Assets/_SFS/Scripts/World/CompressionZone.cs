using UnityEngine;
using SFS.Core;

namespace SFS.World
{
    /// <summary>
    /// Beat 3: Compression
    /// An area of increased sensory/cognitive load.
    /// "This isn't failure — this is strain."
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class CompressionZone : MonoBehaviour
    {
        [Header("Audio")]
        [Tooltip("Additional ambient layers to activate")]
        public AudioSource[] additionalAmbience;
        public float fadeInDuration = 1f;

        [Header("Visual")]
        [Tooltip("Particle systems to activate (environmental motion)")]
        public ParticleSystem[] environmentalParticles;

        [Tooltip("Lights to intensify")]
        public Light[] lightsToIntensify;
        public float intensityMultiplier = 1.5f;

        [Header("Post Processing (Optional)")]
        [Tooltip("Name of post-processing volume to enable")]
        public GameObject postProcessVolume;

        float[] originalVolumes;
        float[] originalIntensities;
        bool isInZone;
        float progress;

        void Awake()
        {
            // Store original values
            if (additionalAmbience != null && additionalAmbience.Length > 0)
            {
                originalVolumes = new float[additionalAmbience.Length];
                for (int i = 0; i < additionalAmbience.Length; i++)
                {
                    if (additionalAmbience[i])
                    {
                        originalVolumes[i] = additionalAmbience[i].volume;
                        additionalAmbience[i].volume = 0f;
                    }
                }
            }

            if (lightsToIntensify != null && lightsToIntensify.Length > 0)
            {
                originalIntensities = new float[lightsToIntensify.Length];
                for (int i = 0; i < lightsToIntensify.Length; i++)
                {
                    if (lightsToIntensify[i])
                        originalIntensities[i] = lightsToIntensify[i].intensity;
                }
            }

            // Start particles stopped
            if (environmentalParticles != null)
            {
                foreach (var ps in environmentalParticles)
                {
                    if (ps) ps.Stop();
                }
            }

            if (postProcessVolume)
                postProcessVolume.SetActive(false);
        }

        void Update()
        {
            float targetProgress = isInZone ? 1f : 0f;
            progress = Mathf.MoveTowards(progress, targetProgress, Time.deltaTime / fadeInDuration);

            // Fade audio
            if (additionalAmbience != null && originalVolumes != null)
            {
                for (int i = 0; i < additionalAmbience.Length; i++)
                {
                    if (additionalAmbience[i] && i < originalVolumes.Length)
                    {
                        additionalAmbience[i].volume = originalVolumes[i] * progress;
                    }
                }
            }

            // Intensify lights
            if (lightsToIntensify != null && originalIntensities != null)
            {
                for (int i = 0; i < lightsToIntensify.Length; i++)
                {
                    if (lightsToIntensify[i] && i < originalIntensities.Length)
                    {
                        float target = originalIntensities[i] * Mathf.Lerp(1f, intensityMultiplier, progress);
                        lightsToIntensify[i].intensity = target;
                    }
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            isInZone = true;

            // Start particles
            if (environmentalParticles != null)
            {
                foreach (var ps in environmentalParticles)
                {
                    if (ps) ps.Play();
                }
            }

            // Enable post-processing
            if (postProcessVolume)
                postProcessVolume.SetActive(true);

            // Start audio
            if (additionalAmbience != null)
            {
                foreach (var source in additionalAmbience)
                {
                    if (source && !source.isPlaying) source.Play();
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            isInZone = false;

            // Stop particles
            if (environmentalParticles != null)
            {
                foreach (var ps in environmentalParticles)
                {
                    if (ps) ps.Stop();
                }
            }

            // Disable post-processing
            if (postProcessVolume)
                postProcessVolume.SetActive(false);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.7f, 0.3f, 0.3f, 0.25f);
            var col = GetComponent<Collider>();
            if (col is BoxCollider box)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(box.center, box.size);
            }
        }
    }
}
