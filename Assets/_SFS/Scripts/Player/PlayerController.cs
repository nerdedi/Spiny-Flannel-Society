using UnityEngine;
using SFS.Core;

namespace SFS.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 6f;
        public float acceleration = 14f;
        public float deceleration = 18f;
        public float airControl = 0.65f;

        [Header("Jump")]
        public float jumpHeight = 1.6f;
        public float gravity = -22f;
        public float jumpCutMultiplier = 0.5f;

        [Header("Rotation")]
        public float turnSpeed = 14f;

        [Header("References")]
        public Transform cameraTransform; // assign Main Camera transform
        public PlayerAnimatorDriver animatorDriver;

        CharacterController cc;
        Vector3 velocity;
        float currentSpeed;
        float coyoteCounter;
        float jumpBufferCounter;

        // simple input (swap to Input System later if desired)
        Vector2 moveInput;
        bool jumpPressed;
        bool jumpHeld;
        bool jumpReleased;

        public bool ControlsLocked { get; private set; }

        void Awake()
        {
            cc = GetComponent<CharacterController>();
        }

        void OnEnable()
        {
            GameEvents.OnPauseChanged += OnPauseChanged;
            GameEvents.OnSettingsChanged += ApplySettings;
        }

        void OnDisable()
        {
            GameEvents.OnPauseChanged -= OnPauseChanged;
            GameEvents.OnSettingsChanged -= ApplySettings;
        }

        void Start()
        {
            if (!cameraTransform && Camera.main) cameraTransform = Camera.main.transform;
            ApplySettings();
        }

        void ApplySettings()
        {
            if (SettingsManager.Instance == null) return;
            // Pull in accessibility timings:
            var s = SettingsManager.Instance.Data;
            // used per-frame in Update; keep local if you like
        }

        void Update()
        {
            if (ControlsLocked) { animatorDriver?.SetMove(0f, cc.isGrounded, velocity.y); return; }
            ReadInput();
            TickMovement(Time.deltaTime);
        }

        void ReadInput()
        {
            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            moveInput = Vector2.ClampMagnitude(moveInput, 1f);
            jumpPressed = Input.GetButtonDown("Jump");
            jumpHeld = Input.GetButton("Jump");
            jumpReleased = Input.GetButtonUp("Jump");
        }

        void TickMovement(float dt)
        {
            bool grounded = cc.isGrounded;

            // Timers from settings (so accessibility sliders actually work)
            float coyoteTime = SettingsManager.Instance ? SettingsManager.Instance.Data.coyoteTime : 0.12f;
            float jumpBuffer = SettingsManager.Instance ? SettingsManager.Instance.Data.jumpBuffer : 0.12f;

            if (grounded) coyoteCounter = coyoteTime;
            else coyoteCounter -= dt;

            if (jumpPressed) jumpBufferCounter = jumpBuffer;
            else jumpBufferCounter -= dt;

            if (grounded && velocity.y < 0f)
                velocity.y = -2f; // stick to ground

            // Camera-relative move
            Vector3 inputDir = new Vector3(moveInput.x, 0f, moveInput.y);
            Vector3 camForward = Vector3.forward;
            Vector3 camRight = Vector3.right;
            if (cameraTransform)
            {
                camForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
                camRight = cameraTransform.right;
            }
            Vector3 desiredDir = (camForward * inputDir.z + camRight * inputDir.x).normalized;

            // Smooth speed
            float targetSpeed = desiredDir.magnitude * moveSpeed;
            float rate = (targetSpeed > currentSpeed) ? acceleration : deceleration;
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, rate * dt);

            // Move (air control)
            float control = grounded ? 1f : airControl;
            Vector3 move = desiredDir * currentSpeed * control;
            cc.Move(move * dt);

            // Rotate to face travel direction
            if (desiredDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(desiredDir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * dt);
            }

            // Jump (buffer + coyote)
            if (jumpBufferCounter > 0f && coyoteCounter > 0f)
            {
                float jumpVel = Mathf.Sqrt(jumpHeight * -2f * gravity);
                velocity.y = jumpVel;
                jumpBufferCounter = 0f;
                coyoteCounter = 0f;
                animatorDriver?.TriggerJump();
            }

            // Variable jump
            if (jumpReleased && velocity.y > 0f)
            {
                velocity.y *= jumpCutMultiplier;
            }

            // Gravity
            velocity.y += gravity * dt;
            cc.Move(velocity * dt);

            // Animation data
            animatorDriver?.SetMove(currentSpeed / moveSpeed, grounded, velocity.y);
            if (grounded) animatorDriver?.TriggerLandIfNeeded();
        }

        void OnPauseChanged(bool paused)
        {
            ControlsLocked = paused;
        }

        public void LockControls(bool locked)
        {
            ControlsLocked = locked;
            if (locked) animatorDriver?.SetMove(0f, cc.isGrounded, velocity.y);
        }
    }
}
