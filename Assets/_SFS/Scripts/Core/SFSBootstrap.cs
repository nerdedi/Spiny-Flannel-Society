using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace SFS.Core
{
    /// <summary>
    /// Bootstrap script that validates render pipeline, logs startup info,
    /// and ensures the game is ready to run.
    /// Attach to a GameObject in your startup scene or use [RuntimeInitializeOnLoadMethod].
    /// </summary>
    public class SFSBootstrap : MonoBehaviour
    {
        [Header("Startup Configuration")]
        [Tooltip("Scene to load after bootstrap (leave empty to stay in current scene)")]
        public string MainSceneName = "SFS_MainScene";

        [Tooltip("Enable verbose logging during startup")]
        public bool VerboseLogging = true;

        [Header("Pipeline Validation")]
        [Tooltip("Expected render pipeline type")]
        public string ExpectedPipeline = "UniversalRenderPipeline";

        private void Awake()
        {
            ValidateRenderPipeline();
            LogStartupInfo();
        }

        private void Start()
        {
            if (VerboseLogging)
            {
                Debug.Log("[SFS] Bootstrap complete. Game ready.");
            }

            // Load main scene if specified and not already there
            if (!string.IsNullOrEmpty(MainSceneName) &&
                SceneManager.GetActiveScene().name != MainSceneName)
            {
                LoadMainScene();
            }
        }

        private void ValidateRenderPipeline()
        {
            var pipeline = GraphicsSettings.currentRenderPipeline;

            if (pipeline == null)
            {
                Debug.LogWarning("[SFS] No Scriptable Render Pipeline active. Using Built-in Renderer.");
                Debug.LogWarning("[SFS] To use URP: Edit > Project Settings > Graphics > Scriptable Render Pipeline Settings");
                return;
            }

            string pipelineName = pipeline.GetType().Name;

            if (VerboseLogging)
            {
                Debug.Log($"[SFS] Render Pipeline: {pipelineName}");
                Debug.Log($"[SFS] Pipeline Asset: {pipeline.name}");
            }

            if (!pipelineName.Contains("Universal"))
            {
                Debug.LogWarning($"[SFS] Expected URP but found: {pipelineName}");
            }
            else
            {
                Debug.Log("[SFS] URP active and configured correctly.");
            }
        }

        private void LogStartupInfo()
        {
            if (!VerboseLogging) return;

            Debug.Log("═══════════════════════════════════════════════════════════════");
            Debug.Log("  SPINY FLANNEL SOCIETY - Bootstrap");
            Debug.Log("═══════════════════════════════════════════════════════════════");
            Debug.Log($"  Unity Version: {Application.unityVersion}");
            Debug.Log($"  Platform: {Application.platform}");
            Debug.Log($"  Scene: {SceneManager.GetActiveScene().name}");
            Debug.Log($"  Quality Level: {QualitySettings.names[QualitySettings.GetQualityLevel()]}");
            Debug.Log("═══════════════════════════════════════════════════════════════");
        }

        private void LoadMainScene()
        {
            if (VerboseLogging)
            {
                Debug.Log($"[SFS] Loading main scene: {MainSceneName}");
            }

            // Check if scene is in build settings
            int sceneIndex = SceneUtility.GetBuildIndexByScenePath($"Assets/_SFS/Scenes/{MainSceneName}.unity");

            if (sceneIndex >= 0)
            {
                SceneManager.LoadScene(MainSceneName);
            }
            else if (VerboseLogging)
            {
                Debug.LogWarning($"[SFS] Scene '{MainSceneName}' not found in Build Settings. Add it via File > Build Settings.");
            }
        }

        /// <summary>
        /// Static initialization that runs before any scene loads.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoad()
        {
            Debug.Log("[SFS] Spiny Flannel Society initializing...");

            // Validate critical systems
            if (GraphicsSettings.currentRenderPipeline != null)
            {
                Debug.Log($"[SFS] Pipeline ready: {GraphicsSettings.currentRenderPipeline.GetType().Name}");
            }
        }
    }
}
