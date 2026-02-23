using UnityEngine;

namespace SFS.Animation
{
    /// <summary>
    /// Unified animation driver that feeds locomotion continuously and
    /// fires one-shot verbs on demand.
    ///
    /// Attach to the same GameObject as the Animator.
    /// PlayerController / PlayerMotor calls this — nothing else touches the Animator directly.
    ///
    /// Design contract:
    ///   • Locomotion: called every frame via SetLocomotion()
    ///   • Verbs/actions: called once via Play*() / Set*()
    ///   • Sensory state: called when defaults change via Set*()
    ///   • All parameters use AnimParams hashes — no string bugs
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class CharacterAnimationDriver : MonoBehaviour
    {
        [Header("References")]
        public Animator animator;

        [Header("Tuning")]
        [Tooltip("Smooth time for Speed parameter")]
        [Range(0.01f, 0.3f)]
        public float speedDamp = 0.1f;

        [Tooltip("Smooth time for VerticalVel parameter")]
        [Range(0.01f, 0.2f)]
        public float verticalDamp = 0.05f;

        [Tooltip("Smooth time for Overload/Calm parameters")]
        [Range(0.05f, 0.5f)]
        public float sensoryDamp = 0.15f;

        [Header("Sensory Scaling")]
        [Tooltip("When visual_clutter default is rewritten, scale animation intensity")]
        [Range(0f, 1f)]
        public float reducedMotionScale = 0.5f;

        // ── Internal state ──────────────────────────────────────
        bool wasGrounded;
        float currentOverload;
        float currentCalm;
        bool isValid;

        void Reset()
        {
            animator = GetComponent<Animator>();
        }

        void Awake()
        {
            if (!animator) animator = GetComponent<Animator>();
            isValid = animator != null && animator.runtimeAnimatorController != null;

            if (!isValid)
            {
                Debug.LogWarning(
                    $"[SFS] CharacterAnimationDriver on '{name}': " +
                    "Animator or RuntimeAnimatorController is missing. " +
                    "Animation will not play. Assign a controller in the Animator component.");
            }
        }

        // ═════════════════════════════════════════════════════════
        //  LOCOMOTION (called every frame)
        // ═════════════════════════════════════════════════════════

        /// <summary>
        /// Feed movement state to the Animator.
        /// Called every frame by PlayerMotor / PlayerController.
        /// </summary>
        /// <param name="speed01">Normalised speed (0 = idle, 1 = full run)</param>
        /// <param name="grounded">Is the character on the ground?</param>
        /// <param name="verticalVel">Y velocity (positive = rising, negative = falling)</param>
        public void SetLocomotion(float speed01, bool grounded, float verticalVel)
        {
            if (!isValid) return;

            animator.SetFloat(AnimParams.Speed, speed01, speedDamp, Time.deltaTime);
            animator.SetBool(AnimParams.Grounded, grounded);
            animator.SetFloat(AnimParams.VerticalVel, verticalVel, verticalDamp, Time.deltaTime);

            // Auto-detect landing
            if (grounded && !wasGrounded)
            {
                animator.ResetTrigger(AnimParams.Jump);
                animator.SetTrigger(AnimParams.Land);
            }

            wasGrounded = grounded;
        }

        // ═════════════════════════════════════════════════════════
        //  MOVEMENT ACTIONS (called once per action)
        // ═════════════════════════════════════════════════════════

        public void PlayJump()
        {
            if (!isValid) return;
            animator.ResetTrigger(AnimParams.Land);
            animator.SetTrigger(AnimParams.Jump);
        }

        public void PlayLand()
        {
            if (!isValid) return;
            animator.ResetTrigger(AnimParams.Jump);
            animator.SetTrigger(AnimParams.Land);
        }

        public void PlayDash()
        {
            if (!isValid) return;
            animator.SetTrigger(AnimParams.Dash);
        }

        public void PlayGrapple()
        {
            if (!isValid) return;
            animator.SetTrigger(AnimParams.Grapple);
        }

        public void SetGlide(bool on)
        {
            if (!isValid) return;
            animator.SetBool(AnimParams.Glide, on);
        }

        public void SetWallRun(bool on)
        {
            if (!isValid) return;
            animator.SetBool(AnimParams.WallRun, on);
        }

        // ═════════════════════════════════════════════════════════
        //  TRANSLATION VERBS (Read / Rewrite)
        // ═════════════════════════════════════════════════════════

        /// <summary>Player uses Read Default — subtle pause + focus gesture.</summary>
        public void PlayReadDefault()
        {
            if (!isValid) return;
            animator.SetTrigger(AnimParams.ReadDefault);
        }

        /// <summary>Player uses Rewrite Default via Cushion — open gesture + glyph.</summary>
        public void PlayRewriteCushion()
        {
            if (!isValid) return;
            animator.SetTrigger(AnimParams.RewriteCushion);
        }

        /// <summary>Player uses Rewrite Default via Guard — pinning gesture + structure.</summary>
        public void PlayRewriteGuard()
        {
            if (!isValid) return;
            animator.SetTrigger(AnimParams.RewriteGuard);
        }

        // ═════════════════════════════════════════════════════════
        //  WINDPRINT COST REACTIONS ("body truth")
        // ═════════════════════════════════════════════════════════

        /// <summary>Entropy Bleed cost: micro-stagger, gaze flicker, hand tremor.</summary>
        public void PlayEntropyBleed()
        {
            if (!isValid) return;
            animator.SetTrigger(AnimParams.EntropyBleed);
        }

        /// <summary>Route Lock cost: rigid stance, narrowed scanning.</summary>
        public void PlayRouteLock()
        {
            if (!isValid) return;
            animator.SetTrigger(AnimParams.RouteLock);
        }

        // ═════════════════════════════════════════════════════════
        //  SYMBOLIC COMBAT VERBS
        // ═════════════════════════════════════════════════════════

        public void PlayPulse()      { if (isValid) animator.SetTrigger(AnimParams.Pulse); }
        public void PlayThreadLash() { if (isValid) animator.SetTrigger(AnimParams.ThreadLash); }
        public void PlayEdgeClaim()  { if (isValid) animator.SetTrigger(AnimParams.EdgeClaim); }
        public void PlayRetune()     { if (isValid) animator.SetTrigger(AnimParams.Retune); }

        /// <summary>Radiant Hold is sustained — true to start, false to end.</summary>
        public void SetRadiantHold(bool on)
        {
            if (!isValid) return;
            animator.SetBool(AnimParams.RadiantHold, on);
        }

        // ═════════════════════════════════════════════════════════
        //  SENSORY / EMOTIONAL STATE
        // ═════════════════════════════════════════════════════════

        /// <summary>
        /// Set sensory overload level (drives fidget, stim, breathing intensity).
        /// 0 = calm, 1 = maximum overload.
        /// </summary>
        public void SetOverload(float overload01)
        {
            if (!isValid) return;
            currentOverload = Mathf.Lerp(currentOverload, overload01, sensoryDamp);
            animator.SetFloat(AnimParams.Overload, currentOverload);
        }

        /// <summary>
        /// Set calm level (drives relaxed idle, slower micro-movements).
        /// 0 = not calm, 1 = full rest.
        /// </summary>
        public void SetCalm(float calm01)
        {
            if (!isValid) return;
            currentCalm = Mathf.Lerp(currentCalm, calm01, sensoryDamp);
            animator.SetFloat(AnimParams.Calm, currentCalm);
        }

        /// <summary>Set emotion tone for blend tree selection.</summary>
        public void SetEmotionTone(EmotionalTone tone)
        {
            if (!isValid) return;
            animator.SetInteger(AnimParams.EmotionTone, (int)tone);
        }

        // ═════════════════════════════════════════════════════════
        //  CONTEXT
        // ═════════════════════════════════════════════════════════

        public void SetInRestZone(bool inRest)
        {
            if (!isValid) return;
            animator.SetBool(AnimParams.InRestZone, inRest);
        }

        public void SetWithCompanion(bool with)
        {
            if (!isValid) return;
            animator.SetBool(AnimParams.WithCompanion, with);
        }
    }
}
