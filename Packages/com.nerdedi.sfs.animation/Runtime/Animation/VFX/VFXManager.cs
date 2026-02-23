using UnityEngine;
using System;
using System.Collections.Generic;

namespace SFS.Animation
{
    /// <summary>
    /// Coordinates all visual effects for Spiny Flannel Society.
    /// Handles particle systems, trails, screen effects, and post-processing triggers.
    /// Designed for market-ready quality with story beat integration.
    /// </summary>
    public class VFXManager : MonoBehaviour
    {
        #region Singleton

        public static VFXManager Instance { get; private set; }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        #endregion

        #region Configuration

        [Header("═══════════════════════════════════════")]
        [Header("         VFX MANAGER                   ")]
        [Header("  Spiny Flannel Society Effects        ")]
        [Header("═══════════════════════════════════════")]

        [Header("VFX Pools")]
        public VFXPoolSettings poolSettings = new VFXPoolSettings();

        [Header("Combat Verb VFX")]
        public CombatVFXSettings combatVFX = new CombatVFXSettings();

        [Header("Windprint VFX")]
        public WindprintVFXSettings windprintVFX = new WindprintVFXSettings();

        [Header("Collectible VFX")]
        public CollectibleVFXSettings collectibleVFX = new CollectibleVFXSettings();

        [Header("Movement VFX")]
        public MovementVFXSettings movementVFX = new MovementVFXSettings();

        [Header("Screen Effects")]
        public ScreenEffectSettings screenEffects = new ScreenEffectSettings();

        [Header("Story Beat VFX")]
        public StoryBeatVFXSettings storyVFX = new StoryBeatVFXSettings();

        [Header("Pattern VFX (Antagonists)")]
        public PatternVFXSettings patternVFX = new PatternVFXSettings();

        #endregion

        #region Settings Classes

        [Serializable]
        public class VFXPoolSettings
        {
            public int initialPoolSize = 20;
            public int maxPoolSize = 100;
            public float effectLifetime = 3f;
        }

        [Serializable]
        public class CombatVFXSettings
        {
            [Header("Pulse")]
            public Color pulseColor = new Color(0.4f, 0.8f, 1f, 1f);
            [Range(0.5f, 3f)] public float pulseRadius = 1.5f;
            [Range(0.2f, 1f)] public float pulseDuration = 0.4f;
            public int pulseParticleCount = 30;

            [Header("Thread Lash")]
            public Color threadLashColor = new Color(1f, 0.9f, 0.5f, 1f);
            [Range(1f, 5f)] public float threadLength = 3f;
            [Range(0.3f, 0.8f)] public float threadDuration = 0.5f;
            public int threadSegments = 10;

            [Header("Radiant Hold")]
            public Color radiantHoldColor = new Color(1f, 0.7f, 0.3f, 1f);
            [Range(1f, 4f)] public float holdRadius = 2f;
            [Range(0.5f, 2f)] public float holdDuration = 1.2f;
            public int holdRingCount = 3;

            [Header("Edge Claim")]
            public Color edgeClaimColor = new Color(0.6f, 1f, 0.6f, 1f);
            [Range(0.5f, 3f)] public float edgeExpand = 2f;
            [Range(0.4f, 1f)] public float edgeDuration = 0.6f;

            [Header("Re-tune")]
            public Gradient reTuneGradient;
            [Range(1f, 5f)] public float reTuneRadius = 3f;
            [Range(1f, 3f)] public float reTuneDuration = 2f;
            public int reTuneWaveCount = 5;
        }

        [Serializable]
        public class WindprintVFXSettings
        {
            [Header("Cushion Mode")]
            public Color cushionColor = new Color(0.6f, 0.9f, 1f, 0.7f);
            [Range(0.5f, 2f)] public float cushionRadius = 1f;
            [Range(0.5f, 2f)] public float cushionPulseSpeed = 1f;
            public int cushionParticleRate = 10;

            [Header("Guard Mode")]
            public Color guardColor = new Color(1f, 0.8f, 0.4f, 0.8f);
            [Range(0.3f, 1.5f)] public float guardRadius = 0.8f;
            public int guardParticleRate = 15;

            [Header("Mode Transition")]
            [Range(0.2f, 0.8f)] public float transitionDuration = 0.4f;
            public int transitionBurstCount = 20;
        }

        [Serializable]
        public class CollectibleVFXSettings
        {
            [Header("Standard Collect")]
            public Color collectColor = new Color(1f, 0.95f, 0.6f, 1f);
            [Range(0.3f, 1f)] public float collectBurstDuration = 0.5f;
            public int collectParticleCount = 25;

            [Header("Principle Collect (Major)")]
            public Color principleColor = new Color(1f, 0.7f, 0.3f, 1f);
            [Range(0.5f, 1.5f)] public float principleBurstDuration = 1f;
            public int principleParticleCount = 50;
            public bool principleSlowMotion = true;
            [Range(0.1f, 0.5f)] public float principleSlowScale = 0.3f;

            [Header("Trail to Player")]
            public bool collectTrailToPlayer = true;
            [Range(0.2f, 1f)] public float trailDuration = 0.4f;
        }

        [Serializable]
        public class MovementVFXSettings
        {
            [Header("Footsteps")]
            public bool footstepDust = true;
            public Color dustColor = new Color(0.8f, 0.7f, 0.6f, 0.5f);
            public int dustParticleCount = 5;

            [Header("Land Impact")]
            public bool landImpact = true;
            [Range(0.2f, 0.6f)] public float impactDuration = 0.3f;
            public int impactParticleCount = 15;

            [Header("Air Dash")]
            public bool airDashTrail = true;
            public Color dashColor = new Color(0.5f, 0.8f, 1f, 0.6f);
            [Range(0.2f, 0.5f)] public float dashTrailDuration = 0.3f;

            [Header("Wall Run")]
            public bool wallRunSparks = true;
            public Color sparkColor = new Color(1f, 0.9f, 0.7f, 0.8f);
            public int sparkRate = 8;

            [Header("Double Jump")]
            public bool doubleJumpRing = true;
            public Color ringColor = new Color(0.4f, 0.9f, 1f, 0.7f);
            [Range(0.5f, 1.5f)] public float ringRadius = 1f;
        }

        [Serializable]
        public class ScreenEffectSettings
        {
            [Header("Damage Flash")]
            public Color damageFlashColor = new Color(1f, 0.3f, 0.3f, 0.4f);
            [Range(0.1f, 0.5f)] public float damageFlashDuration = 0.2f;

            [Header("Story Beat Transition")]
            public Color storyTransitionColor = new Color(0f, 0f, 0f, 1f);
            [Range(0.5f, 2f)] public float storyFadeDuration = 1f;

            [Header("Drift Corruption")]
            [Range(0f, 0.1f)] public float driftAberration = 0.05f;
            [Range(0f, 0.3f)] public float driftVignette = 0.2f;
            [Range(0f, 0.5f)] public float driftDistortion = 0.3f;

            [Header("Belonging (Finale)")]
            public Color belongingGlowColor = new Color(1f, 0.95f, 0.8f, 0.3f);
            [Range(0.5f, 3f)] public float belongingBloomIntensity = 1.5f;
        }

        [Serializable]
        public class StoryBeatVFXSettings
        {
            [Header("Emotional Tone Colors")]
            public Color gentleColor = new Color(0.7f, 0.9f, 1f, 1f);
            public Color hopefulColor = new Color(1f, 0.95f, 0.7f, 1f);
            public Color melancholicColor = new Color(0.6f, 0.6f, 0.8f, 1f);
            public Color groundedColor = new Color(0.7f, 0.85f, 0.6f, 1f);
            public Color tenderColor = new Color(1f, 0.8f, 0.85f, 1f);

            [Header("Beat Transition")]
            [Range(0.5f, 2f)] public float transitionDuration = 1f;
            public bool particleBurst = true;
            public int burstCount = 40;
        }

        [Serializable]
        public class PatternVFXSettings
        {
            [Header("Echo Form")]
            public Color echoColor = new Color(0.5f, 0.5f, 0.6f, 0.8f);
            public int echoLoopParticles = 15;

            [Header("Distortion")]
            public Color distortionColor = new Color(0.8f, 0.3f, 0.8f, 0.7f);
            [Range(0f, 0.1f)] public float distortionShader = 0.05f;

            [Header("Noise Beast")]
            public Color noiseColor = new Color(1f, 0.4f, 0.4f, 0.6f);
            public int noiseParticleRate = 30;

            [Header("Resolution (Pattern Dissolving)")]
            public Gradient resolutionGradient;
            [Range(0.5f, 2f)] public float resolutionDuration = 1f;
            public int resolutionParticleCount = 60;
        }

        #endregion

        #region Runtime State

        Dictionary<VFXType, Queue<VFXInstance>> pools = new Dictionary<VFXType, Queue<VFXInstance>>();
        List<VFXInstance> activeEffects = new List<VFXInstance>();

        // Screen effect state
        float currentDriftLevel;
        Color currentScreenTint = Color.clear;
        float screenTintFade;

        // Windprint state
        WindprintMode currentWindprintMode = WindprintMode.None;
        GameObject windprintEffect;

        // Current emotional tone
        EmotionalTone currentTone = EmotionalTone.Gentle;

        #endregion

        #region Lifecycle

        void Start()
        {
            InitializePools();
            SubscribeToEvents();

            // Initialize gradient if null
            if (combatVFX.reTuneGradient == null)
            {
                combatVFX.reTuneGradient = new Gradient();
                combatVFX.reTuneGradient.SetKeys(
                    new GradientColorKey[] {
                        new GradientColorKey(Color.cyan, 0f),
                        new GradientColorKey(Color.white, 0.5f),
                        new GradientColorKey(Color.yellow, 1f)
                    },
                    new GradientAlphaKey[] {
                        new GradientAlphaKey(1f, 0f),
                        new GradientAlphaKey(1f, 0.5f),
                        new GradientAlphaKey(0f, 1f)
                    }
                );
            }

            if (patternVFX.resolutionGradient == null)
            {
                patternVFX.resolutionGradient = new Gradient();
                patternVFX.resolutionGradient.SetKeys(
                    new GradientColorKey[] {
                        new GradientColorKey(Color.white, 0f),
                        new GradientColorKey(Color.cyan, 0.5f),
                        new GradientColorKey(Color.clear, 1f)
                    },
                    new GradientAlphaKey[] {
                        new GradientAlphaKey(1f, 0f),
                        new GradientAlphaKey(0.8f, 0.5f),
                        new GradientAlphaKey(0f, 1f)
                    }
                );
            }
        }

        void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        void Update()
        {
            float dt = Time.deltaTime;

            UpdateActiveEffects(dt);
            UpdateScreenEffects(dt);
            UpdateWindprintEffect(dt);
        }

        #endregion

        #region Event Subscriptions

        void SubscribeToEvents()
        {
            AnimationEvents.OnVFXRequest += HandleVFXRequest;
            AnimationEvents.OnScreenEffect += HandleScreenEffect;
            AnimationEvents.OnCombatVerbUsed += HandleCombatVerb;
            AnimationEvents.OnWindprintModeChanged += HandleWindprintChange;
            AnimationEvents.OnPlayerDamaged += HandlePlayerDamaged;
            AnimationEvents.OnCollectibleCollected += HandleCollectible;
            AnimationEvents.OnDriftIntensityChanged += HandleDriftChange;
            AnimationEvents.OnStoryBeatChanged += HandleStoryBeat;
            AnimationEvents.OnEmotionalToneChanged += HandleToneChange;
            AnimationEvents.OnPlayerAction += HandlePlayerAction;
            AnimationEvents.OnPatternResolved += HandlePatternResolved;
        }

        void UnsubscribeFromEvents()
        {
            AnimationEvents.OnVFXRequest -= HandleVFXRequest;
            AnimationEvents.OnScreenEffect -= HandleScreenEffect;
            AnimationEvents.OnCombatVerbUsed -= HandleCombatVerb;
            AnimationEvents.OnWindprintModeChanged -= HandleWindprintChange;
            AnimationEvents.OnPlayerDamaged -= HandlePlayerDamaged;
            AnimationEvents.OnCollectibleCollected -= HandleCollectible;
            AnimationEvents.OnDriftIntensityChanged -= HandleDriftChange;
            AnimationEvents.OnStoryBeatChanged -= HandleStoryBeat;
            AnimationEvents.OnEmotionalToneChanged -= HandleToneChange;
            AnimationEvents.OnPlayerAction -= HandlePlayerAction;
            AnimationEvents.OnPatternResolved -= HandlePatternResolved;
        }

        #endregion

        #region Event Handlers

        void HandleVFXRequest(VFXType type, Vector3 position, Transform parent)
        {
            SpawnVFX(type, position, Quaternion.identity, parent);
        }

        void HandleScreenEffect(ScreenEffectType type, float intensity, float duration)
        {
            switch (type)
            {
                case ScreenEffectType.DamageFlash:
                    currentScreenTint = screenEffects.damageFlashColor * intensity;
                    screenTintFade = duration > 0 ? duration : screenEffects.damageFlashDuration;
                    break;

                case ScreenEffectType.StoryTransition:
                    currentScreenTint = screenEffects.storyTransitionColor * intensity;
                    screenTintFade = duration > 0 ? duration : screenEffects.storyFadeDuration;
                    break;

                case ScreenEffectType.BelongingGlow:
                    currentScreenTint = screenEffects.belongingGlowColor * intensity;
                    screenTintFade = duration > 0 ? duration : 5f;
                    break;
            }
        }

        void HandleCombatVerb(CombatVerb verb, Vector3 position, Vector3 direction, Transform target)
        {
            switch (verb)
            {
                case CombatVerb.Pulse:
                    SpawnPulseEffect(position);
                    break;
                case CombatVerb.ThreadLash:
                    SpawnThreadLashEffect(position, direction, target);
                    break;
                case CombatVerb.RadiantHold:
                    SpawnRadiantHoldEffect(position);
                    break;
                case CombatVerb.EdgeClaim:
                    SpawnEdgeClaimEffect(position);
                    break;
                case CombatVerb.ReTune:
                    SpawnReTuneEffect(position);
                    break;
            }
        }

        void HandleWindprintChange(WindprintMode from, WindprintMode to)
        {
            currentWindprintMode = to;
            SpawnWindprintTransitionEffect();
        }

        void HandlePlayerDamaged(DamageType type, float amount, Vector3 hitDirection)
        {
            AnimationEvents.ScreenEffect(ScreenEffectType.DamageFlash, amount, screenEffects.damageFlashDuration);
        }

        void HandleCollectible(CollectibleType type, Vector3 position)
        {
            if (type == CollectibleType.Principle)
            {
                SpawnPrincipleCollectEffect(position);
            }
            else
            {
                SpawnCollectEffect(position);
            }
        }

        void HandleDriftChange(float previous, float current)
        {
            currentDriftLevel = current;
        }

        void HandleStoryBeat(object previousBeat, object currentBeat)
        {
            if (storyVFX.particleBurst)
            {
                // Spawn story transition particles
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player)
                {
                    SpawnStoryTransitionEffect(player.transform.position);
                }
            }
        }

        void HandleToneChange(EmotionalTone from, EmotionalTone to)
        {
            currentTone = to;
        }

        void HandlePlayerAction(PlayerAction action)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (!player) return;

            Vector3 pos = player.transform.position;

            switch (action)
            {
                case PlayerAction.Land:
                    if (movementVFX.landImpact)
                        SpawnLandImpactEffect(pos);
                    break;

                case PlayerAction.DoubleJump:
                    if (movementVFX.doubleJumpRing)
                        SpawnDoubleJumpRingEffect(pos);
                    break;

                case PlayerAction.AirDash:
                    if (movementVFX.airDashTrail)
                        SpawnAirDashTrail(player.transform);
                    break;
            }
        }

        void HandlePatternResolved(Transform pattern, CombatVerb verb)
        {
            SpawnPatternResolutionEffect(pattern.position);
        }

        #endregion

        #region VFX Spawning

        void SpawnVFX(VFXType type, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            var instance = GetOrCreateVFXInstance(type);
            instance.transform.position = position;
            instance.transform.rotation = rotation;

            if (parent)
                instance.transform.SetParent(parent);

            instance.Activate(poolSettings.effectLifetime);
            activeEffects.Add(instance);
        }

        void SpawnPulseEffect(Vector3 position)
        {
            var instance = GetOrCreateVFXInstance(VFXType.CombatPulse);
            instance.transform.position = position;
            instance.SetColor(combatVFX.pulseColor);
            instance.SetScale(combatVFX.pulseRadius);
            instance.Activate(combatVFX.pulseDuration);
            activeEffects.Add(instance);

            // Animate pulse expansion
            instance.AnimateScale(0f, combatVFX.pulseRadius * 2f, combatVFX.pulseDuration);
            instance.AnimateAlpha(1f, 0f, combatVFX.pulseDuration);
        }

        void SpawnThreadLashEffect(Vector3 position, Vector3 direction, Transform target)
        {
            var instance = GetOrCreateVFXInstance(VFXType.CombatThreadLash);
            instance.transform.position = position;
            instance.SetColor(combatVFX.threadLashColor);

            Vector3 endPos = target ? target.position : position + direction.normalized * combatVFX.threadLength;
            instance.SetLinePoints(position, endPos, combatVFX.threadSegments);
            instance.Activate(combatVFX.threadDuration);
            activeEffects.Add(instance);
        }

        void SpawnRadiantHoldEffect(Vector3 position)
        {
            for (int i = 0; i < combatVFX.holdRingCount; i++)
            {
                var instance = GetOrCreateVFXInstance(VFXType.CombatHold);
                instance.transform.position = position;
                instance.SetColor(combatVFX.radiantHoldColor);

                float delay = i * 0.15f;
                float radius = combatVFX.holdRadius * (1f + i * 0.3f);
                instance.AnimateRing(radius, combatVFX.holdDuration, delay);
                instance.Activate(combatVFX.holdDuration + delay);
                activeEffects.Add(instance);
            }
        }

        void SpawnEdgeClaimEffect(Vector3 position)
        {
            var instance = GetOrCreateVFXInstance(VFXType.CombatEdge);
            instance.transform.position = position;
            instance.SetColor(combatVFX.edgeClaimColor);
            instance.AnimateBox(Vector3.zero, Vector3.one * combatVFX.edgeExpand, combatVFX.edgeDuration);
            instance.Activate(combatVFX.edgeDuration);
            activeEffects.Add(instance);
        }

        void SpawnReTuneEffect(Vector3 position)
        {
            for (int i = 0; i < combatVFX.reTuneWaveCount; i++)
            {
                var instance = GetOrCreateVFXInstance(VFXType.CombatReTune);
                instance.transform.position = position;
                instance.SetGradient(combatVFX.reTuneGradient);

                float delay = i * 0.2f;
                instance.AnimateWave(combatVFX.reTuneRadius, combatVFX.reTuneDuration, delay);
                instance.Activate(combatVFX.reTuneDuration + delay);
                activeEffects.Add(instance);
            }
        }

        void SpawnWindprintTransitionEffect()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (!player) return;

            var instance = GetOrCreateVFXInstance(VFXType.WindprintTransition);
            instance.transform.position = player.transform.position;
            instance.transform.SetParent(player.transform);

            Color color = currentWindprintMode == WindprintMode.Cushion
                ? windprintVFX.cushionColor
                : windprintVFX.guardColor;

            instance.SetColor(color);
            instance.SpawnBurst(windprintVFX.transitionBurstCount);
            instance.Activate(windprintVFX.transitionDuration);
            activeEffects.Add(instance);
        }

        void SpawnCollectEffect(Vector3 position)
        {
            var instance = GetOrCreateVFXInstance(VFXType.Collect);
            instance.transform.position = position;
            instance.SetColor(collectibleVFX.collectColor);
            instance.SpawnBurst(collectibleVFX.collectParticleCount);
            instance.Activate(collectibleVFX.collectBurstDuration);
            activeEffects.Add(instance);

            if (collectibleVFX.collectTrailToPlayer)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player)
                {
                    instance.AnimateToTarget(player.transform, collectibleVFX.trailDuration);
                }
            }
        }

        void SpawnPrincipleCollectEffect(Vector3 position)
        {
            var instance = GetOrCreateVFXInstance(VFXType.PrincipleCollect);
            instance.transform.position = position;
            instance.SetColor(collectibleVFX.principleColor);
            instance.SpawnBurst(collectibleVFX.principleParticleCount);
            instance.Activate(collectibleVFX.principleBurstDuration);
            activeEffects.Add(instance);

            if (collectibleVFX.principleSlowMotion)
            {
                // Brief slow-motion
                Time.timeScale = collectibleVFX.principleSlowScale;
                Invoke(nameof(RestoreTimeScale), collectibleVFX.principleBurstDuration * 0.5f);
            }
        }

        void SpawnLandImpactEffect(Vector3 position)
        {
            var instance = GetOrCreateVFXInstance(VFXType.LandImpact);
            instance.transform.position = position;
            instance.SetColor(movementVFX.dustColor);
            instance.SpawnBurst(movementVFX.impactParticleCount);
            instance.Activate(movementVFX.impactDuration);
            activeEffects.Add(instance);
        }

        void SpawnDoubleJumpRingEffect(Vector3 position)
        {
            var instance = GetOrCreateVFXInstance(VFXType.DoubleJumpRing);
            instance.transform.position = position;
            instance.SetColor(movementVFX.ringColor);
            instance.AnimateRing(movementVFX.ringRadius, 0.4f, 0f);
            instance.Activate(0.5f);
            activeEffects.Add(instance);
        }

        void SpawnAirDashTrail(Transform player)
        {
            var instance = GetOrCreateVFXInstance(VFXType.AirDashTrail);
            instance.transform.position = player.position;
            instance.SetColor(movementVFX.dashColor);
            instance.SetTrailTarget(player);
            instance.Activate(movementVFX.dashTrailDuration);
            activeEffects.Add(instance);
        }

        void SpawnStoryTransitionEffect(Vector3 position)
        {
            var instance = GetOrCreateVFXInstance(VFXType.StoryTransition);
            instance.transform.position = position;
            instance.SetColor(GetToneColor(currentTone));
            instance.SpawnBurst(storyVFX.burstCount);
            instance.Activate(storyVFX.transitionDuration);
            activeEffects.Add(instance);
        }

        void SpawnPatternResolutionEffect(Vector3 position)
        {
            var instance = GetOrCreateVFXInstance(VFXType.PatternResolution);
            instance.transform.position = position;
            instance.SetGradient(patternVFX.resolutionGradient);
            instance.SpawnBurst(patternVFX.resolutionParticleCount);
            instance.AnimateScale(1f, 3f, patternVFX.resolutionDuration);
            instance.AnimateAlpha(1f, 0f, patternVFX.resolutionDuration);
            instance.Activate(patternVFX.resolutionDuration);
            activeEffects.Add(instance);
        }

        #endregion

        #region Pool Management

        void InitializePools()
        {
            foreach (VFXType type in Enum.GetValues(typeof(VFXType)))
            {
                pools[type] = new Queue<VFXInstance>();

                for (int i = 0; i < poolSettings.initialPoolSize / Enum.GetValues(typeof(VFXType)).Length + 1; i++)
                {
                    var instance = CreateVFXInstance(type);
                    instance.gameObject.SetActive(false);
                    pools[type].Enqueue(instance);
                }
            }
        }

        VFXInstance GetOrCreateVFXInstance(VFXType type)
        {
            if (pools[type].Count > 0)
            {
                var instance = pools[type].Dequeue();
                instance.gameObject.SetActive(true);
                instance.Reset();
                return instance;
            }

            if (activeEffects.Count < poolSettings.maxPoolSize)
            {
                return CreateVFXInstance(type);
            }

            // Recycle oldest
            var oldest = activeEffects[0];
            activeEffects.RemoveAt(0);
            oldest.Reset();
            return oldest;
        }

        VFXInstance CreateVFXInstance(VFXType type)
        {
            var go = new GameObject($"VFX_{type}");
            go.transform.SetParent(transform);

            var instance = go.AddComponent<VFXInstance>();
            instance.vfxType = type;
            instance.Initialize();

            return instance;
        }

        void ReturnToPool(VFXInstance instance)
        {
            instance.gameObject.SetActive(false);
            instance.transform.SetParent(transform);
            instance.Reset();

            if (pools.ContainsKey(instance.vfxType))
            {
                pools[instance.vfxType].Enqueue(instance);
            }
        }

        #endregion

        #region Updates

        void UpdateActiveEffects(float dt)
        {
            for (int i = activeEffects.Count - 1; i >= 0; i--)
            {
                var effect = activeEffects[i];
                effect.UpdateEffect(dt);

                if (effect.IsComplete)
                {
                    activeEffects.RemoveAt(i);
                    ReturnToPool(effect);
                }
            }
        }

        void UpdateScreenEffects(float dt)
        {
            if (screenTintFade > 0)
            {
                screenTintFade -= dt;
                if (screenTintFade <= 0)
                {
                    currentScreenTint = Color.clear;
                }
            }
        }

        void UpdateWindprintEffect(float dt)
        {
            // Continuous windprint visual handled by dedicated component
        }

        void RestoreTimeScale()
        {
            Time.timeScale = 1f;
        }

        #endregion

        #region Utilities

        Color GetToneColor(EmotionalTone tone)
        {
            return tone switch
            {
                EmotionalTone.Gentle => storyVFX.gentleColor,
                EmotionalTone.Hopeful => storyVFX.hopefulColor,
                EmotionalTone.Melancholic => storyVFX.melancholicColor,
                EmotionalTone.Grounded => storyVFX.groundedColor,
                EmotionalTone.Tender => storyVFX.tenderColor,
                _ => Color.white
            };
        }

        #endregion

        #region Public API

        /// <summary>Trigger a custom VFX at position</summary>
        public void TriggerVFX(VFXType type, Vector3 position)
        {
            AnimationEvents.VFXRequest(type, position, null);
        }

        /// <summary>Trigger screen flash</summary>
        public void TriggerScreenFlash(Color color, float duration)
        {
            currentScreenTint = color;
            screenTintFade = duration;
        }

        /// <summary>Get current screen tint for post-processing</summary>
        public Color GetCurrentScreenTint()
        {
            float alpha = Mathf.Clamp01(screenTintFade);
            return new Color(currentScreenTint.r, currentScreenTint.g, currentScreenTint.b, currentScreenTint.a * alpha);
        }

        /// <summary>Get current drift distortion amount</summary>
        public float GetDriftDistortion()
        {
            return currentDriftLevel * screenEffects.driftDistortion;
        }

        #endregion
    }

    /// <summary>
    /// Individual VFX instance with animation capabilities
    /// </summary>
    public class VFXInstance : MonoBehaviour
    {
        public VFXType vfxType;

        float lifetime;
        float elapsed;
        bool isActive;

        // Animation state
        float scaleFrom, scaleTo, scaleDuration;
        float alphaFrom, alphaTo, alphaDuration;
        float ringRadius, ringDuration, ringDelay;
        Transform targetTransform;
        float targetDuration;

        // Visual
        Color currentColor = Color.white;
        Gradient currentGradient;

        // Particles
        ParticleSystem particles;

        // Line (for Thread Lash)
        LineRenderer lineRenderer;

        // Mesh (for custom shapes)
        MeshFilter meshFilter;
        MeshRenderer meshRenderer;

        public bool IsComplete => elapsed >= lifetime;

        public void Initialize()
        {
            // Create visual components based on type
            switch (vfxType)
            {
                case VFXType.CombatThreadLash:
                    lineRenderer = gameObject.AddComponent<LineRenderer>();
                    lineRenderer.startWidth = 0.1f;
                    lineRenderer.endWidth = 0.05f;
                    lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                    break;

                case VFXType.CombatPulse:
                case VFXType.CombatHold:
                case VFXType.CombatEdge:
                case VFXType.DoubleJumpRing:
                    CreateMeshVisual();
                    break;

                default:
                    CreateParticleSystem();
                    break;
            }
        }

        void CreateParticleSystem()
        {
            particles = gameObject.AddComponent<ParticleSystem>();
            var main = particles.main;
            main.startLifetime = 1f;
            main.startSpeed = 2f;
            main.startSize = 0.1f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.playOnAwake = false;

            var emission = particles.emission;
            emission.enabled = false;

            var shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.1f;

            var renderer = particles.GetComponent<ParticleSystemRenderer>();
            renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
        }

        void CreateMeshVisual()
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Sprites/Default"));

            // Create simple quad mesh
            var mesh = new Mesh();
            mesh.vertices = new Vector3[]
            {
                new Vector3(-0.5f, 0f, -0.5f),
                new Vector3(0.5f, 0f, -0.5f),
                new Vector3(0.5f, 0f, 0.5f),
                new Vector3(-0.5f, 0f, 0.5f)
            };
            mesh.triangles = new int[] { 0, 2, 1, 0, 3, 2 };
            mesh.RecalculateNormals();
            meshFilter.mesh = mesh;
        }

        public void Activate(float duration)
        {
            lifetime = duration;
            elapsed = 0f;
            isActive = true;
        }

        public void Reset()
        {
            elapsed = 0f;
            isActive = false;
            transform.localScale = Vector3.one;
            scaleFrom = scaleTo = 1f;
            alphaFrom = alphaTo = 1f;
            targetTransform = null;

            if (particles) particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            if (lineRenderer) lineRenderer.positionCount = 0;
        }

        public void UpdateEffect(float dt)
        {
            if (!isActive) return;

            elapsed += dt;
            float t = Mathf.Clamp01(elapsed / lifetime);

            // Scale animation
            if (scaleDuration > 0)
            {
                float scaleT = Mathf.Clamp01(elapsed / scaleDuration);
                float scale = Mathf.Lerp(scaleFrom, scaleTo, scaleT);
                transform.localScale = Vector3.one * scale;
            }

            // Alpha animation
            if (alphaDuration > 0)
            {
                float alphaT = Mathf.Clamp01(elapsed / alphaDuration);
                float alpha = Mathf.Lerp(alphaFrom, alphaTo, alphaT);
                SetAlpha(alpha);
            }

            // Ring animation
            if (ringDuration > 0 && elapsed > ringDelay)
            {
                float ringT = Mathf.Clamp01((elapsed - ringDelay) / ringDuration);
                float radius = Mathf.Lerp(0f, ringRadius, ringT);
                float alpha = 1f - ringT;
                transform.localScale = Vector3.one * radius * 2f;
                SetAlpha(alpha);
            }

            // Move to target
            if (targetTransform && targetDuration > 0)
            {
                float targetT = Mathf.Clamp01(elapsed / targetDuration);
                transform.position = Vector3.Lerp(transform.position, targetTransform.position, targetT);
            }
        }

        public void SetColor(Color color)
        {
            currentColor = color;

            if (particles)
            {
                var main = particles.main;
                main.startColor = color;
            }

            if (lineRenderer)
            {
                lineRenderer.startColor = color;
                lineRenderer.endColor = color;
            }

            if (meshRenderer)
            {
                meshRenderer.material.color = color;
            }
        }

        public void SetAlpha(float alpha)
        {
            Color c = currentColor;
            c.a = alpha;

            if (meshRenderer)
            {
                meshRenderer.material.color = c;
            }
        }

        public void SetGradient(Gradient gradient)
        {
            currentGradient = gradient;

            if (particles)
            {
                var colorOverLifetime = particles.colorOverLifetime;
                colorOverLifetime.enabled = true;
                colorOverLifetime.color = gradient;
            }
        }

        public void SetScale(float scale)
        {
            transform.localScale = Vector3.one * scale;
        }

        public void AnimateScale(float from, float to, float duration)
        {
            scaleFrom = from;
            scaleTo = to;
            scaleDuration = duration;
            transform.localScale = Vector3.one * from;
        }

        public void AnimateAlpha(float from, float to, float duration)
        {
            alphaFrom = from;
            alphaTo = to;
            alphaDuration = duration;
        }

        public void AnimateRing(float radius, float duration, float delay)
        {
            ringRadius = radius;
            ringDuration = duration;
            ringDelay = delay;
        }

        public void AnimateBox(Vector3 from, Vector3 to, float duration)
        {
            // Simple box expansion
            AnimateScale(from.magnitude, to.magnitude, duration);
        }

        public void AnimateWave(float radius, float duration, float delay)
        {
            AnimateRing(radius, duration, delay);
        }

        public void AnimateToTarget(Transform target, float duration)
        {
            targetTransform = target;
            targetDuration = duration;
        }

        public void SetLinePoints(Vector3 start, Vector3 end, int segments)
        {
            if (!lineRenderer) return;

            lineRenderer.positionCount = segments;
            for (int i = 0; i < segments; i++)
            {
                float t = (float)i / (segments - 1);
                lineRenderer.SetPosition(i, Vector3.Lerp(start, end, t));
            }
        }

        public void SpawnBurst(int count)
        {
            if (particles)
            {
                particles.Emit(count);
            }
        }

        public void SetTrailTarget(Transform target)
        {
            targetTransform = target;
        }
    }
}
