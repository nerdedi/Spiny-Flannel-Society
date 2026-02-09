using UnityEngine;
using SFS.Animation;

namespace SFS.Player
{
    /// <summary>
    /// Enhanced player controller with smooth movement, coyote time,
    /// jump buffering, and accessibility settings.
    /// Works with both Unity Animator and procedural animation systems.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class EnhancedPlayerController : MonoBehaviour
    {
        #region Inspector Fields

        [Header("═══ MOVEMENT ═══")]
        [Tooltip("Maximum movement speed")]
        [Range(3f, 15f)] public float moveSpeed = 7f;

        [Tooltip("How quickly player reaches max speed")]
        [Range(5f, 30f)] public float acceleration = 16f;

        [Tooltip("How quickly player stops")]
        [Range(5f, 40f)] public float deceleration = 20f;

        [Tooltip("Movement control while airborne (0-1)")]
        [Range(0f, 1f)] public float airControl = 0.6f;

        [Header("═══ JUMP ═══")]
        [Tooltip("Jump height in units")]
        [Range(0.5f, 5f)] public float jumpHeight = 2f;

        [Tooltip("Gravity strength")]
        [Range(-50f, -10f)] public float gravity = -25f;

        [Tooltip("Multiplier when releasing jump early")]
        [Range(0.1f, 1f)] public float jumpCutMultiplier = 0.4f;

        [Tooltip("Extra downward force at apex for snappy feel")]
        [Range(0f, 20f)] public float apexGravityBoost = 5f;

        [Tooltip("Velocity threshold for apex detection")]
        [Range(0f, 3f)] public float apexThreshold = 1.5f;

        [Header("═══ ACCESSIBILITY (Timing Forgiveness) ═══")]
        [Tooltip("Time after leaving ground where jump still works")]
        [Range(0f, 0.3f)] public float coyoteTime = 0.12f;

        [Tooltip("Time before landing where jump input is remembered")]
        [Range(0f, 0.3f)] public float jumpBuffer = 0.15f;

        [Header("═══ ROTATION ═══")]
        [Tooltip("How quickly player rotates to face movement")]
        [Range(5f, 30f)] public float turnSpeed = 14f;

        [Tooltip("Use camera-relative movement")]
        public bool cameraRelativeMovement = true;

        [Header("═══ REFERENCES ═══")]
        [Tooltip("Camera transform for relative movement")]
        public Transform cameraTransform;

        [Tooltip("Animator driver for Unity Animator")]
        public PlayerAnimatorDriver animatorDriver;

        [Tooltip("Procedural animator (auto-detected if null)")]
        public AdvancedProceduralAnimator proceduralAnimator;

        #endregion

        #region Runtime State

        CharacterController cc;
        Vector3 velocity;
        Vector3 moveDirection;
        float currentSpeed;
        float coyoteCounter;
        float jumpBufferCounter;
        bool jumpPressed;
        bool jumpHeld;
        bool jumpReleased;
        bool wasGrounded;

        public bool ControlsLocked { get; private set; }
        public bool IsGrounded => cc.isGrounded;
        public Vector3 Velocity => velocity;
        public float CurrentSpeedNormalized => Mathf.Clamp01(currentSpeed / moveSpeed);

        #endregion

        #region Lifecycle

        void Awake()
        {
            cc = GetComponent<CharacterController>();
        }

        void Start()
        {
            // Auto-find camera
            if (!cameraTransform && Camera.main)
                cameraTransform = Camera.main.transform;

            // Auto-find animators
            if (!animatorDriver)
                animatorDriver = GetComponent<PlayerAnimatorDriver>();

            if (!proceduralAnimator)
                proceduralAnimator = GetComponentInChildren<AdvancedProceduralAnimator>();
        }

        void Update()
        {
            if (ControlsLocked)
            {
                UpdateAnimators(0f, cc.isGrounded, velocity.y, Vector3.zero);
                return;
            }

            ReadInput();
            TickMovement(Time.deltaTime);
        }

        #endregion

        #region Input

        void ReadInput()
        {
            jumpPressed = Input.GetButtonDown("Jump");
            jumpHeld = Input.GetButton("Jump");
            jumpReleased = Input.GetButtonUp("Jump");
        }

        Vector2 GetMoveInput()
        {
            var input = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            );
            return Vector2.ClampMagnitude(input, 1f);
        }

        #endregion

        #region Movement

        void TickMovement(float dt)
        {
            bool grounded = cc.isGrounded;
            Vector2 input = GetMoveInput();

            // ─── Timers ───
            UpdateTimers(dt, grounded);

            // ─── Calculate Move Direction ───
            Vector3 inputDir = CalculateInputDirection(input);

            // ─── Horizontal Movement ───
            ApplyHorizontalMovement(inputDir, grounded, dt);

            // ─── Rotation ───
            if (inputDir.sqrMagnitude > 0.01f)
            {
                ApplyRotation(inputDir, dt);
            }

            // ─── Jump ───
            ProcessJump(grounded);

            // ─── Gravity ───
            ApplyGravity(dt);

            // ─── Move Character ───
            cc.Move(velocity * dt);

            // ─── Landing Detection ───
            if (grounded && !wasGrounded)
            {
                OnLand();
            }
            wasGrounded = grounded;

            // ─── Update Animators ───
            UpdateAnimators(CurrentSpeedNormalized, grounded, velocity.y, moveDirection);
        }

        void UpdateTimers(float dt, bool grounded)
        {
            // Coyote time
            if (grounded)
                coyoteCounter = coyoteTime;
            else
                coyoteCounter -= dt;

            // Jump buffer
            if (jumpPressed)
                jumpBufferCounter = jumpBuffer;
            else
                jumpBufferCounter -= dt;
        }

        Vector3 CalculateInputDirection(Vector2 input)
        {
            if (input.sqrMagnitude < 0.01f)
                return Vector3.zero;

            if (cameraRelativeMovement && cameraTransform)
            {
                Vector3 camForward = cameraTransform.forward;
                Vector3 camRight = cameraTransform.right;
                camForward.y = 0f;
                camRight.y = 0f;
                camForward.Normalize();
                camRight.Normalize();

                return (camForward * input.y + camRight * input.x).normalized;
            }
            else
            {
                return new Vector3(input.x, 0f, input.y).normalized;
            }
        }

        void ApplyHorizontalMovement(Vector3 inputDir, bool grounded, float dt)
        {
            float targetSpeed = inputDir.magnitude * moveSpeed;
            float control = grounded ? 1f : airControl;
            float accel = targetSpeed > currentSpeed ? acceleration : deceleration;

            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accel * control * dt);

            if (inputDir.sqrMagnitude > 0.01f)
            {
                moveDirection = inputDir;
            }

            velocity.x = moveDirection.x * currentSpeed;
            velocity.z = moveDirection.z * currentSpeed;
        }

        void ApplyRotation(Vector3 direction, float dt)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                turnSpeed * dt
            );
        }

        void ProcessJump(bool grounded)
        {
            // Can jump if grounded or within coyote time
            bool canJump = grounded || coyoteCounter > 0f;

            // Jump if buffered and can jump
            if (jumpBufferCounter > 0f && canJump)
            {
                ExecuteJump();
            }

            // Jump cut - reduce upward velocity when releasing jump early
            if (jumpReleased && velocity.y > 0f)
            {
                velocity.y *= jumpCutMultiplier;
            }
        }

        void ExecuteJump()
        {
            // v = sqrt(2 * g * h)
            velocity.y = Mathf.Sqrt(-2f * gravity * jumpHeight);

            // Reset timers
            coyoteCounter = 0f;
            jumpBufferCounter = 0f;

            // Notify animators
            animatorDriver?.TriggerJump();
            proceduralAnimator?.OnJump();
        }

        void ApplyGravity(float dt)
        {
            if (cc.isGrounded && velocity.y < 0f)
            {
                velocity.y = -2f; // Small downward force to stay grounded
            }
            else
            {
                float grav = gravity;

                // Extra gravity at apex for snappier feel
                if (Mathf.Abs(velocity.y) < apexThreshold)
                {
                    grav -= apexGravityBoost;
                }

                velocity.y += grav * dt;
            }
        }

        void OnLand()
        {
            float impactVelocity = Mathf.Abs(velocity.y);

            animatorDriver?.TriggerLand();
            proceduralAnimator?.OnLand(impactVelocity);
        }

        #endregion

        #region Animation Integration

        void UpdateAnimators(float speed, bool grounded, float yVel, Vector3 moveDir)
        {
            animatorDriver?.SetMove(speed, grounded, yVel);
            proceduralAnimator?.SetState(speed, grounded, yVel, moveDir);
        }

        #endregion

        #region Public API

        /// <summary>
        /// Lock player controls (for cutscenes, menus, etc.)
        /// </summary>
        public void LockControls()
        {
            ControlsLocked = true;
            velocity = Vector3.zero;
            currentSpeed = 0f;
        }

        /// <summary>
        /// Unlock player controls
        /// </summary>
        public void UnlockControls()
        {
            ControlsLocked = false;
        }

        /// <summary>
        /// Teleport player to position
        /// </summary>
        public void TeleportTo(Vector3 position)
        {
            cc.enabled = false;
            transform.position = position;
            cc.enabled = true;
            velocity = Vector3.zero;
        }

        /// <summary>
        /// Apply external force (knockback, wind, etc.)
        /// </summary>
        public void AddForce(Vector3 force)
        {
            velocity += force;
        }

        /// <summary>
        /// Trigger death animation
        /// </summary>
        public void Die()
        {
            LockControls();
            animatorDriver?.TriggerDie();
            proceduralAnimator?.OnDeath();
        }

        /// <summary>
        /// Trigger damage reaction
        /// </summary>
        public void TakeDamage()
        {
            proceduralAnimator?.OnDamage();
        }

        /// <summary>
        /// Trigger collect animation
        /// </summary>
        public void OnCollect()
        {
            proceduralAnimator?.OnCollect();
        }

        /// <summary>
        /// Update accessibility settings at runtime
        /// </summary>
        public void UpdateAccessibilitySettings(float newCoyoteTime, float newJumpBuffer)
        {
            coyoteTime = newCoyoteTime;
            jumpBuffer = newJumpBuffer;
        }

        #endregion
    }
}
