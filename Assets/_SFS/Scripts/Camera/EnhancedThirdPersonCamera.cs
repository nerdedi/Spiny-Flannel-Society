using UnityEngine;

namespace SFS.Camera
{
    /// <summary>
    /// Smooth third-person camera with orbit, zoom, collision avoidance,
    /// and story beat response for cinematic moments.
    /// </summary>
    public class EnhancedThirdPersonCamera : MonoBehaviour
    {
        #region Inspector Fields

        [Header("═══ TARGET ═══")]
        [Tooltip("Target to follow (usually the player)")]
        public Transform target;

        [Tooltip("Offset from target's pivot")]
        public Vector3 targetOffset = new Vector3(0f, 1.5f, 0f);

        [Header("═══ ORBIT SETTINGS ═══")]
        [Tooltip("Default distance from target")]
        [Range(2f, 20f)] public float defaultDistance = 6f;

        [Tooltip("Minimum zoom distance")]
        [Range(1f, 10f)] public float minDistance = 2f;

        [Tooltip("Maximum zoom distance")]
        [Range(5f, 30f)] public float maxDistance = 15f;

        [Tooltip("Horizontal rotation speed")]
        [Range(50f, 300f)] public float horizontalSpeed = 150f;

        [Tooltip("Vertical rotation speed")]
        [Range(50f, 300f)] public float verticalSpeed = 120f;

        [Tooltip("Minimum vertical angle")]
        [Range(-80f, 0f)] public float minVerticalAngle = -20f;

        [Tooltip("Maximum vertical angle")]
        [Range(0f, 80f)] public float maxVerticalAngle = 70f;

        [Header("═══ SMOOTHING ═══")]
        [Tooltip("Position follow smoothness")]
        [Range(0.01f, 0.5f)] public float positionSmoothing = 0.08f;

        [Tooltip("Rotation smoothness")]
        [Range(0.01f, 0.5f)] public float rotationSmoothing = 0.05f;

        [Tooltip("Zoom smoothness")]
        [Range(0.05f, 0.5f)] public float zoomSmoothing = 0.15f;

        [Header("═══ COLLISION ═══")]
        [Tooltip("Enable collision avoidance")]
        public bool enableCollision = true;

        [Tooltip("Layers to collide with")]
        public LayerMask collisionLayers = -1;

        [Tooltip("Radius of collision sphere")]
        [Range(0.1f, 1f)] public float collisionRadius = 0.3f;

        [Tooltip("How quickly camera recovers after collision")]
        [Range(1f, 10f)] public float collisionRecoverySpeed = 4f;

        [Header("═══ INPUT ═══")]
        [Tooltip("Mouse sensitivity multiplier")]
        [Range(0.1f, 3f)] public float mouseSensitivity = 1f;

        [Tooltip("Scroll zoom sensitivity")]
        [Range(0.5f, 5f)] public float scrollSensitivity = 2f;

        [Tooltip("Invert Y axis")]
        public bool invertY = false;

        [Tooltip("Require right mouse button for orbit")]
        public bool requireRightClick = false;

        [Header("═══ AUTO BEHAVIOR ═══")]
        [Tooltip("Auto-rotate behind player when moving")]
        public bool autoRotateBehindPlayer = false;

        [Tooltip("Delay before auto-rotate kicks in")]
        [Range(0f, 5f)] public float autoRotateDelay = 2f;

        [Tooltip("Speed of auto-rotation")]
        [Range(1f, 10f)] public float autoRotateSpeed = 3f;

        [Header("═══ STORY BEAT RESPONSE ═══")]
        [Tooltip("Allow story beats to modify camera")]
        public bool respondToStoryBeats = true;

        #endregion

        #region Runtime State

        float currentDistance;
        float targetDistance;
        float currentYaw;
        float currentPitch;
        float lastInputTime;
        float currentCollisionDistance;
        Vector3 smoothedPosition;
        Vector3 currentVelocity;

        // Story beat override
        bool inCinematicMode;
        Vector3 cinematicPosition;
        Quaternion cinematicRotation;
        float cinematicBlend;

        #endregion

        #region Lifecycle

        void Awake()
        {
            // Initialize default values
            currentDistance = defaultDistance;
            targetDistance = defaultDistance;
            currentCollisionDistance = defaultDistance;

            // Start with current rotation
            Vector3 angles = transform.eulerAngles;
            currentYaw = angles.y;
            currentPitch = angles.x;
        }

        void Start()
        {
            // Auto-find player if no target
            if (!target)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player) target = player.transform;
            }

            // Cursor setup
            if (!requireRightClick)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            // Initialize position
            if (target)
            {
                smoothedPosition = target.position + targetOffset;
            }
        }

        void LateUpdate()
        {
            if (!target) return;

            // Handle cinematic override
            if (inCinematicMode)
            {
                UpdateCinematicMode();
                return;
            }

            HandleInput();
            HandleZoom();
            UpdatePosition();
            HandleCollision();
            ApplyTransform();
        }

        #endregion

        #region Input Handling

        void HandleInput()
        {
            bool canOrbit = !requireRightClick || Input.GetMouseButton(1);

            if (canOrbit)
            {
                float mouseX = Input.GetAxis("Mouse X") * horizontalSpeed * mouseSensitivity * Time.deltaTime;
                float mouseY = Input.GetAxis("Mouse Y") * verticalSpeed * mouseSensitivity * Time.deltaTime;

                if (Mathf.Abs(mouseX) > 0.01f || Mathf.Abs(mouseY) > 0.01f)
                {
                    lastInputTime = Time.time;
                }

                currentYaw += mouseX;
                currentPitch += invertY ? mouseY : -mouseY;
                currentPitch = Mathf.Clamp(currentPitch, minVerticalAngle, maxVerticalAngle);
            }

            // Auto-rotate behind player
            if (autoRotateBehindPlayer && target && Time.time - lastInputTime > autoRotateDelay)
            {
                // Get player's forward direction
                float targetYaw = target.eulerAngles.y;
                currentYaw = Mathf.LerpAngle(currentYaw, targetYaw, autoRotateSpeed * Time.deltaTime);
            }

            // Handle cursor visibility
            if (requireRightClick)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                else if (Input.GetMouseButtonUp(1))
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }

        void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;

            if (Mathf.Abs(scroll) > 0.01f)
            {
                targetDistance -= scroll;
                targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
            }

            // Smooth zoom
            currentDistance = Mathf.Lerp(currentDistance, targetDistance, (1f / zoomSmoothing) * Time.deltaTime);
        }

        #endregion

        #region Position & Collision

        void UpdatePosition()
        {
            // Target position with offset
            Vector3 targetPos = target.position + targetOffset;

            // Smooth follow
            smoothedPosition = Vector3.SmoothDamp(
                smoothedPosition,
                targetPos,
                ref currentVelocity,
                positionSmoothing
            );
        }

        void HandleCollision()
        {
            if (!enableCollision)
            {
                currentCollisionDistance = currentDistance;
                return;
            }

            // Calculate camera direction
            Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
            Vector3 direction = rotation * Vector3.back;

            // Raycast for obstacles
            float targetCollisionDist = currentDistance;
            RaycastHit hit;

            if (Physics.SphereCast(
                smoothedPosition,
                collisionRadius,
                direction,
                out hit,
                currentDistance,
                collisionLayers))
            {
                targetCollisionDist = hit.distance - collisionRadius * 0.5f;
                targetCollisionDist = Mathf.Max(targetCollisionDist, minDistance * 0.5f);
            }

            // Smooth collision distance
            if (targetCollisionDist < currentCollisionDistance)
            {
                // Quick snap in when hitting obstacle
                currentCollisionDistance = targetCollisionDist;
            }
            else
            {
                // Slow recovery when obstacle clears
                currentCollisionDistance = Mathf.Lerp(
                    currentCollisionDistance,
                    targetCollisionDist,
                    collisionRecoverySpeed * Time.deltaTime
                );
            }
        }

        void ApplyTransform()
        {
            // Calculate final rotation
            Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);

            // Calculate final position
            Vector3 direction = rotation * Vector3.back;
            float useDist = Mathf.Min(currentDistance, currentCollisionDistance);
            Vector3 position = smoothedPosition + direction * useDist;

            // Smooth rotation
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotation,
                (1f / rotationSmoothing) * Time.deltaTime
            );
            transform.position = position;
        }

        #endregion

        #region Cinematic Mode

        void UpdateCinematicMode()
        {
            cinematicBlend = Mathf.MoveTowards(cinematicBlend, 1f, Time.deltaTime * 2f);

            transform.position = Vector3.Lerp(transform.position, cinematicPosition, cinematicBlend);
            transform.rotation = Quaternion.Slerp(transform.rotation, cinematicRotation, cinematicBlend);
        }

        /// <summary>
        /// Enter cinematic camera mode
        /// </summary>
        public void EnterCinematicMode(Vector3 position, Quaternion rotation)
        {
            inCinematicMode = true;
            cinematicPosition = position;
            cinematicRotation = rotation;
            cinematicBlend = 0f;
        }

        /// <summary>
        /// Exit cinematic mode and return to normal follow
        /// </summary>
        public void ExitCinematicMode()
        {
            inCinematicMode = false;
            // Update yaw/pitch from current rotation
            Vector3 angles = transform.eulerAngles;
            currentYaw = angles.y;
            currentPitch = angles.x;
        }

        #endregion

        #region Public API

        /// <summary>
        /// Set camera target
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            if (target)
            {
                smoothedPosition = target.position + targetOffset;
            }
        }

        /// <summary>
        /// Set orbit angles directly
        /// </summary>
        public void SetOrbit(float yaw, float pitch)
        {
            currentYaw = yaw;
            currentPitch = Mathf.Clamp(pitch, minVerticalAngle, maxVerticalAngle);
        }

        /// <summary>
        /// Set zoom distance
        /// </summary>
        public void SetDistance(float distance)
        {
            targetDistance = Mathf.Clamp(distance, minDistance, maxDistance);
        }

        /// <summary>
        /// Snap to position and rotation immediately (no smoothing)
        /// </summary>
        public void SnapTo(Vector3 position, Quaternion rotation)
        {
            transform.position = position;
            transform.rotation = rotation;

            Vector3 angles = rotation.eulerAngles;
            currentYaw = angles.y;
            currentPitch = angles.x;

            if (target)
            {
                smoothedPosition = target.position + targetOffset;
                currentDistance = Vector3.Distance(position, smoothedPosition);
                targetDistance = currentDistance;
            }
        }

        /// <summary>
        /// Shake camera (for impacts, events)
        /// </summary>
        public void Shake(float intensity = 0.3f, float duration = 0.2f)
        {
            StartCoroutine(ShakeCoroutine(intensity, duration));
        }

        System.Collections.IEnumerator ShakeCoroutine(float intensity, float duration)
        {
            Vector3 originalPos = transform.localPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * intensity;
                float y = Random.Range(-1f, 1f) * intensity;

                transform.localPosition = originalPos + new Vector3(x, y, 0);

                elapsed += Time.deltaTime;
                intensity *= 0.95f; // Decay
                yield return null;
            }

            transform.localPosition = originalPos;
        }

        /// <summary>
        /// Apply story beat modifiers (distance, angle adjustments)
        /// </summary>
        public void ApplyStoryBeatModifier(float distanceMultiplier, float pitchOffset)
        {
            if (!respondToStoryBeats) return;

            targetDistance = defaultDistance * distanceMultiplier;
            currentPitch = Mathf.Clamp(
                currentPitch + pitchOffset,
                minVerticalAngle,
                maxVerticalAngle
            );
        }

        /// <summary>
        /// Reset to default settings
        /// </summary>
        public void ResetToDefaults()
        {
            targetDistance = defaultDistance;
            inCinematicMode = false;
        }

        #endregion

        #region Gizmos

        void OnDrawGizmosSelected()
        {
            if (!target) return;

            // Draw target offset
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(target.position + targetOffset, 0.2f);

            // Draw camera bounds
            Gizmos.color = Color.cyan;
            Vector3 targetPos = target.position + targetOffset;

            Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
            Vector3 dir = rotation * Vector3.back;

            Gizmos.DrawLine(targetPos, targetPos + dir * minDistance);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(targetPos + dir * minDistance, targetPos + dir * maxDistance);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(targetPos + dir * currentDistance, collisionRadius);
        }

        #endregion
    }
}
