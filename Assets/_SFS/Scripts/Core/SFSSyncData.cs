using System;
using System.Collections.Generic;
using UnityEngine;

namespace SFS.Core
{
    /// <summary>
    /// Runtime access to all synced prototype data.
    /// Loads Assets/_SFS/Resources/SyncData/sfs_prototype_data.json via Resources.Load.
    ///
    /// Usage:
    ///   var chapter = SFSSyncData.Instance.GetChapter(3);
    ///   var verb = SFSSyncData.Instance.GetCombatVerb("pulse");
    /// </summary>
    public class SFSSyncData : MonoBehaviour
    {
        public static SFSSyncData Instance { get; private set; }

        [Header("Status")]
        [SerializeField] bool _isLoaded;
        [SerializeField] string _version;
        [SerializeField] string _exportedAt;

        // Parsed data (public read-only access)
        public List<SyncedChapter> Chapters { get; private set; } = new();
        public List<SyncedDistrict> Districts { get; private set; } = new();
        public List<SyncedCharacter> Characters { get; private set; } = new();
        public List<SyncedCombatVerb> CombatVerbs { get; private set; } = new();
        public List<SyncedAntagonist> Antagonists { get; private set; } = new();
        public List<SyncedCivicRule> CivicRules { get; private set; } = new();
        public SyncedPlayerStats PlayerStats { get; private set; }
        public SyncedDriftConfig DriftConfig { get; private set; }

        public bool IsLoaded => _isLoaded;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSyncData();
        }

        /// <summary>Reload from disk (e.g. after a new export in Play mode).</summary>
        public void Reload() => LoadSyncData();

        // ── Lookup helpers ──────────────────────────────────────

        public SyncedChapter GetChapter(int id) =>
            Chapters.Find(c => c.id == id);

        public SyncedDistrict GetDistrict(string id) =>
            Districts.Find(d => d.id == id);

        public SyncedCharacter GetCharacter(string id) =>
            Characters.Find(c => c.id == id);

        public SyncedCombatVerb GetCombatVerb(string id) =>
            CombatVerbs.Find(v => v.id == id);

        public SyncedCivicRule GetCivicRule(string id) =>
            CivicRules.Find(r => r.id == id);

        // ── Loading ─────────────────────────────────────────────

        void LoadSyncData()
        {
            var asset = Resources.Load<TextAsset>("SyncData/sfs_prototype_data");
            if (asset == null)
            {
                Debug.LogWarning("[SFS SyncData] No sync data found. Run: python3 sync_to_unity.py");
                return;
            }

            try
            {
                var payload = JsonUtility.FromJson<SyncPayloadRuntime>(asset.text);
                if (payload == null)
                {
                    Debug.LogError("[SFS SyncData] Failed to parse sync JSON.");
                    return;
                }

                Chapters = payload.chapters ?? new();
                Districts = payload.districts ?? new();
                Characters = payload.characters ?? new();
                CombatVerbs = payload.combatVerbs ?? new();
                Antagonists = payload.antagonists ?? new();
                CivicRules = payload.civicRules ?? new();
                PlayerStats = payload.playerStats;
                DriftConfig = payload.driftConfig;

                _version = payload._meta?.version ?? "?";
                _exportedAt = payload._meta?.exportedAt ?? "?";
                _isLoaded = true;

                Debug.Log($"[SFS SyncData] Loaded v{_version} — " +
                          $"{Chapters.Count} chapters, {Districts.Count} districts, " +
                          $"{CombatVerbs.Count} verbs (exported {_exportedAt})");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SFS SyncData] Parse error: {e.Message}");
            }
        }
    }

    // =====================================================================
    //  Runtime JSON types (same shape as editor, separate for clarity)
    // =====================================================================

    [Serializable]
    public class SyncPayloadRuntime
    {
        public SyncMetaRuntime _meta;
        public List<SyncedChapter> chapters;
        public List<SyncedDistrict> districts;
        public List<SyncedCharacter> characters;
        public List<SyncedCombatVerb> combatVerbs;
        public List<SyncedAntagonist> antagonists;
        public List<SyncedCivicRule> civicRules;
        public SyncedPlayerStats playerStats;
        public SyncedDriftConfig driftConfig;
    }

    [Serializable]
    public class SyncMetaRuntime
    {
        public string generator;
        public string gameTitle;
        public string version;
        public int totalChapters;
        public int totalDistricts;
        public string exportedAt;
    }

    [Serializable]
    public class SyncedChapter
    {
        public int id;
        public string name;
        public string location;
        public string locationName;
        public string theme;
        public string civicRule;
        public string civicRuleDescription;
        public string primaryMechanic;
        public List<string> npcs;
        public List<SyncedDialogueLine> introDialogue;
    }

    [Serializable]
    public class SyncedDialogueLine
    {
        public string speaker;
        public string text;
        public string iconVersion;
        public string minimalVersion;
        public string emotion;
    }

    [Serializable]
    public class SyncedDistrict
    {
        public string id;
        public string name;
        public string description;
        public string colorTheme;
        public string windPattern;
    }

    [Serializable]
    public class SyncedCharacter
    {
        public string id;
        public string name;
        public string role;
        public string description;
        public string voice;
        public List<string> introduces;
    }

    [Serializable]
    public class SyncedCombatVerb
    {
        public string id;
        public int energyCost;
        public float cooldown;
        public float effectRadius;
        public float duration;
        public string description;
    }

    [Serializable]
    public class SyncedAntagonist
    {
        public string id;
        public string name;
        public string description;
        public string resolutionVerb;
        public string secondaryVerb;
        public float baseIntensity;
    }

    [Serializable]
    public class SyncedCivicRule
    {
        public string id;
        public string description;
    }

    [Serializable]
    public class SyncedPlayerStats
    {
        public float speed;
        public float jumpHeight;
        public float wallRunSpeed;
        public float airDashSpeed;
        public float airDashDistance;
        public float glideSpeed;
        public float grappleSpeed;
        public List<float> tripleHopHeights;
        public float tripleHopWindow;
        public float coyoteTime;
        public float jumpBufferTime;
    }

    [Serializable]
    public class SyncedDriftConfig
    {
        public float reductionPerChapter;
        public float min;
        public float max;
    }
}
