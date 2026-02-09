using UnityEngine;
using System;

namespace SFS.Animation
{
    /// <summary>
    /// Production-ready procedural animation system with layered effects,
    /// easing curves, emotional response, and story beat integration.
    /// Works immediately without animation clips.
    /// </summary>
    [DisallowMultipleComponent]
    public class AdvancedProceduralAnimator : MonoBehaviour
    {
        #region Inspector Fields

        [Header("═══ TARGET SETUP ═══")]
        [Tooltip("The visual mesh/model to animate. Auto-finds child if empty.")]
        public Transform visualTarget;

        [Tooltip("Optional secondary targets (arms, accessories) for layered motion")]
        public Transform[] secondaryTargets;

        [Header("═══ IDLE ANIMATION ═══")]
        public IdleSettings idle = new IdleSettings();

        [Header("═══ LOCOMOTION ═══")]
        public LocomotionSettings locomotion = new LocomotionSettings();

        [Header("═══ AIRBORNE ═══")]
        public AirborneSettings airborne = new AirborneSettings();

        [Header("═══ REACTIONS ═══")]
        public ReactionSettings reactions = new ReactionSettings();

        [Header("═══ EMOTIONAL LAYERS ═══")]
        public EmotionalSettings emotional = new EmotionalSettings();

        [Header("═══ EASING ═══")]
        public EasingSettings easing = new EasingSettings();

        #endregion

        #region Settings Classes

        [Serializable]
        public class IdleSettings
        {
            [Range(0.5f, 4f)] public float breatheSpeed = 1.8f;
            [Range(0f, 0.1f)] public float breatheAmount = 0.03f;
            [Range(0f, 0.15f)] public float swayAmount = 0.02f;
            [Range(0.1f, 2f)] public float swaySpeed = 0.7f;
            [Range(0f, 5f)] public float headBobAngle = 2f;
            public bool enableMicroMovements = true;
            [Range(0f, 1f)] public float microMovementIntensity = 0.3f;
        }

        [Serializable]
        public class LocomotionSettings
        {
            [Header("Walk")]
            [Range(4f, 12f)] public float walkBobSpeed = 7f;
            [Range(0f, 0.2f)] public float walkBobHeight = 0.06f;
            [Range(0f, 0.1f)] public float walkBobSide = 0.02f;
            [Range(0f, 8f)] public float walkTilt = 3f;
            [Range(0f, 8f)] public float walkLean = 2f;

            [Header("Run")]
            [Range(8f, 20f)] public float runBobSpeed = 14f;
            [Range(0f, 0.3f)] public float runBobHeight = 0.1f;
            [Range(0f, 0.15f)] public float runBobSide = 0.04f;
            [Range(0f, 12f)] public float runTilt = 5f;
            [Range(0f, 15f)] public float runLean = 8f;

            [Header("Speed Thresholds")]
            [Range(0f, 0.3f)] public float idleThreshold = 0.05f;
            [Range(0.3f, 0.8f)] public float runThreshold = 0.55f;
        }

        [Serializable]
        public class AirborneSettings
        {
            [Header("Jump")]
            [Range(0.5f, 1f)] public float jumpSquash = 0.75f;
            [Range(1f, 1.5f)] public float jumpStretch = 1.25f;
            [Range(0f, 20f)] public float jumpTuckAngle = 10f;

            [Header("Fall")]
            [Range(0f, 0.2f)] public float fallAnticipation = 0.08f;
            [Range(0f, 30f)] public float fallSpreadAngle = 15f;
            public bool enableWindEffect = true;
            [Range(0f, 0.1f)] public float windShakeAmount = 0.02f;

            [Header("Land")]
            [Range(0.4f, 0.9f)] public float landSquash = 0.6f;
            [Range(1.1f, 1.6f)] public float landSpread = 1.3f;
            [Range(0.1f, 0.5f)] public float landRecoveryTime = 0.25f;
        }

        [Serializable]
        public class ReactionSettings
        {
            [Header("Damage")]
            [Range(0.3f, 0.8f)] public float damageSquash = 0.5f;
            [Range(0f, 30f)] public float damageRecoilAngle = 15f;
            [Range(0.1f, 0.5f)] public float damageFlashDuration = 0.2f;

            [Header("Collect")]
            [Range(1f, 1.3f)] public float collectPop = 1.15f;
            [Range(0f, 360f)] public float collectSpinSpeed = 180f;

            [Header("Death")]
            [Range(0.5f, 2f)] public float deathDuration = 1.2f;
            public AnimationCurve deathCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        }

        [Serializable]
        public class EmotionalSettings
        {
            [Tooltip("Current emotional state affects animation intensity")]
            public EmotionalTone currentTone = EmotionalTone.Neutral;

            [Range(0.5f, 2f)] public float gentleMultiplier = 0.6f;
            [Range(0.8f, 1.5f)] public float hopefulMultiplier = 1.2f;
            [Range(0.3f, 1f)] public float melancholicMultiplier = 0.5f;
            [Range(0.7f, 1.3f)] public float groundedMultiplier = 0.9f;

            public bool respondToStoryBeats = true;
        }

        [Serializable]
        public class EasingSettings
        {
            [Range(1f, 20f)] public float squashRecoverySpeed = 10f;
            [Range(1f, 20f)] public float rotationSmoothing = 8f;
            [Range(1f, 20f)] public float positionSmoothing = 12f;
            public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }

        public enum EmotionalTone { Neutral, Gentle, Hopeful, Melancholic, Grounded }

        #endregion

        #region Runtime State

        // Input state
        float currentSpeed;
        bool isGrounded = true;
        bool wasGrounded = true;
        float yVelocity;
        Vector3 moveDirection;

        // Animation state
        float animTime;
        float squashY = 1f;
        float stretchXZ = 1f;
        Vector3 currentOffset;
        Vector3 currentRotation;
        Vector3 targetOffset;
        Vector3 targetRotation;

        // Reaction state
        bool isReacting;
        float reactionTimer;
        ReactionType currentReaction;

        // Base transforms
        Vector3 baseScale;
        Vector3 baseLocalPos;
        Quaternion baseLocalRot;

        enum ReactionType { None, Jump, Land, Damage, Collect, Death }

        #endregion

        #region Lifecycle

        void Awake()
        {
            if (!visualTarget)
            {
                // Try to find a child mesh
                var meshRenderer = GetComponentInChildren<MeshRenderer>();
                if (meshRenderer) visualTarget = meshRenderer.transform;
                else visualTarget = transform;
            }
        }

        void Start()
        {
            CacheBaseTransforms();
        }

        void CacheBaseTransforms()
        {
            baseScale = visualTarget.localScale;
            baseLocalPos = visualTarget.localPosition;
            baseLocalRot = visualTarget.localRotation;
        }

        void Update()
        {
            float dt = Time.deltaTime;
            animTime += dt;

            ProcessReactions(dt);
            CalculateAnimation(dt);
            ApplyAnimation(dt);
        }

        #endregion

        #region Animation Calculation

        void CalculateAnimation(float dt)
        {
            if (isReacting && currentReaction == ReactionType.Death) return;

            float emotionMult = GetEmotionalMultiplier();

            targetOffset = Vector3.zero;
            targetRotation = Vector3.zero;

            if (isGrounded)
            {
                if (currentSpeed < locomotion.idleThreshold)
                {
                    CalculateIdle(emotionMult);
                }
                else if (currentSpeed < locomotion.runThreshold)
                {
                    CalculateWalk(emotionMult);
                }
                else
                {
                    CalculateRun(emotionMult);
                }
            }
            else
            {
                CalculateAirborne(emotionMult);
            }
        }

        void CalculateIdle(float emotionMult)
        {
            // Breathing
            float breathe = Mathf.Sin(animTime * idle.breatheSpeed * Mathf.PI * 2f);
            targetOffset.y = breathe * idle.breatheAmount * emotionMult;

            // Gentle sway
            float sway = Mathf.Sin(animTime * idle.swaySpeed) * idle.swayAmount;
            targetOffset.x = sway * emotionMult;

            // Head bob
            targetRotation.x = Mathf.Sin(animTime * idle.swaySpeed * 0.7f) * idle.headBobAngle * emotionMult;

            // Micro movements
            if (idle.enableMicroMovements)
            {
                float micro = idle.microMovementIntensity * emotionMult;
                targetOffset += new Vector3(
                    Mathf.PerlinNoise(animTime * 2f, 0) * 0.01f * micro,
                    Mathf.PerlinNoise(0, animTime * 1.5f) * 0.005f * micro,
                    Mathf.PerlinNoise(animTime * 1.8f, animTime) * 0.01f * micro
                );
            }

            // Breathing scale
            float breatheScale = 1f + breathe * 0.015f * emotionMult;
            squashY = Mathf.Lerp(squashY, breatheScale, easing.squashRecoverySpeed * Time.deltaTime);
            stretchXZ = Mathf.Lerp(stretchXZ, 2f - breatheScale, easing.squashRecoverySpeed * Time.deltaTime);
        }

        void CalculateWalk(float emotionMult)
        {
            float t = animTime * locomotion.walkBobSpeed;

            // Step bob - use abs(sin) for proper footfall
            targetOffset.y = Mathf.Abs(Mathf.Sin(t)) * locomotion.walkBobHeight * emotionMult;
            targetOffset.x = Mathf.Sin(t * 0.5f) * locomotion.walkBobSide * emotionMult;

            // Tilt with steps
            targetRotation.z = Mathf.Sin(t * 0.5f) * locomotion.walkTilt * emotionMult;

            // Forward lean based on speed
            float speedFactor = Mathf.InverseLerp(locomotion.idleThreshold, locomotion.runThreshold, currentSpeed);
            targetRotation.x = speedFactor * locomotion.walkLean * emotionMult;

            // Reset scale smoothly
            squashY = Mathf.Lerp(squashY, 1f, easing.squashRecoverySpeed * Time.deltaTime);
            stretchXZ = Mathf.Lerp(stretchXZ, 1f, easing.squashRecoverySpeed * Time.deltaTime);
        }

        void CalculateRun(float emotionMult)
        {
            float t = animTime * locomotion.runBobSpeed;

            // Exaggerated bob
            targetOffset.y = Mathf.Abs(Mathf.Sin(t)) * locomotion.runBobHeight * emotionMult;
            targetOffset.x = Mathf.Sin(t * 0.5f) * locomotion.runBobSide * emotionMult;

            // More aggressive tilt
            targetRotation.z = Mathf.Sin(t * 0.5f) * locomotion.runTilt * emotionMult;

            // Strong forward lean
            targetRotation.x = locomotion.runLean * emotionMult;

            // Slight compression during run
            squashY = Mathf.Lerp(squashY, 0.95f, easing.squashRecoverySpeed * Time.deltaTime);
            stretchXZ = Mathf.Lerp(stretchXZ, 1.03f, easing.squashRecoverySpeed * Time.deltaTime);
        }

        void CalculateAirborne(float emotionMult)
        {
            if (yVelocity > 0.5f)
            {
                // Rising - stretch upward
                float stretchAmount = Mathf.Lerp(1f, airborne.jumpStretch, Mathf.Clamp01(yVelocity / 10f));
                squashY = Mathf.Lerp(squashY, stretchAmount, easing.squashRecoverySpeed * Time.deltaTime);
                stretchXZ = Mathf.Lerp(stretchXZ, 1f / Mathf.Sqrt(stretchAmount), easing.squashRecoverySpeed * Time.deltaTime);

                // Tuck pose
                targetRotation.x = -airborne.jumpTuckAngle * emotionMult;
            }
            else
            {
                // Falling - anticipate landing
                float fallFactor = Mathf.Clamp01(-yVelocity / 15f);

                // Spread pose
                targetRotation.x = airborne.fallSpreadAngle * fallFactor * emotionMult;

                // Anticipation squash
                float anticipate = 1f - (fallFactor * airborne.fallAnticipation);
                squashY = Mathf.Lerp(squashY, anticipate, easing.squashRecoverySpeed * Time.deltaTime);

                // Wind shake effect
                if (airborne.enableWindEffect && fallFactor > 0.3f)
                {
                    float shake = airborne.windShakeAmount * fallFactor;
                    targetOffset.x += Mathf.PerlinNoise(animTime * 20f, 0) * shake - shake * 0.5f;
                    targetOffset.z += Mathf.PerlinNoise(0, animTime * 20f) * shake - shake * 0.5f;
                }
            }
        }

        #endregion

        #region Reactions

        void ProcessReactions(float dt)
        {
            if (!isReacting) return;

            reactionTimer -= dt;

            switch (currentReaction)
            {
                case ReactionType.Jump:
                    ProcessJumpReaction();
                    break;
                case ReactionType.Land:
                    ProcessLandReaction();
                    break;
                case ReactionType.Damage:
                    ProcessDamageReaction();
                    break;
                case ReactionType.Collect:
                    ProcessCollectReaction();
                    break;
                case ReactionType.Death:
                    ProcessDeathReaction();
                    break;
            }

            if (reactionTimer <= 0)
            {
                EndReaction();
            }
        }

        void ProcessJumpReaction()
        {
            float t = 1f - (reactionTimer / 0.15f);
            if (t < 0.3f)
            {
                // Initial squat
                squashY = Mathf.Lerp(1f, airborne.jumpSquash, t / 0.3f);
                stretchXZ = 2f - squashY;
            }
            else
            {
                // Spring up
                squashY = Mathf.Lerp(airborne.jumpSquash, airborne.jumpStretch, (t - 0.3f) / 0.7f);
                stretchXZ = 2f - squashY;
            }
        }

        void ProcessLandReaction()
        {
            float t = 1f - (reactionTimer / airborne.landRecoveryTime);
            float curve = easing.transitionCurve.Evaluate(t);

            if (t < 0.3f)
            {
                // Impact squash
                squashY = Mathf.Lerp(1f, airborne.landSquash, t / 0.3f);
                stretchXZ = Mathf.Lerp(1f, airborne.landSpread, t / 0.3f);
            }
            else
            {
                // Recovery
                squashY = Mathf.Lerp(airborne.landSquash, 1f, (t - 0.3f) / 0.7f);
                stretchXZ = Mathf.Lerp(airborne.landSpread, 1f, (t - 0.3f) / 0.7f);
            }
        }

        void ProcessDamageReaction()
        {
            float t = 1f - (reactionTimer / reactions.damageFlashDuration);
            squashY = Mathf.Lerp(reactions.damageSquash, 1f, t);
            stretchXZ = 2f - squashY;
            targetRotation.x = Mathf.Lerp(reactions.damageRecoilAngle, 0, t);
        }

        void ProcessCollectReaction()
        {
            float t = 1f - (reactionTimer / 0.3f);
            float pop = reactions.collectPop * (1f - t * t); // Quick pop, slow settle
            squashY = 1f + (pop - 1f) * (1f - t);
            targetRotation.y += reactions.collectSpinSpeed * Time.deltaTime * (1f - t);
        }

        void ProcessDeathReaction()
        {
            float t = 1f - (reactionTimer / reactions.deathDuration);
            float curve = reactions.deathCurve.Evaluate(t);

            squashY = Mathf.Lerp(1f, 0.1f, curve);
            stretchXZ = Mathf.Lerp(1f, 2f, curve);
            targetRotation.x = Mathf.Lerp(0, 90f, curve);
            targetOffset.y = -baseLocalPos.y * curve * 0.8f;
        }

        void EndReaction()
        {
            isReacting = false;
            currentReaction = ReactionType.None;
        }

        void StartReaction(ReactionType type, float duration)
        {
            isReacting = true;
            currentReaction = type;
            reactionTimer = duration;
        }

        #endregion

        #region Apply Animation

        void ApplyAnimation(float dt)
        {
            // Smooth towards targets
            currentOffset = Vector3.Lerp(currentOffset, targetOffset, easing.positionSmoothing * dt);
            currentRotation = Vector3.Lerp(currentRotation, targetRotation, easing.rotationSmoothing * dt);

            // Calculate final transforms
            Vector3 finalPosition = baseLocalPos + currentOffset;
            Quaternion finalRotation = baseLocalRot * Quaternion.Euler(currentRotation);
            Vector3 finalScale = new Vector3(
                baseScale.x * stretchXZ,
                baseScale.y * squashY,
                baseScale.z * stretchXZ
            );

            // Apply
            visualTarget.localPosition = finalPosition;
            visualTarget.localRotation = finalRotation;
            visualTarget.localScale = finalScale;

            // Apply to secondary targets with delay
            ApplySecondaryTargets(dt);
        }

        void ApplySecondaryTargets(float dt)
        {
            if (secondaryTargets == null) return;

            for (int i = 0; i < secondaryTargets.Length; i++)
            {
                if (!secondaryTargets[i]) continue;

                // Delayed, dampened follow
                float delay = 0.8f - (i * 0.1f);
                Vector3 delayedOffset = currentOffset * delay;
                Vector3 delayedRotation = currentRotation * delay * 0.5f;

                secondaryTargets[i].localPosition = Vector3.Lerp(
                    secondaryTargets[i].localPosition,
                    delayedOffset,
                    easing.positionSmoothing * 0.5f * dt
                );
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// Update animation state from PlayerController
        /// </summary>
        public void SetState(float speed01, bool grounded, float yVel, Vector3 moveDir = default)
        {
            currentSpeed = speed01;
            yVelocity = yVel;
            moveDirection = moveDir;

            // Landing detection
            if (grounded && !isGrounded)
            {
                OnLand(Mathf.Abs(yVelocity));
            }

            wasGrounded = isGrounded;
            isGrounded = grounded;
        }

        /// <summary>
        /// Trigger jump animation
        /// </summary>
        public void OnJump()
        {
            StartReaction(ReactionType.Jump, 0.15f);
        }

        /// <summary>
        /// Trigger land animation with impact strength
        /// </summary>
        public void OnLand(float impactVelocity = 5f)
        {
            float intensity = Mathf.Clamp01(impactVelocity / 15f);
            airborne.landSquash = Mathf.Lerp(0.8f, 0.5f, intensity);
            StartReaction(ReactionType.Land, airborne.landRecoveryTime);
        }

        /// <summary>
        /// Trigger damage reaction
        /// </summary>
        public void OnDamage()
        {
            StartReaction(ReactionType.Damage, reactions.damageFlashDuration);
        }

        /// <summary>
        /// Trigger collect animation
        /// </summary>
        public void OnCollect()
        {
            StartReaction(ReactionType.Collect, 0.3f);
        }

        /// <summary>
        /// Trigger death animation
        /// </summary>
        public void OnDeath()
        {
            StartReaction(ReactionType.Death, reactions.deathDuration);
        }

        /// <summary>
        /// Set emotional tone for animation intensity
        /// </summary>
        public void SetEmotionalTone(EmotionalTone tone)
        {
            emotional.currentTone = tone;
        }

        /// <summary>
        /// Set emotional tone from story beat int (0-3)
        /// </summary>
        public void SetEmotionalTone(int toneIndex)
        {
            emotional.currentTone = (EmotionalTone)Mathf.Clamp(toneIndex, 0, 4);
        }

        float GetEmotionalMultiplier()
        {
            return emotional.currentTone switch
            {
                EmotionalTone.Gentle => emotional.gentleMultiplier,
                EmotionalTone.Hopeful => emotional.hopefulMultiplier,
                EmotionalTone.Melancholic => emotional.melancholicMultiplier,
                EmotionalTone.Grounded => emotional.groundedMultiplier,
                _ => 1f
            };
        }

        /// <summary>
        /// Reset to base pose
        /// </summary>
        public void ResetToBase()
        {
            visualTarget.localPosition = baseLocalPos;
            visualTarget.localRotation = baseLocalRot;
            visualTarget.localScale = baseScale;
            squashY = 1f;
            stretchXZ = 1f;
            currentOffset = Vector3.zero;
            currentRotation = Vector3.zero;
            isReacting = false;
        }

        #endregion
    }
}
