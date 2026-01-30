using UnityEngine;
using UnityEngine.Playables;
using SFS.Core;

namespace SFS.Narrative
{
    /// <summary>
    /// Beat 1: Soft Arrival
    /// Handles the opening moment: welcome without pressure.
    /// "You're allowed to take up space here."
    /// </summary>
    public class ArrivalIntro : MonoBehaviour
    {
        [Header("Timeline")]
        [Tooltip("Opening camera pullback + ambient establish")]
        public PlayableDirector introTimeline;

        [Header("Player Setup")]
        [Tooltip("How long to lock controls during intro")]
        public float controlLockDuration = 2f;
        public bool skipOnInput = true;

        [Header("Audio")]
        public AudioSource ambientIntro;
        public float ambientFadeIn = 3f;

        [Header("Camera")]
        [Tooltip("Starting camera position (pulled back = uncertainty)")]
        public Transform startCameraPosition;

        bool introComplete;
        float lockTimer;
        float audioProgress;
        SFS.Player.PlayerController playerController;

        void Start()
        {
            // Find player
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player)
            {
                playerController = player.GetComponent<SFS.Player.PlayerController>();
            }

            // Set initial story beat
            if (StoryBeatManager.Instance)
            {
                StoryBeatManager.Instance.SetBeatImmediate(StoryBeat.Arrival);
            }

            // Position camera
            if (startCameraPosition && UnityEngine.Camera.main)
            {
                UnityEngine.Camera.main.transform.position = startCameraPosition.position;
                UnityEngine.Camera.main.transform.rotation = startCameraPosition.rotation;
            }

            // Lock player briefly
            if (playerController)
            {
                playerController.LockControls(true);
            }

            lockTimer = controlLockDuration;

            // Start ambient
            if (ambientIntro)
            {
                ambientIntro.volume = 0f;
                ambientIntro.Play();
            }

            // Play intro timeline
            if (introTimeline)
            {
                introTimeline.stopped += OnIntroComplete;
                introTimeline.Play();
            }
        }

        void Update()
        {
            if (introComplete) return;

            // Fade in ambient
            if (ambientIntro && audioProgress < 1f)
            {
                audioProgress += Time.deltaTime / ambientFadeIn;
                ambientIntro.volume = Mathf.Lerp(0f, 0.7f, audioProgress);
            }

            // Count down lock timer
            if (lockTimer > 0f)
            {
                lockTimer -= Time.deltaTime;

                // Allow skip
                if (skipOnInput && Input.anyKeyDown)
                {
                    lockTimer = 0f;
                }

                if (lockTimer <= 0f && !introTimeline)
                {
                    CompleteIntro();
                }
            }
        }

        void OnIntroComplete(PlayableDirector director)
        {
            CompleteIntro();
        }

        void CompleteIntro()
        {
            if (introComplete) return;
            introComplete = true;

            // Unlock player
            if (playerController)
            {
                playerController.LockControls(false);
            }
        }

        void OnDrawGizmosSelected()
        {
            if (startCameraPosition)
            {
                Gizmos.color = new Color(0.5f, 0.8f, 1f, 0.5f);
                Gizmos.DrawWireSphere(startCameraPosition.position, 0.3f);
                Gizmos.DrawLine(startCameraPosition.position, startCameraPosition.position + startCameraPosition.forward * 3f);
            }
        }
    }
}
