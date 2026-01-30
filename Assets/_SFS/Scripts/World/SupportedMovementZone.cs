using UnityEngine;
using SFS.Core;

namespace SFS.World
{
    /// <summary>
    /// Beat 6: Shared Difficulty
    /// Makes previously difficult sections easier when companion is present.
    /// "The world doesn't get easier — you don't face it alone."
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class SupportedMovementZone : MonoBehaviour
    {
        [Header("Timing Forgiveness")]
        [Tooltip("Extra coyote time when supported")]
        public float bonusCoyoteTime = 0.08f;
        [Tooltip("Extra jump buffer when supported")]
        public float bonusJumpBuffer = 0.08f;

        [Header("Visual Guidance")]
        [Tooltip("Subtle path indicators to enable")]
        public GameObject[] pathIndicators;

        [Tooltip("Opacity for path hints (accessibility)")]
        [Range(0f, 1f)]
        public float indicatorOpacity = 0.4f;

        [Header("Audio")]
        public AudioSource supportiveAmbience;

        bool playerInZone;
        bool hasCompanion;

        void Start()
        {
            // Hide indicators initially
            SetIndicatorsActive(false);
        }

        void OnEnable()
        {
            StoryBeatEvents.OnCompanionJoined += OnCompanionJoined;
            StoryBeatEvents.OnCompanionLeft += OnCompanionLeft;
        }

        void OnDisable()
        {
            StoryBeatEvents.OnCompanionJoined -= OnCompanionJoined;
            StoryBeatEvents.OnCompanionLeft -= OnCompanionLeft;
        }

        void OnCompanionJoined(Transform companion)
        {
            hasCompanion = true;
            if (playerInZone) ApplySupport();
        }

        void OnCompanionLeft()
        {
            hasCompanion = false;
            RemoveSupport();
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            playerInZone = true;

            if (hasCompanion)
            {
                ApplySupport();
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            playerInZone = false;
            RemoveSupport();
        }

        void ApplySupport()
        {
            // Show visual guidance
            SetIndicatorsActive(true);

            // Apply timing bonuses via settings
            if (SettingsManager.Instance)
            {
                var data = SettingsManager.Instance.Data;
                // Store originals and add bonus
                // Note: In a full implementation, you'd want to track the original values
                data.coyoteTime += bonusCoyoteTime;
                data.jumpBuffer += bonusJumpBuffer;
            }

            // Play supportive audio
            if (supportiveAmbience && !supportiveAmbience.isPlaying)
            {
                supportiveAmbience.Play();
            }
        }

        void RemoveSupport()
        {
            SetIndicatorsActive(false);

            // Remove timing bonuses
            if (SettingsManager.Instance)
            {
                var data = SettingsManager.Instance.Data;
                data.coyoteTime = Mathf.Max(0.04f, data.coyoteTime - bonusCoyoteTime);
                data.jumpBuffer = Mathf.Max(0.04f, data.jumpBuffer - bonusJumpBuffer);
            }

            if (supportiveAmbience)
            {
                supportiveAmbience.Stop();
            }
        }

        void SetIndicatorsActive(bool active)
        {
            if (pathIndicators == null) return;

            foreach (var indicator in pathIndicators)
            {
                if (indicator)
                {
                    indicator.SetActive(active);

                    // Set opacity if has renderer
                    var renderer = indicator.GetComponent<Renderer>();
                    if (renderer && renderer.material.HasProperty("_Color"))
                    {
                        var color = renderer.material.color;
                        color.a = active ? indicatorOpacity : 0f;
                        renderer.material.color = color;
                    }
                }
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.2f, 0.8f, 0.6f, 0.25f);
            var col = GetComponent<Collider>();
            if (col is BoxCollider box)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(box.center, box.size);
            }
        }
    }
}
