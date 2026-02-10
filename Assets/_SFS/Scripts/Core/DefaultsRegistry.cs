using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SFS.Core
{
    /// <summary>
    /// Central registry of all rewritable defaults in the Society.
    ///
    /// THIS IS THE THEMATIC HEART OF THE CODEBASE:
    ///   "The Society failed because it imposed rigid defaults.
    ///    You restore it by rewriting them."
    ///
    /// Any system can query:
    ///   DefaultsRegistry.Instance.Get("timing_window")
    /// and automatically receive the current value for the game state.
    ///
    /// When the player Rewrites a default, the registry fires
    /// OnDefaultRead / OnDefaultRewritten so every listening system
    /// (audio, visuals, movement, combat) responds automatically.
    /// </summary>
    public class DefaultsRegistry : MonoBehaviour
    {
        public static DefaultsRegistry Instance { get; private set; }

        // ── Events ──────────────────────────────────────────────
        /// <summary>Fired when any default is Read. Passes the key.</summary>
        public static event Action<string> OnDefaultRead;

        /// <summary>Fired when any default is Rewritten. Passes the key.</summary>
        public static event Action<string> OnDefaultRewritten;

        /// <summary>Fired when any default value changes. Passes key + new value.</summary>
        public static event Action<string, float> OnDefaultValueChanged;

        // ── Storage ─────────────────────────────────────────────
        readonly Dictionary<string, Default> _defaults = new();

        // ── Lifecycle ───────────────────────────────────────────

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            RegisterAll();
        }

        // ═════════════════════════════════════════════════════════
        //  PUBLIC API — Systems call these
        // ═════════════════════════════════════════════════════════

        /// <summary>Get the current value of a default. Systems call this every frame or on-demand.</summary>
        public float Get(string key)
        {
            return _defaults.TryGetValue(key, out var d) ? d.CurrentValue : 0f;
        }

        /// <summary>Player uses Read Default on a specific default.</summary>
        public string Read(string key)
        {
            if (!_defaults.TryGetValue(key, out var d)) return null;
            string description = d.Read();
            OnDefaultRead?.Invoke(key);
            Debug.Log($"[SFS] Read Default: {key}");
            return description;
        }

        /// <summary>Player uses Rewrite Default on a specific default.</summary>
        public bool Rewrite(string key)
        {
            if (!_defaults.TryGetValue(key, out var d)) return false;
            float oldValue = d.CurrentValue;
            bool success = d.Rewrite();
            if (success)
            {
                OnDefaultRewritten?.Invoke(key);
                OnDefaultValueChanged?.Invoke(key, d.CurrentValue);
                Debug.Log($"[SFS] Rewrite Default: {key} ({oldValue} → {d.CurrentValue})");
            }
            return success;
        }

        /// <summary>Get the full Default object for UI / inspection.</summary>
        public Default GetDefault(string key)
        {
            return _defaults.TryGetValue(key, out var d) ? d : null;
        }

        /// <summary>Defaults the player can currently Read (not yet read).</summary>
        public List<Default> ListReadable()
        {
            return _defaults.Values.Where(d => !d.IsRead).ToList();
        }

        /// <summary>Defaults the player has Read but not yet Rewritten.</summary>
        public List<Default> ListRewritable()
        {
            return _defaults.Values.Where(d => d.IsRead && !d.IsRewritten).ToList();
        }

        /// <summary>Victory check: are all defaults rewritten?</summary>
        public bool AllRewritten()
        {
            return _defaults.Values.All(d => d.IsRewritten);
        }

        /// <summary>Fraction of defaults rewritten (0.0 → 1.0).</summary>
        public float Progress
        {
            get
            {
                int total = _defaults.Count;
                return total == 0 ? 0f : _defaults.Values.Count(d => d.IsRewritten) / (float)total;
            }
        }

        public int ReadCount => _defaults.Values.Count(d => d.IsRead);
        public int RewrittenCount => _defaults.Values.Count(d => d.IsRewritten);
        public int TotalCount => _defaults.Count;

        /// <summary>All defaults in a given category.</summary>
        public List<Default> ByCategory(DefaultCategory category)
        {
            return _defaults.Values.Where(d => d.Category == category).ToList();
        }

        /// <summary>Reset all defaults to rigid state (new game).</summary>
        public void ResetAll()
        {
            foreach (var d in _defaults.Values)
                d.Reset();
        }

        // ═════════════════════════════════════════════════════════
        //  REGISTRATION — All 15 defaults from the prototype
        // ═════════════════════════════════════════════════════════

        void Register(Default d) => _defaults[d.Key] = d;

        void RegisterAll()
        {
            // ── TIMING ──────────────────────────────────────────
            Register(new Default(
                "timing_window", "Timing Window Width",
                "Assumes all players react within 200 ms. Penalises slower processing speeds.",
                DefaultCategory.Timing,
                rigidValue: 0.2f,      // 200 ms — punishing
                rewrittenValue: 0.5f   // 500 ms — generous
            ));

            Register(new Default(
                "platform_rhythm", "Platform Rhythm",
                "Platforms move at one fixed tempo. No accommodation for observation before action.",
                DefaultCategory.Timing,
                rigidValue: 1.0f,
                rewrittenValue: 0.6f
            ));

            Register(new Default(
                "coyote_time", "Coyote Time",
                "Zero grace period after leaving a ledge. Assumes instant spatial awareness.",
                DefaultCategory.Timing,
                rigidValue: 0.0f,
                rewrittenValue: 0.2f
            ));

            Register(new Default(
                "jump_buffer", "Jump Buffer Window",
                "No input buffering. Requires frame-perfect timing.",
                DefaultCategory.Timing,
                rigidValue: 0.0f,
                rewrittenValue: 0.15f
            ));

            // ── SENSORY ─────────────────────────────────────────
            Register(new Default(
                "visual_clutter", "Visual Density",
                "All particle effects, decorations, and ambient motion rendered simultaneously. Assumes high sensory filtering.",
                DefaultCategory.Sensory,
                rigidValue: 1.0f,
                rewrittenValue: 0.4f
            ));

            Register(new Default(
                "audio_layering", "Audio Layering",
                "Multiple concurrent audio streams with no ducking. Assumes ability to parse layered sound.",
                DefaultCategory.Sensory,
                rigidValue: 1.0f,
                rewrittenValue: 0.5f
            ));

            Register(new Default(
                "screen_shake", "Screen Shake Intensity",
                "Full camera shake on impacts. Assumes vestibular comfort.",
                DefaultCategory.Sensory,
                rigidValue: 1.0f,
                rewrittenValue: 0.0f
            ));

            // ── ROUTING ─────────────────────────────────────────
            Register(new Default(
                "route_strictness", "Route Strictness",
                "Single valid path through each area. Penalises alternative approaches.",
                DefaultCategory.Routing,
                rigidValue: 1.0f,
                rewrittenValue: 0.3f
            ));

            Register(new Default(
                "safe_route_visibility", "Safe Route Visibility",
                "Accessible routes are hidden behind harder paths. Assumes safe routes are 'easy mode'.",
                DefaultCategory.Routing,
                rigidValue: 0.0f,
                rewrittenValue: 1.0f
            ));

            // ── SOCIAL ──────────────────────────────────────────
            Register(new Default(
                "communication_rigidity", "Communication Mode",
                "Only one expression style is accepted. Penalises non-verbal or icon-based communication.",
                DefaultCategory.Social,
                rigidValue: 1.0f,
                rewrittenValue: 0.0f
            ));

            Register(new Default(
                "social_script_penalty", "Social Script Penalty",
                "NPCs penalise 'unexpected' dialogue responses. Assumes one correct conversational flow.",
                DefaultCategory.Social,
                rigidValue: 1.0f,
                rewrittenValue: 0.0f
            ));

            // ── FAILURE ─────────────────────────────────────────
            Register(new Default(
                "failure_penalty", "Failure Penalty",
                "Falling or missing a jump resets significant progress. Assumes failure is deviation, not information.",
                DefaultCategory.Failure,
                rigidValue: 1.0f,
                rewrittenValue: 0.1f
            ));

            Register(new Default(
                "retry_cost", "Retry Cost",
                "Retrying a section costs resources. Assumes learning happens on the first attempt.",
                DefaultCategory.Failure,
                rigidValue: 1.0f,
                rewrittenValue: 0.0f
            ));

            // ── CONSENT ─────────────────────────────────────────
            Register(new Default(
                "consent_gates", "Consent Gates",
                "No confirmation before danger zones. Assumes willingness to proceed.",
                DefaultCategory.Consent,
                rigidValue: 0.0f,
                rewrittenValue: 1.0f
            ));

            Register(new Default(
                "opt_out_available", "Opt-Out Availability",
                "No way to leave an encounter once started. Assumes commitment is always free.",
                DefaultCategory.Consent,
                rigidValue: 0.0f,
                rewrittenValue: 1.0f
            ));

            Debug.Log($"[SFS] DefaultsRegistry loaded: {_defaults.Count} defaults across {System.Enum.GetValues(typeof(DefaultCategory)).Length} categories");
        }
    }
}
