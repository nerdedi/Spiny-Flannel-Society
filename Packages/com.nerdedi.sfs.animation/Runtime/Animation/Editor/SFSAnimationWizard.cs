using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace SFS.Animation.Editor
{
    /// <summary>
    /// One-click animation package setup wizard for Spiny Flannel Society.
    /// Creates prefabs, configures animators, and sets up the complete animation system.
    /// </summary>
    public class SFSAnimationWizard : EditorWindow
    {
        #region Window Setup

        [MenuItem("SFS/Animation/Setup Wizard")]
        public static void ShowWindow()
        {
            var window = GetWindow<SFSAnimationWizard>("SFS Animation Setup");
            window.minSize = new Vector2(450, 600);
        }

        #endregion

        #region State

        Vector2 scrollPosition;
        bool setupComplete;
        List<string> setupLog = new List<string>();

        // Setup options
        bool createAnimationPrefabs = true;
        bool createVFXManager = true;
        bool configurePlayer = true;
        bool configureNPCs = true;
        bool createArchitecturePrefabs = true;
        bool createPatternPrefabs = true;
        bool setupDemoScene = true;

        // Asset paths
        const string PREFABS_PATH = "Assets/_SFS/Prefabs";
        const string ANIMATION_PREFABS_PATH = "Assets/_SFS/Prefabs/Animation";
        const string CHARACTER_PREFABS_PATH = "Assets/_SFS/Prefabs/Characters";
        const string ENVIRONMENT_PREFABS_PATH = "Assets/_SFS/Prefabs/Environment";
        const string VFX_PREFABS_PATH = "Assets/_SFS/Prefabs/VFX";

        #endregion

        #region GUI

        void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            DrawHeader();
            EditorGUILayout.Space(10);

            if (!setupComplete)
            {
                DrawSetupOptions();
                EditorGUILayout.Space(20);
                DrawSetupButton();
            }
            else
            {
                DrawSetupLog();
                EditorGUILayout.Space(10);
                DrawResetButton();
            }

            EditorGUILayout.Space(20);
            DrawQuickActions();

            EditorGUILayout.EndScrollView();
        }

        void DrawHeader()
        {
            EditorGUILayout.LabelField("═══════════════════════════════════════", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("    SPINY FLANNEL SOCIETY", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("    Animation Package Setup Wizard", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("═══════════════════════════════════════", EditorStyles.boldLabel);

            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox(
                "This wizard sets up the complete animation package for your project.\n\n" +
                "• Creates prefabs for all characters and systems\n" +
                "• Configures VFX Manager and event system\n" +
                "• Sets up living architecture templates\n" +
                "• Optionally creates a demo scene\n\n" +
                "All existing prefabs will be preserved. Select options below.",
                MessageType.Info);
        }

        void DrawSetupOptions()
        {
            EditorGUILayout.LabelField("Setup Options", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            using (new EditorGUILayout.VerticalScope("box"))
            {
                createAnimationPrefabs = EditorGUILayout.ToggleLeft(
                    new GUIContent("Create Core Animation Prefabs",
                        "Creates VFX Manager, Event Coordinator, and Animation State Machine prefabs"),
                    createAnimationPrefabs);

                createVFXManager = EditorGUILayout.ToggleLeft(
                    new GUIContent("Setup VFX Manager",
                        "Creates and configures the VFX Manager singleton"),
                    createVFXManager);

                configurePlayer = EditorGUILayout.ToggleLeft(
                    new GUIContent("Configure Player Animation",
                        "Adds TranslatorAnimationController to Player objects"),
                    configurePlayer);

                configureNPCs = EditorGUILayout.ToggleLeft(
                    new GUIContent("Configure NPC Animations",
                        "Creates prefabs for DAZIE, June, Winton, and companion NPCs"),
                    configureNPCs);

                createArchitecturePrefabs = EditorGUILayout.ToggleLeft(
                    new GUIContent("Create Architecture Prefabs",
                        "Creates living architecture prefabs: ramps, platforms, bridges, etc."),
                    createArchitecturePrefabs);

                createPatternPrefabs = EditorGUILayout.ToggleLeft(
                    new GUIContent("Create Pattern Prefabs",
                        "Creates antagonistic pattern prefabs: Echo Form, Distortion, Noise Beast"),
                    createPatternPrefabs);

                EditorGUILayout.Space(5);

                setupDemoScene = EditorGUILayout.ToggleLeft(
                    new GUIContent("Setup Demo Scene",
                        "Creates a demo scene with all animation systems configured"),
                    setupDemoScene);
            }
        }

        void DrawSetupButton()
        {
            EditorGUILayout.Space(10);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Run Setup Wizard", GUILayout.Height(40), GUILayout.Width(200)))
                {
                    RunSetup();
                }

                GUILayout.FlexibleSpace();
            }
        }

        void DrawSetupLog()
        {
            EditorGUILayout.LabelField("Setup Complete!", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            using (new EditorGUILayout.VerticalScope("box"))
            {
                foreach (var log in setupLog)
                {
                    EditorGUILayout.LabelField(log);
                }
            }
        }

        void DrawResetButton()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Reset Wizard", GUILayout.Height(30), GUILayout.Width(150)))
                {
                    setupComplete = false;
                    setupLog.Clear();
                }

                GUILayout.FlexibleSpace();
            }
        }

        void DrawQuickActions()
        {
            EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);

            using (new EditorGUILayout.VerticalScope("box"))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Add Player Animation"))
                    {
                        AddPlayerAnimationToSelected();
                    }

                    if (GUILayout.Button("Add NPC Animation"))
                    {
                        AddNPCAnimationToSelected();
                    }
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Add Architecture Anim"))
                    {
                        AddArchitectureAnimationToSelected();
                    }

                    if (GUILayout.Button("Add Pattern Anim"))
                    {
                        AddPatternAnimationToSelected();
                    }
                }

                EditorGUILayout.Space(5);

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Create VFX Manager"))
                    {
                        CreateVFXManagerInScene();
                    }

                    if (GUILayout.Button("Find & Configure All"))
                    {
                        FindAndConfigureAll();
                    }
                }
            }
        }

        #endregion

        #region Setup Execution

        void RunSetup()
        {
            setupLog.Clear();

            Log("Starting SFS Animation Setup...");

            // Create folder structure
            CreateFolderStructure();

            if (createAnimationPrefabs)
            {
                CreateCoreAnimationPrefabs();
            }

            if (createVFXManager)
            {
                CreateVFXManagerPrefab();
            }

            if (configurePlayer)
            {
                ConfigurePlayerPrefabs();
            }

            if (configureNPCs)
            {
                ConfigureNPCPrefabs();
            }

            if (createArchitecturePrefabs)
            {
                CreateArchitecturePrefabs();
            }

            if (createPatternPrefabs)
            {
                CreatePatternPrefabs();
            }

            if (setupDemoScene)
            {
                SetupDemoScene();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Log("Setup complete!");
            setupComplete = true;
        }

        void CreateFolderStructure()
        {
            EnsureFolder(PREFABS_PATH);
            EnsureFolder(ANIMATION_PREFABS_PATH);
            EnsureFolder(CHARACTER_PREFABS_PATH);
            EnsureFolder(ENVIRONMENT_PREFABS_PATH);
            EnsureFolder(VFX_PREFABS_PATH);

            Log("✓ Folder structure created");
        }

        void EnsureFolder(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parent = Path.GetDirectoryName(path).Replace("\\", "/");
                string folder = Path.GetFileName(path);

                if (!AssetDatabase.IsValidFolder(parent))
                {
                    EnsureFolder(parent);
                }

                AssetDatabase.CreateFolder(parent, folder);
            }
        }

        void CreateCoreAnimationPrefabs()
        {
            // Event Coordinator
            var eventCoordinator = new GameObject("SFS_EventCoordinator");
            SavePrefab(eventCoordinator, ANIMATION_PREFABS_PATH + "/SFS_EventCoordinator.prefab");
            DestroyImmediate(eventCoordinator);

            Log("✓ Core animation prefabs created");
        }

        void CreateVFXManagerPrefab()
        {
            var vfxManager = new GameObject("SFS_VFXManager");
            vfxManager.AddComponent<VFXManager>();
            SavePrefab(vfxManager, VFX_PREFABS_PATH + "/SFS_VFXManager.prefab");
            DestroyImmediate(vfxManager);

            Log("✓ VFX Manager prefab created");
        }

        void ConfigurePlayerPrefabs()
        {
            // Create player prefab template
            var player = new GameObject("SFS_Player");
            player.tag = "Player";

            // Add required components
            var controller = player.AddComponent<CharacterController>();
            controller.radius = 0.5f;
            controller.height = 2f;
            controller.center = Vector3.up;

            player.AddComponent<Player.TranslatorAnimationController>();

            // Create visual child
            var visual = CreatePlayerVisual();
            visual.transform.SetParent(player.transform);
            visual.transform.localPosition = Vector3.zero;

            SavePrefab(player, CHARACTER_PREFABS_PATH + "/SFS_Player.prefab");
            DestroyImmediate(player);

            Log("✓ Player prefab configured");
        }

        GameObject CreatePlayerVisual()
        {
            var visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visual.name = "Visual";
            DestroyImmediate(visual.GetComponent<Collider>());

            var renderer = visual.GetComponent<MeshRenderer>();
            if (renderer)
            {
                renderer.material = new Material(Shader.Find("Standard"));
                renderer.material.color = new Color(0.4f, 0.6f, 0.8f);
            }

            return visual;
        }

        void ConfigureNPCPrefabs()
        {
            // DAZIE
            CreateNPCPrefab("SFS_DAZIE", "DAZIE");

            // June
            CreateNPCPrefab("SFS_June", "June");

            // Winton
            CreateNPCPrefab("SFS_Winton", "Winton");

            // Generic Society Member
            CreateNPCPrefab("SFS_SocietyMember", "SocietyMember");

            // Companion
            CreateNPCPrefab("SFS_Companion", "Companion");

            Log("✓ NPC prefabs created (DAZIE, June, Winton, Society Member, Companion)");
        }

        void CreateNPCPrefab(string name, string npcType)
        {
            var npc = new GameObject(name);

            // Add appropriate animator
            switch (npcType)
            {
                case "DAZIE":
                    npc.AddComponent<NPC.DAZIEAnimator>();
                    break;
                case "June":
                    npc.AddComponent<NPC.JuneAnimator>();
                    break;
                case "Winton":
                    npc.AddComponent<NPC.WintonAnimator>();
                    break;
                case "SocietyMember":
                    npc.AddComponent<NPC.SocietyMemberAnimator>();
                    break;
                case "Companion":
                    npc.AddComponent<NPC.CompanionAnimator>();
                    break;
                default:
                    npc.AddComponent<NPC.NPCAnimationController>();
                    break;
            }

            // Create visual
            var visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visual.name = "Visual";
            visual.transform.SetParent(npc.transform);
            visual.transform.localPosition = Vector3.zero;
            DestroyImmediate(visual.GetComponent<Collider>());

            SavePrefab(npc, CHARACTER_PREFABS_PATH + "/" + name + ".prefab");
            DestroyImmediate(npc);
        }

        void CreateArchitecturePrefabs()
        {
            CreateArchitecturePrefab("SFS_Platform", LivingArchitectureAnimator.ArchitectureType.Platform);
            CreateArchitecturePrefab("SFS_Ramp", LivingArchitectureAnimator.ArchitectureType.Ramp);
            CreateArchitecturePrefab("SFS_Bridge", LivingArchitectureAnimator.ArchitectureType.Bridge);
            CreateArchitecturePrefab("SFS_Signage", LivingArchitectureAnimator.ArchitectureType.Signage);
            CreateArchitecturePrefab("SFS_SafePocket", LivingArchitectureAnimator.ArchitectureType.SafePocket);
            CreateArchitecturePrefab("SFS_Canopy", LivingArchitectureAnimator.ArchitectureType.Canopy);
            CreateArchitecturePrefab("SFS_ConsentGate", LivingArchitectureAnimator.ArchitectureType.ConsentGate);
            CreateArchitecturePrefab("SFS_DesignTerminal", LivingArchitectureAnimator.ArchitectureType.DesignTerminal);

            Log("✓ Architecture prefabs created (8 types)");
        }

        void CreateArchitecturePrefab(string name, LivingArchitectureAnimator.ArchitectureType type)
        {
            var architecture = new GameObject(name);

            var animator = architecture.AddComponent<LivingArchitectureAnimator>();
            animator.architectureType = type;

            // Create visual based on type
            GameObject visual;
            switch (type)
            {
                case LivingArchitectureAnimator.ArchitectureType.Platform:
                    visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    visual.transform.localScale = new Vector3(3f, 0.3f, 3f);
                    break;
                case LivingArchitectureAnimator.ArchitectureType.Ramp:
                    visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    visual.transform.localScale = new Vector3(2f, 0.2f, 4f);
                    visual.transform.localRotation = Quaternion.Euler(15f, 0f, 0f);
                    break;
                case LivingArchitectureAnimator.ArchitectureType.Bridge:
                    visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    visual.transform.localScale = new Vector3(1.5f, 0.2f, 8f);
                    break;
                default:
                    visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    visual.transform.localScale = new Vector3(2f, 2f, 0.2f);
                    break;
            }

            visual.name = "Visual";
            visual.transform.SetParent(architecture.transform);
            visual.transform.localPosition = Vector3.zero;

            animator.visualTarget = visual.transform;

            SavePrefab(architecture, ENVIRONMENT_PREFABS_PATH + "/" + name + ".prefab");
            DestroyImmediate(architecture);
        }

        void CreatePatternPrefabs()
        {
            CreatePatternPrefab("SFS_EchoForm", Patterns.PatternType.EchoForm);
            CreatePatternPrefab("SFS_Distortion", Patterns.PatternType.Distortion);
            CreatePatternPrefab("SFS_NoiseBeast", Patterns.PatternType.NoiseBeast);

            Log("✓ Antagonistic pattern prefabs created (3 types)");
        }

        void CreatePatternPrefab(string name, Patterns.PatternType type)
        {
            var pattern = new GameObject(name);

            var animator = pattern.AddComponent<Patterns.AntagonisticPatternAnimator>();
            animator.patternType = type;

            // Create visual
            var visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            visual.name = "Visual";
            visual.transform.SetParent(pattern.transform);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localScale = Vector3.one * 1.5f;

            var renderer = visual.GetComponent<MeshRenderer>();
            if (renderer)
            {
                renderer.material = new Material(Shader.Find("Standard"));
                switch (type)
                {
                    case Patterns.PatternType.EchoForm:
                        renderer.material.color = new Color(0.5f, 0.5f, 0.6f);
                        break;
                    case Patterns.PatternType.Distortion:
                        renderer.material.color = new Color(0.8f, 0.3f, 0.8f);
                        break;
                    case Patterns.PatternType.NoiseBeast:
                        renderer.material.color = new Color(1f, 0.4f, 0.4f);
                        break;
                }
            }

            animator.visualTarget = visual.transform;

            SavePrefab(pattern, ENVIRONMENT_PREFABS_PATH + "/" + name + ".prefab");
            DestroyImmediate(pattern);
        }

        void SetupDemoScene()
        {
            // Check if we're in a valid scene
            var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();

            // Create managers if they don't exist
            if (!FindObjectOfType<VFXManager>())
            {
                var vfxManager = new GameObject("SFS_VFXManager");
                vfxManager.AddComponent<VFXManager>();
            }

            // Create player if tagged Player doesn't exist
            if (!GameObject.FindGameObjectWithTag("Player"))
            {
                var player = new GameObject("Player");
                player.tag = "Player";
                player.AddComponent<CharacterController>();
                player.AddComponent<Player.TranslatorAnimationController>();

                var visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                visual.name = "Visual";
                visual.transform.SetParent(player.transform);
                visual.transform.localPosition = Vector3.zero;
                DestroyImmediate(visual.GetComponent<Collider>());
            }

            // Create some demo architecture
            CreateDemoArchitecture();

            Log("✓ Demo scene configured");
        }

        void CreateDemoArchitecture()
        {
            // Ground
            var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "Ground";
            ground.transform.position = new Vector3(0, -0.5f, 0);
            ground.transform.localScale = new Vector3(30, 1, 30);

            // Demo platform
            var platform = new GameObject("DemoPlatform");
            var platformAnim = platform.AddComponent<LivingArchitectureAnimator>();
            platformAnim.architectureType = LivingArchitectureAnimator.ArchitectureType.Platform;

            var platformVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            platformVisual.name = "Visual";
            platformVisual.transform.SetParent(platform.transform);
            platformVisual.transform.localScale = new Vector3(3, 0.3f, 3);
            platformAnim.visualTarget = platformVisual.transform;

            platform.transform.position = new Vector3(5, 1, 0);

            // Demo ramp
            var ramp = new GameObject("DemoRamp");
            var rampAnim = ramp.AddComponent<LivingArchitectureAnimator>();
            rampAnim.architectureType = LivingArchitectureAnimator.ArchitectureType.Ramp;

            var rampVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rampVisual.name = "Visual";
            rampVisual.transform.SetParent(ramp.transform);
            rampVisual.transform.localScale = new Vector3(2, 0.2f, 4);
            rampVisual.transform.localRotation = Quaternion.Euler(15, 0, 0);
            rampAnim.visualTarget = rampVisual.transform;

            ramp.transform.position = new Vector3(-5, 0.5f, 3);
        }

        void SavePrefab(GameObject obj, string path)
        {
            PrefabUtility.SaveAsPrefabAsset(obj, path);
        }

        void Log(string message)
        {
            setupLog.Add(message);
            Debug.Log("[SFS Animation] " + message);
        }

        #endregion

        #region Quick Actions

        void AddPlayerAnimationToSelected()
        {
            var selected = Selection.activeGameObject;
            if (!selected)
            {
                EditorUtility.DisplayDialog("No Selection", "Please select a GameObject in the scene or hierarchy.", "OK");
                return;
            }

            if (!selected.GetComponent<Player.TranslatorAnimationController>())
            {
                Undo.AddComponent<Player.TranslatorAnimationController>(selected);
                Debug.Log($"Added TranslatorAnimationController to {selected.name}");
            }
            else
            {
                Debug.Log($"{selected.name} already has TranslatorAnimationController");
            }
        }

        void AddNPCAnimationToSelected()
        {
            var selected = Selection.activeGameObject;
            if (!selected)
            {
                EditorUtility.DisplayDialog("No Selection", "Please select a GameObject.", "OK");
                return;
            }

            // Show menu for NPC type selection
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Generic NPC"), false, () => AddNPCComponent<NPC.NPCAnimationController>(selected));
            menu.AddItem(new GUIContent("DAZIE (Mentor)"), false, () => AddNPCComponent<NPC.DAZIEAnimator>(selected));
            menu.AddItem(new GUIContent("June (Sensory Architect)"), false, () => AddNPCComponent<NPC.JuneAnimator>(selected));
            menu.AddItem(new GUIContent("Winton (System Ghost)"), false, () => AddNPCComponent<NPC.WintonAnimator>(selected));
            menu.AddItem(new GUIContent("Society Member"), false, () => AddNPCComponent<NPC.SocietyMemberAnimator>(selected));
            menu.AddItem(new GUIContent("Companion"), false, () => AddNPCComponent<NPC.CompanionAnimator>(selected));
            menu.ShowAsContext();
        }

        void AddNPCComponent<T>(GameObject target) where T : Component
        {
            if (!target.GetComponent<T>())
            {
                Undo.AddComponent<T>(target);
                Debug.Log($"Added {typeof(T).Name} to {target.name}");
            }
        }

        void AddArchitectureAnimationToSelected()
        {
            var selected = Selection.activeGameObject;
            if (!selected)
            {
                EditorUtility.DisplayDialog("No Selection", "Please select a GameObject.", "OK");
                return;
            }

            // Show menu for architecture type selection
            var menu = new GenericMenu();
            foreach (var type in System.Enum.GetValues(typeof(LivingArchitectureAnimator.ArchitectureType)))
            {
                var archType = (LivingArchitectureAnimator.ArchitectureType)type;
                menu.AddItem(new GUIContent(archType.ToString()), false, () =>
                {
                    var animator = selected.GetComponent<LivingArchitectureAnimator>();
                    if (!animator)
                    {
                        animator = Undo.AddComponent<LivingArchitectureAnimator>(selected);
                    }
                    animator.architectureType = archType;
                    Debug.Log($"Configured {selected.name} as {archType}");
                });
            }
            menu.ShowAsContext();
        }

        void AddPatternAnimationToSelected()
        {
            var selected = Selection.activeGameObject;
            if (!selected)
            {
                EditorUtility.DisplayDialog("No Selection", "Please select a GameObject.", "OK");
                return;
            }

            var menu = new GenericMenu();
            foreach (var type in System.Enum.GetValues(typeof(Patterns.PatternType)))
            {
                var patternType = (Patterns.PatternType)type;
                menu.AddItem(new GUIContent(patternType.ToString()), false, () =>
                {
                    var animator = selected.GetComponent<Patterns.AntagonisticPatternAnimator>();
                    if (!animator)
                    {
                        animator = Undo.AddComponent<Patterns.AntagonisticPatternAnimator>(selected);
                    }
                    animator.patternType = patternType;
                    Debug.Log($"Configured {selected.name} as {patternType}");
                });
            }
            menu.ShowAsContext();
        }

        void CreateVFXManagerInScene()
        {
            if (FindObjectOfType<VFXManager>())
            {
                EditorUtility.DisplayDialog("Already Exists", "VFX Manager already exists in the scene.", "OK");
                return;
            }

            var vfxManager = new GameObject("SFS_VFXManager");
            Undo.RegisterCreatedObjectUndo(vfxManager, "Create VFX Manager");
            vfxManager.AddComponent<VFXManager>();
            Selection.activeGameObject = vfxManager;

            Debug.Log("Created VFX Manager in scene");
        }

        void FindAndConfigureAll()
        {
            int configured = 0;

            // Find all objects with PlayerController and add animation
            var playerControllers = FindObjectsOfType<SFS.Player.PlayerController>();
            foreach (var pc in playerControllers)
            {
                if (!pc.GetComponent<Player.TranslatorAnimationController>())
                {
                    pc.gameObject.AddComponent<Player.TranslatorAnimationController>();
                    configured++;
                }
            }

            // Ensure VFX Manager exists
            if (!FindObjectOfType<VFXManager>())
            {
                var vfxManager = new GameObject("SFS_VFXManager");
                vfxManager.AddComponent<VFXManager>();
                configured++;
            }

            EditorUtility.DisplayDialog("Configuration Complete",
                $"Configured {configured} objects with animation components.", "OK");
        }

        #endregion
    }
}
