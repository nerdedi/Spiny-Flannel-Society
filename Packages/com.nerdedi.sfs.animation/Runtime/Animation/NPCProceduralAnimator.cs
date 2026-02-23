using UnityEngine;
using System;

namespace SFS.Animation
{
    /// <summary>
    /// Procedural NPC animator with synchronized group behaviors,
    /// acknowledgment reactions, and idle variance.
    /// </summary>
    public class NPCProceduralAnimator : MonoBehaviour
    {
        [Header("═══ TARGET ═══")]
        public Transform visualTarget;

        [Header("═══ IDLE VARIANTS ═══")]
        public IdleVariant currentVariant = IdleVariant.Breathe;
        [Range(0f, 1f)] public float variantBlend = 0f;

        [Header("═══ BREATHE ═══")]
        [Range(0.5f, 3f)] public float breatheSpeed = 1.5f;
        [Range(0f, 0.05f)] public float breatheAmount = 0.025f;

        [Header("═══ SHIFT ═══")]
        [Range(0.3f, 1.5f)] public float shiftSpeed = 0.6f;
        [Range(0f, 0.08f)] public float shiftAmount = 0.04f;
        [Range(0f, 10f)] public float shiftTilt = 5f;

        [Header("═══ LOOK AROUND ═══")]
        [Range(0.2f, 1f)] public float lookSpeed = 0.4f;
        [Range(0f, 45f)] public float lookAngle = 25f;
        [Range(0f, 20f)] public float lookTilt = 8f;

        [Header("═══ ACKNOWLEDGMENT ═══")]
        [Range(0.2f, 0.8f)] public float acknowledgeDuration = 0.5f;
        [Range(0f, 0.15f)] public float nodDepth = 0.08f;
        [Range(0f, 20f)] public float nodAngle = 12f;

        [Header("═══ BELONGING (Final State) ═══")]
        [Range(0.8f, 2f)] public float belongingBreathSpeed = 1.2f;
        [Range(0f, 0.04f)] public float belongingSwayAmount = 0.02f;
        public bool inBelongingState = false;

        [Header("═══ GROUP SYNC ═══")]
        [Tooltip("Phase offset for synchronized group animations")]
        [Range(0f, 1f)] public float syncPhase = 0f;
        public bool syncWithGroup = false;
        public Transform groupLeader;

        [Header("═══ SMOOTHING ═══")]
        [Range(2f, 15f)] public float smoothingSpeed = 8f;

        public enum IdleVariant { Breathe, Shift, LookAround }

        // State
        float animTime;
        bool isAcknowledging;
        float ackTimer;
        Vector3 baseScale;
        Vector3 baseLocalPos;
        Quaternion baseLocalRot;
        Vector3 currentOffset;
        Vector3 currentRotation;

        void Awake()
        {
            if (!visualTarget)
            {
                var meshRenderer = GetComponentInChildren<MeshRenderer>();
                visualTarget = meshRenderer ? meshRenderer.transform : transform;
            }
        }

        void Start()
        {
            baseScale = visualTarget.localScale;
            baseLocalPos = visualTarget.localPosition;
            baseLocalRot = visualTarget.localRotation;

            // Randomize phase for variety
            if (!syncWithGroup)
            {
                syncPhase = UnityEngine.Random.value;
            }
        }

        void Update()
        {
            float dt = Time.deltaTime;

            // Sync with group leader if applicable
            if (syncWithGroup && groupLeader)
            {
                var leaderAnim = groupLeader.GetComponent<NPCProceduralAnimator>();
                if (leaderAnim)
                {
                    animTime = leaderAnim.animTime + syncPhase;
                }
            }
            else
            {
                animTime += dt;
            }

            Vector3 targetOffset = Vector3.zero;
            Vector3 targetRotation = Vector3.zero;

            if (isAcknowledging)
            {
                CalculateAcknowledge(ref targetOffset, ref targetRotation, dt);
            }
            else if (inBelongingState)
            {
                CalculateBelonging(ref targetOffset, ref targetRotation);
            }
            else
            {
                CalculateIdle(ref targetOffset, ref targetRotation);
            }

            // Smooth
            currentOffset = Vector3.Lerp(currentOffset, targetOffset, smoothingSpeed * dt);
            currentRotation = Vector3.Lerp(currentRotation, targetRotation, smoothingSpeed * dt);

            // Apply
            visualTarget.localPosition = baseLocalPos + currentOffset;
            visualTarget.localRotation = baseLocalRot * Quaternion.Euler(currentRotation);
        }

        void CalculateIdle(ref Vector3 offset, ref Vector3 rotation)
        {
            switch (currentVariant)
            {
                case IdleVariant.Breathe:
                    CalculateBreathe(ref offset, ref rotation);
                    break;
                case IdleVariant.Shift:
                    CalculateShift(ref offset, ref rotation);
                    break;
                case IdleVariant.LookAround:
                    CalculateLookAround(ref offset, ref rotation);
                    break;
            }
        }

        void CalculateBreathe(ref Vector3 offset, ref Vector3 rotation)
        {
            float phase = animTime * breatheSpeed * Mathf.PI * 2f + syncPhase * Mathf.PI * 2f;
            offset.y = Mathf.Sin(phase) * breatheAmount;

            // Subtle scale breathing
            float breatheScale = 1f + Mathf.Sin(phase) * 0.01f;
            visualTarget.localScale = baseScale * breatheScale;
        }

        void CalculateShift(ref Vector3 offset, ref Vector3 rotation)
        {
            float phase = animTime * shiftSpeed + syncPhase * Mathf.PI * 2f;

            // Weight shift side to side
            offset.x = Mathf.Sin(phase) * shiftAmount;
            offset.y = Mathf.Abs(Mathf.Sin(phase * 0.5f)) * shiftAmount * 0.3f;

            // Tilt with shift
            rotation.z = Mathf.Sin(phase) * shiftTilt;

            // Add breathing underneath
            float breathe = Mathf.Sin(animTime * breatheSpeed * Mathf.PI * 2f);
            offset.y += breathe * breatheAmount * 0.5f;
        }

        void CalculateLookAround(ref Vector3 offset, ref Vector3 rotation)
        {
            float phase = animTime * lookSpeed + syncPhase * Mathf.PI * 2f;

            // Slow head turns
            rotation.y = Mathf.Sin(phase) * lookAngle;
            rotation.x = Mathf.Sin(phase * 0.7f) * lookTilt;

            // Slight body follow
            offset.x = Mathf.Sin(phase) * 0.02f;

            // Breathing
            float breathe = Mathf.Sin(animTime * breatheSpeed * Mathf.PI * 2f);
            offset.y = breathe * breatheAmount * 0.6f;
        }

        void CalculateAcknowledge(ref Vector3 offset, ref Vector3 rotation, float dt)
        {
            ackTimer -= dt;
            float t = 1f - (ackTimer / acknowledgeDuration);

            if (t < 0.3f)
            {
                // Quick nod down
                float nodT = t / 0.3f;
                offset.y = -nodDepth * nodT;
                rotation.x = nodAngle * nodT;
            }
            else if (t < 0.6f)
            {
                // Hold
                offset.y = -nodDepth;
                rotation.x = nodAngle;
            }
            else
            {
                // Return
                float returnT = (t - 0.6f) / 0.4f;
                offset.y = -nodDepth * (1f - returnT);
                rotation.x = nodAngle * (1f - returnT);
            }

            if (ackTimer <= 0)
            {
                isAcknowledging = false;
            }
        }

        void CalculateBelonging(ref Vector3 offset, ref Vector3 rotation)
        {
            float phase = animTime * belongingBreathSpeed * Mathf.PI * 2f;

            // Peaceful, synchronized breathing
            offset.y = Mathf.Sin(phase) * breatheAmount * 1.2f;

            // Gentle unified sway
            offset.x = Mathf.Sin(phase * 0.3f + syncPhase * Mathf.PI * 2f) * belongingSwayAmount;

            // Slight lean toward center (if group)
            if (syncWithGroup && groupLeader)
            {
                Vector3 toLeader = (groupLeader.position - transform.position).normalized;
                rotation.y = Mathf.Atan2(toLeader.x, toLeader.z) * Mathf.Rad2Deg * 0.1f;
            }
        }

        #region Public API

        /// <summary>
        /// Trigger acknowledgment animation (when player approaches/interacts)
        /// </summary>
        public void Acknowledge()
        {
            isAcknowledging = true;
            ackTimer = acknowledgeDuration;
        }

        /// <summary>
        /// Set idle variant
        /// </summary>
        public void SetIdleVariant(IdleVariant variant)
        {
            currentVariant = variant;
        }

        /// <summary>
        /// Set idle variant by index
        /// </summary>
        public void SetIdleVariant(int index)
        {
            currentVariant = (IdleVariant)Mathf.Clamp(index, 0, 2);
        }

        /// <summary>
        /// Enter the belonging/unified state (end game)
        /// </summary>
        public void EnterBelongingState()
        {
            inBelongingState = true;
        }

        /// <summary>
        /// Set sync phase for group coordination
        /// </summary>
        public void SetSyncPhase(float phase)
        {
            syncPhase = phase % 1f;
        }

        /// <summary>
        /// Join a synchronization group
        /// </summary>
        public void JoinGroup(Transform leader, float phaseOffset = 0f)
        {
            groupLeader = leader;
            syncWithGroup = true;
            syncPhase = phaseOffset;
        }

        /// <summary>
        /// Leave synchronization group
        /// </summary>
        public void LeaveGroup()
        {
            syncWithGroup = false;
            groupLeader = null;
        }

        #endregion
    }
}
