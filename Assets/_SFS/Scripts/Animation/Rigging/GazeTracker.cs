using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace SFS.Animation.Rigging
{
    /// <summary>
    /// Makes the player character look at nearby "Readable" objects —
    /// defaults that can be Read via Translation Verbs.
    ///
    /// Uses Unity Animation Rigging's MultiAimConstraint so the head
    /// rotates naturally without custom bone math.
    ///
    /// HOW IT WORKS:
    ///   1. Scans for nearby GameObjects tagged "Readable" or on the Readable layer
    ///   2. Picks the closest one within the gaze cone (field of view)
    ///   3. Smoothly blends the MultiAimConstraint weight up
    ///   4. When the player moves away or looks away, blends weight down
    ///   5. Integrates with TranslationVerbBridge: weight goes to 1.0 during Read
    ///
    /// SETUP (done automatically by RiggingSetupWizard):
    ///   - Rig GameObject with Rig component under the player
    ///   - Head bone with MultiAimConstraint pointing at a GazeTarget transform
    ///   - This script moves the GazeTarget to track the nearest readable
    /// </summary>
    public class GazeTracker : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("The MultiAimConstraint on the head bone (auto-found if null)")]
        public MultiAimConstraint headAimConstraint;

        [Tooltip("The transform that the head aims at (we move this)")]
        public Transform gazeTarget;

        [Tooltip("The player's eye-level transform (used for LOS checks)")]
        public Transform eyePoint;

        [Header("Detection")]
        [Tooltip("How far can the player 'notice' a readable object")]
        public float gazeRange = 8f;

        [Tooltip("Cone half-angle in degrees — how far off-center can they notice")]
        [Range(15f, 90f)]
        public float gazeConeAngle = 55f;

        [Tooltip("Layer mask for readable objects")]
        public LayerMask readableLayer = ~0;

        [Tooltip("Tag for readable objects (set to empty to rely on layer only)")]
        public string readableTag = "Readable";

        [Header("Blending")]
        [Tooltip("How fast the gaze blends toward a new target")]
        public float blendInSpeed = 4f;

        [Tooltip("How fast the gaze releases when no target")]
        public float blendOutSpeed = 6f;

        [Tooltip("Minimum weight — keeps a tiny head bob even with no target")]
        [Range(0f, 0.1f)]
        public float idleWeight = 0f;

        [Tooltip("Maximum weight during passive scanning")]
        [Range(0.3f, 0.8f)]
        public float passiveMaxWeight = 0.55f;

        [Tooltip("Weight during active Read verb (overrides passive)")]
        [Range(0.8f, 1f)]
        public float activeReadWeight = 1f;

        [Header("State")]
        [SerializeField] float currentWeight;
        [SerializeField] Transform currentTarget;

        // ── Internal ────────────────────────────────────────────
        bool isActiveRead;
        float targetWeight;
        Collider[] scanBuffer = new Collider[16];
        Vector3 smoothVelocity;

        void Start()
        {
            // Try to auto-find references
            if (!headAimConstraint)
                headAimConstraint = GetComponentInChildren<MultiAimConstraint>();

            if (!eyePoint)
                eyePoint = transform; // fallback to root

            if (headAimConstraint != null)
                currentWeight = headAimConstraint.weight;
        }

        void LateUpdate()
        {
            if (headAimConstraint == null || gazeTarget == null) return;

            // During active Read, lock onto the pending target
            if (isActiveRead && currentTarget != null)
            {
                targetWeight = activeReadWeight;
            }
            else
            {
                // Passive scanning
                Transform nearest = FindNearestReadable();

                if (nearest != null)
                {
                    currentTarget = nearest;
                    targetWeight = passiveMaxWeight;
                }
                else
                {
                    targetWeight = idleWeight;
                }
            }

            // Smoothly move gaze target position
            if (currentTarget != null)
            {
                Vector3 aimPoint = currentTarget.position;

                // If the target has a collider, aim at its center
                var col = currentTarget.GetComponent<Collider>();
                if (col != null)
                    aimPoint = col.bounds.center;

                gazeTarget.position = Vector3.SmoothDamp(
                    gazeTarget.position, aimPoint,
                    ref smoothVelocity, 0.1f);
            }

            // Blend constraint weight
            float speed = targetWeight > currentWeight ? blendInSpeed : blendOutSpeed;
            currentWeight = Mathf.MoveTowards(currentWeight, targetWeight, speed * Time.deltaTime);
            headAimConstraint.weight = currentWeight;
        }

        // ═════════════════════════════════════════════════════════
        //  SCANNING
        // ═════════════════════════════════════════════════════════

        Transform FindNearestReadable()
        {
            int hits = Physics.OverlapSphereNonAlloc(
                eyePoint.position, gazeRange, scanBuffer, readableLayer);

            Transform best = null;
            float bestDist = float.MaxValue;
            Vector3 forward = eyePoint.forward;

            for (int i = 0; i < hits; i++)
            {
                var obj = scanBuffer[i];
                if (obj.transform == transform) continue; // skip self

                // Tag filter (if configured)
                if (!string.IsNullOrEmpty(readableTag) && !obj.CompareTag(readableTag))
                    continue;

                Vector3 toObj = obj.bounds.center - eyePoint.position;
                float dist = toObj.magnitude;

                // Cone check
                float angle = Vector3.Angle(forward, toObj);
                if (angle > gazeConeAngle) continue;

                // Favour closer objects
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = obj.transform;
                }
            }

            return best;
        }

        // ═════════════════════════════════════════════════════════
        //  PUBLIC API (called by TranslationVerbBridge)
        // ═════════════════════════════════════════════════════════

        /// <summary>Lock gaze onto a specific target during Read Default.</summary>
        public void BeginReadGaze(Transform target)
        {
            isActiveRead = true;
            if (target != null)
                currentTarget = target;
        }

        /// <summary>Release gaze lock after Read completes.</summary>
        public void EndReadGaze()
        {
            isActiveRead = false;
        }

        /// <summary>Force-set a gaze target (e.g., during cutscene).</summary>
        public void SetGazeOverride(Transform target, float weight = 1f)
        {
            currentTarget = target;
            targetWeight = weight;
            isActiveRead = true;
        }

        /// <summary>Clear any override.</summary>
        public void ClearGazeOverride()
        {
            isActiveRead = false;
            currentTarget = null;
        }
    }
}
