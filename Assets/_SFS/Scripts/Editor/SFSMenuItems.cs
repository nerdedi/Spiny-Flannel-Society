#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace SFS.Editor
{
    /// <summary>
    /// Main menu items for Spiny Flannel Society setup and utilities.
    /// </summary>
    public static class SFSMenuItems
    {
        [MenuItem("SFS/Setup/Create All ScriptableObject Folders")]
        public static void CreateSOFolders()
        {
            string[] folders = {
                "Assets/_SFS/ScriptableObjects/Audio",
                "Assets/_SFS/ScriptableObjects/Config",
                "Assets/_SFS/ScriptableObjects/Narrative"
            };

            foreach (var folder in folders)
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }

            AssetDatabase.Refresh();
            Debug.Log("[SFS] Created ScriptableObject folders");
        }

        [MenuItem("SFS/Setup/Full Project Setup", priority = 0)]
        public static void FullProjectSetup()
        {
            Debug.Log("[SFS] === Starting Full Project Setup ===");

            CreateSOFolders();
            AnimatorSetupWizard.CreatePlayerAnimator();
            AnimatorSetupWizard.CreateNPCAnimator();
            TimelineTemplateCreator.CreateAllTimelines();
            TimelineTemplateCreator.CreateDirectorPrefab();

            Debug.Log("[SFS] === Full Project Setup Complete ===");
            Debug.Log("[SFS] Next: Run 'SFS/Setup/Build Test Scene' to create a playable test level");
        }

        [MenuItem("SFS/Documentation/Open Story Beats Reference")]
        public static void OpenStoryBeatsReference()
        {
            string content = @"# Spiny Flannel Society - Story Beats Reference

## Emotional Curve
Gentle → Melancholic → Gentle → Hopeful → Melancholic → Hopeful → Grounded

---

## Beat 1: Soft Arrival (Gentle)
**Purpose:** Welcome the player without pressure.
**Camera:** Pulls back (distance = uncertainty)
**Audio:** Present but not comforting yet
**Animation:** Subtle idle (breathing, shifting weight)
**Message:** ""You're allowed to take up space here.""

---

## Beat 2: First Contact (Hopeful)
**Purpose:** Show that interaction is safe and rewarding.
**Camera:** Neutral, engaged
**Audio:** Gentle affirming cue (not celebratory)
**Animation:** Soft pickup response
**Message:** ""The world responds when you reach out.""

---

## Beat 3: Compression (Melancholic)
**Purpose:** Introduce friction without punishment.
**Camera:** Closer (tension)
**Audio:** Layers stack
**Animation:** Faster idle, heavier landing
**Message:** ""This isn't failure — this is strain.""

---

## Beat 4: Choosing Rest (Gentle)
**Purpose:** Reframe pause and withdrawal as agency.
**Camera:** Stabilizes
**Audio:** Softens or drops out
**Animation:** Visibly slows
**Message:** ""Stepping back is an action, not avoidance.""

---

## Beat 5: The Society (Hopeful + Tender)
**Purpose:** Introduce community without demand.
**Camera:** Frames player within group
**Audio:** Warm, communal
**Animation:** Synchronized micro-movements
**Message:** ""You don't have to explain yourself here.""

---

## Beat 6: Shared Difficulty (Melancholic → Hopeful)
**Purpose:** Show that support changes the experience.
**Camera:** Active but supported framing
**Audio:** Maintained presence
**Animation:** Shared rhythm with companion
**Message:** ""The world doesn't get easier — you don't face it alone.""

---

## Beat 7: Quiet Belonging (Grounded Hope)
**Purpose:** End with stability, not spectacle.
**Camera:** Slowly widens, then holds
**Audio:** Calmer than at start
**Animation:** Group idle loop, player at rest
**Message:** ""You are allowed to remain.""

---

## Animator Parameters

| Parameter | Type | Usage |
|-----------|------|-------|
| Speed | Float | 0-1 locomotion blend |
| Grounded | Bool | Ground detection |
| YVelocity | Float | Jump/fall |
| IdleIntensity | Float | Beat-driven variation |
| EmotionTone | Int | 0=Gentle, 1=Hopeful, 2=Melancholic, 3=Grounded |
| InRestZone | Bool | Slowed idle state |
| WithCompanion | Bool | Companioned variant |
| Jump/Land/Die | Trigger | Actions |

---

## Required Animation Clips

### Player
- Idle_Breathe
- Idle_Shift
- Idle_LookAround
- Walk
- Run
- Jump_Start
- Jump_Loop
- Land_Soft
- Land_Heavy
- Rest_Idle (slower variant)

### NPCs
- Idle_Breathe
- Idle_Shift
- Idle_LookAround
- Acknowledged (subtle head turn)
- BelongingIdle (final group loop)
- SyncedSway (shared micro-motion)

### Companion
- Walk_Sync
- Idle_Present
";

            string path = "Assets/_SFS/Documentation/StoryBeatsReference.md";
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            File.WriteAllText(path, content);
            AssetDatabase.Refresh();

            var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
        }

        [MenuItem("SFS/Documentation/Open Scene Setup Guide")]
        public static void OpenSceneSetupGuide()
        {
            string content = @"# Spiny Flannel Society - Scene Setup Guide

## Hierarchy Structure

```
--- GAME SYSTEMS ---
├── SettingsManager (singleton, DontDestroyOnLoad)
├── StoryBeatManager (tracks current beat)
├── RespawnSystem (handles death/respawn)
└── StoryBeatAudioController
    ├── BaseAmbient (AudioSource)
    ├── TensionLayer (AudioSource)
    ├── CalmLayer (AudioSource)
    └── SocietyLayer (AudioSource)

Player (Tag: Player)
├── CharacterController
├── PlayerController
├── CollectibleTracker
├── PlayerAnimatorDriver
├── FootstepAudio
└── Visual (with Animator + PlayerAnimator controller)

CameraRig
├── Main Camera
├── ThirdPersonCameraRig
└── StoryBeatCameraModifier

--- WORLD ---
├── Ground
├── Beat1_Arrival (StoryBeatZone)
│   └── ArrivalIntro
├── FirstCollectible (FirstCollectibleFeedback)
├── Beat2_FirstContact (StoryBeatZone)
├── Beat3_Compression (StoryBeatZone)
│   └── CompressionZone
├── Beat4_Rest (StoryBeatZone)
│   ├── RestZone
│   └── Checkpoint
├── Beat5_Society (StoryBeatZone)
│   └── SocietyGroup
│       ├── SocietyMember_0
│       ├── SocietyMember_1
│       └── ...
├── Companion (starts inactive)
├── Beat6_SharedDifficulty (StoryBeatZone)
│   └── SupportedMovementZone
├── Beat7_Belonging (StoryBeatZone)
│   └── BelongingEnding
└── Collectibles...

Canvas
├── CollectibleCounterUI
└── PauseMenu (PauseManager)
```

## Zone Setup

### StoryBeatZone
- Add BoxCollider (Is Trigger = true)
- Set Beat enum
- Optional: assign entry cutscene PlayableDirector

### RestZone
- Fades ambient audio
- Dims lights (optional)
- Triggers InRestZone on animator

### CompressionZone
- Adds ambient layers
- Activates particles
- Intensifies lights

### SupportedMovementZone
- Only active with companion
- Adds timing forgiveness
- Shows path indicators

## Timeline Bindings

For each Timeline:
1. Create PlayableDirector in scene
2. Assign Timeline asset
3. Bind tracks:
   - Animation tracks → Character Animators
   - Audio tracks → AudioSources
   - (If using Cinemachine) Camera tracks → Virtual Cameras

## Testing Flow

1. Play scene
2. Walk forward through zones
3. Verify:
   - [ ] Camera distance changes per beat
   - [ ] Audio layers fade appropriately
   - [ ] Idle animation intensity varies
   - [ ] Rest zone calms everything
   - [ ] Society members acknowledge player
   - [ ] Companion appears at Beat 6
   - [ ] Ending holds gracefully
";

            string path = "Assets/_SFS/Documentation/SceneSetupGuide.md";
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            File.WriteAllText(path, content);
            AssetDatabase.Refresh();

            var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
        }

        [MenuItem("SFS/Utility/Log Current Story Beat")]
        public static void LogCurrentBeat()
        {
            if (!Application.isPlaying)
            {
                Debug.Log("[SFS] Enter Play mode to check story beat");
                return;
            }

            var manager = Object.FindObjectOfType<Core.StoryBeatManager>();
            if (manager)
            {
                Debug.Log($"[SFS] Current Beat: {manager.CurrentBeat} | Tone: {manager.CurrentTone} | Idle Intensity: {manager.CurrentIdleIntensity}");
            }
            else
            {
                Debug.LogWarning("[SFS] No StoryBeatManager found in scene");
            }
        }

        [MenuItem("SFS/Utility/Force Beat Transition")]
        public static void OpenBeatTransitionWindow()
        {
            BeatTransitionWindow.ShowWindow();
        }
    }

    public class BeatTransitionWindow : EditorWindow
    {
        public static void ShowWindow()
        {
            GetWindow<BeatTransitionWindow>("Force Beat Transition");
        }

        void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Enter Play mode to force beat transitions", MessageType.Info);
                return;
            }

            var manager = FindObjectOfType<Core.StoryBeatManager>();
            if (!manager)
            {
                EditorGUILayout.HelpBox("No StoryBeatManager in scene", MessageType.Warning);
                return;
            }

            EditorGUILayout.LabelField("Current Beat:", manager.CurrentBeat.ToString());
            EditorGUILayout.LabelField("Current Tone:", manager.CurrentTone.ToString());
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Force Transition To:");

            foreach (Core.StoryBeat beat in System.Enum.GetValues(typeof(Core.StoryBeat)))
            {
                if (GUILayout.Button(beat.ToString()))
                {
                    manager.TransitionTo(beat);
                }
            }
        }
    }
}
#endif
