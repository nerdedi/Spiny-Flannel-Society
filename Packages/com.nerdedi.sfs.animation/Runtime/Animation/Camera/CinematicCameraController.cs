using UnityEngine;
using System;
using System.Collections.Generic;

namespace SFS.Animation
{
    /// <summary>
    /// Sophisticated camera animation system for Spiny Flannel Society.
    /// Handles story beat transitions, combat verb cinematics, and emotional framing.
    /// Supports the gentle, neuroaffirming tone of the game.
    /// </summary>
    public class CinematicCameraController : MonoBehaviour
    {
        #region Configuration

        [Header("═══════════════════════════════════════")]
        [Header("    CINEMATIC CAMERA CONTROLLER        ")]
        [Header("═══════════════════════════════════════")]

        [Header("Target")]
        public Transform followTarget;
        public Transform lookAtTarget;

        [Header("═══ BASE MOVEMENT ═══")]
        public MovementSettings movement = new MovementSettings();

        [Header("═══ STORY BEAT FRAMING ═══")]
        public StoryFramingSettings storyFraming = new StoryFramingSettings();

        [Header("═══ COMBAT VFX CAMERA ═══")]
        public CombatCameraSettings combatCamera = new CombatCameraSettings();

        [Header("═══ EMOTIONAL TONE ═══")]
        public EmotionalCameraSettings emotionalCamera = new EmotionalCameraSettings();

        [Header("═══ DRIFT EFFECTS ═══")]
        public DriftCameraSettings driftCamera = new DriftCameraSettings();

        [Header("═══ TRANSITIONS ═══")]
        public TransitionSettings transitions = new TransitionSettings();

        #endregion

        #region Settings Classes

        [Serializable]
        public class MovementSettings
        {
            [Header("Follow")]
            public Vector3 followOffset = new Vector3(0f, 3f, -6f);
            [Range(0.5f, 10f)] public float followSpeed = 3f;
            [Range(0.5f, 10f)] public float rotateSpeed = 4f;

            [Header("Smoothing")]
            [Range(0f, 1f)] public float positionSmoothTime = 0.2f;
            [Range(0f, 1f)] public float rotationSmoothTime = 0.15f;

            [Header("Look Ahead")]
            public bool lookAhead = true;
            [Range(0f, 3f)] public float lookAheadDistance = 1.5f;
            [Range(0f, 1f)] public float lookAheadSpeed = 0.3f;
        }

        [Serializable]
        public class StoryFramingSettings
        {
            [Header("Chapter Transitions")]
            [Range(0.5f, 3f)] public float chapterTransitionDuration = 2f;
            public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

            [Header("Arrival Intro (Chapter 1)")]
            public Vector3 arrivalSweepStart = new Vector3(20f, 15f, -20f);
            [Range(1f, 5f)] public float arrivalSweepDuration = 3f;

            [Header("Belonging Ending (Chapter 12)")]
            public Vector3 belongingPullBack = new Vector3(0f, 10f, -15f);
            [Range(2f, 6f)] public float belongingDuration = 4f;

            [Header("Conversation Framing")]
            public Vector3 conversationOffset = new Vector3(2f, 1.5f, -3f);
            [Range(0f, 1f)] public float conversationTransition = 0.5f;
        }

        [Serializable]
        public class CombatCameraSettings
        {
            [Header("Combat Verb Focus")]
            [Range(0.1f, 1f)] public float verbFocusDuration = 0.4f;
            [Range(0.8f, 1f)] public float zoomInAmount = 0.9f;

            [Header("Pulse")]
            [Range(0f, 0.5f)] public float pulseShake = 0.2f;
            [Range(0.1f, 0.5f)] public float pulsePunchIn = 0.3f;

            [Header("Thread Lash")]
            [Range(0f, 0.3f)] public float threadLashTrack = 0.2f;
            public bool threadLashFollowLine = true;

            [Header("Radiant Hold")]
            [Range(0.5f, 2f)] public float holdCircleTime = 1f;
            [Range(1f, 3f)] public float holdPullBack = 1.5f;

            [Header("Edge Claim")]
            [Range(0f, 0.3f)] public float edgeClaimSnap = 0.15f;

            [Header("Re-tune")]
            [Range(0.5f, 2f)] public float reTuneOrbitSpeed = 1f;
            [Range(0f, 30f)] public float reTuneOrbitAngle = 20f;
        }

        [Serializable]
        public class EmotionalCameraSettings
        {
            [Header("Gentle")]
            [Range(0.8f, 1.2f)] public float gentleZoom = 1.05f;
            [Range(0.5f, 1.5f)] public float gentleSpeed = 0.8f;

            [Header("Hopeful")]
            [Range(0.8f, 1.2f)] public float hopefulZoom = 1f;
            public Vector3 hopefulTilt = new Vector3(-5f, 0f, 0f);

            [Header("Melancholic")]
            [Range(0.8f, 1.2f)] public float melancholicZoom = 1.1f;
            [Range(0f, 1f)] public float melancholicWeight = 0.3f;

            [Header("Grounded")]
            [Range(0.8f, 1.2f)] public float groundedZoom = 1f;
            public Vector3 groundedOffset = new Vector3(0f, -0.3f, 0.5f);

            [Header("Tender")]
            [Range(0.8f, 1.2f)] public float tenderZoom = 1.08f;
            [Range(0f, 0.5f)] public float tenderSway = 0.1f;
        }

        [Serializable]
        public class DriftCameraSettings
        {
            [Header("Corruption Effects")]
            [Range(0f, 0.3f)] public float maxShake = 0.15f;
            [Range(0f, 30f)] public float maxTilt = 15f;
            [Range(0f, 0.2f)] public float maxJitter = 0.1f;

            [Header("Rejection Force")]
            [Range(0f, 0.5f)] public float rejectionPush = 0.2f;
        }

        [Serializable]
        public class TransitionSettings
        {
            [Header("Fade")]
            [Range(0.3f, 2f)] public float fadeInDuration = 0.8f;
            [Range(0.3f, 2f)] public float fadeOutDuration = 0.6f;

            [Header("Cut")]
            [Range(0f, 0.1f)] public float cutSnap = 0.05f;

            [Header("Blend")]
            [Range(0.5f, 2f)] public float blendDuration = 1.2f;
            public AnimationCurve blendCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }

        #endregion

        #region Runtime State

        float animTime;

        // Core state
        Vector3 currentPosition;
        Quaternion currentRotation;
        Vector3 targetPosition;
        Quaternion targetRotation;

        // Velocity for smoothing
        Vector3 positionVelocity;
        float rotationVelocity;

        // Look ahead
        Vector3 lookAheadOffset;
        Vector3 lastTargetPosition;

        // Story beat state
        bool inStoryTransition;
        Vector3 storyTransitionStart;
        Vector3 storyTransitionEnd;
        Quaternion storyRotationStart;
        Quaternion storyRotationEnd;
        float storyTransitionTimer;
        float storyTransitionDuration;

        // Combat state
        bool inCombatFocus;
        CombatVerb activeCombatVerb;
        Vector3 combatFocusPoint;
        float combatFocusTimer;

        // Emotional state
        EmotionalTone currentTone = EmotionalTone.Gentle;
        float toneBlendFactor = 1f;

        // Drift state
        float currentDriftLevel;

        // Shake state
        float shakeIntensity;
        float shakeDuration;
        float shakeTimer;

        // Animation output
        Vector3 additionalOffset;
        Vector3 additionalRotation;
        float zoomModifier = 1f;

        #endregion

        #region Lifecycle

        void Start()
        {
            if (!followTarget)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player)
                {
                    followTarget = player.transform;
                    lookAtTarget = player.transform;
                }
            }

            if (followTarget)
            {
                lastTargetPosition = followTarget.position;
            }

            currentPosition = transform.position;
            currentRotation = transform.rotation;

            SubscribeToEvents();
        }

        void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        void SubscribeToEvents()
        {
            AnimationEvents.OnStoryBeatChanged += HandleStoryBeatChange;
            AnimationEvents.OnEmotionalToneChanged += HandleToneChange;
            AnimationEvents.OnCombatVerbUsed += HandleCombatVerb;
            AnimationEvents.OnDriftIntensityChanged += HandleDriftChange;
            AnimationEvents.OnPlayerDamaged += HandlePlayerDamaged;
        }

        void UnsubscribeFromEvents()
        {
            AnimationEvents.OnStoryBeatChanged -= HandleStoryBeatChange;
            AnimationEvents.OnEmotionalToneChanged -= HandleToneChange;
            AnimationEvents.OnCombatVerbUsed -= HandleCombatVerb;
            AnimationEvents.OnDriftIntensityChanged -= HandleDriftChange;
            AnimationEvents.OnPlayerDamaged -= HandlePlayerDamaged;
        }

        void LateUpdate()
        {
            float dt = Time.deltaTime;
            animTime += dt;

            UpdateLookAhead(dt);
            UpdateStoryTransition(dt);
            UpdateCombatFocus(dt);
            UpdateShake(dt);

            CalculateTargetPosition();
            CalculateEmotionalModifiers();
            CalculateDriftEffects();

            ApplyMovement(dt);
        }

        #endregion

        #region Movement Calculation

        void UpdateLookAhead(float dt)
        {
            if (!followTarget || !movement.lookAhead) return;

            Vector3 velocity = (followTarget.position - lastTargetPosition) / dt;
            lastTargetPosition = followTarget.position;

            Vector3 targetLookAhead = velocity.normalized * movement.lookAheadDistance;
            targetLookAhead.y = 0; // Keep horizontal

            lookAheadOffset = Vector3.Lerp(lookAheadOffset, targetLookAhead, movement.lookAheadSpeed);
        }

        void CalculateTargetPosition()
        {
            if (!followTarget) return;

            // Start with base follow position
            Vector3 baseOffset = movement.followOffset * zoomModifier;

            // Apply emotional modifiers
            baseOffset += GetEmotionalOffset();

            // Apply additional offset from effects
            baseOffset += additionalOffset;

            // Convert offset to world space relative to target
            Vector3 rotatedOffset = followTarget.TransformDirection(baseOffset);

            targetPosition = followTarget.position + rotatedOffset + lookAheadOffset;

            // Calculate look target
            Vector3 lookTarget = lookAtTarget ? lookAtTarget.position : followTarget.position;
            lookTarget.y += 1f; // Offset to character center

            Vector3 lookDirection = lookTarget - targetPosition;
            if (lookDirection != Vector3.zero)
            {
                targetRotation = Quaternion.LookRotation(lookDirection);
            }

            // Apply emotional rotation
            targetRotation *= Quaternion.Euler(GetEmotionalTilt());

            // Apply additional rotation
            targetRotation *= Quaternion.Euler(additionalRotation);
        }

        void CalculateEmotionalModifiers()
        {
            zoomModifier = 1f;

            switch (currentTone)
            {
                case EmotionalTone.Gentle:
                    zoomModifier = emotionalCamera.gentleZoom;
                    break;
                case EmotionalTone.Hopeful:
                    zoomModifier = emotionalCamera.hopefulZoom;
                    break;
                case EmotionalTone.Melancholic:
                    zoomModifier = emotionalCamera.melancholicZoom;
                    break;
                case EmotionalTone.Grounded:
                    zoomModifier = emotionalCamera.groundedZoom;
                    break;
                case EmotionalTone.Tender:
                    zoomModifier = emotionalCamera.tenderZoom;
                    break;
            }
        }

        Vector3 GetEmotionalOffset()
        {
            switch (currentTone)
            {
                case EmotionalTone.Grounded:
                    return emotionalCamera.groundedOffset;
                case EmotionalTone.Tender:
                    float sway = Mathf.Sin(animTime) * emotionalCamera.tenderSway;
                    return new Vector3(sway, 0, 0);
                case EmotionalTone.Melancholic:
                    return Vector3.down * emotionalCamera.melancholicWeight;
                default:
                    return Vector3.zero;
            }
        }

        Vector3 GetEmotionalTilt()
        {
            switch (currentTone)
            {
                case EmotionalTone.Hopeful:
                    return emotionalCamera.hopefulTilt;
                default:
                    return Vector3.zero;
            }
        }

        void CalculateDriftEffects()
        {
            if (currentDriftLevel < 0.1f)
            {
                additionalOffset = Vector3.zero;
                additionalRotation = Vector3.zero;
                return;
            }

            float drift = currentDriftLevel;

            // Jitter
            float jitterX = (Mathf.PerlinNoise(animTime * 5f, 0f) - 0.5f) * driftCamera.maxJitter * drift;
            float jitterY = (Mathf.PerlinNoise(0f, animTime * 5f) - 0.5f) * driftCamera.maxJitter * drift;
            additionalOffset = new Vector3(jitterX, jitterY, 0f);

            // Tilt
            float tilt = Mathf.Sin(animTime * 2f) * driftCamera.maxTilt * drift;
            additionalRotation = new Vector3(0f, 0f, tilt);
        }

        void ApplyMovement(float dt)
        {
            if (inStoryTransition)
            {
                ApplyStoryTransition();
            }
            else if (inCombatFocus)
            {
                ApplyCombatFocus(dt);
            }
            else
            {
                ApplyNormalFollow(dt);
            }

            ApplyShake();

            transform.position = currentPosition;
            transform.rotation = currentRotation;
        }

        void ApplyNormalFollow(float dt)
        {
            float speedMod = currentTone == EmotionalTone.Gentle ? emotionalCamera.gentleSpeed : 1f;

            currentPosition = Vector3.SmoothDamp(
                currentPosition,
                targetPosition,
                ref positionVelocity,
                movement.positionSmoothTime / speedMod
            );

            currentRotation = Quaternion.Slerp(
                currentRotation,
                targetRotation,
                movement.rotateSpeed * speedMod * dt
            );
        }

        void ApplyStoryTransition()
        {
            float t = storyTransitionTimer / storyTransitionDuration;
            float curvedT = storyFraming.transitionCurve.Evaluate(t);

            currentPosition = Vector3.Lerp(storyTransitionStart, storyTransitionEnd, curvedT);
            currentRotation = Quaternion.Slerp(storyRotationStart, storyRotationEnd, curvedT);
        }

        void ApplyCombatFocus(float dt)
        {
            float t = combatFocusTimer / combatCamera.verbFocusDuration;

            // Blend between normal follow and combat position
            Vector3 combatTarget = combatFocusPoint + movement.followOffset * combatCamera.zoomInAmount;

            currentPosition = Vector3.Lerp(currentPosition, combatTarget, t * 5f * dt);

            // Look at combat point
            Vector3 lookDir = combatFocusPoint - currentPosition;
            if (lookDir != Vector3.zero)
            {
                Quaternion combatRotation = Quaternion.LookRotation(lookDir);
                currentRotation = Quaternion.Slerp(currentRotation, combatRotation, t * 5f * dt);
            }
        }

        #endregion

        #region Story Transitions

        void UpdateStoryTransition(float dt)
        {
            if (!inStoryTransition) return;

            storyTransitionTimer += dt;

            if (storyTransitionTimer >= storyTransitionDuration)
            {
                inStoryTransition = false;
                currentPosition = storyTransitionEnd;
                currentRotation = storyRotationEnd;
            }
        }

        public void TriggerArrivalSweep()
        {
            inStoryTransition = true;
            storyTransitionStart = storyFraming.arrivalSweepStart;
            storyTransitionEnd = targetPosition;
            storyRotationStart = Quaternion.LookRotation(followTarget.position - storyFraming.arrivalSweepStart);
            storyRotationEnd = targetRotation;
            storyTransitionDuration = storyFraming.arrivalSweepDuration;
            storyTransitionTimer = 0f;

            currentPosition = storyTransitionStart;
        }

        public void TriggerBelongingPullback()
        {
            inStoryTransition = true;
            storyTransitionStart = currentPosition;
            storyTransitionEnd = followTarget.position + storyFraming.belongingPullBack;
            storyRotationStart = currentRotation;
            storyRotationEnd = Quaternion.LookRotation(followTarget.position - storyTransitionEnd);
            storyTransitionDuration = storyFraming.belongingDuration;
            storyTransitionTimer = 0f;
        }

        public void TriggerConversationFrame(Transform conversationPartner)
        {
            if (!conversationPartner || !followTarget) return;

            Vector3 midPoint = (followTarget.position + conversationPartner.position) * 0.5f;
            Vector3 sideOffset = Vector3.Cross(
                (conversationPartner.position - followTarget.position).normalized,
                Vector3.up
            ) * storyFraming.conversationOffset.x;

            Vector3 newTarget = midPoint + sideOffset +
                Vector3.up * storyFraming.conversationOffset.y +
                Vector3.back * storyFraming.conversationOffset.z;

            inStoryTransition = true;
            storyTransitionStart = currentPosition;
            storyTransitionEnd = newTarget;
            storyRotationStart = currentRotation;
            storyRotationEnd = Quaternion.LookRotation(midPoint - newTarget);
            storyTransitionDuration = storyFraming.conversationTransition;
            storyTransitionTimer = 0f;
        }

        #endregion

        #region Combat Focus

        void UpdateCombatFocus(float dt)
        {
            if (!inCombatFocus) return;

            combatFocusTimer += dt;

            // Apply verb-specific camera movement
            ApplyVerbSpecificCamera(dt);

            if (combatFocusTimer >= combatCamera.verbFocusDuration)
            {
                inCombatFocus = false;
            }
        }

        void ApplyVerbSpecificCamera(float dt)
        {
            float t = combatFocusTimer / combatCamera.verbFocusDuration;

            switch (activeCombatVerb)
            {
                case CombatVerb.Pulse:
                    // Punch in and slight shake
                    additionalOffset = Vector3.forward * combatCamera.pulsePunchIn * (1f - t);
                    shakeIntensity = combatCamera.pulseShake * (1f - t);
                    break;

                case CombatVerb.RadiantHold:
                    // Slow orbit
                    float angle = animTime * combatCamera.reTuneOrbitSpeed * 30f;
                    additionalOffset = new Vector3(
                        Mathf.Sin(angle * Mathf.Deg2Rad) * combatCamera.holdPullBack,
                        0f,
                        Mathf.Cos(angle * Mathf.Deg2Rad) * combatCamera.holdPullBack - combatCamera.holdPullBack
                    );
                    break;

                case CombatVerb.ReTune:
                    // Gentle orbit around target
                    float orbitAngle = t * combatCamera.reTuneOrbitAngle;
                    additionalRotation = new Vector3(0f, Mathf.Sin(orbitAngle * Mathf.Deg2Rad) * 10f, 0f);
                    break;
            }
        }

        #endregion

        #region Shake

        void UpdateShake(float dt)
        {
            if (shakeTimer > 0)
            {
                shakeTimer -= dt;
            }
            else
            {
                shakeIntensity = 0f;
            }
        }

        void ApplyShake()
        {
            if (shakeIntensity <= 0) return;

            float shakeX = (Mathf.PerlinNoise(animTime * 25f, 0f) - 0.5f) * 2f * shakeIntensity;
            float shakeY = (Mathf.PerlinNoise(0f, animTime * 25f) - 0.5f) * 2f * shakeIntensity;

            currentPosition += new Vector3(shakeX, shakeY, 0f);
        }

        public void TriggerShake(float intensity, float duration)
        {
            shakeIntensity = Mathf.Max(shakeIntensity, intensity);
            shakeDuration = duration;
            shakeTimer = duration;
        }

        #endregion

        #region Event Handlers

        void HandleStoryBeatChange(object previousBeat, object currentBeat)
        {
            // Check for specific story beats
            string beatName = currentBeat?.ToString() ?? "";

            if (beatName.Contains("Arrival") || beatName.Contains("Chapter1"))
            {
                TriggerArrivalSweep();
            }
            else if (beatName.Contains("Belonging") || beatName.Contains("Chapter12"))
            {
                TriggerBelongingPullback();
            }
        }

        void HandleToneChange(EmotionalTone from, EmotionalTone to)
        {
            currentTone = to;
        }

        void HandleCombatVerb(CombatVerb verb, Vector3 position, Vector3 direction, Transform target)
        {
            inCombatFocus = true;
            activeCombatVerb = verb;
            combatFocusPoint = position;
            combatFocusTimer = 0f;
        }

        void HandleDriftChange(float previous, float current)
        {
            currentDriftLevel = current;
        }

        void HandlePlayerDamaged(DamageType type, float amount, Vector3 hitDirection)
        {
            TriggerShake(0.1f + amount * 0.2f, 0.3f);
        }

        #endregion

        #region Public API

        /// <summary>Set the camera follow target</summary>
        public void SetFollowTarget(Transform target)
        {
            followTarget = target;
            lookAtTarget = target;
        }

        /// <summary>Instantly snap camera to target position</summary>
        public void SnapToTarget()
        {
            if (!followTarget) return;

            CalculateTargetPosition();
            currentPosition = targetPosition;
            currentRotation = targetRotation;
            positionVelocity = Vector3.zero;
        }

        /// <summary>Override emotional tone for cinematic purposes</summary>
        public void SetEmotionalTone(EmotionalTone tone)
        {
            currentTone = tone;
        }

        #endregion
    }
}
