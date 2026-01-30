using UnityEngine;
using SFS.Core;

namespace SFS.Narrative
{
    /// <summary>
    /// Beat 6: Shared Difficulty
    /// Companion that follows player with synchronized movement.
    /// "Support changes the experience."
    /// </summary>
    public class Companion : MonoBehaviour
    {
        [Header("Follow")]
        public Transform player;
        public float followDistance = 2f;
        public float followSpeed = 5f;
        public float catchUpSpeed = 8f;
        public float matchDistanceThreshold = 3f;

        [Header("Animation Sync")]
        public Animator animator;
        [Tooltip("Reference to player's animator for rhythm matching")]
        public Animator playerAnimator;

        [Header("Rhythm")]
        [Tooltip("How closely to match player's movement rhythm")]
        [Range(0f, 1f)]
        public float rhythmSync = 0.8f;

        // Animator parameters
        static readonly int Speed = Animator.StringToHash("Speed");
        static readonly int Grounded = Animator.StringToHash("Grounded");
        static readonly int YVelocity = Animator.StringToHash("YVelocity");

        Vector3 targetPosition;
        float currentSpeed;
        bool isActive;

        void Start()
        {
            if (!player)
            {
                var playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj)
                {
                    player = playerObj.transform;
                    playerAnimator = playerObj.GetComponentInChildren<Animator>();
                }
            }

            // Start inactive
            gameObject.SetActive(false);
        }

        void OnEnable()
        {
            StoryBeatEvents.OnBeatChanged += OnBeatChanged;
        }

        void OnDisable()
        {
            StoryBeatEvents.OnBeatChanged -= OnBeatChanged;
        }

        void OnBeatChanged(StoryBeat previous, StoryBeat current)
        {
            // Activate companion for Beat 6
            if (current == StoryBeat.SharedDifficulty && !isActive)
            {
                ActivateCompanion();
            }
            // Could deactivate after Beat 6 if desired
        }

        void ActivateCompanion()
        {
            isActive = true;
            gameObject.SetActive(true);

            // Position behind player
            if (player)
            {
                transform.position = player.position - player.forward * followDistance;
                transform.rotation = player.rotation;
            }

            StoryBeatEvents.CompanionJoined(transform);
        }

        public void Deactivate()
        {
            isActive = false;
            StoryBeatEvents.CompanionLeft();
            gameObject.SetActive(false);
        }

        void Update()
        {
            if (!player || !isActive) return;

            FollowPlayer();
            SyncAnimation();
        }

        void FollowPlayer()
        {
            // Target position is behind and to the side of player
            Vector3 offset = -player.forward * followDistance + player.right * 0.5f;
            targetPosition = player.position + offset;

            // Calculate distance
            float dist = Vector3.Distance(transform.position, targetPosition);

            // Use faster speed when too far
            float speed = dist > matchDistanceThreshold ? catchUpSpeed : followSpeed;

            // Move toward target
            Vector3 moveDir = (targetPosition - transform.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Face movement direction
            if (moveDir.sqrMagnitude > 0.001f)
            {
                Vector3 flatDir = new Vector3(moveDir.x, 0, moveDir.z);
                if (flatDir.sqrMagnitude > 0.001f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(flatDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
                }
            }

            // Track speed for animation
            currentSpeed = dist > 0.1f ? speed / followSpeed : 0f;
        }

        void SyncAnimation()
        {
            if (!animator) return;

            // Base animation on movement
            animator.SetFloat(Speed, Mathf.Clamp01(currentSpeed));
            animator.SetBool(Grounded, true);
            animator.SetFloat(YVelocity, 0f);

            // Sync with player's rhythm if available
            if (playerAnimator && rhythmSync > 0f)
            {
                // Match player's animation speed/phase for footstep sync
                float playerSpeed = playerAnimator.GetFloat(Speed);
                float blendedSpeed = Mathf.Lerp(currentSpeed, playerSpeed, rhythmSync);
                animator.SetFloat(Speed, Mathf.Clamp01(blendedSpeed));
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 0.5f);

            if (player)
            {
                Gizmos.color = new Color(0, 1, 1, 0.3f);
                Gizmos.DrawLine(transform.position, player.position);
            }
        }
    }
}
