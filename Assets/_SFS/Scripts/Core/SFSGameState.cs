using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SFS.Core
{
    /// <summary>
    /// Macro game state: chapter progress, drift intensity, phase tracking.
    /// Ported from the Python prototype's GameState dataclass.
    ///
    /// Singleton MonoBehaviour — lives on the same GameObject as DefaultsRegistry.
    /// </summary>
    public class SFSGameState : MonoBehaviour
    {
        public static SFSGameState Instance { get; private set; }

        // ── Events ──────────────────────────────────────────────
        public static event Action<int> OnChapterAdvanced;
        public static event Action<Phase> OnPhaseChanged;
        public static event Action<float> OnDriftChanged;
        public static event Action<string> OnCivicRuleRestored;

        // ── Phase enum ──────────────────────────────────────────
        public enum Phase
        {
            AxiomActive,         // Before the game's timeline
            StandardDefaults,    // The crisis state
            TheDrift,            // Active corruption — game begins here
            AxiomRestoring,      // Player is making progress
            PluralCoherence      // Victory
        }

        // ── State ───────────────────────────────────────────────
        [Header("Narrative")]
        public int CurrentChapter = 1;
        public int TotalChapters = 12;
        public Phase CurrentPhase = Phase.TheDrift;

        [Header("Drift")]
        [Range(0f, 1f)]
        public float DriftIntensity = 1.0f;

        [Header("Progress")]
        public List<string> CivicRulesRestored = new();
        public List<string> ElectivesCompleted = new();
        public List<string> DistrictsVisited = new();

        [Header("Windprint Record")]
        public int CushionUses;
        public int GuardUses;

        // ── Constants ───────────────────────────────────────────
        const float DRIFT_REDUCTION_PER_CHAPTER = 1f / 12f;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        void Start()
        {
            // Listen to defaults being rewritten to auto-reduce drift
            DefaultsRegistry.OnDefaultRewritten += OnDefaultRewritten;
        }

        void OnDestroy()
        {
            DefaultsRegistry.OnDefaultRewritten -= OnDefaultRewritten;
        }

        // ═════════════════════════════════════════════════════════
        //  PUBLIC API
        // ═════════════════════════════════════════════════════════

        /// <summary>Advance to the next chapter. Returns false if at the end.</summary>
        public bool AdvanceChapter()
        {
            if (CurrentChapter >= TotalChapters) return false;
            CurrentChapter++;
            ReduceDrift();
            UpdatePhase();
            OnChapterAdvanced?.Invoke(CurrentChapter);
            Debug.Log($"[SFS] Chapter {CurrentChapter}: drift={DriftIntensity:F2}");
            return true;
        }

        /// <summary>Mark a civic rule as restored.</summary>
        public void RestoreCivicRule(string ruleId)
        {
            if (CivicRulesRestored.Contains(ruleId)) return;
            CivicRulesRestored.Add(ruleId);
            ReduceDrift();
            OnCivicRuleRestored?.Invoke(ruleId);
        }

        /// <summary>Record a windprint mode use.</summary>
        public void RecordWindprintUse(bool isCushion)
        {
            if (isCushion) CushionUses++;
            else GuardUses++;
        }

        /// <summary>Record visiting a district.</summary>
        public void VisitDistrict(string districtId)
        {
            if (!DistrictsVisited.Contains(districtId))
                DistrictsVisited.Add(districtId);
        }

        /// <summary>Fraction of chapters completed (0-1).</summary>
        public float ProgressFraction => (CurrentChapter - 1f) / TotalChapters;

        /// <summary>Has the player achieved Plural Coherence?</summary>
        public bool IsVictory => CurrentPhase == Phase.PluralCoherence;

        /// <summary>Player's preferred windprint mode based on usage.</summary>
        public string PreferredMode => CushionUses >= GuardUses ? "cushion" : "guard";

        /// <summary>Reset for new game.</summary>
        public void ResetState()
        {
            CurrentChapter = 1;
            CurrentPhase = Phase.TheDrift;
            DriftIntensity = 1.0f;
            CivicRulesRestored.Clear();
            ElectivesCompleted.Clear();
            DistrictsVisited.Clear();
            CushionUses = 0;
            GuardUses = 0;
        }

        // ── Internal ────────────────────────────────────────────

        void OnDefaultRewritten(string key)
        {
            ReduceDrift();
        }

        void ReduceDrift()
        {
            float old = DriftIntensity;
            DriftIntensity = Mathf.Max(0f, DriftIntensity - DRIFT_REDUCTION_PER_CHAPTER);
            UpdatePhase();
            if (!Mathf.Approximately(old, DriftIntensity))
                OnDriftChanged?.Invoke(DriftIntensity);
        }

        void UpdatePhase()
        {
            Phase newPhase;
            if (DriftIntensity <= 0f)
                newPhase = Phase.PluralCoherence;
            else if (DriftIntensity < 0.5f)
                newPhase = Phase.AxiomRestoring;
            else
                newPhase = Phase.TheDrift;

            if (newPhase != CurrentPhase)
            {
                CurrentPhase = newPhase;
                OnPhaseChanged?.Invoke(CurrentPhase);
                Debug.Log($"[SFS] Phase changed: {CurrentPhase}");
            }
        }
    }
}
