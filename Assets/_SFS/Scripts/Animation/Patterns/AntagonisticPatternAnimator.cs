using UnityEngine;
using System;

namespace SFS.Animation
{
    /// <summary>
    /// Animates antagonistic patterns - Echo Forms, Distortions, and Noise Beasts.
    /// These are embodied consequences of misdesign, not villains.
    /// Resolution through restoration verbs, not violence.
    /// </summary>
    public class AntagonisticPatternAnimator : MonoBehaviour
    {
        #region Configuration

        [Header("═══════════════════════════════════════")]
        [Header("   ANTAGONISTIC PATTERN ANIMATOR       ")]
        [Header("═══════════════════════════════════════")]

        [Header("Target")]
        public Transform visualTarget;

        [Header("Pattern Type")]
        public PatternType patternType = PatternType.Distortion;

        [Header("═══ COMMON SETTINGS ═══")]
        public CommonSettings common = new CommonSettings();

        [Header("═══ ECHO FORM (Coercive Scripts) ═══")]
        public EchoFormSettings echoForm = new EchoFormSettings();

        [Header("═══ DISTORTION (Broken Rules) ═══")]
        public DistortionSettings distortion = new DistortionSettings();

        [Header("═══ NOISE BEAST (Sensory Overload) ═══")]
        public NoiseBeastSettings noiseBeast = new NoiseBeastSettings();

        [Header("═══ RESOLUTION ANIMATION ═══")]
        public ResolutionSettings resolution = new ResolutionSettings();

        [Header("═══ DRIFT RESPONSE ═══")]
        public bool respondToDrift = true;
        [Range(0f, 1f)] public float driftIntensity = 0.5f;

        #endregion

        #region Settings Classes

        public enum PatternType { EchoForm, Distortion, NoiseBeast }

        [Serializable]
        public class CommonSettings
        {
            [Range(0.5f, 3f)] public float baseSpeed = 1f;
            [Range(0f, 1f)] public float intensity = 1f;
            [Range(2f, 20f)] public float smoothing = 8f;
            public bool isActive = true;
            public bool isBeingResolved = false;
        }

        [Serializable]
        public class EchoFormSettings
        {
            [Header("Loop Motion")]
            [Tooltip("Echo Forms repeat coercive patterns")]
            [Range(0.3f, 2f)] public float loopDuration = 1.2f;
            [Range(0f, 0.2f)] public float loopAmplitude = 0.1f;
            [Range(0f, 30f)] public float loopRotation = 15f;

            [Header("Script Execution")]
            [Range(0f, 1f)] public float jerkiness = 0.4f;
            [Range(0f, 0.1f)] public float hesitation = 0.05f;

            [Header("Coercion Pulse")]
            [Range(0.5f, 2f)] public float pulseSpeed = 1f;
            [Range(1f, 1.3f)] public float pulseScale = 1.15f;

            [Header("Dissolution")]
            [Range(0.5f, 2f)] public float dissolveDuration = 1f;
            public AnimationCurve dissolveCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        }

        [Serializable]
        public class DistortionSettings
        {
            [Header("Glitch Effect")]
            [Range(0f, 1f)] public float glitchFrequency = 0.3f;
            [Range(0f, 0.15f)] public float glitchOffset = 0.08f;
            [Range(0f, 45f)] public float glitchRotation = 20f;
            [Range(0.01f, 0.2f)] public float glitchDuration = 0.08f;

            [Header("Phase Shift")]
            [Range(0.2f, 2f)] public float phaseSpeed = 0.8f;
            [Range(0f, 0.1f)] public float phaseWobble = 0.05f;

            [Header("Reality Tear")]
            [Range(0f, 0.05f)] public float tearJitter = 0.02f;
            [Range(0.9f, 1.1f)] public float tearScaleOscillation = 1.05f;

            [Header("Correction")]
            [Range(0.5f, 2f)] public float correctionDuration = 1.2f;
            public AnimationCurve correctionCurve = AnimationCurve.Linear(0, 0, 1, 1);
        }

        [Serializable]
        public class NoiseBeastSettings
        {
            [Header("Sensory Chaos")]
            [Range(1f, 10f)] public float chaosSpeed = 5f;
            [Range(0f, 0.3f)] public float chaosAmplitude = 0.15f;
            [Range(0f, 60f)] public float chaosRotation = 30f;

            [Header("Overload Pulse")]
            [Range(0.5f, 3f)] public float overloadSpeed = 1.5f;
            [Range(0.8f, 1.4f)] public float overloadScaleMin = 0.9f;
            [Range(1f, 1.6f)] public float overloadScaleMax = 1.3f;

            [Header("Storm Effect")]
            [Range(0f, 0.2f)] public float stormJitter = 0.1f;
            [Range(0f, 1f)] public float stormIntensity = 0.7f;

            [Header("Calming")]
            [Range(1f, 4f)] public float calmingDuration = 2f;
            public AnimationCurve calmingCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        }

        [Serializable]
        public class ResolutionSettings
        {
            [Header("When Resolved by Combat Verb")]
            [Range(0f, 0.15f)] public float finalPop = 0.1f;
            [Range(0f, 720f)] public float finalSpin = 180f;
            [Range(0.3f, 1.5f)] public float fadeOutDuration = 0.8f;
            public bool destroyOnResolution = true;
            public VFXType resolutionVFX = VFXType.AxiomRestore;
        }

        #endregion

        #region Runtime State

        float animTime;
        float glitchTimer;
        bool isGlitching;
        float resolutionTimer;
        bool isResolving;
        CombatVerb resolvingVerb;

        Vector3 baseScale;
        Vector3 baseLocalPos;
        Quaternion baseLocalRot;

        Vector3 currentOffset;
        Vector3 targetOffset;
        Vector3 currentRotation;
        Vector3 targetRotation;
        float currentScale = 1f;
        float targetScale = 1f;

        Material cachedMaterial;
        Color originalColor;

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

            var renderer = visualTarget.GetComponent<Renderer>();
            if (renderer)
            {
                cachedMaterial = renderer.material;
                originalColor = cachedMaterial.color;
            }
        }

        void Start()
        {
            baseScale = visualTarget.localScale;
            baseLocalPos = visualTarget.localPosition;
            baseLocalRot = visualTarget.localRotation;

            SubscribeToEvents();
        }

        void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        void SubscribeToEvents()
        {
            AnimationEvents.OnCombatVerbUsed += HandleCombatVerb;
            AnimationEvents.OnDriftIntensityChanged += HandleDriftChange;
        }

        void UnsubscribeFromEvents()
        {
            AnimationEvents.OnCombatVerbUsed -= HandleCombatVerb;
            AnimationEvents.OnDriftIntensityChanged -= HandleDriftChange;
        }

        void Update()
        {
            if (!common.isActive) return;

            float dt = Time.deltaTime;
            animTime += dt * common.baseSpeed;

            if (isResolving)
            {
                UpdateResolution(dt);
            }
            else
            {
                switch (patternType)
                {
                    case PatternType.EchoForm:
                        UpdateEchoForm(dt);
                        break;
                    case PatternType.Distortion:
                        UpdateDistortion(dt);
                        break;
                    case PatternType.NoiseBeast:
                        UpdateNoiseBeast(dt);
                        break;
                }
            }

            ApplyAnimation(dt);
        }

        #endregion

        #region Echo Form Animation

        void UpdateEchoForm(float dt)
        {
            float intensity = common.intensity * driftIntensity;
            float loopPhase = (animTime % echoForm.loopDuration) / echoForm.loopDuration;

            // Looping coercive motion
            float loopT = loopPhase * Mathf.PI * 2f;

            // Jerky, mechanical movement
            float jerkPhase = loopPhase;
            if (echoForm.jerkiness > 0)
            {
                // Quantize to create jerky steps
                int steps = Mathf.RoundToInt(5f / (1f - echoForm.jerkiness + 0.1f));
                jerkPhase = Mathf.Floor(loopPhase * steps) / steps;
            }

            // Loop motion
            targetOffset.x = Mathf.Sin(jerkPhase * Mathf.PI * 2f) * echoForm.loopAmplitude * intensity;
            targetOffset.y = Mathf.Cos(jerkPhase * Mathf.PI * 4f) * echoForm.loopAmplitude * 0.5f * intensity;
            targetOffset.z = Mathf.Sin(jerkPhase * Mathf.PI * 2f + 1f) * echoForm.loopAmplitude * 0.3f * intensity;

            // Loop rotation
            targetRotation.y = Mathf.Sin(jerkPhase * Mathf.PI * 2f) * echoForm.loopRotation * intensity;
            targetRotation.x = Mathf.Sin(jerkPhase * Mathf.PI * 4f) * echoForm.loopRotation * 0.3f * intensity;

            // Hesitation at loop points
            if (loopPhase < echoForm.hesitation || loopPhase > 1f - echoForm.hesitation)
            {
                targetOffset *= 0.3f;
                targetRotation *= 0.3f;
            }

            // Coercion pulse
            float pulsePhase = animTime * echoForm.pulseSpeed * Mathf.PI * 2f;
            targetScale = 1f + (echoForm.pulseScale - 1f) * Mathf.Abs(Mathf.Sin(pulsePhase)) * intensity;
        }

        #endregion

        #region Distortion Animation

        void UpdateDistortion(float dt)
        {
            float intensity = common.intensity * driftIntensity;

            // Phase shift wobble
            float phaseAngle = animTime * distortion.phaseSpeed * Mathf.PI * 2f;
            targetOffset.x = Mathf.Sin(phaseAngle) * distortion.phaseWobble * intensity;
            targetOffset.y = Mathf.Sin(phaseAngle * 1.3f) * distortion.phaseWobble * 0.5f * intensity;
            targetOffset.z = Mathf.Cos(phaseAngle * 0.7f) * distortion.phaseWobble * 0.7f * intensity;

            // Reality tear jitter
            targetOffset.x += (Mathf.PerlinNoise(animTime * 15f, 0f) - 0.5f) * distortion.tearJitter * intensity;
            targetOffset.y += (Mathf.PerlinNoise(0f, animTime * 15f) - 0.5f) * distortion.tearJitter * intensity;

            // Scale oscillation
            float scalePhase = animTime * 3f;
            targetScale = 1f + Mathf.Sin(scalePhase) * (distortion.tearScaleOscillation - 1f) * intensity;

            // Random glitches
            if (!isGlitching && UnityEngine.Random.value < distortion.glitchFrequency * dt)
            {
                TriggerGlitch();
            }

            if (isGlitching)
            {
                glitchTimer -= dt;

                // Glitch offset and rotation
                targetOffset += new Vector3(
                    UnityEngine.Random.Range(-distortion.glitchOffset, distortion.glitchOffset),
                    UnityEngine.Random.Range(-distortion.glitchOffset * 0.5f, distortion.glitchOffset * 0.5f),
                    UnityEngine.Random.Range(-distortion.glitchOffset, distortion.glitchOffset)
                ) * intensity;

                targetRotation.z = UnityEngine.Random.Range(-distortion.glitchRotation, distortion.glitchRotation) * intensity;

                if (glitchTimer <= 0f)
                {
                    isGlitching = false;
                }
            }
        }

        void TriggerGlitch()
        {
            isGlitching = true;
            glitchTimer = distortion.glitchDuration;
        }

        #endregion

        #region Noise Beast Animation

        void UpdateNoiseBeast(float dt)
        {
            float intensity = common.intensity * driftIntensity;
            float stormFactor = noiseBeast.stormIntensity;

            // Chaotic movement
            float chaosT = animTime * noiseBeast.chaosSpeed;
            targetOffset.x = Mathf.PerlinNoise(chaosT, 0f) * noiseBeast.chaosAmplitude * 2f - noiseBeast.chaosAmplitude;
            targetOffset.y = Mathf.PerlinNoise(chaosT * 1.3f, 10f) * noiseBeast.chaosAmplitude * 2f - noiseBeast.chaosAmplitude;
            targetOffset.z = Mathf.PerlinNoise(chaosT * 0.8f, 20f) * noiseBeast.chaosAmplitude * 2f - noiseBeast.chaosAmplitude;
            targetOffset *= intensity * stormFactor;

            // Chaotic rotation
            targetRotation.x = (Mathf.PerlinNoise(chaosT * 1.5f, 30f) - 0.5f) * noiseBeast.chaosRotation * 2f * intensity * stormFactor;
            targetRotation.y = (Mathf.PerlinNoise(chaosT * 1.2f, 40f) - 0.5f) * noiseBeast.chaosRotation * 2f * intensity * stormFactor;
            targetRotation.z = (Mathf.PerlinNoise(chaosT * 0.9f, 50f) - 0.5f) * noiseBeast.chaosRotation * 2f * intensity * stormFactor;

            // Overload pulsing scale
            float overloadPhase = animTime * noiseBeast.overloadSpeed * Mathf.PI * 2f;
            float scaleRange = noiseBeast.overloadScaleMax - noiseBeast.overloadScaleMin;
            targetScale = noiseBeast.overloadScaleMin + (Mathf.Sin(overloadPhase) + 1f) * 0.5f * scaleRange;
            targetScale = Mathf.Lerp(1f, targetScale, intensity * stormFactor);

            // Storm jitter
            targetOffset += new Vector3(
                (UnityEngine.Random.value - 0.5f) * noiseBeast.stormJitter,
                (UnityEngine.Random.value - 0.5f) * noiseBeast.stormJitter,
                (UnityEngine.Random.value - 0.5f) * noiseBeast.stormJitter
            ) * intensity * stormFactor;
        }

        #endregion

        #region Resolution Animation

        void HandleCombatVerb(CombatVerb verb, Vector3 targetPos)
        {
            if (!common.isActive || isResolving) return;

            // Check if we can be resolved by this verb
            float distance = Vector3.Distance(transform.position, targetPos);
            if (distance > 5f) return;

            bool canResolve = CanBeResolvedBy(verb);
            if (canResolve)
            {
                BeginResolution(verb);
            }
        }

        bool CanBeResolvedBy(CombatVerb verb)
        {
            return patternType switch
            {
                PatternType.EchoForm => verb == CombatVerb.ThreadLash || verb == CombatVerb.Pulse,
                PatternType.Distortion => verb == CombatVerb.Pulse || verb == CombatVerb.EdgeClaim,
                PatternType.NoiseBeast => verb == CombatVerb.ReTune || verb == CombatVerb.RadiantHold,
                _ => false
            };
        }

        public void BeginResolution(CombatVerb verb)
        {
            isResolving = true;
            resolutionTimer = 0f;
            resolvingVerb = verb;
            common.isBeingResolved = true;

            AnimationEvents.VFXRequest(resolution.resolutionVFX, transform.position, Quaternion.identity, 1f);
        }

        void UpdateResolution(float dt)
        {
            resolutionTimer += dt;
            float duration = GetResolutionDuration();
            float t = Mathf.Clamp01(resolutionTimer / duration);

            AnimationCurve fadeCurve = patternType switch
            {
                PatternType.EchoForm => echoForm.dissolveCurve,
                PatternType.Distortion => distortion.correctionCurve,
                PatternType.NoiseBeast => noiseBeast.calmingCurve,
                _ => AnimationCurve.Linear(0, 1, 1, 0)
            };

            float curveValue = fadeCurve.Evaluate(t);

            // Pop and spin
            if (t < 0.2f)
            {
                float popT = t / 0.2f;
                targetScale = 1f + resolution.finalPop * Mathf.Sin(popT * Mathf.PI);
            }
            else
            {
                targetScale = Mathf.Lerp(1f, 0f, curveValue);
            }

            targetRotation.y = resolution.finalSpin * t;

            // Fade alpha
            if (cachedMaterial)
            {
                Color color = originalColor;
                color.a = 1f - curveValue;
                cachedMaterial.color = color;
            }

            if (t >= 1f)
            {
                OnResolutionComplete();
            }
        }

        float GetResolutionDuration()
        {
            return patternType switch
            {
                PatternType.EchoForm => echoForm.dissolveDuration,
                PatternType.Distortion => distortion.correctionDuration,
                PatternType.NoiseBeast => noiseBeast.calmingDuration,
                _ => resolution.fadeOutDuration
            };
        }

        void OnResolutionComplete()
        {
            common.isActive = false;

            if (resolution.destroyOnResolution)
            {
                Destroy(gameObject);
            }
        }

        #endregion

        #region Drift Response

        void HandleDriftChange(float previous, float current)
        {
            if (!respondToDrift) return;
            driftIntensity = current;
        }

        /// <summary>Set drift intensity directly (0-1)</summary>
        public void SetDriftIntensity(float intensity)
        {
            driftIntensity = Mathf.Clamp01(intensity);
        }

        #endregion

        #region Apply Animation

        void ApplyAnimation(float dt)
        {
            float speed = common.smoothing * dt;

            currentOffset = Vector3.Lerp(currentOffset, targetOffset, speed);
            currentRotation = Vector3.Lerp(currentRotation, targetRotation, speed);
            currentScale = Mathf.Lerp(currentScale, targetScale, speed);

            visualTarget.localPosition = baseLocalPos + currentOffset;
            visualTarget.localRotation = baseLocalRot * Quaternion.Euler(currentRotation);
            visualTarget.localScale = baseScale * currentScale;
        }

        #endregion

        #region Public API

        /// <summary>Manually trigger resolution (for external systems)</summary>
        public void TriggerResolution()
        {
            BeginResolution(CombatVerb.Pulse);
        }

        /// <summary>Set pattern intensity (0-1)</summary>
        public void SetIntensity(float intensity)
        {
            common.intensity = Mathf.Clamp01(intensity);
        }

        /// <summary>Pause/resume animation</summary>
        public void SetActive(bool active)
        {
            common.isActive = active;
        }

        #endregion
    }
}
