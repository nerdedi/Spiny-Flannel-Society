using UnityEngine;
using SFS.Animation;
using SFS.Core;

namespace SFS.Player
{
    /// <summary>
    /// Minimal, clear movement controller that correctly wires to the
    /// animation system.  Replaces the common "nothing happens" pattern
    /// where the animation driver is never called.
    ///
    /// This script demonstrates:
    ///   1. Feed locomotion EVERY frame (SetLocomotion)
    ///   2. Fire action triggers ONCE (PlayJump, PlayDash)
    ///   3. Pull accessibility values from SettingsManager
    ///
    /// Use this as a reference, or as a drop-in replacement for PlayerController.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMotor : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 7f;
        public float acceleration = 16f;
        public float deceleration = 20f;
        public float airControl = 0.6f;

        [Header("Jump")]
        public float jumpHeight = 2f;
        public float gravity = -25f;
        public float jumpCutMultiplier = 0.4f;

        [Header("Accessibility Defaults")]
        [Tooltip("Pulled from SettingsManager or DefaultsRegistry at runtime")]
        public float coyoteTime = 0.12f;
        public float jumpBuffer = 0.15f;

        [Header("References")]
        public Transform cameraTransform;
        public CharacterAnimationDriver animDriver;

        // Runtime
        CharacterController cc;
        Vector3 velocity;
        float currentSpeed;
        float coyoteCounter;
        float jumpBufferCounter;
        bool controlsLocked;

        void Awake()
        {
            cc = GetComponent<CharacterController>();
        }

        void Start()
        {
            if (!cameraTransform && UnityEngine.Camera.main)
                cameraTransform = UnityEngine.Camera.main.transform;

            if (!animDriver)
                animDriver = GetComponent<CharacterAnimationDriver>();

            ApplyAccessibilitySettings();
        }

        void ApplyAccessibilitySettings()
        {
            // Pull from SettingsManager if available
            if (SettingsManager.Instance == null) return;
            var s = SettingsManager.Instance.Data;
            coyoteTime = s.coyoteTime;
            jumpBuffer = s.jumpBuffer;
        }

        void Update()
        {
            if (controlsLocked)
            {
                // CRITICAL: still feed the animator even when locked,
                // so idle/grounded state stays correct
                animDriver?.SetLocomotion(0f, cc.isGrounded, velocity.y);
                return;
            }

            bool grounded = cc.isGrounded;
            float dt = Time.deltaTime;

            // ── Input ────────────────────────────────────────────
            Vector2 rawInput = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            );
            rawInput = Vector2.ClampMagnitude(rawInput, 1f);
            bool jumpPressed = Input.GetButtonDown("Jump");
            bool jumpReleased = Input.GetButtonUp("Jump");

            // ── Timers ───────────────────────────────────────────
            if (grounded) coyoteCounter = coyoteTime;
            else coyoteCounter -= dt;

            if (jumpPressed) jumpBufferCounter = jumpBuffer;
            else jumpBufferCounter -= dt;

            // ── Ground stick ─────────────────────────────────────
            if (grounded && velocity.y < 0f)
                velocity.y = -2f;

            // ── Camera-relative direction ────────────────────────
            Vector3 camFwd = Vector3.forward;
            Vector3 camRight = Vector3.right;
            if (cameraTransform)
            {
                camFwd = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
                camRight = cameraTransform.right;
            }
            Vector3 desiredDir = (camFwd * rawInput.y + camRight * rawInput.x).normalized;

            // ── Speed (smooth) ───────────────────────────────────
            float targetSpeed = desiredDir.magnitude * moveSpeed;
            float rate = (targetSpeed > currentSpeed) ? acceleration : deceleration;
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, rate * dt);

            // ── Move ─────────────────────────────────────────────
            float control = grounded ? 1f : airControl;
            cc.Move(desiredDir * currentSpeed * control * dt);

            // ── Rotate to face ───────────────────────────────────
            if (desiredDir.sqrMagnitude > 0.001f)
            {
                Quaternion target = Quaternion.LookRotation(desiredDir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, 14f * dt);
            }

            // ── Jump (buffer + coyote) ───────────────────────────
            if (jumpBufferCounter > 0f && coyoteCounter > 0f)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpBufferCounter = 0f;
                coyoteCounter = 0f;
                animDriver?.PlayJump();
            }

            // ── Variable jump height ─────────────────────────────
            if (jumpReleased && velocity.y > 0f)
                velocity.y *= jumpCutMultiplier;

            // ── Gravity ──────────────────────────────────────────
            velocity.y += gravity * dt;
            cc.Move(velocity * dt);

            // ── FEED THE ANIMATOR (every frame!) ─────────────────
            float speed01 = Mathf.Clamp01(currentSpeed / moveSpeed);
            animDriver?.SetLocomotion(speed01, grounded, velocity.y);

            // ── Action triggers ──────────────────────────────────
            if (Input.GetKeyDown(KeyCode.LeftShift))
                animDriver?.PlayDash();
        }

        public void LockControls(bool locked)
        {
            controlsLocked = locked;
        }
    }
}
