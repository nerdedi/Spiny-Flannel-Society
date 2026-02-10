#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;

namespace SFS.Editor
{
    /// <summary>
    /// One-click editor wizard that builds the Chapter 1 opening scene:
    /// "Windgap Academy — Arrival Platform" → Gear Corridor → Courtyard.
    ///
    /// Creates all geometry, lighting, interaction objects, DAZIE triggers,
    /// NPC, chapter title zone, and systems (DefaultsRegistry, GameState,
    /// AudioTriggerManager, DistortionLayerManager).
    ///
    /// Menu: SFS → Setup → Build First Playable Room
    /// </summary>
    public class FirstPlayableRoomBuilder : EditorWindow
    {
        [MenuItem("SFS/Setup/Build First Playable Room")]
        public static void BuildRoom()
        {
            if (!EditorUtility.DisplayDialog(
                "Build First Playable Room",
                "This will create a new scene with the Chapter 1 opening.\n\n" +
                "Windgap Academy: Arrival Platform → Gear Corridor → Courtyard.\n\n" +
                "Continue?", "Build", "Cancel"))
                return;

            string scenePath = "Assets/_SFS/Scenes/Chapter1_WindgapAcademy.unity";
            string dir = Path.GetDirectoryName(scenePath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            // ── Systems ─────────────────────────────────────────
            BuildSystems();

            // ── Lighting ────────────────────────────────────────
            BuildLighting();

            // ── Geometry ────────────────────────────────────────
            BuildArrivalPlatform();
            BuildGearCorridor();
            BuildCourtyard();

            // ── Player ──────────────────────────────────────────
            BuildPlayer();

            // ── Interactables ───────────────────────────────────
            BuildReadableGearMechanism();
            BuildNPC();

            // ── DAZIE Triggers ──────────────────────────────────
            BuildDAZIETriggers();

            // ── Chapter Title ───────────────────────────────────
            BuildChapterTitleZone();

            // Save
            EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log($"[SFS] First playable room built at {scenePath}");
            EditorUtility.DisplayDialog("Done", "Chapter 1 scene built!\n\n" + scenePath, "OK");
        }

        // ═════════════════════════════════════════════════════════
        //  SYSTEMS
        // ═════════════════════════════════════════════════════════

        static void BuildSystems()
        {
            // Core systems on a persistent root
            var systems = new GameObject("── SYSTEMS ──");

            // DefaultsRegistry
            systems.AddComponent<Core.DefaultsRegistry>();

            // GameState
            systems.AddComponent<Core.SFSGameState>();

            // Audio (will need clips assigned manually)
            systems.AddComponent<Audio.AudioTriggerManager>();

            // Distortion Layer Manager
            systems.AddComponent<Visual.DistortionLayerManager>();
        }

        // ═════════════════════════════════════════════════════════
        //  LIGHTING
        // ═════════════════════════════════════════════════════════

        static void BuildLighting()
        {
            // Remove default directional light
            var existingLight = GameObject.Find("Directional Light");
            if (existingLight != null) Object.DestroyImmediate(existingLight);

            // Warm directional light — Australian golden hour
            var sunGO = new GameObject("Sun_WarmAmbient");
            var sun = sunGO.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.color = new Color(1f, 0.85f, 0.6f); // warm amber
            sun.intensity = 1.2f;
            sun.shadows = LightShadows.Soft;
            sunGO.transform.rotation = Quaternion.Euler(45f, -30f, 0f);

            // Ambient settings (will need RenderSettings in URP)
            RenderSettings.ambientLight = new Color(0.6f, 0.55f, 0.45f);
            RenderSettings.fogColor = new Color(0.85f, 0.75f, 0.6f);
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogStartDistance = 40f;
            RenderSettings.fogEndDistance = 120f;
        }

        // ═════════════════════════════════════════════════════════
        //  ARRIVAL PLATFORM (Beat 1: 0:00-0:08)
        // ═════════════════════════════════════════════════════════

        static void BuildArrivalPlatform()
        {
            var root = new GameObject("── ARRIVAL PLATFORM ──");
            root.transform.position = Vector3.zero;

            // Sandstone floor — wide, generous
            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "SandstoneFloor";
            floor.transform.SetParent(root.transform);
            floor.transform.localPosition = new Vector3(0, -0.5f, 0);
            floor.transform.localScale = new Vector3(12f, 1f, 20f);
            SetColor(floor, new Color(0.82f, 0.72f, 0.55f)); // sandstone

            // Eucalyptus canopy suggestion — tall cylinders
            for (int i = 0; i < 4; i++)
            {
                float x = (i % 2 == 0 ? -5f : 5f);
                float z = (i < 2 ? -3f : 8f);

                var trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                trunk.name = $"EucalyptusTrunk_{i}";
                trunk.transform.SetParent(root.transform);
                trunk.transform.localPosition = new Vector3(x, 4f, z);
                trunk.transform.localScale = new Vector3(0.4f, 8f, 0.4f);
                SetColor(trunk, new Color(0.65f, 0.6f, 0.5f));

                var canopy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                canopy.name = $"EucalyptusCanopy_{i}";
                canopy.transform.SetParent(root.transform);
                canopy.transform.localPosition = new Vector3(x, 10f, z);
                canopy.transform.localScale = new Vector3(5f, 4f, 5f);
                SetColor(canopy, new Color(0.35f, 0.55f, 0.3f));
            }

            // Low walls — soft brutalist curves (cubes as stand-in)
            var wallL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wallL.name = "Wall_Left";
            wallL.transform.SetParent(root.transform);
            wallL.transform.localPosition = new Vector3(-6.5f, 1.5f, 5f);
            wallL.transform.localScale = new Vector3(1f, 3f, 14f);
            SetColor(wallL, new Color(0.75f, 0.68f, 0.58f));

            var wallR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wallR.name = "Wall_Right";
            wallR.transform.SetParent(root.transform);
            wallR.transform.localPosition = new Vector3(6.5f, 1.5f, 5f);
            wallR.transform.localScale = new Vector3(1f, 3f, 14f);
            SetColor(wallR, new Color(0.75f, 0.68f, 0.58f));
        }

        // ═════════════════════════════════════════════════════════
        //  GEAR CORRIDOR (Beats 2-5: 0:08-0:45)
        // ═════════════════════════════════════════════════════════

        static void BuildGearCorridor()
        {
            var root = new GameObject("── GEAR CORRIDOR ──");
            root.transform.position = new Vector3(0, 0, 15f);

            // Narrower corridor floor
            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "CorridorFloor";
            floor.transform.SetParent(root.transform);
            floor.transform.localPosition = new Vector3(0, -0.5f, 8f);
            floor.transform.localScale = new Vector3(6f, 1f, 18f);
            SetColor(floor, new Color(0.7f, 0.65f, 0.55f));

            // Corridor walls (taller, closer)
            var wallL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wallL.name = "CorridorWall_L";
            wallL.transform.SetParent(root.transform);
            wallL.transform.localPosition = new Vector3(-3.5f, 2.5f, 8f);
            wallL.transform.localScale = new Vector3(1f, 5f, 18f);
            SetColor(wallL, new Color(0.6f, 0.58f, 0.55f));

            var wallR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wallR.name = "CorridorWall_R";
            wallR.transform.SetParent(root.transform);
            wallR.transform.localPosition = new Vector3(3.5f, 2.5f, 8f);
            wallR.transform.localScale = new Vector3(1f, 5f, 18f);
            SetColor(wallR, new Color(0.6f, 0.58f, 0.55f));

            // Three gear platforms — the mechanical obstacle
            for (int i = 0; i < 3; i++)
            {
                var gear = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                gear.name = $"GearPlatform_{i}";
                gear.transform.SetParent(root.transform);
                gear.transform.localPosition = new Vector3(0, 0.2f, 4f + i * 4f);
                gear.transform.localScale = new Vector3(3f, 0.3f, 3f);
                SetColor(gear, new Color(0.5f, 0.5f, 0.55f)); // metallic grey — Drift

                // Add a simple rotation script marker
                var spinner = gear.AddComponent<GearSpinner>();
                spinner.RotationSpeed = 60f + i * 20f;
            }

            // Distortion zone over the gears
            var distortionGO = new GameObject("GearDistortionZone");
            distortionGO.transform.SetParent(root.transform);
            distortionGO.transform.localPosition = new Vector3(0, 1f, 8f);
            var dz = distortionGO.AddComponent<Visual.DistortionZone>();
            dz.AssociatedDefaultKey = "timing_window";
        }

        // ═════════════════════════════════════════════════════════
        //  COURTYARD (Beats 6-7: 0:45-1:00)
        // ═════════════════════════════════════════════════════════

        static void BuildCourtyard()
        {
            var root = new GameObject("── COURTYARD ──");
            root.transform.position = new Vector3(0, 0, 35f);

            // Open floor
            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "CourtyardFloor";
            floor.transform.SetParent(root.transform);
            floor.transform.localPosition = new Vector3(0, -0.5f, 5f);
            floor.transform.localScale = new Vector3(16f, 1f, 12f);
            SetColor(floor, new Color(0.82f, 0.72f, 0.55f)); // sandstone

            // Bench (appears after rewrite — HiddenUntilRestored)
            var bench = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bench.name = "Bench_HiddenUntilRestored";
            bench.transform.SetParent(root.transform);
            bench.transform.localPosition = new Vector3(-3f, 0.4f, 4f);
            bench.transform.localScale = new Vector3(2.5f, 0.5f, 0.8f);
            SetColor(bench, new Color(0.55f, 0.4f, 0.25f)); // wood
            bench.SetActive(false); // Hidden until timing_window rewritten

            // Plant (blooms after rewrite)
            var plant = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            plant.name = "BloomingPlant_HiddenUntilRestored";
            plant.transform.SetParent(root.transform);
            plant.transform.localPosition = new Vector3(4f, 0.6f, 3f);
            plant.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            SetColor(plant, new Color(0.4f, 0.7f, 0.35f));
            plant.SetActive(false);

            // Wind chime (subtle light)
            var chimeGO = new GameObject("WindChime_Light");
            chimeGO.transform.SetParent(root.transform);
            chimeGO.transform.localPosition = new Vector3(0, 4f, 5f);
            var chimeLight = chimeGO.AddComponent<Light>();
            chimeLight.type = LightType.Point;
            chimeLight.color = new Color(0.9f, 0.8f, 0.5f);
            chimeLight.intensity = 0f; // DistortionZone.RestoredLights will drive this
            chimeLight.range = 10f;

            // Courtyard distortion zone (linked to timing_window too)
            var distortionGO = new GameObject("CourtyardDistortionZone");
            distortionGO.transform.SetParent(root.transform);
            distortionGO.transform.localPosition = Vector3.zero;
            var dz = distortionGO.AddComponent<Visual.DistortionZone>();
            dz.AssociatedDefaultKey = "timing_window";
            dz.HiddenUntilRestored = new[] { bench, plant };
            dz.RestoredLights = new[] { chimeLight };

            // Vista beyond (the wider Society view)
            var vistaFloor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            vistaFloor.name = "VistaFloor";
            vistaFloor.transform.SetParent(root.transform);
            vistaFloor.transform.localPosition = new Vector3(0, -0.5f, 16f);
            vistaFloor.transform.localScale = new Vector3(30f, 1f, 20f);
            SetColor(vistaFloor, new Color(0.75f, 0.65f, 0.5f));
        }

        // ═════════════════════════════════════════════════════════
        //  PLAYER
        // ═════════════════════════════════════════════════════════

        static void BuildPlayer()
        {
            var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.tag = "Player";
            player.transform.position = new Vector3(0, 1f, -2f);
            SetColor(player, new Color(0.3f, 0.5f, 0.7f));

            // Add CharacterController
            var cc = player.AddComponent<CharacterController>();
            cc.height = 2f;
            cc.radius = 0.5f;
            cc.center = Vector3.zero;

            // Camera
            var camGO = new GameObject("PlayerCamera");
            camGO.transform.SetParent(player.transform);
            camGO.transform.localPosition = new Vector3(0, 3f, -6f);
            camGO.transform.localRotation = Quaternion.Euler(15f, 0, 0);
            var cam = camGO.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.6f, 0.7f, 0.85f); // sky blue

            // Remove default camera
            var mainCam = GameObject.Find("Main Camera");
            if (mainCam != null) Object.DestroyImmediate(mainCam);
        }

        // ═════════════════════════════════════════════════════════
        //  READABLE GEAR MECHANISM
        // ═════════════════════════════════════════════════════════

        static void BuildReadableGearMechanism()
        {
            var root = new GameObject("ReadableGearMechanism");
            root.transform.position = new Vector3(0, 1.5f, 23f); // center of gear corridor

            // Trigger collider
            var col = root.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 3f;

            // Readable component
            var readable = root.AddComponent<Interaction.ReadableObject>();
            readable.DefaultKey = "timing_window";
            readable.InteractionRadius = 4f;

            // Highlight — amber sphere
            var highlight = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            highlight.name = "HighlightEffect";
            highlight.transform.SetParent(root.transform);
            highlight.transform.localPosition = Vector3.zero;
            highlight.transform.localScale = Vector3.one * 0.6f;
            SetColor(highlight, new Color(1f, 0.75f, 0.2f, 0.5f));
            highlight.SetActive(false);
            readable.HighlightEffect = highlight;

            // Remove collider from highlight (visual only)
            Object.DestroyImmediate(highlight.GetComponent<Collider>());
        }

        // ═════════════════════════════════════════════════════════
        //  NPC
        // ═════════════════════════════════════════════════════════

        static void BuildNPC()
        {
            var npc = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            npc.name = "NPC_WaitingPerson";
            npc.transform.position = new Vector3(-3f, 1f, 39f); // Courtyard bench area
            SetColor(npc, new Color(0.7f, 0.5f, 0.3f));

            var reactor = npc.AddComponent<Interaction.NPCReactor>();
            reactor.TriggerDefaultKey = "timing_window";
            reactor.ReactionDialogue = "Oh — the gears slowed. I've been waiting here for… I don't know how long. I couldn't get through.";

            // Walk target — through the corridor
            var walkTarget = new GameObject("NPC_WalkTarget");
            walkTarget.transform.position = new Vector3(0, 1f, 50f);
            reactor.WalkTarget = walkTarget.transform;
        }

        // ═════════════════════════════════════════════════════════
        //  DAZIE TRIGGERS
        // ═════════════════════════════════════════════════════════

        static void BuildDAZIETriggers()
        {
            // DAZIE line 1: At gear corridor entrance
            CreateDAZIETrigger(
                "DAZIE_GearIntro",
                new Vector3(0, 1f, 18f),
                new Vector3(6f, 4f, 2f),
                "The gears were tuned for one speed of thought. That's the assumption. Can you see it?"
            );

            // DAZIE line 2: After read
            CreateDAZIETrigger(
                "DAZIE_AfterRead",
                new Vector3(0, 1f, 25f),
                new Vector3(6f, 4f, 2f),
                "There it is."
            );

            // DAZIE line 3: Rewrite prompt
            CreateDAZIETrigger(
                "DAZIE_RewritePrompt",
                new Vector3(0, 1f, 27f),
                new Vector3(6f, 4f, 2f),
                "Now you can change it. Use Cushion — widen the window."
            );

            // DAZIE line 4: Courtyard — the thesis
            CreateDAZIETrigger(
                "DAZIE_Thesis",
                new Vector3(0, 1f, 44f),
                new Vector3(12f, 4f, 3f),
                "You didn't fix the gears. You didn't fix the person. You rewrote the assumption that was between them."
            );
        }

        static void CreateDAZIETrigger(string name, Vector3 pos, Vector3 size, string line)
        {
            var go = new GameObject(name);
            go.transform.position = pos;
            var col = go.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = size;
            var trigger = go.AddComponent<Interaction.DAZIETrigger>();
            trigger.DialogueLine = line;
        }

        // ═════════════════════════════════════════════════════════
        //  CHAPTER TITLE ZONE
        // ═════════════════════════════════════════════════════════

        static void BuildChapterTitleZone()
        {
            var go = new GameObject("ChapterTitleTrigger");
            go.transform.position = new Vector3(0, 1f, 47f);
            var col = go.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(14f, 4f, 3f);
            var card = go.AddComponent<Interaction.ChapterTitleCard>();
            card.ChapterTitle = "Chapter 1: Bract Theory";
            card.Subtitle = "Supports without proof.";
        }

        // ═════════════════════════════════════════════════════════
        //  UTILITIES
        // ═════════════════════════════════════════════════════════

        static void SetColor(GameObject go, Color color)
        {
            var rend = go.GetComponent<Renderer>();
            if (rend == null) return;

            // Create a unique material
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = color;
            mat.name = $"SFS_{go.name}_Mat";
            rend.sharedMaterial = mat;
        }
    }

    /// <summary>
    /// Simple gear spinning behaviour — rotates Y axis at a set speed.
    /// Speed will be modified by DefaultsRegistry.timing_window rewrite.
    /// </summary>
    public class GearSpinner : MonoBehaviour
    {
        public float RotationSpeed = 60f;
        float _baseSpeed;

        void Start()
        {
            _baseSpeed = RotationSpeed;
            Core.DefaultsRegistry.OnDefaultRewritten += HandleRewritten;
        }

        void OnDestroy()
        {
            Core.DefaultsRegistry.OnDefaultRewritten -= HandleRewritten;
        }

        void Update()
        {
            transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime);
        }

        void HandleRewritten(string key)
        {
            if (key == "timing_window")
            {
                // Slow down to 40% of original speed
                RotationSpeed = _baseSpeed * 0.4f;
                Debug.Log($"[SFS] Gear slowed: {RotationSpeed}°/s");
            }
        }
    }
}
#endif
