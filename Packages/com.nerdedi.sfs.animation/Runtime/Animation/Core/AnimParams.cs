using UnityEngine;

namespace SFS.Animation
{
    /// <summary>
    /// Central hash table for all Animator parameter names.
    /// Prevents name/type mismatch bugs — the #1 cause of
    /// "parameters change but nothing animates."
    ///
    /// Usage:
    ///   animator.SetFloat(AnimParams.Speed, speed01);
    ///   animator.SetTrigger(AnimParams.Pulse);
    ///
    /// CONTRACT: Every parameter listed here must exist in the
    /// Animator Controller with the correct type (see comments).
    /// Run SFS > Animation > Setup Animator Parameters to auto-create them.
    /// </summary>
    public static class AnimParams
    {
        // ── Locomotion (continuous feeds) ────────────────────────
        public static readonly int Speed       = Animator.StringToHash("Speed");        // float 0-1
        public static readonly int Grounded    = Animator.StringToHash("Grounded");     // bool
        public static readonly int VerticalVel = Animator.StringToHash("VerticalVel");  // float
        public static readonly int Jump        = Animator.StringToHash("Jump");         // trigger
        public static readonly int Land        = Animator.StringToHash("Land");         // trigger
        public static readonly int Dash        = Animator.StringToHash("Dash");         // trigger
        public static readonly int Grapple     = Animator.StringToHash("Grapple");      // trigger
        public static readonly int Glide       = Animator.StringToHash("Glide");        // bool
        public static readonly int WallRun     = Animator.StringToHash("WallRun");      // bool

        // ── Translation Verbs (Read / Rewrite) ──────────────────
        public static readonly int ReadDefault    = Animator.StringToHash("ReadDefault");    // trigger
        public static readonly int RewriteCushion = Animator.StringToHash("RewriteCushion"); // trigger
        public static readonly int RewriteGuard   = Animator.StringToHash("RewriteGuard");  // trigger

        // ── Windprint Costs (body-truth reactions) ──────────────
        public static readonly int EntropyBleed = Animator.StringToHash("EntropyBleed"); // trigger
        public static readonly int RouteLock    = Animator.StringToHash("RouteLock");    // trigger

        // ── Symbolic Combat Verbs ───────────────────────────────
        public static readonly int Pulse       = Animator.StringToHash("Pulse");        // trigger
        public static readonly int ThreadLash  = Animator.StringToHash("ThreadLash");   // trigger
        public static readonly int RadiantHold = Animator.StringToHash("RadiantHold");  // bool (sustained)
        public static readonly int EdgeClaim   = Animator.StringToHash("EdgeClaim");    // trigger
        public static readonly int Retune      = Animator.StringToHash("Retune");       // trigger

        // ── Sensory / Emotional State ───────────────────────────
        public static readonly int Overload      = Animator.StringToHash("Overload");      // float 0-1
        public static readonly int Calm          = Animator.StringToHash("Calm");          // float 0-1
        public static readonly int IdleIntensity = Animator.StringToHash("IdleIntensity"); // float 0-1
        public static readonly int EmotionTone   = Animator.StringToHash("EmotionTone");  // int (enum)

        // ── Story / Context ─────────────────────────────────────
        public static readonly int InRestZone    = Animator.StringToHash("InRestZone");    // bool
        public static readonly int WithCompanion = Animator.StringToHash("WithCompanion"); // bool
    }
}
