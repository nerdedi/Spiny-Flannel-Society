using UnityEngine;
using SFS.Core;

namespace SFS.World
{
    /// <summary>
    /// Beat 4: Choosing Rest
    /// A safe zone where motion and sound reduce.
    /// "Stepping back is an action, not avoidance."
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class RestZone : MonoBehaviour
    {
        [Header("Audio")]
        [Tooltip("Audio source to fade out when entering")]
        public AudioSource ambientToReduce;
        public float fadeTargetVolume = 0.2f;
        public float fadeDuration = 1.5f;

        [Header("Visual")]
        [Tooltip("Lights to dim")]
        public Light[] lightsToDim;
        public float dimIntensity = 0.5f;

        float originalVolume;
        float[] originalLightIntensities;
        bool isInZone;
        float fadeProgress;

        void Awake()
        {
            if (ambientToReduce)
                originalVolume = ambientToReduce.volume;

            if (lightsToDim != null && lightsToDim.Length > 0)
            {
                originalLightIntensities = new float[lightsToDim.Length];
                for (int i = 0; i < lightsToDim.Length; i++)
                {
                    if (lightsToDim[i])
                        originalLightIntensities[i] = lightsToDim[i].intensity;
                }
            }
        }

        void Update()
        {
            if (!ambientToReduce && (lightsToDim == null || lightsToDim.Length == 0)) return;

            float targetProgress = isInZone ? 1f : 0f;
            fadeProgress = Mathf.MoveTowards(fadeProgress, targetProgress, Time.deltaTime / fadeDuration);

            // Fade audio
            if (ambientToReduce)
            {
                ambientToReduce.volume = Mathf.Lerp(originalVolume, fadeTargetVolume, fadeProgress);
            }

            // Dim lights
            if (lightsToDim != null)
            {
                for (int i = 0; i < lightsToDim.Length; i++)
                {
                    if (lightsToDim[i] && originalLightIntensities != null)
                    {
                        lightsToDim[i].intensity = Mathf.Lerp(
                            originalLightIntensities[i],
                            originalLightIntensities[i] * dimIntensity,
                            fadeProgress
                        );
                    }
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            isInZone = true;
            StoryBeatEvents.RestZoneEntered();
        }

        void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            isInZone = false;
            StoryBeatEvents.RestZoneExited();
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.3f, 0.6f, 0.9f, 0.25f);
            var col = GetComponent<Collider>();
            if (col is BoxCollider box)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(box.center, box.size);
            }
        }
    }
}
