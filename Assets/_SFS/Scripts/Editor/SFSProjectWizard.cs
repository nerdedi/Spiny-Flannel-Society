#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;
using System.Collections.Generic;

namespace SFS.Editor
{
    /// <summary>
    /// One-click complete project setup wizard.
    /// Creates everything needed to start the game from scratch.
    /// </summary>
    public class SFSProjectWizard : EditorWindow
    {
        static readonly string[] REQUIRED_FOLDERS = new[]
        {
            "Assets/_SFS",
            "Assets/_SFS/Animations",
            "Assets/_SFS/Animations/Clips",
            "Assets/_SFS/Materials",
            "Assets/_SFS/Prefabs",
            "Assets/_SFS/Prefabs/Characters",
            "Assets/_SFS/Prefabs/Environment",
            "Assets/_SFS/Prefabs/UI",
            "Assets/_SFS/Scenes",
            "Assets/_SFS/ScriptableObjects",
            "Assets/_SFS/Scripts",
            "Assets/_SFS/Timeline",
            "Assets/_SFS/UI"
        };

        Vector2 scrollPos;
        bool foldoutFolders = true;
        bool foldoutAnimations = true;
        bool foldoutPrefabs = true;
        bool foldoutScene = true;

        [MenuItem("SFS/🚀 Project Wizard (Start Here!)", priority = 0)]
        public static void ShowWindow()
        {
            var window = GetWindow<SFSProjectWizard>("SFS Project Wizard");
            window.minSize = new Vector2(450, 600);
        }

        void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            DrawHeader();
            EditorGUILayout.Space(10);

            DrawOneClickSection();
            EditorGUILayout.Space(20);

            DrawManualSections();

            EditorGUILayout.EndScrollView();
        }

        void DrawHeader()
        {
            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter
            };

            EditorGUILayout.LabelField("🌿 Spiny Flannel Society", headerStyle);
            EditorGUILayout.LabelField("Project Setup Wizard", EditorStyles.centeredGreyMiniLabel);

            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox(
                "This wizard sets up everything you need to start developing.\n" +
                "Click the big button below for complete automatic setup!",
                MessageType.Info
            );
        }

        void DrawOneClickSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                fixedHeight = 50
            };

            GUI.backgroundColor = new Color(0.4f, 0.8f, 0.4f);
            if (GUILayout.Button("✨ COMPLETE PROJECT SETUP ✨", buttonStyle))
            {
                RunCompleteSetup();
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.LabelField("Creates: Folders, Animations, Materials, Controllers, Prefabs, and Demo Scene",
                EditorStyles.centeredGreyMiniLabel);

            EditorGUILayout.EndVertical();
        }

        void DrawManualSections()
        {
            EditorGUILayout.LabelField("─── Manual Setup Options ───", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Space(5);

            // Folders
            foldoutFolders = EditorGUILayout.Foldout(foldoutFolders, "📁 Folder Structure", true);
            if (foldoutFolders)
            {
                EditorGUI.indentLevel++;
                foreach (var folder in REQUIRED_FOLDERS)
                {
                    bool exists = Directory.Exists(folder);
                    EditorGUILayout.LabelField(exists ? "✓" : "✗", folder);
                }
                if (GUILayout.Button("Create Missing Folders"))
                {
                    CreateFolderStructure();
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // Animations
            foldoutAnimations = EditorGUILayout.Foldout(foldoutAnimations, "🎬 Animations", true);
            if (foldoutAnimations)
            {
                EditorGUI.indentLevel++;
                if (GUILayout.Button("Generate Placeholder Animation Clips"))
                {
                    GenerateAnimationClips();
                }
                if (GUILayout.Button("Create Animator Controllers"))
                {
                    CreateAnimatorControllers();
                }
                if (GUILayout.Button("Assign Clips to Controllers"))
                {
                    AssignClipsToControllers();
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // Prefabs
            foldoutPrefabs = EditorGUILayout.Foldout(foldoutPrefabs, "🎮 Prefabs", true);
            if (foldoutPrefabs)
            {
                EditorGUI.indentLevel++;
                if (GUILayout.Button("Create Player Prefab"))
                {
                    CreatePlayerPrefab();
                }
                if (GUILayout.Button("Create NPC Prefab"))
                {
                    CreateNPCPrefab();
                }
                if (GUILayout.Button("Create Collectible Prefab"))
                {
                    CreateCollectiblePrefab();
                }
                if (GUILayout.Button("Create All Materials"))
                {
                    CreateMaterials();
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // Scene
            foldoutScene = EditorGUILayout.Foldout(foldoutScene, "🌍 Scene", true);
            if (foldoutScene)
            {
                EditorGUI.indentLevel++;
                if (GUILayout.Button("Build Demo Scene"))
                {
                    BuildDemoScene();
                }
                if (GUILayout.Button("Add Managers to Scene"))
                {
                    AddManagersToScene();
                }
                EditorGUI.indentLevel--;
            }
        }

        #region Complete Setup

        [MenuItem("SFS/Setup/Complete Project Setup (All Steps)", priority = 1)]
        public static void RunCompleteSetup()
        {
            EditorUtility.DisplayProgressBar("SFS Setup", "Creating folders...", 0.1f);
            CreateFolderStructure();

            EditorUtility.DisplayProgressBar("SFS Setup", "Creating materials...", 0.2f);
            CreateMaterials();

            EditorUtility.DisplayProgressBar("SFS Setup", "Generating animations...", 0.3f);
            GenerateAnimationClips();

            EditorUtility.DisplayProgressBar("SFS Setup", "Creating animator controllers...", 0.5f);
            CreateAnimatorControllers();

            EditorUtility.DisplayProgressBar("SFS Setup", "Assigning clips...", 0.6f);
            AssignClipsToControllers();

            EditorUtility.DisplayProgressBar("SFS Setup", "Creating prefabs...", 0.7f);
            CreatePlayerPrefab();
            CreateNPCPrefab();
            CreateCollectiblePrefab();
            CreateEnvironmentPrefabs();

            EditorUtility.DisplayProgressBar("SFS Setup", "Building demo scene...", 0.9f);
            BuildDemoScene();

            EditorUtility.ClearProgressBar();

            Debug.Log("<color=green>═══════════════════════════════════════════</color>");
            Debug.Log("<color=green>✨ SFS PROJECT SETUP COMPLETE! ✨</color>");
            Debug.Log("<color=green>═══════════════════════════════════════════</color>");
            Debug.Log("[SFS] Open scene: Assets/_SFS/Scenes/SFS_DemoScene.unity");
            Debug.Log("[SFS] Press Play to test!");

            EditorUtility.DisplayDialog(
                "Setup Complete!",
                "Your Spiny Flannel Society project is ready!\n\n" +
                "• Open: SFS_DemoScene in Assets/_SFS/Scenes/\n" +
                "• Press Play to test movement\n" +
                "• WASD to move, Space to jump\n\n" +
                "Check the Console for any warnings.",
                "Let's Go!"
            );
        }

        #endregion

        #region Folder Structure

        static void CreateFolderStructure()
        {
            foreach (var folder in REQUIRED_FOLDERS)
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                    Debug.Log($"[SFS] Created: {folder}");
                }
            }
            AssetDatabase.Refresh();
        }

        #endregion

        #region Materials

        static void CreateMaterials()
        {
            CreateMaterial("PlayerMaterial", new Color(0.4f, 0.7f, 0.9f));
            CreateMaterial("NPCMaterial", new Color(0.9f, 0.6f, 0.4f));
            CreateMaterial("GroundMaterial", new Color(0.85f, 0.8f, 0.7f));
            CreateMaterial("CollectibleMaterial", new Color(1f, 0.85f, 0.3f));
            CreateMaterial("PlatformMaterial", new Color(0.7f, 0.75f, 0.8f));
            CreateMaterial("WallMaterial", new Color(0.6f, 0.55f, 0.5f));
            CreateMaterial("RestZoneMaterial", new Color(0.5f, 0.8f, 0.6f, 0.5f), true);
            AssetDatabase.SaveAssets();
        }

        static void CreateMaterial(string name, Color color, bool transparent = false)
        {
            string path = $"Assets/_SFS/Materials/{name}.mat";
            if (File.Exists(path)) return;

            var mat = new Material(Shader.Find(transparent ? "Standard" : "Standard"));
            mat.color = color;

            if (transparent)
            {
                mat.SetFloat("_Mode", 3);
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
            }

            AssetDatabase.CreateAsset(mat, path);
            Debug.Log($"[SFS] Created material: {name}");
        }

        #endregion

        #region Animation Clips

        static void GenerateAnimationClips()
        {
            string clipPath = "Assets/_SFS/Animations/Clips";
            if (!Directory.Exists(clipPath))
                Directory.CreateDirectory(clipPath);

            // Player clips
            CreateLoopingBobClip("Player_Idle", clipPath, 2f, 0.04f);
            CreateLoopingBobClip("Player_Walk", clipPath, 7f, 0.07f);
            CreateLoopingBobClip("Player_Run", clipPath, 12f, 0.1f);
            CreateJumpClip("Player_Jump", clipPath);
            CreateFallClip("Player_Fall", clipPath);
            CreateLandClip("Player_Land", clipPath);
            CreateDeathClip("Player_Die", clipPath);

            // NPC clips
            CreateLoopingBobClip("NPC_Idle_Breathe", clipPath, 1.5f, 0.025f);
            CreateLoopingBobClip("NPC_Idle_Shift", clipPath, 0.6f, 0.04f);
            CreateLoopingBobClip("NPC_Idle_LookAround", clipPath, 0.4f, 0.02f);
            CreateNodClip("NPC_Acknowledged", clipPath);
            CreateLoopingBobClip("NPC_BelongingIdle", clipPath, 1.2f, 0.03f);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[SFS] Animation clips generated");
        }

        static void CreateLoopingBobClip(string name, string folder, float speed, float amount)
        {
            string path = $"{folder}/{name}.anim";
            if (File.Exists(path)) return;

            var clip = new AnimationClip { name = name };
            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            float duration = 1f / (speed / 4f);
            var curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f));
            curve.AddKey(new Keyframe(duration * 0.25f, amount));
            curve.AddKey(new Keyframe(duration * 0.5f, 0f));
            curve.AddKey(new Keyframe(duration * 0.75f, amount));
            curve.AddKey(new Keyframe(duration, 0f));

            for (int i = 0; i < curve.keys.Length; i++)
            {
                AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Auto);
                AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Auto);
            }

            clip.SetCurve("", typeof(Transform), "localPosition.y", curve);
            AssetDatabase.CreateAsset(clip, path);
        }

        static void CreateJumpClip(string name, string folder)
        {
            string path = $"{folder}/{name}.anim";
            if (File.Exists(path)) return;

            var clip = new AnimationClip { name = name };
            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = false;
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            var scaleY = AnimationCurve.EaseInOut(0f, 1f, 0.4f, 1.2f);
            scaleY.AddKey(new Keyframe(0.1f, 0.8f));
            clip.SetCurve("", typeof(Transform), "localScale.y", scaleY);

            var scaleXZ = AnimationCurve.EaseInOut(0f, 1f, 0.4f, 0.9f);
            scaleXZ.AddKey(new Keyframe(0.1f, 1.1f));
            clip.SetCurve("", typeof(Transform), "localScale.x", scaleXZ);
            clip.SetCurve("", typeof(Transform), "localScale.z", scaleXZ);

            AssetDatabase.CreateAsset(clip, path);
        }

        static void CreateFallClip(string name, string folder)
        {
            string path = $"{folder}/{name}.anim";
            if (File.Exists(path)) return;

            var clip = new AnimationClip { name = name };
            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            var curve = new AnimationCurve();
            curve.AddKey(0f, 1.02f);
            curve.AddKey(0.5f, 0.98f);
            curve.AddKey(1f, 1.02f);
            clip.SetCurve("", typeof(Transform), "localScale.y", curve);

            AssetDatabase.CreateAsset(clip, path);
        }

        static void CreateLandClip(string name, string folder)
        {
            string path = $"{folder}/{name}.anim";
            if (File.Exists(path)) return;

            var clip = new AnimationClip { name = name };
            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = false;
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            var scaleY = new AnimationCurve();
            scaleY.AddKey(0f, 1f);
            scaleY.AddKey(0.08f, 0.65f);
            scaleY.AddKey(0.25f, 1.08f);
            scaleY.AddKey(0.4f, 1f);
            clip.SetCurve("", typeof(Transform), "localScale.y", scaleY);

            var scaleXZ = new AnimationCurve();
            scaleXZ.AddKey(0f, 1f);
            scaleXZ.AddKey(0.08f, 1.25f);
            scaleXZ.AddKey(0.25f, 0.95f);
            scaleXZ.AddKey(0.4f, 1f);
            clip.SetCurve("", typeof(Transform), "localScale.x", scaleXZ);
            clip.SetCurve("", typeof(Transform), "localScale.z", scaleXZ);

            AssetDatabase.CreateAsset(clip, path);
        }

        static void CreateDeathClip(string name, string folder)
        {
            string path = $"{folder}/{name}.anim";
            if (File.Exists(path)) return;

            var clip = new AnimationClip { name = name };
            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = false;
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            var scaleY = AnimationCurve.EaseInOut(0f, 1f, 1f, 0.1f);
            clip.SetCurve("", typeof(Transform), "localScale.y", scaleY);

            var scaleXZ = AnimationCurve.EaseInOut(0f, 1f, 1f, 1.8f);
            clip.SetCurve("", typeof(Transform), "localScale.x", scaleXZ);
            clip.SetCurve("", typeof(Transform), "localScale.z", scaleXZ);

            var rotX = AnimationCurve.EaseInOut(0f, 0f, 1f, 90f);
            clip.SetCurve("", typeof(Transform), "localEulerAngles.x", rotX);

            AssetDatabase.CreateAsset(clip, path);
        }

        static void CreateNodClip(string name, string folder)
        {
            string path = $"{folder}/{name}.anim";
            if (File.Exists(path)) return;

            var clip = new AnimationClip { name = name };
            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = false;
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            var posY = new AnimationCurve();
            posY.AddKey(0f, 0f);
            posY.AddKey(0.15f, -0.08f);
            posY.AddKey(0.35f, 0.02f);
            posY.AddKey(0.5f, 0f);
            clip.SetCurve("", typeof(Transform), "localPosition.y", posY);

            AssetDatabase.CreateAsset(clip, path);
        }

        #endregion

        #region Animator Controllers

        static void CreateAnimatorControllers()
        {
            CreatePlayerAnimatorController();
            CreateNPCAnimatorController();
            Debug.Log("[SFS] Animator controllers created");
        }

        static void CreatePlayerAnimatorController()
        {
            string path = "Assets/_SFS/Animations/PlayerAnimator.controller";

            var controller = AnimatorController.CreateAnimatorControllerAtPath(path);

            // Parameters
            controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
            controller.AddParameter("Grounded", AnimatorControllerParameterType.Bool);
            controller.AddParameter("YVelocity", AnimatorControllerParameterType.Float);
            controller.AddParameter("IdleIntensity", AnimatorControllerParameterType.Float);
            controller.AddParameter("EmotionTone", AnimatorControllerParameterType.Int);
            controller.AddParameter("InRestZone", AnimatorControllerParameterType.Bool);
            controller.AddParameter("WithCompanion", AnimatorControllerParameterType.Bool);
            controller.AddParameter("Jump", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Land", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Die", AnimatorControllerParameterType.Trigger);

            var rootSM = controller.layers[0].stateMachine;

            // States
            var idle = rootSM.AddState("Idle", new Vector3(300, 0, 0));
            var walk = rootSM.AddState("Walk", new Vector3(300, 70, 0));
            var run = rootSM.AddState("Run", new Vector3(300, 140, 0));
            var jump = rootSM.AddState("Jump", new Vector3(550, 0, 0));
            var fall = rootSM.AddState("Fall", new Vector3(550, 70, 0));
            var land = rootSM.AddState("Land", new Vector3(550, 140, 0));
            var die = rootSM.AddState("Die", new Vector3(750, 70, 0));

            rootSM.defaultState = idle;

            // Transitions
            AddTransition(idle, walk, "Speed", AnimatorConditionMode.Greater, 0.1f);
            AddTransition(walk, run, "Speed", AnimatorConditionMode.Greater, 0.55f);
            AddTransition(run, walk, "Speed", AnimatorConditionMode.Less, 0.55f);
            AddTransition(walk, idle, "Speed", AnimatorConditionMode.Less, 0.1f);
            AddTransition(run, idle, "Speed", AnimatorConditionMode.Less, 0.1f);

            // Jump
            var anyToJump = rootSM.AddAnyStateTransition(jump);
            anyToJump.AddCondition(AnimatorConditionMode.If, 0, "Jump");
            anyToJump.hasExitTime = false;
            anyToJump.duration = 0.1f;

            AddTransition(jump, fall, "YVelocity", AnimatorConditionMode.Less, 0f, false, 0.15f);

            // Fall from ground states
            var idleToFall = idle.AddTransition(fall);
            idleToFall.AddCondition(AnimatorConditionMode.IfNot, 0, "Grounded");
            idleToFall.AddCondition(AnimatorConditionMode.Less, 0f, "YVelocity");
            idleToFall.hasExitTime = false;
            idleToFall.duration = 0.1f;

            // Land
            var fallToLand = fall.AddTransition(land);
            fallToLand.AddCondition(AnimatorConditionMode.If, 0, "Grounded");
            fallToLand.hasExitTime = false;
            fallToLand.duration = 0.05f;

            var landToIdle = land.AddTransition(idle);
            landToIdle.hasExitTime = true;
            landToIdle.exitTime = 0.85f;
            landToIdle.duration = 0.15f;

            // Die
            var anyToDie = rootSM.AddAnyStateTransition(die);
            anyToDie.AddCondition(AnimatorConditionMode.If, 0, "Die");
            anyToDie.hasExitTime = false;
            anyToDie.duration = 0.1f;

            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
        }

        static void CreateNPCAnimatorController()
        {
            string path = "Assets/_SFS/Animations/NPCAnimator.controller";

            var controller = AnimatorController.CreateAnimatorControllerAtPath(path);

            controller.AddParameter("IdleVariant", AnimatorControllerParameterType.Int);
            controller.AddParameter("SyncPhase", AnimatorControllerParameterType.Float);
            controller.AddParameter("Acknowledged", AnimatorControllerParameterType.Bool);
            controller.AddParameter("BelongingIdle", AnimatorControllerParameterType.Trigger);

            var rootSM = controller.layers[0].stateMachine;

            var breathe = rootSM.AddState("Idle_Breathe", new Vector3(300, 0, 0));
            var shift = rootSM.AddState("Idle_Shift", new Vector3(300, 70, 0));
            var look = rootSM.AddState("Idle_LookAround", new Vector3(300, 140, 0));
            var ack = rootSM.AddState("Acknowledged", new Vector3(550, 70, 0));
            var belong = rootSM.AddState("BelongingIdle", new Vector3(750, 70, 0));

            rootSM.defaultState = breathe;

            // Variant transitions
            AddTransition(breathe, shift, "IdleVariant", AnimatorConditionMode.Equals, 1);
            AddTransition(breathe, look, "IdleVariant", AnimatorConditionMode.Equals, 2);
            AddTransition(shift, breathe, "IdleVariant", AnimatorConditionMode.Equals, 0);
            AddTransition(look, breathe, "IdleVariant", AnimatorConditionMode.Equals, 0);

            // Acknowledge
            var anyToAck = rootSM.AddAnyStateTransition(ack);
            anyToAck.AddCondition(AnimatorConditionMode.If, 0, "Acknowledged");
            anyToAck.hasExitTime = false;
            anyToAck.duration = 0.2f;

            var ackToIdle = ack.AddTransition(breathe);
            ackToIdle.hasExitTime = true;
            ackToIdle.exitTime = 0.9f;
            ackToIdle.duration = 0.2f;

            // Belonging
            var anyToBelong = rootSM.AddAnyStateTransition(belong);
            anyToBelong.AddCondition(AnimatorConditionMode.If, 0, "BelongingIdle");
            anyToBelong.hasExitTime = false;
            anyToBelong.duration = 0.5f;

            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
        }

        static void AddTransition(AnimatorState from, AnimatorState to, string param,
            AnimatorConditionMode mode, float threshold, bool hasExitTime = false, float duration = 0.15f)
        {
            var t = from.AddTransition(to);
            t.AddCondition(mode, threshold, param);
            t.hasExitTime = hasExitTime;
            t.duration = duration;
        }

        static void AssignClipsToControllers()
        {
            AssignPlayerClips();
            AssignNPCClips();
            Debug.Log("[SFS] Animation clips assigned to controllers");
        }

        static void AssignPlayerClips()
        {
            string controllerPath = "Assets/_SFS/Animations/PlayerAnimator.controller";
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
            if (!controller) return;

            var clipPath = "Assets/_SFS/Animations/Clips";
            var stateMachine = controller.layers[0].stateMachine;

            foreach (var childState in stateMachine.states)
            {
                var state = childState.state;
                string clipName = $"Player_{state.name}";
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{clipPath}/{clipName}.anim");
                if (clip) state.motion = clip;
            }

            EditorUtility.SetDirty(controller);
        }

        static void AssignNPCClips()
        {
            string controllerPath = "Assets/_SFS/Animations/NPCAnimator.controller";
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
            if (!controller) return;

            var clipPath = "Assets/_SFS/Animations/Clips";
            var stateMachine = controller.layers[0].stateMachine;

            foreach (var childState in stateMachine.states)
            {
                var state = childState.state;
                string clipName = $"NPC_{state.name}";
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{clipPath}/{clipName}.anim");
                if (clip) state.motion = clip;
            }

            EditorUtility.SetDirty(controller);
        }

        #endregion

        #region Prefabs

        static void CreatePlayerPrefab()
        {
            string path = "Assets/_SFS/Prefabs/Characters/Player.prefab";
            if (File.Exists(path)) return;

            var player = new GameObject("Player");

            // Visual
            var visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visual.name = "Visual";
            visual.transform.SetParent(player.transform);
            visual.transform.localPosition = Vector3.up;
            Object.DestroyImmediate(visual.GetComponent<Collider>());

            var mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/_SFS/Materials/PlayerMaterial.mat");
            if (mat) visual.GetComponent<MeshRenderer>().sharedMaterial = mat;

            // Components
            var cc = player.AddComponent<CharacterController>();
            cc.center = Vector3.up;
            cc.height = 2f;
            cc.radius = 0.5f;

            // Try to add scripts if they exist
            TryAddComponent(player, "SFS.Player.PlayerController");
            TryAddComponent(player, "SFS.Player.PlayerAnimatorDriver");

            var procAnim = TryAddComponent(visual, "SFS.Animation.AdvancedProceduralAnimator");
            if (procAnim == null)
                TryAddComponent(visual, "SFS.Player.SimpleProceduralAnimator");

            // Animator
            var animator = visual.AddComponent<Animator>();
            var controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(
                "Assets/_SFS/Animations/PlayerAnimator.controller");
            if (controller) animator.runtimeAnimatorController = controller;

            // Save prefab
            PrefabUtility.SaveAsPrefabAsset(player, path);
            Object.DestroyImmediate(player);
            Debug.Log("[SFS] Created Player prefab");
        }

        static void CreateNPCPrefab()
        {
            string path = "Assets/_SFS/Prefabs/Characters/NPC.prefab";
            if (File.Exists(path)) return;

            var npc = new GameObject("NPC");

            var visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visual.name = "Visual";
            visual.transform.SetParent(npc.transform);
            visual.transform.localPosition = Vector3.up;
            Object.DestroyImmediate(visual.GetComponent<Collider>());

            var mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/_SFS/Materials/NPCMaterial.mat");
            if (mat) visual.GetComponent<MeshRenderer>().sharedMaterial = mat;

            // Collider on root
            var col = npc.AddComponent<CapsuleCollider>();
            col.center = Vector3.up;
            col.height = 2f;
            col.radius = 0.5f;

            // Animator
            var animator = visual.AddComponent<Animator>();
            var controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(
                "Assets/_SFS/Animations/NPCAnimator.controller");
            if (controller) animator.runtimeAnimatorController = controller;

            TryAddComponent(visual, "SFS.Animation.NPCProceduralAnimator");

            PrefabUtility.SaveAsPrefabAsset(npc, path);
            Object.DestroyImmediate(npc);
            Debug.Log("[SFS] Created NPC prefab");
        }

        static void CreateCollectiblePrefab()
        {
            string path = "Assets/_SFS/Prefabs/Environment/Collectible.prefab";
            if (File.Exists(path)) return;

            var collectible = new GameObject("Collectible");

            var visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            visual.name = "Visual";
            visual.transform.SetParent(collectible.transform);
            visual.transform.localPosition = Vector3.up * 0.5f;
            visual.transform.localScale = Vector3.one * 0.5f;
            Object.DestroyImmediate(visual.GetComponent<Collider>());

            var mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/_SFS/Materials/CollectibleMaterial.mat");
            if (mat) visual.GetComponent<MeshRenderer>().sharedMaterial = mat;

            // Trigger collider
            var col = collectible.AddComponent<SphereCollider>();
            col.center = Vector3.up * 0.5f;
            col.radius = 0.5f;
            col.isTrigger = true;

            // Spin script
            var spin = collectible.AddComponent<SimpleSpinner>();

            TryAddComponent(collectible, "SFS.World.Collectible");

            PrefabUtility.SaveAsPrefabAsset(collectible, path);
            Object.DestroyImmediate(collectible);
            Debug.Log("[SFS] Created Collectible prefab");
        }

        static void CreateEnvironmentPrefabs()
        {
            CreatePlatformPrefab();
            CreateGroundPrefab();
        }

        static void CreatePlatformPrefab()
        {
            string path = "Assets/_SFS/Prefabs/Environment/Platform.prefab";
            if (File.Exists(path)) return;

            var platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
            platform.name = "Platform";
            platform.transform.localScale = new Vector3(3f, 0.5f, 3f);

            var mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/_SFS/Materials/PlatformMaterial.mat");
            if (mat) platform.GetComponent<MeshRenderer>().sharedMaterial = mat;

            PrefabUtility.SaveAsPrefabAsset(platform, path);
            Object.DestroyImmediate(platform);
        }

        static void CreateGroundPrefab()
        {
            string path = "Assets/_SFS/Prefabs/Environment/Ground.prefab";
            if (File.Exists(path)) return;

            var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(50f, 1f, 50f);

            var mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/_SFS/Materials/GroundMaterial.mat");
            if (mat) ground.GetComponent<MeshRenderer>().sharedMaterial = mat;

            PrefabUtility.SaveAsPrefabAsset(ground, path);
            Object.DestroyImmediate(ground);
        }

        static Component TryAddComponent(GameObject go, string typeName)
        {
            var type = System.Type.GetType(typeName + ", Assembly-CSharp");
            if (type != null)
            {
                return go.AddComponent(type);
            }
            return null;
        }

        #endregion

        #region Scene Building

        static void BuildDemoScene()
        {
            // Create new scene
            var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(
                UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects,
                UnityEditor.SceneManagement.NewSceneMode.Single
            );

            // Ground
            var groundPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_SFS/Prefabs/Environment/Ground.prefab");
            if (groundPrefab)
            {
                var ground = (GameObject)PrefabUtility.InstantiatePrefab(groundPrefab);
                ground.transform.position = new Vector3(0, -0.5f, 0);
            }
            else
            {
                var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
                ground.name = "Ground";
                ground.transform.position = new Vector3(0, -0.5f, 0);
                ground.transform.localScale = new Vector3(50, 1, 50);
            }

            // Platforms
            CreateScenePlatform(new Vector3(-5, 1, 5), new Vector3(4, 0.5f, 4));
            CreateScenePlatform(new Vector3(5, 2, 5), new Vector3(4, 0.5f, 4));
            CreateScenePlatform(new Vector3(0, 3.5f, 10), new Vector3(5, 0.5f, 5));
            CreateScenePlatform(new Vector3(-8, 5, 10), new Vector3(3, 0.5f, 3));
            CreateScenePlatform(new Vector3(8, 5, 10), new Vector3(3, 0.5f, 3));

            // Player
            var playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_SFS/Prefabs/Characters/Player.prefab");
            if (playerPrefab)
            {
                var player = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
                player.transform.position = new Vector3(0, 0, 0);
                player.tag = "Player";
            }

            // NPCs
            var npcPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_SFS/Prefabs/Characters/NPC.prefab");
            if (npcPrefab)
            {
                for (int i = 0; i < 3; i++)
                {
                    var npc = (GameObject)PrefabUtility.InstantiatePrefab(npcPrefab);
                    npc.name = $"NPC_{i + 1}";
                    npc.transform.position = new Vector3(-4 + i * 4, 0, 8);
                }
            }

            // Collectibles
            var collectiblePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_SFS/Prefabs/Environment/Collectible.prefab");
            if (collectiblePrefab)
            {
                Vector3[] positions = new[]
                {
                    new Vector3(-5, 2f, 5),
                    new Vector3(5, 3f, 5),
                    new Vector3(0, 4.5f, 10),
                    new Vector3(-8, 6f, 10),
                    new Vector3(8, 6f, 10),
                };

                for (int i = 0; i < positions.Length; i++)
                {
                    var c = (GameObject)PrefabUtility.InstantiatePrefab(collectiblePrefab);
                    c.name = $"Collectible_{i + 1}";
                    c.transform.position = positions[i];
                }
            }

            // Managers
            AddManagersToScene();

            // Lighting
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.8f, 0.85f, 0.95f);
            RenderSettings.ambientEquatorColor = new Color(0.75f, 0.8f, 0.85f);
            RenderSettings.ambientGroundColor = new Color(0.4f, 0.45f, 0.5f);

            // Save scene
            string scenePath = "Assets/_SFS/Scenes/SFS_DemoScene.unity";
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log($"[SFS] Demo scene saved to {scenePath}");
        }

        static void CreateScenePlatform(Vector3 position, Vector3 scale)
        {
            var platformPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_SFS/Prefabs/Environment/Platform.prefab");
            GameObject platform;

            if (platformPrefab)
            {
                platform = (GameObject)PrefabUtility.InstantiatePrefab(platformPrefab);
            }
            else
            {
                platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
            }

            platform.name = "Platform";
            platform.transform.position = position;
            platform.transform.localScale = scale;
        }

        static void AddManagersToScene()
        {
            // Game Manager
            if (!GameObject.Find("GameManager"))
            {
                var gm = new GameObject("GameManager");
                TryAddComponent(gm, "SFS.Core.StoryBeatManager");
                TryAddComponent(gm, "SFS.Core.SettingsManager");
            }

            // UI Manager
            if (!GameObject.Find("UIManager"))
            {
                var ui = new GameObject("UIManager");
                TryAddComponent(ui, "SFS.UI.PauseManager");
            }
        }

        #endregion
    }

    // Simple utility component for collectible spinning
    public class SimpleSpinner : MonoBehaviour
    {
        public float speed = 90f;
        public float bobSpeed = 2f;
        public float bobAmount = 0.2f;

        Vector3 startPos;

        void Start() => startPos = transform.position;

        void Update()
        {
            transform.Rotate(Vector3.up, speed * Time.deltaTime);
            transform.position = startPos + Vector3.up * Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        }
    }
}
#endif
