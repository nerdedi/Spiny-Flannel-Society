#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using SFS.Core;
using SFS.Player;
using SFS.Camera;
using SFS.World;
using SFS.Narrative;
using SFS.UI;

namespace SFS.Editor
{
    /// <summary>
    /// Creates a complete test scene with all 7 story beats laid out
    /// using placeholder geometry. Perfect for flow testing.
    /// </summary>
    public class TestSceneBuilder : EditorWindow
    {
        [MenuItem("SFS/Setup/Build Test Scene")]
        public static void BuildTestScene()
        {
            // Create new scene
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            scene.name = "SFS_TestScene";

            // Remove default directional light, we'll make our own
            var defaultLight = GameObject.Find("Directional Light");
            if (defaultLight) DestroyImmediate(defaultLight);

            // ========== CORE SYSTEMS ==========
            CreateCoreSystems();

            // ========== PLAYER ==========
            CreatePlayer();

            // ========== CAMERA ==========
            CreateCameraRig();

            // ========== ENVIRONMENT ==========
            CreateEnvironment();

            // ========== UI ==========
            CreateUI();

            // ========== LIGHTING ==========
            CreateLighting();

            // Save scene
            string scenePath = "Assets/_SFS/Scenes/TestScene.unity";
            System.IO.Directory.CreateDirectory("Assets/_SFS/Scenes");
            EditorSceneManager.SaveScene(scene, scenePath);

            Debug.Log("[SFS] Test scene created! Walk through the zones to experience all 7 beats.");
        }

        static void CreateCoreSystems()
        {
            var gameRoot = new GameObject("--- GAME SYSTEMS ---");

            // Settings Manager
            var settingsObj = new GameObject("SettingsManager");
            settingsObj.transform.SetParent(gameRoot.transform);
            settingsObj.AddComponent<SettingsManager>();

            // Story Beat Manager
            var storyObj = new GameObject("StoryBeatManager");
            storyObj.transform.SetParent(gameRoot.transform);
            storyObj.AddComponent<StoryBeatManager>();

            // Respawn System
            var respawnObj = new GameObject("RespawnSystem");
            respawnObj.transform.SetParent(gameRoot.transform);
            var respawn = respawnObj.AddComponent<RespawnSystem>();

            // Audio Controller
            var audioObj = new GameObject("StoryBeatAudioController");
            audioObj.transform.SetParent(gameRoot.transform);
            var audioCtrl = audioObj.AddComponent<SFS.Audio.StoryBeatAudioController>();

            // Create audio sources
            audioCtrl.baseAmbient = CreateAudioSource(audioObj, "BaseAmbient");
            audioCtrl.tensionLayer = CreateAudioSource(audioObj, "TensionLayer");
            audioCtrl.calmLayer = CreateAudioSource(audioObj, "CalmLayer");
            audioCtrl.societyLayer = CreateAudioSource(audioObj, "SocietyLayer");
        }

        static AudioSource CreateAudioSource(GameObject parent, string name)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent.transform);
            var source = obj.AddComponent<AudioSource>();
            source.loop = true;
            source.playOnAwake = false;
            source.spatialBlend = 0f; // 2D
            return source;
        }

        static void CreatePlayer()
        {
            var player = new GameObject("Player");
            player.tag = "Player";
            player.layer = LayerMask.NameToLayer("Default");
            player.transform.position = new Vector3(0, 1, 0);

            // Character Controller
            var cc = player.AddComponent<CharacterController>();
            cc.height = 2f;
            cc.radius = 0.5f;
            cc.center = new Vector3(0, 1, 0);

            // Player Controller
            player.AddComponent<PlayerController>();

            // Collectible Tracker
            player.AddComponent<CollectibleTracker>();

            // Visual placeholder
            var visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visual.name = "Visual";
            visual.transform.SetParent(player.transform);
            visual.transform.localPosition = new Vector3(0, 1, 0);
            DestroyImmediate(visual.GetComponent<Collider>());

            var mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.3f, 0.5f, 0.7f);
            visual.GetComponent<Renderer>().material = mat;

            // Add animator (will need controller assigned)
            var animator = visual.AddComponent<Animator>();

            // Animator Driver
            var driver = player.AddComponent<PlayerAnimatorDriver>();
            driver.animator = animator;

            // Set as default spawn
            var respawn = Object.FindObjectOfType<RespawnSystem>();
            if (respawn)
            {
                respawn.defaultSpawn = player.transform;
            }
        }

        static void CreateCameraRig()
        {
            var mainCam = UnityEngine.Camera.main;
            if (!mainCam)
            {
                var camObj = new GameObject("Main Camera");
                camObj.tag = "MainCamera";
                mainCam = camObj.AddComponent<UnityEngine.Camera>();
                camObj.AddComponent<AudioListener>();
            }

            mainCam.transform.position = new Vector3(0, 4, -6);

            // Third Person Rig
            var rig = mainCam.gameObject.AddComponent<ThirdPersonCameraRig>();
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player) rig.target = player.transform;

            // Story Beat Camera Modifier
            var modifier = mainCam.gameObject.AddComponent<StoryBeatCameraModifier>();
            modifier.cameraRig = rig;
        }

        static void CreateEnvironment()
        {
            var world = new GameObject("--- WORLD ---");

            // Ground
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.SetParent(world.transform);
            ground.transform.localScale = new Vector3(20, 1, 20);
            var groundMat = new Material(Shader.Find("Standard"));
            groundMat.color = new Color(0.2f, 0.25f, 0.2f);
            ground.GetComponent<Renderer>().material = groundMat;

            // ===== BEAT 1: ARRIVAL (z = 0-10) =====
            CreateBeatZone(world.transform, StoryBeat.Arrival, new Vector3(0, 2, 5), new Vector3(10, 4, 10), "Beat1_Arrival");

            var arrivalIntro = new GameObject("ArrivalIntro");
            arrivalIntro.transform.SetParent(world.transform);
            arrivalIntro.AddComponent<ArrivalIntro>();

            // ===== BEAT 2: FIRST CONTACT (z = 10-20) =====
            CreateBeatZone(world.transform, StoryBeat.FirstContact, new Vector3(0, 2, 15), new Vector3(10, 4, 10), "Beat2_FirstContact");

            // First collectible
            var firstCollect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            firstCollect.name = "FirstCollectible";
            firstCollect.transform.SetParent(world.transform);
            firstCollect.transform.position = new Vector3(0, 1.5f, 12);
            firstCollect.transform.localScale = Vector3.one * 0.5f;
            firstCollect.GetComponent<Collider>().isTrigger = true;
            firstCollect.AddComponent<FirstCollectibleFeedback>();
            var collectMat = new Material(Shader.Find("Standard"));
            collectMat.color = new Color(1f, 0.8f, 0.3f);
            collectMat.EnableKeyword("_EMISSION");
            collectMat.SetColor("_EmissionColor", new Color(0.5f, 0.4f, 0.1f));
            firstCollect.GetComponent<Renderer>().material = collectMat;

            // ===== BEAT 3: COMPRESSION (z = 20-35) =====
            CreateBeatZone(world.transform, StoryBeat.Compression, new Vector3(0, 2, 27), new Vector3(6, 4, 14), "Beat3_Compression");

            var compressionZone = CreateTriggerZone(world.transform, "CompressionZone", new Vector3(0, 2, 27), new Vector3(6, 4, 14));
            compressionZone.AddComponent<CompressionZone>();

            // Narrow walls
            CreateWall(world.transform, new Vector3(-4, 2, 27), new Vector3(1, 4, 14), "WallLeft");
            CreateWall(world.transform, new Vector3(4, 2, 27), new Vector3(1, 4, 14), "WallRight");

            // ===== BEAT 4: CHOOSING REST (z = 35-45) =====
            CreateBeatZone(world.transform, StoryBeat.ChoosingRest, new Vector3(0, 2, 40), new Vector3(12, 4, 10), "Beat4_Rest");

            var restZone = CreateTriggerZone(world.transform, "RestZone", new Vector3(0, 2, 40), new Vector3(12, 4, 10));
            restZone.AddComponent<RestZone>();

            // Checkpoint
            var checkpoint = CreateTriggerZone(world.transform, "Checkpoint", new Vector3(0, 1, 38), new Vector3(4, 2, 2));
            checkpoint.AddComponent<Checkpoint>();

            // ===== BEAT 5: THE SOCIETY (z = 45-60) =====
            CreateBeatZone(world.transform, StoryBeat.TheSociety, new Vector3(0, 2, 52), new Vector3(16, 4, 14), "Beat5_Society");

            // Society Group
            var societyGroup = new GameObject("SocietyGroup");
            societyGroup.transform.SetParent(world.transform);
            societyGroup.transform.position = new Vector3(0, 0, 55);
            var group = societyGroup.AddComponent<SocietyGroup>();

            // Create society members
            var members = new SocietyMember[5];
            Vector3[] memberPositions = {
                new Vector3(-3, 0, 54),
                new Vector3(-1.5f, 0, 56),
                new Vector3(0, 0, 55),
                new Vector3(1.5f, 0, 56),
                new Vector3(3, 0, 54)
            };

            for (int i = 0; i < 5; i++)
            {
                var member = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                member.name = $"SocietyMember_{i}";
                member.transform.SetParent(societyGroup.transform);
                member.transform.position = memberPositions[i];
                DestroyImmediate(member.GetComponent<Collider>());

                var memberComp = member.AddComponent<SocietyMember>();
                memberComp.group = group;
                memberComp.syncOffset = i * 0.2f;
                members[i] = memberComp;

                var memberMat = new Material(Shader.Find("Standard"));
                memberMat.color = new Color(0.6f, 0.5f, 0.4f);
                member.GetComponent<Renderer>().material = memberMat;
            }
            group.members = members;

            // ===== BEAT 6: SHARED DIFFICULTY (z = 60-80) =====
            CreateBeatZone(world.transform, StoryBeat.SharedDifficulty, new Vector3(0, 2, 70), new Vector3(8, 4, 20), "Beat6_SharedDifficulty");

            var supportZone = CreateTriggerZone(world.transform, "SupportedMovementZone", new Vector3(0, 2, 70), new Vector3(8, 4, 20));
            supportZone.AddComponent<SupportedMovementZone>();

            // Companion (starts inactive)
            var companion = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            companion.name = "Companion";
            companion.transform.SetParent(world.transform);
            companion.transform.position = new Vector3(-2, 1, 62);
            DestroyImmediate(companion.GetComponent<Collider>());
            companion.AddComponent<Companion>();
            var compMat = new Material(Shader.Find("Standard"));
            compMat.color = new Color(0.4f, 0.6f, 0.5f);
            companion.GetComponent<Renderer>().material = compMat;

            // Platform challenges
            CreatePlatform(world.transform, new Vector3(-2, 0.5f, 68), new Vector3(3, 1, 3));
            CreatePlatform(world.transform, new Vector3(2, 1f, 72), new Vector3(3, 1, 3));
            CreatePlatform(world.transform, new Vector3(-1, 1.5f, 76), new Vector3(3, 1, 3));

            // ===== BEAT 7: QUIET BELONGING (z = 80-95) =====
            CreateBeatZone(world.transform, StoryBeat.QuietBelonging, new Vector3(0, 2, 87), new Vector3(14, 4, 14), "Beat7_Belonging");

            var endingObj = new GameObject("BelongingEnding");
            endingObj.transform.SetParent(world.transform);
            endingObj.transform.position = new Vector3(0, 0, 90);
            var ending = endingObj.AddComponent<BelongingEnding>();
            ending.societyGroup = group;

            // Final camera position
            var finalCam = new GameObject("FinalCameraPosition");
            finalCam.transform.SetParent(world.transform);
            finalCam.transform.position = new Vector3(0, 5, 82);
            finalCam.transform.LookAt(new Vector3(0, 1, 90));
            ending.finalCameraPosition = finalCam.transform;

            // More collectibles throughout
            CreateCollectible(world.transform, new Vector3(2, 1, 18));
            CreateCollectible(world.transform, new Vector3(-2, 1, 32));
            CreateCollectible(world.transform, new Vector3(0, 1, 42));
            CreateCollectible(world.transform, new Vector3(3, 1, 58));
            CreateCollectible(world.transform, new Vector3(0, 2, 74));
        }

        static void CreateBeatZone(Transform parent, StoryBeat beat, Vector3 position, Vector3 size, string name)
        {
            var zone = new GameObject(name);
            zone.transform.SetParent(parent);
            zone.transform.position = position;

            var box = zone.AddComponent<BoxCollider>();
            box.size = size;
            box.isTrigger = true;

            var beatZone = zone.AddComponent<StoryBeatZone>();
            beatZone.beat = beat;
        }

        static GameObject CreateTriggerZone(Transform parent, string name, Vector3 position, Vector3 size)
        {
            var zone = new GameObject(name);
            zone.transform.SetParent(parent);
            zone.transform.position = position;

            var box = zone.AddComponent<BoxCollider>();
            box.size = size;
            box.isTrigger = true;

            return zone;
        }

        static void CreateWall(Transform parent, Vector3 position, Vector3 size, string name)
        {
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.SetParent(parent);
            wall.transform.position = position;
            wall.transform.localScale = size;

            var mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.35f, 0.35f, 0.4f);
            wall.GetComponent<Renderer>().material = mat;
        }

        static void CreatePlatform(Transform parent, Vector3 position, Vector3 size)
        {
            var platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
            platform.name = "Platform";
            platform.transform.SetParent(parent);
            platform.transform.position = position;
            platform.transform.localScale = size;

            var mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.4f, 0.45f, 0.4f);
            platform.GetComponent<Renderer>().material = mat;
        }

        static void CreateCollectible(Transform parent, Vector3 position)
        {
            var collect = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            collect.name = "Collectible";
            collect.transform.SetParent(parent);
            collect.transform.position = position;
            collect.transform.localScale = Vector3.one * 0.4f;
            collect.GetComponent<Collider>().isTrigger = true;
            collect.AddComponent<Collectible>();

            var mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.9f, 0.7f, 0.2f);
            collect.GetComponent<Renderer>().material = mat;
        }

        static void CreateUI()
        {
            // Create Canvas
            var canvasObj = new GameObject("Canvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            // Collectible Counter (placeholder - needs TMPro)
            var counterObj = new GameObject("CollectibleCounter");
            counterObj.transform.SetParent(canvasObj.transform);
            var counterRect = counterObj.AddComponent<RectTransform>();
            counterRect.anchorMin = new Vector2(0, 1);
            counterRect.anchorMax = new Vector2(0, 1);
            counterRect.pivot = new Vector2(0, 1);
            counterRect.anchoredPosition = new Vector2(20, -20);
            counterRect.sizeDelta = new Vector2(200, 50);
            counterObj.AddComponent<CollectibleCounterUI>();

            // Pause Menu (hidden by default)
            var pauseMenu = new GameObject("PauseMenu");
            pauseMenu.transform.SetParent(canvasObj.transform);
            pauseMenu.SetActive(false);

            var pauseRect = pauseMenu.AddComponent<RectTransform>();
            pauseRect.anchorMin = Vector2.zero;
            pauseRect.anchorMax = Vector2.one;
            pauseRect.sizeDelta = Vector2.zero;

            var pauseImage = pauseMenu.AddComponent<UnityEngine.UI.Image>();
            pauseImage.color = new Color(0, 0, 0, 0.7f);

            // Pause Manager
            var pauseManager = canvasObj.AddComponent<PauseManager>();
            pauseManager.pauseMenuRoot = pauseMenu;
        }

        static void CreateLighting()
        {
            // Main directional light
            var lightObj = new GameObject("Directional Light");
            var light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(1f, 0.95f, 0.9f);
            light.intensity = 1f;
            light.shadows = LightShadows.Soft;
            lightObj.transform.rotation = Quaternion.Euler(50, -30, 0);

            // Ambient
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.3f, 0.35f, 0.4f);

            // Fog for atmosphere
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogColor = new Color(0.5f, 0.55f, 0.6f);
            RenderSettings.fogStartDistance = 30f;
            RenderSettings.fogEndDistance = 100f;
        }
    }
}
#endif
