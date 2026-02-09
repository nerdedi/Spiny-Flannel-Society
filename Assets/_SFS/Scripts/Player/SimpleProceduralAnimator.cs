using UnityEngine;

namespace SFS.Player
{
    /// <summary>
    /// Code-based animation that works immediately without Unity animation clips.
    /// Attach to your player character for instant animations.
    /// This is a fallback/supplement when you don't have animation clips ready.
    /// </summary>
    public class SimpleProceduralAnimator : MonoBehaviour
    {
        [Header("Visual Target (the mesh/model to animate)")]
        [Tooltip("Drag your player model here. If empty, uses this transform.")]
        public Transform visualTarget;

        [Header("Idle Animation")]
        public float idleBobSpeed = 2f;
        public float idleBobAmount = 0.05f;
        public float idleBreathScale = 0.02f;

        [Header("Walk/Run Animation")]
        public float walkBobSpeed = 8f;
        public float walkBobAmount = 0.08f;
        public float walkTiltAmount = 3f;
        public float runBobSpeed = 12f;
        public float runBobAmount = 0.12f;

        [Header("Jump Animation")]
        public float jumpSquash = 0.8f;
        public float jumpStretch = 1.2f;
        public float landSquash = 0.7f;
        public float squashRecoverySpeed = 8f;

        // State
        float currentSpeed;
        bool isGrounded = true;
        bool wasGrounded = true;
        float yVelocity;
        float animTime;
        float squashAmount = 1f;
        float stretchAmount = 1f;
        Vector3 baseScale;
        Vector3 baseLocalPos;

        void Start()
        {
            if (!visualTarget) visualTarget = transform;
            baseScale = visualTarget.localScale;
            baseLocalPos = visualTarget.localPosition;
        }

        void Update()
        {
            animTime += Time.deltaTime;

            // Recover squash/stretch
            squashAmount = Mathf.Lerp(squashAmount, 1f, squashRecoverySpeed * Time.deltaTime);
            stretchAmount = Mathf.Lerp(stretchAmount, 1f, squashRecoverySpeed * Time.deltaTime);

            // Landing detection
            if (isGrounded && !wasGrounded)
            {
                OnLand();
            }
            wasGrounded = isGrounded;

            // Calculate animation
            Vector3 offset = Vector3.zero;
            Vector3 scale = baseScale;
            Vector3 rotation = Vector3.zero;

            if (isGrounded)
            {
                if (currentSpeed < 0.1f)
                {
                    // Idle: gentle bob + breathing
                    offset.y = Mathf.Sin(animTime * idleBobSpeed) * idleBobAmount;
                    float breathe = 1f + Mathf.Sin(animTime * idleBobSpeed * 0.5f) * idleBreathScale;
                    scale = baseScale * breathe;
                }
                else if (currentSpeed < 0.6f)
                {
                    // Walk: step bob
                    offset.y = Mathf.Abs(Mathf.Sin(animTime * walkBobSpeed)) * walkBobAmount;
                    offset.x = Mathf.Sin(animTime * walkBobSpeed * 0.5f) * walkBobAmount * 0.3f;
                    rotation.z = Mathf.Sin(animTime * walkBobSpeed * 0.5f) * walkTiltAmount;
                }
                else
                {
                    // Run: faster, more pronounced
                    offset.y = Mathf.Abs(Mathf.Sin(animTime * runBobSpeed)) * runBobAmount;
                    offset.x = Mathf.Sin(animTime * runBobSpeed * 0.5f) * runBobAmount * 0.4f;
                    rotation.z = Mathf.Sin(animTime * runBobSpeed * 0.5f) * walkTiltAmount * 1.5f;
                }
            }
            else
            {
                // Airborne: stretch when rising, prepare for land when falling
                if (yVelocity > 0.5f)
                {
                    // Rising
                    scale.y = baseScale.y * stretchAmount;
                    scale.x = baseScale.x / Mathf.Sqrt(stretchAmount);
                    scale.z = baseScale.z / Mathf.Sqrt(stretchAmount);
                }
                else
                {
                    // Falling - anticipate landing
                    float fallAnticipation = Mathf.Clamp01(-yVelocity / 10f);
                    scale.y = baseScale.y * (1f - fallAnticipation * 0.1f);
                }
            }

            // Apply squash/stretch
            scale.y *= squashAmount;
            scale.x *= (2f - squashAmount); // inverse on x
            scale.z *= (2f - squashAmount);

            // Apply to visual
            visualTarget.localPosition = baseLocalPos + offset;
            visualTarget.localScale = scale;
            visualTarget.localEulerAngles = rotation;
        }

        /// <summary>
        /// Call from PlayerController or PlayerAnimatorDriver to update state.
        /// </summary>
        public void SetState(float speed01, bool grounded, float yVel)
        {
            currentSpeed = speed01;
            isGrounded = grounded;
            yVelocity = yVel;
        }

        /// <summary>
        /// Call when player initiates a jump.
        /// </summary>
        public void OnJump()
        {
            squashAmount = jumpSquash;
            stretchAmount = jumpStretch;
        }

        /// <summary>
        /// Called when landing is detected.
        /// </summary>
        void OnLand()
        {
            squashAmount = landSquash;
        }

        /// <summary>
        /// Visual feedback for taking damage or dying.
        /// </summary>
        public void OnDamage()
        {
            squashAmount = 0.5f;
        }
    }
}
