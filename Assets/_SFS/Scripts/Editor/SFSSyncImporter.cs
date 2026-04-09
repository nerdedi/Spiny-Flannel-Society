using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SFS.Editor
{
    /// <summary>
    /// Watches Assets/_SFS/Resources/SyncData/sfs_prototype_data.json and
    /// automatically imports Python prototype data into Unity systems.
    ///
    /// The Python script (sync_to_unity.py) exports game data as JSON.
    /// This importer detects changes via AssetPostprocessor and updates
    /// DemoData ScriptableObjects + logs a summary to the console.
    ///
    /// Flow:  Python files → sync_to_unity.py → JSON → this importer → Unity data
    /// </summary>
    public class SFSSyncImporter : AssetPostprocessor
    {
        const string SyncDataPath = "Assets/_SFS/Resources/SyncData/sfs_prototype_data.json";

        /// <summary>
        /// Called automatically by Unity whenever assets are imported/changed.
        /// </summary>
        static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            foreach (string path in importedAssets)
            {
                if (path == SyncDataPath)
                {
                    Debug.Log("[SFS Sync] Detected sync data change — importing…");
                    ImportSyncData();
                    return;
                }
            }
        }

        /// <summary>
        /// Manual trigger from the menu bar.
        /// </summary>
        [MenuItem("SFS/Sync/Import Prototype Data", priority = 100)]
        public static void ImportSyncDataMenu()
        {
            ImportSyncData();
        }

        /// <summary>
        /// Launch the Python watcher in the background.
        /// </summary>
        [MenuItem("SFS/Sync/Start Python Watcher", priority = 101)]
        public static void StartPythonWatcher()
        {
            string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            string script = Path.Combine(projectRoot, "sync_to_unity.py");

            if (!File.Exists(script))
            {
                Debug.LogError($"[SFS Sync] sync_to_unity.py not found at: {script}");
                return;
            }

            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "python3",
                Arguments = $"\"{script}\" --watch",
                WorkingDirectory = projectRoot,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            try
            {
                System.Diagnostics.Process.Start(psi);
                Debug.Log("[SFS Sync] Python watcher started. It will auto-export when Python files change.");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SFS Sync] Failed to start watcher: {e.Message}");
            }
        }

        /// <summary>
        /// One-shot export from the menu.
        /// </summary>
        [MenuItem("SFS/Sync/Export Now (one-shot)", priority = 102)]
        public static void ExportNow()
        {
            string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            string script = Path.Combine(projectRoot, "sync_to_unity.py");

            if (!File.Exists(script))
            {
                Debug.LogError($"[SFS Sync] sync_to_unity.py not found at: {script}");
                return;
            }

            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "python3",
                Arguments = $"\"{script}\"",
                WorkingDirectory = projectRoot,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            try
            {
                var proc = System.Diagnostics.Process.Start(psi);
                string stdout = proc.StandardOutput.ReadToEnd();
                string stderr = proc.StandardError.ReadToEnd();
                proc.WaitForExit();

                if (proc.ExitCode == 0)
                {
                    Debug.Log($"[SFS Sync] Export succeeded:\n{stdout}");
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogError($"[SFS Sync] Export failed:\n{stderr}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[SFS Sync] Failed to run export: {e.Message}");
            }
        }

        // =====================================================================
        //  IMPORT LOGIC
        // =====================================================================

        static void ImportSyncData()
        {
            string fullPath = Path.GetFullPath(SyncDataPath);
            if (!File.Exists(fullPath))
            {
                Debug.LogWarning($"[SFS Sync] No sync data found at {SyncDataPath}. Run sync_to_unity.py first.");
                return;
            }

            string json = File.ReadAllText(fullPath);
            SyncPayload payload;

            try
            {
                payload = JsonUtility.FromJson<SyncPayload>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SFS Sync] Failed to parse JSON: {e.Message}");
                return;
            }

            if (payload == null)
            {
                Debug.LogError("[SFS Sync] Parsed payload is null.");
                return;
            }

            // Update DemoData ScriptableObject
            UpdateDemoData(payload);

            Debug.Log($"[SFS Sync] Import complete — " +
                      $"{payload.chapters?.Count ?? 0} chapters, " +
                      $"{payload.districts?.Count ?? 0} districts, " +
                      $"{payload.characters?.Count ?? 0} characters, " +
                      $"{payload.combatVerbs?.Count ?? 0} combat verbs, " +
                      $"{payload.civicRules?.Count ?? 0} civic rules " +
                      $"(v{payload._meta?.version ?? "?"})");
        }

        static void UpdateDemoData(SyncPayload payload)
        {
            // Find existing DemoData assets
            string[] guids = AssetDatabase.FindAssets("t:DemoData", new[] { "Assets/_SFS" });
            Core.DemoData demoData = null;

            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                demoData = AssetDatabase.LoadAssetAtPath<Core.DemoData>(path);
            }

            if (demoData == null)
            {
                Debug.LogWarning("[SFS Sync] No DemoData asset found. Create one via Assets > Create > SFS > Demo Data");
                return;
            }

            Undo.RecordObject(demoData, "SFS Sync Import");

            // Update metadata
            if (payload._meta != null)
            {
                demoData.GameTitle = payload._meta.gameTitle ?? demoData.GameTitle;
                demoData.Version = payload._meta.version ?? demoData.Version;
                demoData.TotalChapters = payload._meta.totalChapters;
            }

            // Update chapters
            if (payload.chapters != null && payload.chapters.Count > 0)
            {
                demoData.ChapterPreviews.Clear();
                foreach (var ch in payload.chapters)
                {
                    demoData.ChapterPreviews.Add(new Core.ChapterInfo
                    {
                        Number = ch.id,
                        Title = ch.name ?? "",
                        Summary = $"{ch.theme ?? ""} — {ch.civicRuleDescription ?? ""}",
                    });
                }
            }

            // Update districts
            if (payload.districts != null && payload.districts.Count > 0)
            {
                demoData.Districts.Clear();
                foreach (var d in payload.districts)
                {
                    demoData.Districts.Add(new Core.DistrictInfo
                    {
                        Name = d.name ?? "",
                        Description = d.description ?? "",
                        DriftLevel = 0.5f, // Default; could be refined
                    });
                }
            }

            // Update combat verbs
            if (payload.combatVerbs != null && payload.combatVerbs.Count > 0)
            {
                demoData.CombatVerbs.Clear();
                foreach (var v in payload.combatVerbs)
                {
                    demoData.CombatVerbs.Add(new Core.CombatVerb
                    {
                        Name = (v.id ?? "").ToUpper().Replace("_", " "),
                        Description = v.description ?? "",
                        Unlocked = false,
                    });
                }
            }

            EditorUtility.SetDirty(demoData);
            AssetDatabase.SaveAssets();
            Debug.Log("[SFS Sync] DemoData ScriptableObject updated.");
        }
    }

    // =========================================================================
    //  JSON payload types (mirrors sync_to_unity.py output)
    // =========================================================================
    //  JsonUtility requires concrete [Serializable] classes with public fields.
    //  Nested generics (Dictionary) are not supported, so we flatten where needed.
    // =========================================================================

    [Serializable]
    public class SyncPayload
    {
        public SyncMeta _meta;
        public List<SyncChapter> chapters;
        public List<SyncDistrict> districts;
        public List<SyncCharacter> characters;
        public List<SyncCombatVerb> combatVerbs;
        public List<SyncAntagonist> antagonists;
        public List<SyncCivicRule> civicRules;
        public SyncPlayerStats playerStats;
        public SyncDriftConfig driftConfig;
        public SyncRewriting rewriting;
    }

    [Serializable]
    public class SyncMeta
    {
        public string generator;
        public string gameTitle;
        public string version;
        public int totalChapters;
        public int totalDistricts;
        public string exportedAt;
    }

    [Serializable]
    public class SyncChapter
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
        public List<SyncDialogueLine> introDialogue;
    }

    [Serializable]
    public class SyncDialogueLine
    {
        public string speaker;
        public string text;
        public string iconVersion;
        public string minimalVersion;
        public string emotion;
    }

    [Serializable]
    public class SyncDistrict
    {
        public string id;
        public string name;
        public string description;
        public string colorTheme;
        public string windPattern;
    }

    [Serializable]
    public class SyncCharacter
    {
        public string id;
        public string name;
        public string role;
        public string description;
        public string voice;
        public List<string> introduces;
    }

    [Serializable]
    public class SyncCombatVerb
    {
        public string id;
        public int energyCost;
        public float cooldown;
        public float effectRadius;
        public float duration;
        public string description;
    }

    [Serializable]
    public class SyncAntagonist
    {
        public string id;
        public string name;
        public string description;
        public string resolutionVerb;
        public string secondaryVerb;
        public float baseIntensity;
    }

    [Serializable]
    public class SyncCivicRule
    {
        public string id;
        public string description;
    }

    [Serializable]
    public class SyncPlayerStats
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
    public class SyncDriftConfig
    {
        public float reductionPerChapter;
        public float min;
        public float max;
    }

    [Serializable]
    public class SyncRewriting
    {
        public int energyCost;
        public float cooldown;
        public float scanRadius;
        public float terminalInteractionRange;
    }
}
