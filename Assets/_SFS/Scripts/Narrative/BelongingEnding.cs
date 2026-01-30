using UnityEngine;
using UnityEngine.Playables;
using SFS.Core;

namespace SFS.Narrative
{
    /// <summary>
    /// Beat 7: Quiet Belonging
    /// Handles the final moment: presence, not triumph.
    /// "You are allowed to remain."
    /// </summary>
    public class BelongingEnding : MonoBehaviour
    {
        [Header("Trigger")]
        public bool triggerOnBeat = true;

        [Header("Timeline")]
        [Tooltip("Final camera pullback + hold")]
        public PlayableDirector endingTimeline;

        [Header("Group Animation")]
        public SocietyGroup societyGroup;
        [Tooltip("Trigger name for final group idle loop")]
        public string finalIdleTrigger = "BelongingIdle";

        [Header("Audio")]
        public AudioSource finalAmbience;
        public float fadeInDuration = 2f;

        [Header("Camera")]
        [Tooltip("Final camera position (overrides normal follow)")]
        public Transform finalCameraPosition;
        public float cameraMoveSpeed = 1f;

        bool triggered;
        bool cameraMoving;
        UnityEngine.Camera mainCamera;
        float audioFadeProgress;

        void OnEnable()
        {
            if (triggerOnBeat)
            {
                StoryBeatEvents.OnBeatChanged += OnBeatChanged;
            }
        }

        void OnDisable()
        {
            StoryBeatEvents.OnBeatChanged -= OnBeatChanged;
        }

        void OnBeatChanged(StoryBeat previous, StoryBeat current)
        {
            if (current == StoryBeat.QuietBelonging && !triggered)
            {
                TriggerEnding();
            }
        }

        /// <summary>
        /// Can also be called directly by a trigger volume.
        /// </summary>
        public void TriggerEnding()
        {
            if (triggered) return;
            triggered = true;

            mainCamera = UnityEngine.Camera.main;

            // Lock player in place, but keep idle animation
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player)
            {
                var controller = player.GetComponent<SFS.Player.PlayerController>();
                if (controller) controller.LockControls(true);
            }

            // Trigger group idle
            if (societyGroup)
            {
                societyGroup.TriggerSyncedMotion(finalIdleTrigger);
            }

            // Start audio fade
            if (finalAmbience)
            {
                finalAmbience.volume = 0f;
                finalAmbience.Play();
            }

            // Play timeline if assigned
            if (endingTimeline)
            {
                endingTimeline.Play();
            }
            else if (finalCameraPosition)
            {
                // Manual camera move if no timeline
                cameraMoving = true;
            }
        }

        void Update()
        {
            if (!triggered) return;

            // Fade in final ambience
            if (finalAmbience && audioFadeProgress < 1f)
            {
                audioFadeProgress += Time.deltaTime / fadeInDuration;
                finalAmbience.volume = Mathf.Lerp(0f, 1f, audioFadeProgress);
            }

            // Move camera if no timeline
            if (cameraMoving && mainCamera && finalCameraPosition)
            {
                mainCamera.transform.position = Vector3.Lerp(
                    mainCamera.transform.position,
                    finalCameraPosition.position,
                    cameraMoveSpeed * Time.deltaTime
                );

                mainCamera.transform.rotation = Quaternion.Slerp(
                    mainCamera.transform.rotation,
                    finalCameraPosition.rotation,
                    cameraMoveSpeed * Time.deltaTime
                );

                // Stop when close enough
                if (Vector3.Distance(mainCamera.transform.position, finalCameraPosition.position) < 0.1f)
                {
                    cameraMoving = false;
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            if (finalCameraPosition)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(finalCameraPosition.position, 0.3f);
                Gizmos.DrawLine(finalCameraPosition.position, finalCameraPosition.position + finalCameraPosition.forward * 2f);
            }
        }
    }
}
