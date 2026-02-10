using UnityEngine;
using SFS.Animation;

namespace SFS.Player
{
    /// <summary>
    /// Bridges the Translation Verb system (Read Default / Rewrite Default)
    /// to the animation system.
    ///
    /// HOW IT WORKS:
    ///   1. Game logic calls BeginRead() or BeginRewrite()
    ///   2. This fires the Animator trigger
    ///   3. The animation clip calls OnReadReveal() / OnRewriteCommit()
    ///      via Animation Events at the exact right frame
    ///   4. The world changes on that frame
    ///
    /// This ensures the visual gesture and the mechanical effect are
    /// perfectly synchronised — the rewrite "lands" when the hand gesture lands.
    ///
    /// WIRING:
    ///   - Attach to the player GameObject (same as CharacterAnimationDriver)
    ///   - Drag references in Inspector
    ///   - In your ReadDefault / RewriteDefault animation clips, add
    ///     Animation Events calling OnReadReveal() / OnRewriteCommit()
    /// </summary>
    public class TranslationVerbBridge : MonoBehaviour
    {
        [Header("Animation")]
        public CharacterAnimationDriver animDriver;

        [Header("Verb System (assign your game logic scripts)")]
        [Tooltip("The MonoBehaviour that holds your DefaultsRegistry reference")]
        public MonoBehaviour defaultsHolder;

        // Internal state for pending operations
        string pendingDefaultKey;
        string pendingRewriteMode; // "cushion" or "guard"

        void Start()
        {
            if (!animDriver)
                animDriver = GetComponent<CharacterAnimationDriver>();
        }

        // ═════════════════════════════════════════════════════════
        //  CALLED BY GAME LOGIC (when player activates a verb)
        // ═════════════════════════════════════════════════════════

        /// <summary>
        /// Player activates Read Default on a world element.
        /// Fires the animation; the world reveal happens on the
        /// Animation Event callback.
        /// </summary>
        public void BeginRead(string defaultKey)
        {
            pendingDefaultKey = defaultKey;
            animDriver?.PlayReadDefault();

            // Fire the animation event system
            AnimationEvents.PlayerActionTriggered(PlayerAction.ReadDefault);
        }

        /// <summary>
        /// Player activates Rewrite Default via Cushion.
        /// The actual registry change happens on OnRewriteCommit().
        /// </summary>
        public void BeginRewriteCushion(string defaultKey)
        {
            pendingDefaultKey = defaultKey;
            pendingRewriteMode = "cushion";
            animDriver?.PlayRewriteCushion();

            AnimationEvents.PlayerActionTriggered(PlayerAction.RewriteCushion);
        }

        /// <summary>
        /// Player activates Rewrite Default via Guard.
        /// The actual registry change happens on OnRewriteCommit().
        /// </summary>
        public void BeginRewriteGuard(string defaultKey)
        {
            pendingDefaultKey = defaultKey;
            pendingRewriteMode = "guard";
            animDriver?.PlayRewriteGuard();

            AnimationEvents.PlayerActionTriggered(PlayerAction.RewriteGuard);
        }

        // ═════════════════════════════════════════════════════════
        //  ANIMATION EVENT CALLBACKS
        //  (called from Animation Clips at the exact gesture frame)
        // ═════════════════════════════════════════════════════════

        /// <summary>
        /// Called by Animation Event at the moment the Read gesture "focuses."
        /// This is when the world reveals the default's assumption.
        /// </summary>
        public void OnReadReveal()
        {
            if (string.IsNullOrEmpty(pendingDefaultKey)) return;

            Debug.Log($"[SFS] Read Default revealed: {pendingDefaultKey}");

            // TODO: Call your DefaultsRegistry.Read(pendingDefaultKey)
            // Example:
            // var registry = FindObjectOfType<DefaultsRegistryBehaviour>();
            // string description = registry.Read(pendingDefaultKey);
            // UIManager.ShowDefaultOverlay(description);

            // Fire event for other systems (particles, audio, UI)
            AnimationEvents.ReadDefaultRevealed(pendingDefaultKey);
        }

        /// <summary>
        /// Called by Animation Event at the moment the Rewrite gesture "commits."
        /// This is when the registry value actually changes.
        /// </summary>
        public void OnRewriteCommit()
        {
            if (string.IsNullOrEmpty(pendingDefaultKey)) return;

            Debug.Log($"[SFS] Rewrite committed: {pendingDefaultKey} via {pendingRewriteMode}");

            // TODO: Call your DefaultsRegistry.Rewrite(pendingDefaultKey)
            // Example:
            // var registry = FindObjectOfType<DefaultsRegistryBehaviour>();
            // bool success = registry.Rewrite(pendingDefaultKey);

            // Fire event for other systems (VFX, audio, world update)
            AnimationEvents.RewriteDefaultCommitted(pendingDefaultKey, pendingRewriteMode);

            // Fire Windprint cost animation
            if (pendingRewriteMode == "cushion")
                animDriver?.PlayEntropyBleed();
            else if (pendingRewriteMode == "guard")
                animDriver?.PlayRouteLock();

            // Clear pending state
            pendingDefaultKey = null;
            pendingRewriteMode = null;
        }

        // ═════════════════════════════════════════════════════════
        //  SYMBOLIC COMBAT VERB TRIGGERS
        //  (convenience methods for encounter system)
        // ═════════════════════════════════════════════════════════

        public void UsePulse()      => animDriver?.PlayPulse();
        public void UseThreadLash() => animDriver?.PlayThreadLash();
        public void UseEdgeClaim()  => animDriver?.PlayEdgeClaim();
        public void UseRetune()     => animDriver?.PlayRetune();

        public void BeginRadiantHold() => animDriver?.SetRadiantHold(true);
        public void EndRadiantHold()   => animDriver?.SetRadiantHold(false);
    }
}
