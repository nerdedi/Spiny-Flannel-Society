using UnityEngine;
using System;
using System.Collections.Generic;

namespace SFS.Animation
{
    /// <summary>
    /// Animates the living architecture of Spiny Flannel Society.
    /// Ramps bloom, platforms knit, signage re-renders, bridges grow.
    /// Architecture responds to people - until the Drift forces standardization.
    /// </summary>
    public class LivingArchitectureAnimator : MonoBehaviour
    {
        #region Configuration

        [Header("═══════════════════════════════════════")]
        [Header("     LIVING ARCHITECTURE ANIMATOR      ")]
        [Header("═══════════════════════════════════════")]

        [Header("Target")]
        public Transform visualTarget;

        [Header("Architecture Type")]
        public ArchitectureType architectureType = ArchitectureType.Platform;

        [Header("═══ AMBIENT MOTION ═══")]
        public AmbientSettings ambient = new AmbientSettings();

        [Header("═══ RESPONSE TO PLAYER ═══")]
        public PlayerResponseSettings playerResponse = new PlayerResponseSettings();

        [Header("═══ DRIFT CORRUPTION ═══")]
        public DriftSettings drift = new DriftSettings();

        [Header("═══ ARCHITECTURE-SPECIFIC ═══")]
        public RampSettings ramp = new RampSettings();
        public PlatformSettings platform = new PlatformSettings();
        public BridgeSettings bridge = new BridgeSettings();
        public SignageSettings signage = new SignageSettings();
        public CanopySettings canopy = new CanopySettings();

        [Header("═══ WINDPRINT RESPONSE ═══")]
        public WindprintResponseSettings windprintResponse = new WindprintResponseSettings();

        #endregion

        #region Settings Classes

        public enum ArchitectureType
        {
            Platform,       // Basic platform
            Ramp,           // Blooming ramps
            Bridge,         // Growing umbel bridges
            Signage,        // Multi-modal signage
            Canopy,         // Eucalyptus glass canopy
            SafePocket,     // Rest area
            ConsentGate,    // Boundary/consent gate
            DesignTerminal  // System rewrite interface
        }

        [Serializable]
        public class AmbientSettings
        {
            [Header("Breathing")]
            [Range(0.3f, 2f)] public float breatheSpeed = 0.8f;
            [Range(0f, 0.03f)] public float breatheAmount = 0.01f;

            [Header("Sway")]
            [Range(0.1f, 1f)] public float swaySpeed = 0.4f;
            [Range(0f, 0.05f)] public float swayAmount = 0.015f;
            [Range(0f, 5f)] public float swayRotation = 2f;

            [Header("Wind Response")]
            public bool respondToWind = true;
            [Range(0f, 1f)] public float windInfluence = 0.3f;
        }

        [Serializable]
        public class PlayerResponseSettings
        {
            public bool respondToPlayer = true;
            [Range(1f, 10f)] public float responseRadius = 5f;
            [Range(0.1f, 2f)] public float responseSpeed = 0.8f;

            [Header("Bloom (Welcoming)")]
            [Range(0f, 0.1f)] public float bloomScale = 0.05f;
            [Range(0f, 0.05f)] public float bloomRise = 0.02f;

            [Header("Attention")]
            [Range(0f, 15f)] public float attentionTurn = 8f;
        }

        [Serializable]
        public class DriftSettings
        {
            [Tooltip("Current drift corruption level")]
            [Range(0f, 1f)] public float driftLevel = 0f;

            [Header("Corruption Effects")]
            [Range(0f, 0.1f)] public float corruptionJitter = 0.03f;
            [Range(0f, 20f)] public float corruptionTwist = 10f;
            [Range(0.8f, 1f)] public float corruptionScaleLoss = 0.95f;

            [Header("Rejection (When Drift is High)")]
            [Range(0f, 0.2f)] public float rejectionPull = 0.08f;
            [Range(0f, 30f)] public float rejectionTilt = 15f;
        }

        [Serializable]
        public class RampSettings
        {
            [Header("Bloom Animation")]
            [Range(0.2f, 1f)] public float bloomDuration = 0.6f;
            public AnimationCurve bloomCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

            [Header("Retract (Drift Behavior)")]
            [Range(0.3f, 1f)] public float retractDuration = 0.5f;
            [Range(0f, 0.5f)] public float retractDelay = 0.2f;

            [Header("Scale")]
            [Range(0f, 1f)] public float retractedScale = 0.1f;
            public bool isBloomedByDefault = true;
        }

        [Serializable]
        public class PlatformSettings
        {
            [Header("Rhythm")]
            [Range(0.5f, 3f)] public float rhythmPeriod = 1.5f;
            [Range(0f, 0.15f)] public float rhythmBob = 0.05f;

            [Header("Timing Hazard")]
            [Tooltip("Platform disappears on rhythm when Drift is high")]
            public bool hasTimingHazard = false;
            [Range(0.2f, 0.8f)] public float hazardActiveRatio = 0.5f;

            [Header("Knitting")]
            [Range(0.3f, 1.5f)] public float knitDuration = 0.8f;
            public bool startsUnknitted = false;
        }

        [Serializable]
        public class BridgeSettings
        {
            [Header("Growth (Umbel Style)")]
            [Range(0.5f, 3f)] public float growthDuration = 1.5f;
            public AnimationCurve growthCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

            [Header("Cluster Sway")]
            [Range(0.2f, 1f)] public float clusterSwaySpeed = 0.5f;
            [Range(0f, 8f)] public float clusterSwayAngle = 4f;

            [Header("V-Anchor Response")]
            [Range(0f, 10f)] public float anchorSettleAngle = 5f;
        }

        [Serializable]
        public class SignageSettings
        {
            [Header("Multi-Modal Render")]
            [Range(0.2f, 1f)] public float renderDuration = 0.5f;

            [Header("Corruption (Drift)")]
            [Range(0f, 0.1f)] public float corruptFlicker = 0.05f;
            [Range(0f, 30f)] public float corruptSkew = 15f;

            [Header("Clarity (Restored)")]
            [Range(0f, 0.02f)] public float clarityGlow = 0.01f;
        }

        [Serializable]
        public class CanopySettings
        {
            [Header("Filter Movement")]
            [Range(0.1f, 0.5f)] public float filterSpeed = 0.25f;
            [Range(0f, 0.08f)] public float filterSway = 0.04f;

            [Header("Light Dappling")]
            [Range(0f, 0.05f)] public float dappleAmount = 0.02f;
            [Range(1f, 5f)] public float dappleSpeed = 2f;
        }

        [Serializable]
        public class WindprintResponseSettings
        {
            [Header("Cushion Mode Response")]
            [Range(1f, 1.2f)] public float cushionBloom = 1.1f;
            [Range(0f, 0.03f)] public float cushionSoften = 0.015f;

            [Header("Guard Mode Response")]
            [Range(0.9f, 1f)] public float guardStabilize = 0.95f;
            [Range(0f, 1f)] public float guardRhythmLock = 0.8f;
        }

        #endregion

        #region Runtime State

        float animTime;
        float stateBlend;

        // Response tracking
        Transform playerTransform;
        float playerDistance = 100f;
        bool playerInRange;

        // Windprint state
        WindprintMode currentWindprintMode = WindprintMode.None;

        // State
        bool isBloomed = true;
        bool isKnitted = true;
        float bloomProgress = 1f;
        float knitProgress = 1f;
        float retractTimer;

        // Animation output
        Vector3 baseScale;
        Vector3 baseLocalPos;
        Quaternion baseLocalRot;

        Vector3 currentOffset;
        Vector3 targetOffset;
        Vector3 currentRotation;
        Vector3 targetRotation;
        float currentScaleMult = 1f;
        float targetScaleMult = 1f;

        // Wind
        Vector3 currentWindForce;

        #endregion

        #region Lifecycle

        void Awake()
        {
            if (!visualTarget)
            {
                var meshRenderer = GetComponentInChildren<MeshRenderer>();
                if (meshRenderer) visualTarget = meshRenderer.transform;
                if (!visualTarget) visualTarget = transform;
            }
        }

        void Start()
        {
            baseScale = visualTarget.localScale;
            baseLocalPos = visualTarget.localPosition;
            baseLocalRot = visualTarget.localRotation;

            // Find player
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player) playerTransform = player.transform;

            // Initialize state based on settings
            isBloomed = (architectureType == ArchitectureType.Ramp) ? ramp.isBloomedByDefault : true;
            isKnitted = (architectureType == ArchitectureType.Platform) ? !platform.startsUnknitted : true;
            bloomProgress = isBloomed ? 1f : 0f;
            knitProgress = isKnitted ? 1f : 0f;

            SubscribeToEvents();
        }

        void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        void SubscribeToEvents()
        {
            AnimationEvents.OnWindprintModeChanged += HandleWindprintChange;
            AnimationEvents.OnDriftIntensityChanged += HandleDriftChange;
            AnimationEvents.OnArchitectureRespond += HandleArchitectureCommand;
            AnimationEvents.OnWindPulse += HandleWindPulse;
        }

        void UnsubscribeFromEvents()
        {
            AnimationEvents.OnWindprintModeChanged -= HandleWindprintChange;
            AnimationEvents.OnDriftIntensityChanged -= HandleDriftChange;
            AnimationEvents.OnArchitectureRespond -= HandleArchitectureCommand;
            AnimationEvents.OnWindPulse -= HandleWindPulse;
        }

        void Update()
        {
            float dt = Time.deltaTime;
            animTime += dt;

            UpdatePlayerDistance();
            UpdateStateTransitions(dt);

            targetOffset = Vector3.zero;
            targetRotation = Vector3.zero;
            targetScaleMult = 1f;

            // Calculate animations based on type
            CalculateAmbientMotion();
            CalculateTypeSpecificAnimation();
            CalculatePlayerResponse();
            CalculateDriftEffects();
            CalculateWindprintEffects();

            ApplyAnimation(dt);
        }

        #endregion

        #region Animation Calculations

        void CalculateAmbientMotion()
        {
            // Breathing
            float breathePhase = animTime * ambient.breatheSpeed * Mathf.PI * 2f;
            targetOffset.y += Mathf.Sin(breathePhase) * ambient.breatheAmount;

            // Sway
            float swayPhase = animTime * ambient.swaySpeed;
            targetOffset.x += Mathf.Sin(swayPhase) * ambient.swayAmount;
            targetRotation.z += Mathf.Sin(swayPhase) * ambient.swayRotation;

            // Wind response
            if (ambient.respondToWind)
            {
                targetOffset += currentWindForce * ambient.windInfluence * 0.01f;
                targetRotation.z += currentWindForce.x * ambient.windInfluence * 5f;
            }
        }

        void CalculateTypeSpecificAnimation()
        {
            switch (architectureType)
            {
                case ArchitectureType.Ramp:
                    AnimateRamp();
                    break;
                case ArchitectureType.Platform:
                    AnimatePlatform();
                    break;
                case ArchitectureType.Bridge:
                    AnimateBridge();
                    break;
                case ArchitectureType.Signage:
                    AnimateSignage();
                    break;
                case ArchitectureType.Canopy:
                    AnimateCanopy();
                    break;
                case ArchitectureType.SafePocket:
                    AnimateSafePocket();
                    break;
                case ArchitectureType.ConsentGate:
                    AnimateConsentGate();
                    break;
                case ArchitectureType.DesignTerminal:
                    AnimateDesignTerminal();
                    break;
            }
        }

        void AnimateRamp()
        {
            // Bloom/retract based on state
            float scaleFromBloom = Mathf.Lerp(ramp.retractedScale, 1f, bloomProgress);
            targetScaleMult *= scaleFromBloom;

            // Emergence animation when blooming
            if (bloomProgress < 1f && bloomProgress > 0f)
            {
                float emergenceOffset = (1f - bloomProgress) * 0.1f;
                targetOffset.y -= emergenceOffset;

                // Unfurl rotation
                targetRotation.x = (1f - bloomProgress) * 30f;
            }
        }

        void AnimatePlatform()
        {
            // Rhythm bob
            float rhythmPhase = animTime / platform.rhythmPeriod;
            float rhythmT = rhythmPhase - Mathf.Floor(rhythmPhase);

            if (currentWindprintMode == WindprintMode.Guard)
            {
                // Guard mode locks rhythm
                rhythmT = 0.5f; // Stabilized
            }

            targetOffset.y += Mathf.Sin(rhythmT * Mathf.PI * 2f) * platform.rhythmBob;

            // Timing hazard
            if (platform.hasTimingHazard && drift.driftLevel > 0.3f)
            {
                bool hazardActive = rhythmT < platform.hazardActiveRatio;
                if (hazardActive && drift.driftLevel > 0.5f)
                {
                    targetScaleMult *= Mathf.Lerp(1f, 0.2f, (drift.driftLevel - 0.5f) * 2f);
                }
            }

            // Knit state
            targetScaleMult *= Mathf.Lerp(0.1f, 1f, knitProgress);
        }

        void AnimateBridge()
        {
            // Cluster sway (umbel gardens style)
            float swayPhase = animTime * bridge.clusterSwaySpeed;
            targetRotation.z += Mathf.Sin(swayPhase) * bridge.clusterSwayAngle;
            targetRotation.x += Mathf.Sin(swayPhase * 0.7f) * bridge.clusterSwayAngle * 0.5f;

            // V-anchor settle
            if (playerInRange)
            {
                targetRotation.x += bridge.anchorSettleAngle * 0.3f;
            }
        }

        void AnimateSignage()
        {
            // Drift corruption
            if (drift.driftLevel > 0.2f)
            {
                float corruptPhase = animTime * 10f;
                bool flickerOff = Mathf.PerlinNoise(corruptPhase, 0f) > (1f - drift.driftLevel);

                if (flickerOff)
                {
                    targetScaleMult *= 0.8f;
                    targetOffset.x += UnityEngine.Random.Range(-signage.corruptFlicker, signage.corruptFlicker);
                }

                targetRotation.z += Mathf.Sin(corruptPhase * 3f) * signage.corruptSkew * drift.driftLevel;
            }
            else
            {
                // Clarity glow pulse
                float clarityPhase = animTime * 2f;
                targetScaleMult *= 1f + Mathf.Sin(clarityPhase * Mathf.PI * 2f) * signage.clarityGlow;
            }
        }

        void AnimateCanopy()
        {
            // Filter sway
            float filterPhase = animTime * canopy.filterSpeed;
            targetOffset.x += Mathf.Sin(filterPhase) * canopy.filterSway;
            targetOffset.z += Mathf.Cos(filterPhase * 0.7f) * canopy.filterSway * 0.5f;

            // Dappling
            float dapplePhase = animTime * canopy.dappleSpeed;
            float dapple = Mathf.PerlinNoise(dapplePhase, transform.position.x);
            targetScaleMult *= 1f + (dapple - 0.5f) * canopy.dappleAmount * 2f;
        }

        void AnimateSafePocket()
        {
            // Gentle, protective pulse
            float pulsePhase = animTime * 0.8f * Mathf.PI * 2f;
            targetScaleMult *= 1f + Mathf.Sin(pulsePhase) * 0.02f;

            // Warm glow (slight scale up when player near)
            if (playerInRange)
            {
                float proximity = 1f - Mathf.Clamp01(playerDistance / playerResponse.responseRadius);
                targetScaleMult *= 1f + proximity * 0.05f;
            }
        }

        void AnimateConsentGate()
        {
            // Ready pulse
            float pulsePhase = animTime * 1.2f * Mathf.PI * 2f;
            targetOffset.y += Mathf.Sin(pulsePhase) * 0.01f;

            // Frame stability
            if (currentWindprintMode == WindprintMode.Guard)
            {
                // Solid, locked
                targetRotation = Vector3.zero;
            }
        }

        void AnimateDesignTerminal()
        {
            // Active processing
            float processPhase = animTime * 3f;

            // Holographic shimmer
            targetOffset.y += Mathf.Sin(processPhase * Mathf.PI * 2f) * 0.005f;

            // Rotation scan
            targetRotation.y = Mathf.Sin(processPhase * 0.5f) * 5f;

            if (playerInRange)
            {
                // Attention - face player
                if (playerTransform)
                {
                    Vector3 toPlayer = playerTransform.position - transform.position;
                    toPlayer.y = 0;
                    float angle = Vector3.SignedAngle(transform.forward, toPlayer.normalized, Vector3.up);
                    targetRotation.y = Mathf.Clamp(angle, -30f, 30f);
                }
            }
        }

        void CalculatePlayerResponse()
        {
            if (!playerResponse.respondToPlayer || !playerInRange) return;

            float proximity = 1f - Mathf.Clamp01(playerDistance / playerResponse.responseRadius);
            proximity = Mathf.SmoothStep(0f, 1f, proximity);

            // Bloom toward player
            targetScaleMult *= 1f + playerResponse.bloomScale * proximity;
            targetOffset.y += playerResponse.bloomRise * proximity;

            // Turn toward player
            if (playerTransform && playerResponse.attentionTurn > 0)
            {
                Vector3 toPlayer = playerTransform.position - transform.position;
                toPlayer.y = 0;
                float angle = Vector3.SignedAngle(transform.forward, toPlayer.normalized, Vector3.up);
                targetRotation.y += Mathf.Clamp(angle, -playerResponse.attentionTurn, playerResponse.attentionTurn) * proximity;
            }
        }

        void CalculateDriftEffects()
        {
            if (drift.driftLevel < 0.1f) return;

            float driftFactor = drift.driftLevel;

            // Corruption jitter
            float jitterPhase = animTime * 8f;
            targetOffset.x += (Mathf.PerlinNoise(jitterPhase, 0f) - 0.5f) * drift.corruptionJitter * 2f * driftFactor;
            targetOffset.z += (Mathf.PerlinNoise(0f, jitterPhase) - 0.5f) * drift.corruptionJitter * 2f * driftFactor;

            // Corruption twist
            targetRotation.z += Mathf.Sin(jitterPhase * 0.5f) * drift.corruptionTwist * driftFactor;

            // Scale loss
            targetScaleMult *= Mathf.Lerp(1f, drift.corruptionScaleLoss, driftFactor);

            // Rejection (architecture rejects difference)
            if (playerInRange && driftFactor > 0.5f)
            {
                float rejection = (driftFactor - 0.5f) * 2f;

                // Pull away from player
                if (playerTransform)
                {
                    Vector3 awayFromPlayer = (transform.position - playerTransform.position).normalized;
                    targetOffset += awayFromPlayer * drift.rejectionPull * rejection;
                }

                targetRotation.x += drift.rejectionTilt * rejection;
            }
        }

        void CalculateWindprintEffects()
        {
            switch (currentWindprintMode)
            {
                case WindprintMode.Cushion:
                    // Soften and bloom
                    targetScaleMult *= windprintResponse.cushionBloom;
                    targetOffset.y += windprintResponse.cushionSoften;
                    break;

                case WindprintMode.Guard:
                    // Stabilize and lock
                    targetScaleMult *= windprintResponse.guardStabilize;
                    // Reduce all motion
                    targetOffset *= (1f - windprintResponse.guardRhythmLock);
                    targetRotation *= (1f - windprintResponse.guardRhythmLock);
                    break;
            }
        }

        #endregion

        #region State Management

        void UpdatePlayerDistance()
        {
            if (!playerTransform)
            {
                playerDistance = 100f;
                playerInRange = false;
                return;
            }

            playerDistance = Vector3.Distance(transform.position, playerTransform.position);
            playerInRange = playerDistance <= playerResponse.responseRadius;
        }

        void UpdateStateTransitions(float dt)
        {
            // Bloom transition
            float bloomTarget = isBloomed ? 1f : 0f;
            float bloomSpeed = isBloomed ? (1f / ramp.bloomDuration) : (1f / ramp.retractDuration);
            bloomProgress = Mathf.MoveTowards(bloomProgress, bloomTarget, bloomSpeed * dt);

            // Knit transition
            float knitTarget = isKnitted ? 1f : 0f;
            knitProgress = Mathf.MoveTowards(knitProgress, knitTarget, dt / platform.knitDuration);

            // Auto-retract for ramps when Drift is high
            if (architectureType == ArchitectureType.Ramp && drift.driftLevel > 0.6f)
            {
                if (playerInRange)
                {
                    retractTimer += dt;
                    if (retractTimer > ramp.retractDelay)
                    {
                        isBloomed = false;
                    }
                }
            }
            else
            {
                retractTimer = 0f;
                if (!isBloomed && drift.driftLevel < 0.4f)
                {
                    isBloomed = true; // Restore when Drift reduces
                }
            }
        }

        #endregion

        #region Event Handlers

        void HandleWindprintChange(WindprintMode from, WindprintMode to)
        {
            currentWindprintMode = to;
        }

        void HandleDriftChange(float previous, float current)
        {
            drift.driftLevel = current;
        }

        void HandleArchitectureCommand(Transform target, ArchitectureResponse response)
        {
            if (target != transform) return;

            switch (response)
            {
                case ArchitectureResponse.RampBloom:
                    isBloomed = true;
                    break;
                case ArchitectureResponse.PlatformKnit:
                    isKnitted = true;
                    break;
                case ArchitectureResponse.SafePocketSpawn:
                    // Trigger spawn animation
                    targetScaleMult = 0f;
                    break;
            }
        }

        void HandleWindPulse(Vector3 direction, float strength)
        {
            currentWindForce = direction * strength;
        }

        #endregion

        #region Apply Animation

        void ApplyAnimation(float dt)
        {
            float speed = 8f * dt;

            currentOffset = Vector3.Lerp(currentOffset, targetOffset, speed);
            currentRotation = Vector3.Lerp(currentRotation, targetRotation, speed);
            currentScaleMult = Mathf.Lerp(currentScaleMult, targetScaleMult, speed);

            visualTarget.localPosition = baseLocalPos + currentOffset;
            visualTarget.localRotation = baseLocalRot * Quaternion.Euler(currentRotation);
            visualTarget.localScale = baseScale * currentScaleMult;
        }

        #endregion

        #region Public API

        /// <summary>Trigger a bloom animation</summary>
        public void TriggerBloom()
        {
            isBloomed = true;
            AnimationEvents.ArchitectureRespond(transform, ArchitectureResponse.RampBloom);
        }

        /// <summary>Trigger a retract animation</summary>
        public void TriggerRetract()
        {
            isBloomed = false;
        }

        /// <summary>Trigger platform knitting</summary>
        public void TriggerKnit()
        {
            isKnitted = true;
            AnimationEvents.ArchitectureRespond(transform, ArchitectureResponse.PlatformKnit);
        }

        /// <summary>Set drift level directly</summary>
        public void SetDriftLevel(float level)
        {
            drift.driftLevel = Mathf.Clamp01(level);
        }

        #endregion
    }
}
