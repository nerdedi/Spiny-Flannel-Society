using UnityEngine;
using SFS.Core;

namespace SFS.Narrative
{
    /// <summary>
    /// Beat 5: The Society
    /// Individual NPC that idles with the group.
    /// "You don't have to explain yourself here."
    /// </summary>
    public class SocietyMember : MonoBehaviour
    {
        [Header("Animator")]
        public Animator animator;

        [Header("Synchronization")]
        [Tooltip("Reference to group controller for synchronized motion")]
        public SocietyGroup group;

        [Tooltip("Offset in sync cycle (0-1) for variety")]
        [Range(0f, 1f)]
        public float syncOffset = 0f;

        [Header("Awareness")]
        [Tooltip("Look at player when nearby")]
        public bool acknowledgePlayer = true;
        public float acknowledgeDistance = 5f;
        public float lookSpeed = 2f;

        // Animator parameters
        static readonly int IdleVariant = Animator.StringToHash("IdleVariant");
        static readonly int SyncPhase = Animator.StringToHash("SyncPhase");
        static readonly int Acknowledged = Animator.StringToHash("Acknowledged");

        Transform playerTransform;
        bool hasAcknowledged;

        void Start()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player) playerTransform = player.transform;

            // Set initial idle variant for variety
            if (animator)
            {
                animator.SetInteger(IdleVariant, Random.Range(0, 3));
            }
        }

        void Update()
        {
            if (!animator) return;

            // Update sync phase from group
            if (group)
            {
                float phase = (group.SyncPhase + syncOffset) % 1f;
                animator.SetFloat(SyncPhase, phase);
            }

            // Acknowledge player when close
            if (acknowledgePlayer && playerTransform)
            {
                float dist = Vector3.Distance(transform.position, playerTransform.position);

                if (dist < acknowledgeDistance && !hasAcknowledged)
                {
                    hasAcknowledged = true;
                    animator.SetBool(Acknowledged, true);
                }

                // Subtle look-at when acknowledged
                if (hasAcknowledged && dist < acknowledgeDistance)
                {
                    Vector3 dirToPlayer = (playerTransform.position - transform.position).normalized;
                    dirToPlayer.y = 0;

                    if (dirToPlayer.sqrMagnitude > 0.001f)
                    {
                        Quaternion targetRot = Quaternion.LookRotation(dirToPlayer);
                        transform.rotation = Quaternion.Slerp(
                            transform.rotation,
                            targetRot,
                            lookSpeed * Time.deltaTime
                        );
                    }
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            if (acknowledgePlayer)
            {
                Gizmos.color = new Color(0.9f, 0.7f, 0.2f, 0.3f);
                Gizmos.DrawWireSphere(transform.position, acknowledgeDistance);
            }
        }
    }
}
