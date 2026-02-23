using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace SFS.Animation.Rigging
{
    /// <summary>
    /// Controls Two-Bone IK on the character's hands for Translation Verbs
    /// (Read Default, Rewrite Cushion/Guard) and Windprint gestures.
    ///
    /// The IK targets are positioned by animation clips or procedurally —
    /// this script blends constraint weights so the hand reaches toward
    /// the target only when a verb is active.
    ///
    /// DESIGN INTENT:
    ///   • Read Default:       Right hand rises, palm open → "scanning" gesture
    ///   • Rewrite Cushion:    Both hands extend, cupping → "softening" gesture
    ///   • Rewrite Guard:      Both hands push forward → "walling" gesture
    ///   • Windprint Cushion:  Hands float at sides → "flow" posture
    ///   • Windprint Guard:    Fists locked at hips → "rooted" posture
    ///
    /// SETUP (done by RiggingSetupWizard):
    ///   - Two TwoBoneIKConstraint components (left hand, right hand)
    ///   - IK target transforms positioned in front of character
    ///   - This script drives weight 0→1 when verbs fire
    /// </summary>
    public class IKVerbRig : MonoBehaviour
    {
        [Header("IK Constraints")]
        [Tooltip("Two-Bone IK for the right hand")]
        public TwoBoneIKConstraint rightHandIK;

        [Tooltip("Two-Bone IK for the left hand")]
        public TwoBoneIKConstraint leftHandIK;

        [Header("IK Targets (move these to shape the gesture)")]
        public Transform rightHandTarget;
        public Transform leftHandTarget;

        [Header("Gesture Poses")]
        [Tooltip("Local position of right hand during Read Default")]
        public Vector3 readPoseRight = new Vector3(0.3f, 1.2f, 0.5f);

        [Tooltip("Local positions for Rewrite Cushion (open, cupping)")]
        public Vector3 cushionPoseRight = new Vector3(0.4f, 1.0f, 0.6f);
        public Vector3 cushionPoseLeft  = new Vector3(-0.4f, 1.0f, 0.6f);

        [Tooltip("Local positions for Rewrite Guard (push forward)")]
        public Vector3 guardPoseRight = new Vector3(0.25f, 1.1f, 0.7f);
        public Vector3 guardPoseLeft  = new Vector3(-0.25f, 1.1f, 0.7f);

        [Header("Windprint Mode Poses")]
        [Tooltip("Hands float at sides for Cushion windprint")]
        public Vector3 windprintCushionRight = new Vector3(0.45f, 0.9f, 0.1f);
        public Vector3 windprintCushionLeft  = new Vector3(-0.45f, 0.9f, 0.1f);

        [Tooltip("Fists locked at hips for Guard windprint")]
        public Vector3 windprintGuardRight = new Vector3(0.3f, 0.75f, 0.0f);
        public Vector3 windprintGuardLeft  = new Vector3(-0.3f, 0.75f, 0.0f);

        [Header("Blending")]
        [Tooltip("How fast IK blends in when a verb fires")]
        public float blendInSpeed = 8f;

        [Tooltip("How fast IK blends out after verb completes")]
        public float blendOutSpeed = 5f;

        [Tooltip("How fast IK targets move to new pose positions")]
        public float poseLerpSpeed = 10f;

        // ── State ───────────────────────────────────────────────
        public enum VerbState
        {
            None,
            ReadDefault,
            RewriteCushion,
            RewriteGuard,
            WindprintCushion,
            WindprintGuard
        }

        [Header("Debug")]
        [SerializeField] VerbState activeVerb = VerbState.None;
        [SerializeField] float rightWeight;
        [SerializeField] float leftWeight;

        float rightTargetWeight;
        float leftTargetWeight;

        Vector3 rightTargetPos;
        Vector3 leftTargetPos;

        void Start()
        {
            // Default IK off
            if (rightHandIK) rightHandIK.weight = 0f;
            if (leftHandIK)  leftHandIK.weight = 0f;
        }

        void LateUpdate()
        {
            UpdateTargetPositions();
            UpdateWeights();
            ApplyTargetPositions();
        }

        void UpdateTargetPositions()
        {
            switch (activeVerb)
            {
                case VerbState.ReadDefault:
                    rightTargetPos = readPoseRight;
                    rightTargetWeight = 1f;
                    leftTargetWeight = 0f;
                    break;

                case VerbState.RewriteCushion:
                    rightTargetPos = cushionPoseRight;
                    leftTargetPos  = cushionPoseLeft;
                    rightTargetWeight = 1f;
                    leftTargetWeight  = 1f;
                    break;

                case VerbState.RewriteGuard:
                    rightTargetPos = guardPoseRight;
                    leftTargetPos  = guardPoseLeft;
                    rightTargetWeight = 1f;
                    leftTargetWeight  = 1f;
                    break;

                case VerbState.WindprintCushion:
                    rightTargetPos = windprintCushionRight;
                    leftTargetPos  = windprintCushionLeft;
                    rightTargetWeight = 0.7f; // softer blend for ambient mode
                    leftTargetWeight  = 0.7f;
                    break;

                case VerbState.WindprintGuard:
                    rightTargetPos = windprintGuardRight;
                    leftTargetPos  = windprintGuardLeft;
                    rightTargetWeight = 0.8f;
                    leftTargetWeight  = 0.8f;
                    break;

                default: // None
                    rightTargetWeight = 0f;
                    leftTargetWeight  = 0f;
                    break;
            }
        }

        void UpdateWeights()
        {
            // Right hand
            float rightSpeed = rightTargetWeight > rightWeight ? blendInSpeed : blendOutSpeed;
            rightWeight = Mathf.MoveTowards(rightWeight, rightTargetWeight, rightSpeed * Time.deltaTime);
            if (rightHandIK) rightHandIK.weight = rightWeight;

            // Left hand
            float leftSpeed = leftTargetWeight > leftWeight ? blendInSpeed : blendOutSpeed;
            leftWeight = Mathf.MoveTowards(leftWeight, leftTargetWeight, leftSpeed * Time.deltaTime);
            if (leftHandIK) leftHandIK.weight = leftWeight;
        }

        void ApplyTargetPositions()
        {
            if (rightHandTarget && rightWeight > 0.01f)
            {
                Vector3 worldPos = transform.TransformPoint(rightTargetPos);
                rightHandTarget.position = Vector3.Lerp(
                    rightHandTarget.position, worldPos,
                    poseLerpSpeed * Time.deltaTime);
            }

            if (leftHandTarget && leftWeight > 0.01f)
            {
                Vector3 worldPos = transform.TransformPoint(leftTargetPos);
                leftHandTarget.position = Vector3.Lerp(
                    leftHandTarget.position, worldPos,
                    poseLerpSpeed * Time.deltaTime);
            }
        }

        // ═════════════════════════════════════════════════════════
        //  PUBLIC API (called by TranslationVerbBridge / game logic)
        // ═════════════════════════════════════════════════════════

        /// <summary>Activate IK for Read Default gesture.</summary>
        public void BeginReadDefault()
        {
            activeVerb = VerbState.ReadDefault;
        }

        /// <summary>Activate IK for Rewrite Cushion gesture.</summary>
        public void BeginRewriteCushion()
        {
            activeVerb = VerbState.RewriteCushion;
        }

        /// <summary>Activate IK for Rewrite Guard gesture.</summary>
        public void BeginRewriteGuard()
        {
            activeVerb = VerbState.RewriteGuard;
        }

        /// <summary>Set ambient Windprint mode IK (persists until cleared).</summary>
        public void SetWindprintMode(bool isCushion)
        {
            activeVerb = isCushion ? VerbState.WindprintCushion : VerbState.WindprintGuard;
        }

        /// <summary>End any active verb/windprint IK.</summary>
        public void EndVerb()
        {
            activeVerb = VerbState.None;
        }

        /// <summary>End verb but keep windprint mode if it was active.</summary>
        public void EndVerbKeepWindprint()
        {
            if (activeVerb == VerbState.WindprintCushion ||
                activeVerb == VerbState.WindprintGuard)
                return; // keep windprint

            activeVerb = VerbState.None;
        }

        /// <summary>Current active verb state (for debug / UI).</summary>
        public VerbState CurrentVerb => activeVerb;
    }
}
