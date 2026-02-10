using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace SFS.Visual
{
    /// <summary>
    /// Manages Drift distortion layers — visual corruption that recedes
    /// as the player Reads and Rewrites defaults.
    ///
    /// Each DistortionZone in the scene registers with this manager.
    /// When a zone's associated default is rewritten, the zone transitions
    /// from Drift visuals (grey, metallic, glitchy) to restored visuals
    /// (amber, wooden, blooming).
    ///
    /// Works with:
    ///   - Material Property Blocks (per-renderer tint/dissolve)
    ///   - Post-processing Volume weights
    ///   - Shader keyword toggles
    ///   - Particle system emission rates
    /// </summary>
    public class DistortionLayerManager : MonoBehaviour
    {
        public static DistortionLayerManager Instance { get; private set; }

        // ── Events ──────────────────────────────────────────────
        public static event Action<string> OnZoneRestored;
        public static event Action<float> OnGlobalDistortionChanged;

        // ── Inspector ───────────────────────────────────────────
        [Header("Global Drift Visuals")]
        public Volume DriftPostProcessVolume;
        public float DistortionTransitionDuration = 3f;

        [Header("Color Palette")]
        public Color DriftTint = new(0.45f, 0.45f, 0.50f, 1f);  // grey-blue
        public Color RestoredTint = new(0.85f, 0.65f, 0.25f, 1f); // amber

        [Header("Shader Properties")]
        public string DriftAmountProperty = "_DriftAmount";
        public string TintProperty = "_BaseColor";
        public string DissolveProperty = "_DissolveProgress";

        // ── State ───────────────────────────────────────────────
        readonly Dictionary<string, DistortionZone> _zones = new();
        float _globalDistortion = 1f;
        float _targetGlobalDistortion = 1f;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(this); return; }
            Instance = this;
        }

        void Start()
        {
            Core.DefaultsRegistry.OnDefaultRewritten += HandleDefaultRewritten;
            Core.SFSGameState.OnDriftChanged += HandleDriftChanged;
        }

        void OnDestroy()
        {
            Core.DefaultsRegistry.OnDefaultRewritten -= HandleDefaultRewritten;
            Core.SFSGameState.OnDriftChanged -= HandleDriftChanged;
        }

        void Update()
        {
            // Smooth global distortion
            _globalDistortion = Mathf.MoveTowards(
                _globalDistortion, _targetGlobalDistortion,
                Time.deltaTime / Mathf.Max(DistortionTransitionDuration, 0.01f));

            // Drive post-processing volume weight
            if (DriftPostProcessVolume != null)
                DriftPostProcessVolume.weight = _globalDistortion;

            // Update all zone transitions
            foreach (var zone in _zones.Values)
                zone.UpdateTransition(Time.deltaTime);
        }

        // ═════════════════════════════════════════════════════════
        //  PUBLIC API
        // ═════════════════════════════════════════════════════════

        /// <summary>Register a zone. Called by DistortionZone.OnEnable.</summary>
        public void RegisterZone(DistortionZone zone)
        {
            _zones[zone.AssociatedDefaultKey] = zone;
            zone.SetDistortion(1f); // Start fully corrupted
        }

        /// <summary>Unregister a zone. Called by DistortionZone.OnDisable.</summary>
        public void UnregisterZone(DistortionZone zone)
        {
            _zones.Remove(zone.AssociatedDefaultKey);
        }

        /// <summary>Current global distortion level (0 = fully restored, 1 = full drift).</summary>
        public float GlobalDistortion => _globalDistortion;

        /// <summary>Force a zone to a specific distortion level.</summary>
        public void SetZoneDistortion(string defaultKey, float amount)
        {
            if (_zones.TryGetValue(defaultKey, out var zone))
                zone.SetDistortion(amount);
        }

        // ── Event Handlers ──────────────────────────────────────

        void HandleDefaultRewritten(string key)
        {
            if (_zones.TryGetValue(key, out var zone))
            {
                zone.BeginRestoration();
                OnZoneRestored?.Invoke(key);
                Debug.Log($"[SFS Distortion] Zone '{key}' restoration begun.");
            }
        }

        void HandleDriftChanged(float drift)
        {
            _targetGlobalDistortion = drift;
            OnGlobalDistortionChanged?.Invoke(drift);
        }
    }

    /// <summary>
    /// A region in the world whose visuals are corrupted by Drift.
    /// Attach to a parent GameObject; all child Renderers and ParticleSystems
    /// will be affected.
    ///
    /// Set AssociatedDefaultKey to match a key in DefaultsRegistry.
    /// When that default is rewritten, this zone transitions from
    /// Drift visuals to restored visuals.
    /// </summary>
    public class DistortionZone : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Key from DefaultsRegistry that controls this zone.")]
        public string AssociatedDefaultKey;

        [Header("Visual Targets")]
        public Renderer[] AffectedRenderers;
        public ParticleSystem[] DriftParticles;
        public GameObject[] HiddenUntilRestored;
        public Light[] RestoredLights;

        [Header("Transition")]
        public float TransitionDuration = 2.5f;
        public AnimationCurve TransitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        // ── State ───────────────────────────────────────────────
        float _distortion = 1f;
        float _targetDistortion = 1f;
        bool _transitioning;

        readonly MaterialPropertyBlock _mpb = new();

        void OnEnable()
        {
            if (AffectedRenderers == null || AffectedRenderers.Length == 0)
                AffectedRenderers = GetComponentsInChildren<Renderer>();
            if (DriftParticles == null || DriftParticles.Length == 0)
                DriftParticles = GetComponentsInChildren<ParticleSystem>();

            if (DistortionLayerManager.Instance != null)
                DistortionLayerManager.Instance.RegisterZone(this);
        }

        void OnDisable()
        {
            if (DistortionLayerManager.Instance != null)
                DistortionLayerManager.Instance.UnregisterZone(this);
        }

        /// <summary>Begin the restoration transition (drift → restored).</summary>
        public void BeginRestoration()
        {
            _targetDistortion = 0f;
            _transitioning = true;
        }

        /// <summary>Set distortion directly (0 = restored, 1 = full drift).</summary>
        public void SetDistortion(float amount)
        {
            _distortion = Mathf.Clamp01(amount);
            _targetDistortion = _distortion;
            ApplyDistortion();
        }

        /// <summary>Called by manager each frame during transitions.</summary>
        public void UpdateTransition(float deltaTime)
        {
            if (!_transitioning) return;

            _distortion = Mathf.MoveTowards(
                _distortion, _targetDistortion,
                deltaTime / Mathf.Max(TransitionDuration, 0.01f));

            ApplyDistortion();

            if (Mathf.Approximately(_distortion, _targetDistortion))
                _transitioning = false;
        }

        void ApplyDistortion()
        {
            float t = TransitionCurve.Evaluate(1f - _distortion); // 0=drift, 1=restored
            var mgr = DistortionLayerManager.Instance;
            if (mgr == null) return;

            // ── Material tinting ────────────────────────────────
            Color tint = Color.Lerp(mgr.DriftTint, mgr.RestoredTint, t);
            foreach (var rend in AffectedRenderers)
            {
                if (rend == null) continue;
                rend.GetPropertyBlock(_mpb);
                _mpb.SetColor(mgr.TintProperty, tint);
                _mpb.SetFloat(mgr.DriftAmountProperty, _distortion);
                _mpb.SetFloat(mgr.DissolveProperty, 1f - t);
                rend.SetPropertyBlock(_mpb);
            }

            // ── Drift particles fade out as zone restores ───────
            foreach (var ps in DriftParticles)
            {
                if (ps == null) continue;
                var emission = ps.emission;
                emission.rateOverTimeMultiplier = _distortion * 10f;
            }

            // ── Hidden objects appear on restoration ────────────
            if (HiddenUntilRestored != null)
            {
                bool show = t > 0.5f;
                foreach (var go in HiddenUntilRestored)
                    if (go != null) go.SetActive(show);
            }

            // ── Lights brighten on restoration ──────────────────
            if (RestoredLights != null)
            {
                foreach (var light in RestoredLights)
                    if (light != null) light.intensity = t * 2f;
            }
        }
    }
}
