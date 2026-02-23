using UnityEngine;
using System;
using System.Collections.Generic;

namespace SFS.Animation
{
    /// <summary>
    /// Base NPC animation controller with story beat response and acknowledgment.
    /// Extended by character-specific animators.
    /// </summary>
    public class NPCAnimationController : MonoBehaviour
    {
        #region Configuration

        [Header("═══════════════════════════════════════")]
        [Header("      NPC ANIMATION CONTROLLER         ")]
        [Header("═══════════════════════════════════════")]

        [Header("Target")]
        [Tooltip("The visual model to animate")]
        public Transform visualTarget;

        [Header("═══ CHARACTER TYPE ═══")]
        public NPCCharacterType characterType = NPCCharacterType.SocietyMember;

        [Header("═══ IDLE ANIMATION ═══")]
        public NPCIdleSettings idle = new NPCIdleSettings();

        [Header("═══ ACKNOWLEDGMENT ═══")]
        public AcknowledgmentSettings acknowledgment = new AcknowledgmentSettings();

        [Header("═══ EMOTIONAL RESPONSE ═══")]
        public NPCEmotionalSettings emotional = new NPCEmotionalSettings();

        [Header("═══ GROUP BEHAVIOR ═══")]
        public GroupSettings group = new GroupSettings();

        [Header("═══ BELONGING STATE ═══")]
        public BelongingSettings belonging = new BelongingSettings();

        [Header("═══ SMOOTHING ═══")]
        [Range(2f, 20f)] public float smoothingSpeed = 10f;

        #endregion

        #region Settings Classes

        [Serializable]
        public class NPCIdleSettings
        {
            [Header("Breathing")]
            [Range(0.5f, 3f)] public float breatheSpeed = 1.5f;
            [Range(0f, 0.06f)] public float breatheAmount = 0.025f;
            [Range(0f, 0.02f)] public float breatheScaleAmount = 0.01f;

            [Header("Weight Shift")]
            [Range(0.2f, 1.5f)] public float shiftSpeed = 0.5f;
            [Range(0f, 0.08f)] public float shiftAmount = 0.03f;
            [Range(0f, 12f)] public float shiftTilt = 5f;

            [Header("Look Around")]
            [Range(0.1f, 0.8f)] public float lookSpeed = 0.3f;
            [Range(0f, 50f)] public float lookAngle = 30f;
            [Range(0f, 15f)] public float lookTilt = 8f;
            public bool trackPlayer = true;
            [Range(0f, 1f)] public float playerTrackingWeight = 0.3f;

            [Header("Variance")]
            public IdleVariant primaryVariant = IdleVariant.Breathe;
            [Range(0f, 1f)] public float variantChangeChance = 0.1f;
            [Range(2f, 10f)] public float minVariantDuration = 4f;
        }

        [Serializable]
        public class AcknowledgmentSettings
        {
            [Header("Nod")]
            [Range(0.2f, 0.8f)] public float nodDuration = 0.5f;
            [Range(0f, 0.12f)] public float nodDepth = 0.06f;
            [Range(0f, 25f)] public float nodAngle = 15f;

            [Header("Wave")]
            [Range(0.4f, 1.2f)] public float waveDuration = 0.8f;
            [Range(0f, 45f)] public float waveArc = 30f;
            [Range(2f, 5f)] public float waveCycles = 3f;

            [Header("Attention")]
            [Range(0f, 0.15f)] public float attentionPop = 0.08f;
            [Range(0f, 15f)] public float attentionTurn = 10f;
        }

        [Serializable]
        public class NPCEmotionalSettings
        {
            public bool respondToStoryBeats = true;
            [Range(0.3f, 2f)] public float toneTransitionTime = 1f;

            [Header("Intensity by Tone")]
            [Range(0.3f, 1f)] public float gentleMultiplier = 0.6f;
            [Range(0.8f, 1.4f)] public float hopefulMultiplier = 1.1f;
            [Range(0.2f, 0.7f)] public float melancholicMultiplier = 0.4f;
            [Range(0.5f, 1f)] public float groundedMultiplier = 0.8f;
        }

        [Serializable]
        public class GroupSettings
        {
            public bool enableGroupSync = false;
            public Transform groupLeader;
            [Range(0f, 1f)] public float syncPhaseOffset = 0f;
            [Range(0f, 1f)] public float syncWeight = 0.5f;
        }

        [Serializable]
        public class BelongingSettings
        {
            [Header("Final State (Chapter 12)")]
            public bool inBelongingState = false;
            [Range(0.8f, 2f)] public float belongingBreathSpeed = 1.2f;
            [Range(0f, 0.04f)] public float belongingSwayAmount = 0.025f;
            [Range(0f, 8f)] public float belongingGlow = 0.02f;
        }

        public enum IdleVariant { Breathe, Shift, LookAround }
        public enum NPCCharacterType { SocietyMember, DAZIE, June, Winton, Companion }

        #endregion

        #region Runtime State

        protected float animTime;
        protected IdleVariant currentVariant;
        protected float variantTimer;

        protected bool isAcknowledging;
        protected float ackTimer;
        protected AcknowledgeType currentAckType;
        protected Transform ackTarget;

        protected EmotionalTone currentTone = EmotionalTone.Gentle;
        protected float toneMultiplier = 1f;
        protected float targetToneMultiplier = 1f;

        protected Vector3 baseScale;
        protected Vector3 baseLocalPos;
        protected Quaternion baseLocalRot;

        protected Vector3 currentOffset;
        protected Vector3 targetOffset;
        protected Vector3 currentRotation;
        protected Vector3 targetRotation;
        protected float currentScaleMult = 1f;

        protected Transform playerTransform;

        #endregion

        #region Lifecycle

        protected virtual void Awake()
        {
            if (!visualTarget)
            {
                var meshRenderer = GetComponentInChildren<MeshRenderer>();
                if (meshRenderer) visualTarget = meshRenderer.transform;
                var skinned = GetComponentInChildren<SkinnedMeshRenderer>();
                if (skinned) visualTarget = skinned.transform;
                if (!visualTarget) visualTarget = transform;
            }
        }

        protected virtual void Start()
        {
            CacheBaseTransforms();
            currentVariant = idle.primaryVariant;
            variantTimer = idle.minVariantDuration;

            // Find player
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player) playerTransform = player.transform;

            // Random phase offset for variety
            if (!group.enableGroupSync)
            {
                animTime = UnityEngine.Random.value * 10f;
            }

            SubscribeToEvents();
        }

        protected virtual void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        void SubscribeToEvents()
        {
            AnimationEvents.OnNPCAcknowledge += HandleAcknowledge;
            AnimationEvents.OnNPCGroupSync += HandleGroupSync;
            AnimationEvents.OnNPCEmotionChanged += HandleEmotionChange;
            AnimationEvents.OnNPCBelongingReached += HandleBelonging;
            AnimationEvents.OnStoryBeatAnimation += HandleStoryBeat;
        }

        void UnsubscribeFromEvents()
        {
            AnimationEvents.OnNPCAcknowledge -= HandleAcknowledge;
            AnimationEvents.OnNPCGroupSync -= HandleGroupSync;
            AnimationEvents.OnNPCEmotionChanged -= HandleEmotionChange;
            AnimationEvents.OnNPCBelongingReached -= HandleBelonging;
            AnimationEvents.OnStoryBeatAnimation -= HandleStoryBeat;
        }

        void CacheBaseTransforms()
        {
            baseScale = visualTarget.localScale;
            baseLocalPos = visualTarget.localPosition;
            baseLocalRot = visualTarget.localRotation;
        }

        protected virtual void Update()
        {
            float dt = Time.deltaTime;

            UpdateEmotionalState(dt);
            UpdateVariantTimer(dt);

            if (group.enableGroupSync && group.groupLeader)
            {
                SyncWithLeader();
            }
            else
            {
                animTime += dt;
            }

            targetOffset = Vector3.zero;
            targetRotation = Vector3.zero;

            if (isAcknowledging)
            {
                UpdateAcknowledgment(dt);
            }
            else if (belonging.inBelongingState)
            {
                CalculateBelonging();
            }
            else
            {
                CalculateIdle();
            }

            ApplyPlayerTracking();
            SmoothAndApply(dt);
        }

        #endregion

        #region Idle Animation

        void UpdateVariantTimer(float dt)
        {
            if (isAcknowledging || belonging.inBelongingState) return;

            variantTimer -= dt;
            if (variantTimer <= 0f && UnityEngine.Random.value < idle.variantChangeChance)
            {
                // Change variant
                var variants = Enum.GetValues(typeof(IdleVariant));
                currentVariant = (IdleVariant)variants.GetValue(UnityEngine.Random.Range(0, variants.Length));
                variantTimer = idle.minVariantDuration + UnityEngine.Random.value * 3f;
            }
            else if (variantTimer <= 0f)
            {
                variantTimer = idle.minVariantDuration;
            }
        }

        protected virtual void CalculateIdle()
        {
            switch (currentVariant)
            {
                case IdleVariant.Breathe:
                    CalculateBreathe();
                    break;
                case IdleVariant.Shift:
                    CalculateShift();
                    break;
                case IdleVariant.LookAround:
                    CalculateLookAround();
                    break;
            }
        }

        protected virtual void CalculateBreathe()
        {
            float intensity = toneMultiplier;
            float phase = animTime * idle.breatheSpeed * Mathf.PI * 2f;

            targetOffset.y = Mathf.Sin(phase) * idle.breatheAmount * intensity;
            currentScaleMult = 1f + Mathf.Sin(phase) * idle.breatheScaleAmount * intensity;
        }

        protected virtual void CalculateShift()
        {
            float intensity = toneMultiplier;
            float phase = animTime * idle.shiftSpeed;

            targetOffset.x = Mathf.Sin(phase) * idle.shiftAmount * intensity;
            targetOffset.y = Mathf.Abs(Mathf.Sin(phase * 0.5f)) * idle.shiftAmount * 0.3f * intensity;
            targetRotation.z = Mathf.Sin(phase) * idle.shiftTilt * intensity;

            // Add breathing underneath
            float breathePhase = animTime * idle.breatheSpeed * Mathf.PI * 2f;
            targetOffset.y += Mathf.Sin(breathePhase) * idle.breatheAmount * 0.5f * intensity;
        }

        protected virtual void CalculateLookAround()
        {
            float intensity = toneMultiplier;
            float phase = animTime * idle.lookSpeed;

            targetRotation.y = Mathf.Sin(phase) * idle.lookAngle * intensity;
            targetRotation.x = Mathf.Sin(phase * 0.7f) * idle.lookTilt * intensity;
            targetOffset.x = Mathf.Sin(phase) * 0.015f * intensity;

            // Breathing
            float breathePhase = animTime * idle.breatheSpeed * Mathf.PI * 2f;
            targetOffset.y = Mathf.Sin(breathePhase) * idle.breatheAmount * 0.6f * intensity;
        }

        void ApplyPlayerTracking()
        {
            if (!idle.trackPlayer || !playerTransform) return;

            Vector3 toPlayer = playerTransform.position - transform.position;
            toPlayer.y = 0;

            if (toPlayer.sqrMagnitude > 0.1f)
            {
                float angle = Vector3.SignedAngle(transform.forward, toPlayer.normalized, Vector3.up);
                angle = Mathf.Clamp(angle, -idle.lookAngle, idle.lookAngle);
                targetRotation.y = Mathf.Lerp(targetRotation.y, angle, idle.playerTrackingWeight);
            }
        }

        protected virtual void CalculateBelonging()
        {
            float intensity = toneMultiplier;
            float phase = animTime * belonging.belongingBreathSpeed;

            // Calm, settled breathing
            targetOffset.y = Mathf.Sin(phase * Mathf.PI * 2f) * belonging.belongingSwayAmount * 0.5f * intensity;

            // Gentle sway
            targetOffset.x = Mathf.Sin(phase) * belonging.belongingSwayAmount * intensity;
            targetRotation.z = Mathf.Sin(phase) * 3f * intensity;

            // Subtle glow pulse (scale)
            currentScaleMult = 1f + Mathf.Sin(phase * Mathf.PI * 2f) * belonging.belongingGlow;
        }

        #endregion

        #region Acknowledgment

        void HandleAcknowledge(Transform npc, AcknowledgeType type)
        {
            if (npc != transform) return;

            TriggerAcknowledge(type, null);
        }

        public void TriggerAcknowledge(AcknowledgeType type, Transform target = null)
        {
            isAcknowledging = true;
            ackTimer = 0f;
            currentAckType = type;
            ackTarget = target;
        }

        void UpdateAcknowledgment(float dt)
        {
            ackTimer += dt;
            float duration = GetAckDuration(currentAckType);
            float t = Mathf.Clamp01(ackTimer / duration);

            switch (currentAckType)
            {
                case AcknowledgeType.Nod:
                    AnimateNod(t);
                    break;
                case AcknowledgeType.Wave:
                    AnimateWave(t);
                    break;
                case AcknowledgeType.Bow:
                    AnimateBow(t);
                    break;
                case AcknowledgeType.Gesture:
                    AnimateGesture(t);
                    break;
            }

            if (t >= 1f)
            {
                isAcknowledging = false;
            }
        }

        float GetAckDuration(AcknowledgeType type)
        {
            return type switch
            {
                AcknowledgeType.Nod => acknowledgment.nodDuration,
                AcknowledgeType.Wave => acknowledgment.waveDuration,
                AcknowledgeType.Bow => acknowledgment.nodDuration * 1.5f,
                AcknowledgeType.Gesture => acknowledgment.waveDuration * 0.8f,
                _ => 0.5f
            };
        }

        void AnimateNod(float t)
        {
            float intensity = toneMultiplier;

            if (t < 0.3f)
            {
                // Quick dip
                float dipT = t / 0.3f;
                targetOffset.y = -acknowledgment.nodDepth * dipT * intensity;
                targetRotation.x = acknowledgment.nodAngle * dipT * intensity;
            }
            else if (t < 0.6f)
            {
                // Hold
                targetOffset.y = -acknowledgment.nodDepth * intensity;
                targetRotation.x = acknowledgment.nodAngle * intensity;
            }
            else
            {
                // Return
                float returnT = (t - 0.6f) / 0.4f;
                targetOffset.y = -acknowledgment.nodDepth * (1f - returnT) * intensity;
                targetRotation.x = acknowledgment.nodAngle * (1f - returnT) * intensity;
            }

            // Turn toward target
            if (ackTarget)
            {
                Vector3 toTarget = ackTarget.position - transform.position;
                float angle = Vector3.SignedAngle(transform.forward, toTarget.normalized, Vector3.up);
                targetRotation.y = Mathf.Clamp(angle, -acknowledgment.attentionTurn, acknowledgment.attentionTurn);
            }
        }

        void AnimateWave(float t)
        {
            float intensity = toneMultiplier;

            // Wave arc
            float wavePhase = t * acknowledgment.waveCycles * Mathf.PI * 2f;
            float waveEnvelope = Mathf.Sin(t * Mathf.PI);

            targetRotation.z = Mathf.Sin(wavePhase) * acknowledgment.waveArc * waveEnvelope * intensity;

            // Slight body movement
            targetOffset.x = Mathf.Sin(wavePhase) * 0.02f * waveEnvelope * intensity;

            // Attention pop at start
            if (t < 0.15f)
            {
                float popT = t / 0.15f;
                targetOffset.y = acknowledgment.attentionPop * Mathf.Sin(popT * Mathf.PI) * intensity;
            }
        }

        void AnimateBow(float t)
        {
            float intensity = toneMultiplier;

            if (t < 0.4f)
            {
                // Bow down
                float bowT = t / 0.4f;
                targetOffset.y = -acknowledgment.nodDepth * 2f * bowT * intensity;
                targetRotation.x = acknowledgment.nodAngle * 1.5f * bowT * intensity;
            }
            else if (t < 0.7f)
            {
                // Hold
                targetOffset.y = -acknowledgment.nodDepth * 2f * intensity;
                targetRotation.x = acknowledgment.nodAngle * 1.5f * intensity;
            }
            else
            {
                // Return
                float returnT = (t - 0.7f) / 0.3f;
                targetOffset.y = -acknowledgment.nodDepth * 2f * (1f - returnT) * intensity;
                targetRotation.x = acknowledgment.nodAngle * 1.5f * (1f - returnT) * intensity;
            }
        }

        void AnimateGesture(float t)
        {
            float intensity = toneMultiplier;
            float gestureEnvelope = Mathf.Sin(t * Mathf.PI);

            // Emphatic gesture motion
            targetRotation.y = Mathf.Sin(t * Mathf.PI * 3f) * 15f * gestureEnvelope * intensity;
            targetOffset.y = acknowledgment.attentionPop * gestureEnvelope * intensity;
        }

        #endregion

        #region Group Sync

        void HandleGroupSync(Transform leader, float phase)
        {
            if (leader == group.groupLeader)
            {
                animTime = phase + group.syncPhaseOffset;
            }
        }

        void SyncWithLeader()
        {
            var leaderNPC = group.groupLeader.GetComponent<NPCAnimationController>();
            if (leaderNPC)
            {
                float leaderTime = leaderNPC.animTime;
                animTime = Mathf.Lerp(animTime, leaderTime + group.syncPhaseOffset, group.syncWeight);
            }
        }

        #endregion

        #region Emotional Response

        void HandleEmotionChange(Transform npc, EmotionalTone tone)
        {
            if (npc != transform) return;
            SetEmotionalTone(tone);
        }

        void HandleStoryBeat(int chapter, EmotionalTone tone)
        {
            if (!emotional.respondToStoryBeats) return;
            SetEmotionalTone(tone);
        }

        void HandleBelonging(Transform npc)
        {
            if (npc != transform) return;
            belonging.inBelongingState = true;
        }

        public void SetEmotionalTone(EmotionalTone tone)
        {
            currentTone = tone;
            targetToneMultiplier = GetMultiplierForTone(tone);
        }

        float GetMultiplierForTone(EmotionalTone tone)
        {
            return tone switch
            {
                EmotionalTone.Gentle => emotional.gentleMultiplier,
                EmotionalTone.Hopeful => emotional.hopefulMultiplier,
                EmotionalTone.Melancholic => emotional.melancholicMultiplier,
                EmotionalTone.Grounded => emotional.groundedMultiplier,
                _ => 1f
            };
        }

        void UpdateEmotionalState(float dt)
        {
            toneMultiplier = Mathf.MoveTowards(toneMultiplier, targetToneMultiplier, dt / emotional.toneTransitionTime);
        }

        #endregion

        #region Apply Animation

        protected virtual void SmoothAndApply(float dt)
        {
            float speed = smoothingSpeed * dt;

            currentOffset = Vector3.Lerp(currentOffset, targetOffset, speed);
            currentRotation = Vector3.Lerp(currentRotation, targetRotation, speed);

            visualTarget.localPosition = baseLocalPos + currentOffset;
            visualTarget.localRotation = baseLocalRot * Quaternion.Euler(currentRotation);
            visualTarget.localScale = baseScale * currentScaleMult;
        }

        #endregion
    }

    #region Character-Specific Animators

    /// <summary>
    /// DAZIE Vine - Mentor / Systems Ethicist
    /// Calm, precise, non-patronising. Structure-as-care.
    /// </summary>
    public class DAZIEAnimator : NPCAnimationController
    {
        [Header("═══ DAZIE SPECIFIC ═══")]
        [Tooltip("Precise, measured movements")]
        [Range(0.5f, 1f)] public float precisionMultiplier = 0.8f;

        [Tooltip("Teaching gesture frequency")]
        [Range(0.1f, 0.5f)] public float teachingGestureChance = 0.2f;

        [Range(0f, 10f)] public float contemplativePause = 3f;

        float teachingTimer;
        bool inTeachingGesture;
        float gestureTimer;

        protected override void Start()
        {
            base.Start();
            characterType = NPCCharacterType.DAZIE;

            // DAZIE is more measured
            idle.breatheSpeed = 1.3f;
            idle.shiftSpeed = 0.35f;
            idle.lookSpeed = 0.2f;
            acknowledgment.nodDuration = 0.6f; // Deliberate nod
        }

        protected override void Update()
        {
            base.Update();

            // Teaching gesture system
            teachingTimer += Time.deltaTime;
            if (teachingTimer > contemplativePause && !isAcknowledging)
            {
                if (UnityEngine.Random.value < teachingGestureChance * Time.deltaTime)
                {
                    TriggerTeachingGesture();
                    teachingTimer = 0f;
                }
            }
        }

        void TriggerTeachingGesture()
        {
            TriggerAcknowledge(AcknowledgeType.Gesture, playerTransform);
        }

        protected override void CalculateIdle()
        {
            base.CalculateIdle();

            // Apply precision multiplier - smaller, more controlled movements
            targetOffset *= precisionMultiplier;
            targetRotation *= precisionMultiplier;
        }
    }

    /// <summary>
    /// June Corrow - Sensory Architect / Biodesign Maker
    /// Sparse, warm, incisive. Designed quiet infrastructure.
    /// </summary>
    public class JuneAnimator : NPCAnimationController
    {
        [Header("═══ JUNE SPECIFIC ═══")]
        [Tooltip("Sensitivity to environment")]
        [Range(0f, 0.05f)] public float environmentalResponse = 0.02f;

        [Tooltip("Warmth in gestures")]
        [Range(0.8f, 1.3f)] public float warmthMultiplier = 1.1f;

        [Tooltip("Moments of still observation")]
        [Range(1f, 5f)] public float stillnessDuration = 2f;

        bool inStillMoment;
        float stillnessTimer;

        protected override void Start()
        {
            base.Start();
            characterType = NPCCharacterType.June;

            // June is warm but sparse
            idle.breatheSpeed = 1.4f;
            idle.lookSpeed = 0.25f;
            idle.playerTrackingWeight = 0.4f; // More attentive
            acknowledgment.nodAngle = 12f; // Subtler
        }

        protected override void Update()
        {
            stillnessTimer += Time.deltaTime;

            if (stillnessTimer > stillnessDuration && !inStillMoment)
            {
                if (UnityEngine.Random.value < 0.1f)
                {
                    inStillMoment = true;
                    stillnessTimer = 0f;
                }
            }

            if (inStillMoment)
            {
                if (stillnessTimer > 1.5f)
                {
                    inStillMoment = false;
                    stillnessTimer = 0f;
                }
            }

            base.Update();
        }

        protected override void CalculateIdle()
        {
            if (inStillMoment)
            {
                // Just subtle breathing during still moments
                float phase = animTime * idle.breatheSpeed * 0.5f * Mathf.PI * 2f;
                targetOffset.y = Mathf.Sin(phase) * idle.breatheAmount * 0.3f;
                return;
            }

            base.CalculateIdle();

            // Warmth - slightly larger, gentler movements
            targetOffset *= warmthMultiplier;

            // Environmental sensing - subtle reaction to surroundings
            float envPhase = animTime * 0.3f;
            targetRotation.x += Mathf.Sin(envPhase) * 3f;
        }
    }

    /// <summary>
    /// Winton - Civic OS / System-Ghost
    /// Blunt, ethically focused, occasionally dry. Ethereal presence.
    /// </summary>
    public class WintonAnimator : NPCAnimationController
    {
        [Header("═══ WINTON SPECIFIC ═══")]
        [Tooltip("Ethereal hover effect")]
        [Range(0f, 0.1f)] public float hoverAmount = 0.05f;

        [Tooltip("Digital glitch frequency")]
        [Range(0f, 1f)] public float glitchChance = 0.05f;

        [Tooltip("System processing flicker")]
        [Range(0f, 0.03f)] public float processingFlicker = 0.01f;

        [Tooltip("Transparency pulse")]
        public bool enableTransparencyPulse = true;
        [Range(0.8f, 1f)] public float minAlpha = 0.9f;

        Material cachedMaterial;
        float glitchTimer;
        bool isGlitching;

        protected override void Start()
        {
            base.Start();
            characterType = NPCCharacterType.Winton;

            // Winton is precise, digital
            idle.breatheSpeed = 2f; // Faster, more mechanical
            idle.breatheAmount = 0.015f; // Subtle
            idle.shiftAmount = 0.01f;
            idle.lookSpeed = 0.5f; // Quicker scans

            // Cache material for transparency
            var renderer = visualTarget.GetComponent<Renderer>();
            if (renderer) cachedMaterial = renderer.material;
        }

        protected override void CalculateIdle()
        {
            float intensity = toneMultiplier;

            // Ethereal hover
            float hoverPhase = animTime * 1.2f;
            targetOffset.y = Mathf.Sin(hoverPhase * Mathf.PI * 2f) * hoverAmount;

            // Gentle rotation - scanning
            targetRotation.y = Mathf.Sin(animTime * idle.lookSpeed) * idle.lookAngle * 0.5f * intensity;

            // Processing flicker
            float flicker = Mathf.PerlinNoise(animTime * 10f, 0f) * processingFlicker;
            targetOffset.x += flicker;

            // Occasional glitch
            if (!isGlitching && UnityEngine.Random.value < glitchChance * Time.deltaTime)
            {
                isGlitching = true;
                glitchTimer = 0.1f;
            }

            if (isGlitching)
            {
                // Glitch offset
                targetOffset += new Vector3(
                    UnityEngine.Random.Range(-0.05f, 0.05f),
                    UnityEngine.Random.Range(-0.03f, 0.03f),
                    UnityEngine.Random.Range(-0.05f, 0.05f)
                );

                glitchTimer -= Time.deltaTime;
                if (glitchTimer <= 0f) isGlitching = false;
            }

            // Transparency pulse
            if (enableTransparencyPulse && cachedMaterial)
            {
                float alpha = Mathf.Lerp(minAlpha, 1f, (Mathf.Sin(animTime * 2f) + 1f) * 0.5f);
                Color color = cachedMaterial.color;
                color.a = alpha;
                cachedMaterial.color = color;
            }
        }
    }

    /// <summary>
    /// Society Member - Generic residents
    /// Varied personalities for background population.
    /// </summary>
    public class SocietyMemberAnimator : NPCAnimationController
    {
        [Header("═══ SOCIETY MEMBER ═══")]
        [Tooltip("Personality variance")]
        [Range(0f, 1f)] public float personalityVariance = 0.5f;

        [Tooltip("Enable daily activity simulation")]
        public bool enableDailyActivity = true;

        [Tooltip("Activity types")]
        public ActivityType currentActivity = ActivityType.Idle;

        public enum ActivityType { Idle, Walking, Working, Conversing, Resting }

        float personalityOffset;
        float activityBlend;

        protected override void Start()
        {
            base.Start();
            characterType = NPCCharacterType.SocietyMember;

            // Randomize personality
            personalityOffset = UnityEngine.Random.value * 10f;

            // Apply variance to base settings
            idle.breatheSpeed *= 1f + (UnityEngine.Random.value - 0.5f) * personalityVariance * 0.5f;
            idle.shiftSpeed *= 1f + (UnityEngine.Random.value - 0.5f) * personalityVariance * 0.5f;
            idle.lookAngle *= 1f + (UnityEngine.Random.value - 0.5f) * personalityVariance * 0.3f;
        }

        protected override void CalculateIdle()
        {
            base.CalculateIdle();

            // Activity-based modifiers
            switch (currentActivity)
            {
                case ActivityType.Working:
                    // More focused, less looking around
                    targetRotation.y *= 0.3f;
                    targetOffset.y += Mathf.Sin((animTime + personalityOffset) * 3f) * 0.01f;
                    break;

                case ActivityType.Conversing:
                    // More animated
                    targetRotation *= 1.3f;
                    targetOffset *= 1.2f;
                    break;

                case ActivityType.Resting:
                    // Minimal movement
                    targetOffset *= 0.4f;
                    targetRotation *= 0.3f;
                    break;
            }
        }
    }

    /// <summary>
    /// Companion - Follows player, provides support
    /// </summary>
    public class CompanionAnimator : NPCAnimationController
    {
        [Header("═══ COMPANION ═══")]
        [Tooltip("Distance for alert state")]
        [Range(1f, 5f)] public float alertDistance = 3f;

        [Tooltip("Match player emotional state")]
        public bool mirrorPlayerEmotion = true;

        [Tooltip("Support gesture when player struggles")]
        [Range(0f, 1f)] public float supportGestureChance = 0.3f;

        bool isAlert;
        float distanceToPlayer;

        protected override void Start()
        {
            base.Start();
            characterType = NPCCharacterType.Companion;

            // Companion is attentive
            idle.trackPlayer = true;
            idle.playerTrackingWeight = 0.6f;
            idle.lookAngle = 40f;
        }

        protected override void Update()
        {
            // Track distance to player
            if (playerTransform)
            {
                distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
                isAlert = distanceToPlayer > alertDistance;
            }

            base.Update();
        }

        protected override void CalculateIdle()
        {
            base.CalculateIdle();

            if (isAlert)
            {
                // More watchful movements
                targetRotation.y *= 1.5f;

                // Slight lean toward player
                if (playerTransform)
                {
                    Vector3 toPlayer = (playerTransform.position - transform.position).normalized;
                    Vector3 localDir = transform.InverseTransformDirection(toPlayer);
                    targetRotation.z = -localDir.x * 5f;
                }
            }
        }
    }

    #endregion
}
