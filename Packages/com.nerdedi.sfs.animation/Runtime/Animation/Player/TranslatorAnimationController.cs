using UnityEngine;
using System;

namespace SFS.Animation
{
    /// <summary>
    /// Complete Translator (Player) animation controller with full 3D procedural animation.
    /// Handles all movement, combat verbs, Windprint Rig effects, and story beat response.
    /// Works without animation clips - pure code-driven animation for market-ready quality.
    /// </summary>
    [DisallowMultipleComponent]
    public class TranslatorAnimationController : MonoBehaviour
    {
        #region Inspector Configuration

        [Header("═══════════════════════════════════════")]
        [Header("    TRANSLATOR ANIMATION CONTROLLER    ")]
        [Header("═══════════════════════════════════════")]

        [Header("Target Setup")]
        [Tooltip("The visual model/mesh to animate. Auto-finds if empty.")]
        public Transform visualTarget;

        [Tooltip("Optional bone transforms for advanced animation")]
        public AdvancedBoneRig boneRig;

        [Header("═══ IDLE / BREATHING ═══")]
        public IdleAnimationSettings idle = new IdleAnimationSettings();

        [Header("═══ LOCOMOTION ═══")]
        public LocomotionAnimationSettings locomotion = new LocomotionAnimationSettings();

        [Header("═══ AERIAL MOVEMENT ═══")]
        public AerialAnimationSettings aerial = new AerialAnimationSettings();

        [Header("═══ WALL INTERACTION ═══")]
        public WallAnimationSettings wall = new WallAnimationSettings();

        [Header("═══ COMBAT VERBS ═══")]
        public CombatVerbAnimationSettings combat = new CombatVerbAnimationSettings();

        [Header("═══ WINDPRINT RIG ═══")]
        public WindprintAnimationSettings windprint = new WindprintAnimationSettings();

        [Header("═══ REACTIONS ═══")]
        public ReactionAnimationSettings reactions = new ReactionAnimationSettings();

        [Header("═══ EMOTIONAL RESPONSE ═══")]
        public EmotionalAnimationSettings emotional = new EmotionalAnimationSettings();

        [Header("═══ SQUASH & STRETCH ═══")]
        public SquashStretchSettings squashStretch = new SquashStretchSettings();

        [Header("═══ SMOOTHING ═══")]
        public SmoothingSettings smoothing = new SmoothingSettings();

        [Header("═══ DEBUG ═══")]
        public bool showDebugInfo = false;

        #endregion

        #region Settings Classes

        [Serializable]
        public class IdleAnimationSettings
        {
            [Header("Breathing")]
            [Range(0.5f, 4f)] public float breatheSpeed = 1.6f;
            [Range(0f, 0.08f)] public float breatheAmount = 0.025f;
            [Range(0f, 0.03f)] public float breatheScaleAmount = 0.015f;

            [Header("Subtle Sway")]
            [Range(0f, 0.1f)] public float swayAmount = 0.015f;
            [Range(0.2f, 1.5f)] public float swaySpeed = 0.5f;
            [Range(0f, 8f)] public float swayRotation = 2f;

            [Header("Micro-Movements")]
            public bool enableMicroMovements = true;
            [Range(0f, 0.02f)] public float microIntensity = 0.008f;
            [Range(0.5f, 3f)] public float microSpeed = 1.2f;

            [Header("Head Bobbing")]
            [Range(0f, 10f)] public float headBobAngle = 3f;
            [Range(0.3f, 1.5f)] public float headBobSpeed = 0.6f;
        }

        [Serializable]
        public class LocomotionAnimationSettings
        {
            [Header("Walk Cycle")]
            [Range(4f, 14f)] public float walkBobSpeed = 8f;
            [Range(0f, 0.15f)] public float walkBobHeight = 0.045f;
            [Range(0f, 0.08f)] public float walkBobSide = 0.02f;
            [Range(0f, 15f)] public float walkTilt = 4f;
            [Range(0f, 10f)] public float walkLean = 3f;

            [Header("Run Cycle")]
            [Range(8f, 22f)] public float runBobSpeed = 15f;
            [Range(0f, 0.2f)] public float runBobHeight = 0.08f;
            [Range(0f, 0.12f)] public float runBobSide = 0.04f;
            [Range(0f, 20f)] public float runTilt = 7f;
            [Range(0f, 18f)] public float runLean = 12f;

            [Header("Speed Thresholds")]
            [Range(0f, 0.15f)] public float idleThreshold = 0.03f;
            [Range(0.4f, 0.85f)] public float runThreshold = 0.6f;

            [Header("Direction Response")]
            [Range(0f, 25f)] public float turnAnticipation = 10f;
            [Range(0f, 0.08f)] public float accelerationLean = 0.03f;
        }

        [Serializable]
        public class AerialAnimationSettings
        {
            [Header("Jump")]
            [Range(0.6f, 0.95f)] public float jumpSquash = 0.8f;
            [Range(1.05f, 1.4f)] public float jumpStretch = 1.2f;
            [Range(0f, 25f)] public float jumpTuckAngle = 12f;
            [Range(0.1f, 0.4f)] public float jumpAnticipation = 0.15f;

            [Header("Double Jump")]
            [Range(0f, 720f)] public float doubleJumpSpin = 360f;
            [Range(0.5f, 1f)] public float doubleJumpTuck = 0.85f;

            [Header("Air Dash")]
            [Range(1.1f, 1.5f)] public float dashStretch = 1.3f;
            [Range(0f, 0.2f)] public float dashSquashWidth = 0.1f;
            [Range(0f, 45f)] public float dashAngle = 20f;

            [Header("Fall")]
            [Range(0f, 0.08f)] public float fallAnticipation = 0.04f;
            [Range(0f, 35f)] public float fallSpreadAngle = 18f;
            [Range(0f, 0.06f)] public float fallWindEffect = 0.025f;

            [Header("Glide")]
            [Range(0f, 30f)] public float glideSpreadAngle = 22f;
            [Range(0f, 15f)] public float glideBankAngle = 10f;
            [Range(0f, 0.04f)] public float glideOscillation = 0.02f;

            [Header("Land")]
            [Range(0.4f, 0.85f)] public float landSquash = 0.65f;
            [Range(1.1f, 1.5f)] public float landSpread = 1.25f;
            [Range(0.1f, 0.6f)] public float landRecoveryTime = 0.25f;
            [Range(0.3f, 0.8f)] public float hardLandSquash = 0.5f;
        }

        [Serializable]
        public class WallAnimationSettings
        {
            [Header("Wall Run")]
            [Range(0f, 45f)] public float wallRunLean = 25f;
            [Range(0f, 20f)] public float wallRunTilt = 12f;
            [Range(4f, 16f)] public float wallRunBobSpeed = 10f;
            [Range(0f, 0.1f)] public float wallRunBobAmount = 0.05f;

            [Header("Wall Kick")]
            [Range(0f, 540f)] public float wallKickSpin = 180f;
            [Range(0.6f, 0.9f)] public float wallKickTuck = 0.8f;
            [Range(0.1f, 0.4f)] public float wallKickDuration = 0.25f;
        }

        [Serializable]
        public class CombatVerbAnimationSettings
        {
            [Header("Pulse")]
            [Range(0.8f, 1f)] public float pulseChargeSquash = 0.9f;
            [Range(1.1f, 1.4f)] public float pulseReleaseStretch = 1.25f;
            [Range(0.1f, 0.5f)] public float pulseDuration = 0.35f;
            [Range(0f, 360f)] public float pulseArmSpread = 120f;

            [Header("Thread Lash")]
            [Range(0f, 45f)] public float threadLashWindup = 25f;
            [Range(0f, 180f)] public float threadLashArc = 90f;
            [Range(0.15f, 0.5f)] public float threadLashDuration = 0.3f;

            [Header("Radiant Hold")]
            [Range(0.85f, 1f)] public float radiantHoldSquash = 0.95f;
            [Range(0f, 15f)] public float radiantHoldSway = 5f;
            [Range(0f, 0.05f)] public float radiantHoldPulse = 0.02f;

            [Header("Edge Claim")]
            [Range(0.6f, 0.85f)] public float edgeClaimSquash = 0.75f;
            [Range(0f, 30f)] public float edgeClaimStance = 20f;
            [Range(0.2f, 0.6f)] public float edgeClaimDuration = 0.4f;

            [Header("Re-tune")]
            [Range(0f, 720f)] public float retuneSpinSpeed = 180f;
            [Range(0.9f, 1.1f)] public float retunePulseScale = 1.05f;
            [Range(0.3f, 1f)] public float retuneDuration = 0.6f;
        }

        [Serializable]
        public class WindprintAnimationSettings
        {
            [Header("Cushion Mode")]
            [Range(0.95f, 1.05f)] public float cushionScaleBase = 1.02f;
            [Range(0f, 0.03f)] public float cushionPulseAmount = 0.015f;
            [Range(0.5f, 2f)] public float cushionPulseSpeed = 1f;
            [Range(0.5f, 1f)] public float cushionTimingMultiplier = 0.7f;

            [Header("Guard Mode")]
            [Range(0.95f, 1.05f)] public float guardScaleBase = 0.98f;
            [Range(0f, 10f)] public float guardStanceAngle = 5f;
            [Range(0f, 0.02f)] public float guardShimmer = 0.01f;
            [Range(0.3f, 1f)] public float guardRhythmLock = 0.5f;

            [Header("Mode Transition")]
            [Range(0.1f, 1f)] public float modeTransitionTime = 0.4f;
            [Range(0f, 1.2f)] public float transitionFlash = 0.8f;
        }

        [Serializable]
        public class ReactionAnimationSettings
        {
            [Header("Damage")]
            [Range(0.3f, 0.7f)] public float damageSquash = 0.5f;
            [Range(0f, 40f)] public float damageRecoilAngle = 20f;
            [Range(0.1f, 0.5f)] public float damageRecoveryTime = 0.3f;
            [Range(0f, 0.1f)] public float damageShakeAmount = 0.05f;

            [Header("Collect")]
            [Range(1f, 1.25f)] public float collectPop = 1.12f;
            [Range(0f, 360f)] public float collectSpinSpeed = 180f;
            [Range(0.1f, 0.4f)] public float collectDuration = 0.25f;

            [Header("Principle Module Collect")]
            [Range(1.1f, 1.4f)] public float principleGlow = 1.25f;
            [Range(0.3f, 1f)] public float principleDuration = 0.6f;

            [Header("Death")]
            [Range(0.8f, 2f)] public float deathDuration = 1.2f;
            public AnimationCurve deathFadeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
            [Range(0f, 180f)] public float deathFallAngle = 90f;
        }

        [Serializable]
        public class EmotionalAnimationSettings
        {
            [Header("Intensity Multipliers")]
            [Range(0.3f, 1f)] public float gentleMultiplier = 0.6f;
            [Range(0.9f, 1.5f)] public float hopefulMultiplier = 1.15f;
            [Range(0.2f, 0.8f)] public float melancholicMultiplier = 0.45f;
            [Range(0.6f, 1.1f)] public float groundedMultiplier = 0.85f;
            [Range(0.4f, 0.9f)] public float tenderMultiplier = 0.55f;

            [Header("Speed Multipliers")]
            [Range(0.5f, 1f)] public float gentleSpeedMult = 0.8f;
            [Range(0.9f, 1.3f)] public float hopefulSpeedMult = 1.1f;
            [Range(0.4f, 0.9f)] public float melancholicSpeedMult = 0.65f;
            [Range(0.7f, 1.1f)] public float groundedSpeedMult = 0.9f;

            [Header("Response")]
            public bool respondToStoryBeats = true;
            [Range(0.2f, 2f)] public float toneTransitionTime = 0.8f;
        }

        [Serializable]
        public class SquashStretchSettings
        {
            [Range(0.3f, 0.8f)] public float minSquash = 0.5f;
            [Range(1.2f, 2f)] public float maxStretch = 1.5f;
            [Range(5f, 25f)] public float recoverySpeed = 12f;
            public bool preserveVolume = true;
        }

        [Serializable]
        public class SmoothingSettings
        {
            [Range(2f, 25f)] public float positionSmoothing = 14f;
            [Range(2f, 25f)] public float rotationSmoothing = 10f;
            [Range(2f, 25f)] public float scaleSmoothing = 16f;
            [Range(0.5f, 5f)] public float reactionSmoothing = 2f;
        }

        [Serializable]
        public class AdvancedBoneRig
        {
            public Transform root;
            public Transform spine;
            public Transform chest;
            public Transform head;
            public Transform leftArm;
            public Transform rightArm;
            public Transform leftLeg;
            public Transform rightLeg;
            public bool isAssigned => root != null;
        }

        #endregion

        #region Runtime State

        // Input state (set by controller)
        float currentSpeed;
        float normalizedSpeed;
        bool isGrounded = true;
        bool wasGrounded = true;
        bool isWallRunning;
        bool isGliding;
        float yVelocity;
        Vector3 moveDirection;
        Vector3 facingDirection;
        WindprintMode currentWindprintMode = WindprintMode.None;

        // Animation timing
        float animTime;
        float locomotionPhase;

        // Squash/stretch
        float currentSquashY = 1f;
        float currentStretchXZ = 1f;
        float targetSquashY = 1f;
        float targetStretchXZ = 1f;

        // Position/rotation
        Vector3 currentOffset;
        Vector3 targetOffset;
        Vector3 currentRotationEuler;
        Vector3 targetRotationEuler;

        // Reactions
        bool isReacting;
        float reactionTimer;
        ReactionType currentReaction;
        Vector3 reactionDirection;

        // Combat verbs
        bool isPerformingVerb;
        float verbTimer;
        CombatVerb currentVerb;
        Vector3 verbTargetPos;

        // Emotional state
        EmotionalTone currentTone = EmotionalTone.Gentle;
        float toneIntensityMultiplier = 1f;
        float toneSpeedMultiplier = 1f;
        float targetToneIntensity = 1f;
        float targetToneSpeed = 1f;

        // Base transforms
        Vector3 baseScale;
        Vector3 baseLocalPos;
        Quaternion baseLocalRot;

        enum ReactionType { None, Jump, DoubleJump, Land, HardLand, Damage, Collect, PrincipleCollect, Death, AirDash, WallKick }

        #endregion

        #region Lifecycle

        void Awake()
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

        void Start()
        {
            CacheBaseTransforms();
            SubscribeToEvents();
        }

        void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        void CacheBaseTransforms()
        {
            baseScale = visualTarget.localScale;
            baseLocalPos = visualTarget.localPosition;
            baseLocalRot = visualTarget.localRotation;
        }

        void SubscribeToEvents()
        {
            AnimationEvents.OnPlayerAction += HandlePlayerAction;
            AnimationEvents.OnCombatVerbUsed += HandleCombatVerb;
            AnimationEvents.OnWindprintModeChanged += HandleWindprintChange;
            AnimationEvents.OnPlayerDamaged += HandleDamage;
            AnimationEvents.OnPlayerCollected += HandleCollect;
            AnimationEvents.OnPlayerDeath += HandleDeath;
            AnimationEvents.OnPlayerRespawn += HandleRespawn;
            AnimationEvents.OnStoryBeatAnimation += HandleStoryBeat;
        }

        void UnsubscribeFromEvents()
        {
            AnimationEvents.OnPlayerAction -= HandlePlayerAction;
            AnimationEvents.OnCombatVerbUsed -= HandleCombatVerb;
            AnimationEvents.OnWindprintModeChanged -= HandleWindprintChange;
            AnimationEvents.OnPlayerDamaged -= HandleDamage;
            AnimationEvents.OnPlayerCollected -= HandleCollect;
            AnimationEvents.OnPlayerDeath -= HandleDeath;
            AnimationEvents.OnPlayerRespawn -= HandleRespawn;
            AnimationEvents.OnStoryBeatAnimation -= HandleStoryBeat;
        }

        void Update()
        {
            float dt = Time.deltaTime;

            UpdateEmotionalState(dt);

            if (isReacting)
                UpdateReaction(dt);
            else if (isPerformingVerb)
                UpdateCombatVerb(dt);
            else
                UpdateLocomotion(dt);

            UpdateSquashStretch(dt);
            SmoothAndApply(dt);
        }

        #endregion

        #region Public Input Methods

        /// <summary>Set movement state from player controller</summary>
        public void SetMovementState(MovementState state)
        {
            wasGrounded = isGrounded;

            currentSpeed = state.Speed;
            normalizedSpeed = state.NormalizedSpeed;
            isGrounded = state.IsGrounded;
            isWallRunning = state.IsWallRunning;
            isGliding = state.IsGliding;
            yVelocity = state.YVelocity;
            moveDirection = state.MoveDirection;
            facingDirection = state.FacingDirection;

            // Auto-detect landing
            if (isGrounded && !wasGrounded)
            {
                float fallSpeed = Mathf.Abs(yVelocity);
                if (fallSpeed > 15f)
                    TriggerReaction(ReactionType.HardLand);
                else if (fallSpeed > 3f)
                    TriggerReaction(ReactionType.Land);
            }
        }

        /// <summary>Simplified movement input</summary>
        public void SetMove(float speed, bool grounded, float yVel)
        {
            wasGrounded = isGrounded;
            currentSpeed = speed;
            normalizedSpeed = Mathf.Clamp01(speed / 8f);
            isGrounded = grounded;
            yVelocity = yVel;

            if (isGrounded && !wasGrounded && Mathf.Abs(yVel) > 3f)
            {
                TriggerReaction(Mathf.Abs(yVel) > 15f ? ReactionType.HardLand : ReactionType.Land);
            }
        }

        /// <summary>Trigger a jump animation</summary>
        public void TriggerJump()
        {
            TriggerReaction(ReactionType.Jump);
        }

        /// <summary>Trigger a double jump animation</summary>
        public void TriggerDoubleJump()
        {
            TriggerReaction(ReactionType.DoubleJump);
        }

        /// <summary>Trigger an air dash animation</summary>
        public void TriggerAirDash()
        {
            TriggerReaction(ReactionType.AirDash);
        }

        /// <summary>Trigger a wall kick animation</summary>
        public void TriggerWallKick()
        {
            TriggerReaction(ReactionType.WallKick);
        }

        #endregion

        #region Locomotion Animation

        void UpdateLocomotion(float dt)
        {
            animTime += dt * toneSpeedMultiplier;

            targetOffset = Vector3.zero;
            targetRotationEuler = Vector3.zero;

            if (isWallRunning)
            {
                CalculateWallRun();
            }
            else if (isGliding)
            {
                CalculateGlide();
            }
            else if (!isGrounded)
            {
                CalculateAirborne();
            }
            else if (normalizedSpeed < locomotion.idleThreshold)
            {
                CalculateIdle();
            }
            else
            {
                CalculateWalkRun();
            }

            ApplyWindprintEffect();
        }

        void CalculateIdle()
        {
            float intensity = toneIntensityMultiplier;
            float speed = toneSpeedMultiplier;

            // Breathing
            float breathePhase = animTime * idle.breatheSpeed * Mathf.PI * 2f * speed;
            targetOffset.y = Mathf.Sin(breathePhase) * idle.breatheAmount * intensity;

            // Scale breathing
            float breatheScale = 1f + Mathf.Sin(breathePhase) * idle.breatheScaleAmount * intensity;
            targetSquashY = breatheScale;

            // Subtle sway
            float swayPhase = animTime * idle.swaySpeed * speed;
            targetOffset.x = Mathf.Sin(swayPhase) * idle.swayAmount * intensity;
            targetRotationEuler.z = Mathf.Sin(swayPhase) * idle.swayRotation * intensity;

            // Head bob
            float headBobPhase = animTime * idle.headBobSpeed * Mathf.PI * 2f * speed;
            targetRotationEuler.x = Mathf.Sin(headBobPhase) * idle.headBobAngle * intensity;

            // Micro-movements
            if (idle.enableMicroMovements)
            {
                float microPhase = animTime * idle.microSpeed * 7.3f;
                targetOffset.x += Mathf.PerlinNoise(microPhase, 0f) * idle.microIntensity * intensity * 2f - idle.microIntensity * intensity;
                targetOffset.z += Mathf.PerlinNoise(0f, microPhase) * idle.microIntensity * intensity * 2f - idle.microIntensity * intensity;
            }
        }

        void CalculateWalkRun()
        {
            float intensity = toneIntensityMultiplier;
            float speed = toneSpeedMultiplier;

            // Blend between walk and run
            float runBlend = Mathf.InverseLerp(locomotion.idleThreshold, locomotion.runThreshold, normalizedSpeed);
            runBlend = Mathf.SmoothStep(0f, 1f, runBlend);

            // Calculate bob parameters
            float bobSpeed = Mathf.Lerp(locomotion.walkBobSpeed, locomotion.runBobSpeed, runBlend) * speed;
            float bobHeight = Mathf.Lerp(locomotion.walkBobHeight, locomotion.runBobHeight, runBlend) * intensity;
            float bobSide = Mathf.Lerp(locomotion.walkBobSide, locomotion.runBobSide, runBlend) * intensity;
            float tilt = Mathf.Lerp(locomotion.walkTilt, locomotion.runTilt, runBlend) * intensity;
            float lean = Mathf.Lerp(locomotion.walkLean, locomotion.runLean, runBlend) * intensity;

            locomotionPhase += normalizedSpeed * bobSpeed * Time.deltaTime;

            // Vertical bob (two cycles per stride)
            float vertPhase = locomotionPhase * Mathf.PI * 2f;
            targetOffset.y = Mathf.Abs(Mathf.Sin(vertPhase)) * bobHeight;

            // Side-to-side bob
            targetOffset.x = Mathf.Sin(vertPhase * 0.5f) * bobSide;

            // Hip tilt
            targetRotationEuler.z = Mathf.Sin(vertPhase * 0.5f) * tilt;

            // Forward lean when running
            targetRotationEuler.x = lean * runBlend;

            // Direction-based lean
            if (moveDirection.sqrMagnitude > 0.01f)
            {
                Vector3 localDir = transform.InverseTransformDirection(moveDirection);
                targetRotationEuler.z += -localDir.x * locomotion.turnAnticipation * normalizedSpeed;
            }
        }

        void CalculateAirborne()
        {
            float intensity = toneIntensityMultiplier;

            if (yVelocity > 0.5f)
            {
                // Rising - tuck
                float tuckAmount = Mathf.Clamp01(yVelocity / 10f);
                targetRotationEuler.x = -aerial.jumpTuckAngle * tuckAmount * intensity;
                targetSquashY = Mathf.Lerp(1f, aerial.jumpStretch, tuckAmount);
                targetStretchXZ = Mathf.Lerp(1f, aerial.jumpSquash, tuckAmount);
            }
            else
            {
                // Falling - spread
                float fallAmount = Mathf.Clamp01(-yVelocity / 15f);
                targetRotationEuler.x = aerial.fallSpreadAngle * fallAmount * intensity * 0.5f;
                targetOffset.y = -aerial.fallAnticipation * fallAmount;

                // Wind effect
                float windPhase = animTime * 12f;
                targetOffset.x += Mathf.Sin(windPhase) * aerial.fallWindEffect * fallAmount * intensity;
                targetOffset.z += Mathf.Cos(windPhase * 0.7f) * aerial.fallWindEffect * 0.5f * fallAmount * intensity;
            }
        }

        void CalculateWallRun()
        {
            float intensity = toneIntensityMultiplier;
            float speed = toneSpeedMultiplier;

            // Lean into wall
            targetRotationEuler.z = wall.wallRunLean * intensity;
            targetRotationEuler.x = wall.wallRunTilt * intensity;

            // Bob along wall
            float wallPhase = animTime * wall.wallRunBobSpeed * speed;
            targetOffset.y = Mathf.Sin(wallPhase * Mathf.PI * 2f) * wall.wallRunBobAmount * intensity;
        }

        void CalculateGlide()
        {
            float intensity = toneIntensityMultiplier;
            float speed = toneSpeedMultiplier;

            // Spread arms/body
            targetRotationEuler.x = -aerial.glideSpreadAngle * intensity;

            // Bank on turns
            if (moveDirection.sqrMagnitude > 0.01f)
            {
                Vector3 localDir = transform.InverseTransformDirection(moveDirection);
                targetRotationEuler.z = localDir.x * aerial.glideBankAngle * intensity;
            }

            // Gentle oscillation
            float glidePhase = animTime * speed * 2f;
            targetOffset.y = Mathf.Sin(glidePhase) * aerial.glideOscillation * intensity;
        }

        #endregion

        #region Combat Verb Animation

        void HandleCombatVerb(CombatVerb verb, Vector3 targetPos)
        {
            isPerformingVerb = true;
            verbTimer = 0f;
            currentVerb = verb;
            verbTargetPos = targetPos;
        }

        void UpdateCombatVerb(float dt)
        {
            verbTimer += dt;
            float duration = GetVerbDuration(currentVerb);
            float t = Mathf.Clamp01(verbTimer / duration);

            switch (currentVerb)
            {
                case CombatVerb.Pulse:
                    AnimatePulse(t);
                    break;
                case CombatVerb.ThreadLash:
                    AnimateThreadLash(t);
                    break;
                case CombatVerb.RadiantHold:
                    AnimateRadiantHold(t);
                    break;
                case CombatVerb.EdgeClaim:
                    AnimateEdgeClaim(t);
                    break;
                case CombatVerb.ReTune:
                    AnimateReTune(t);
                    break;
            }

            if (t >= 1f)
            {
                isPerformingVerb = false;
            }
        }

        float GetVerbDuration(CombatVerb verb)
        {
            return verb switch
            {
                CombatVerb.Pulse => combat.pulseDuration,
                CombatVerb.ThreadLash => combat.threadLashDuration,
                CombatVerb.RadiantHold => 0.5f,
                CombatVerb.EdgeClaim => combat.edgeClaimDuration,
                CombatVerb.ReTune => combat.retuneDuration,
                _ => 0.3f
            };
        }

        void AnimatePulse(float t)
        {
            float intensity = toneIntensityMultiplier;

            if (t < 0.3f)
            {
                // Charge - squash down
                float chargeT = t / 0.3f;
                targetSquashY = Mathf.Lerp(1f, combat.pulseChargeSquash, chargeT);
                targetStretchXZ = Mathf.Lerp(1f, 1f / combat.pulseChargeSquash, chargeT);
            }
            else if (t < 0.5f)
            {
                // Release - stretch out
                float releaseT = (t - 0.3f) / 0.2f;
                targetSquashY = Mathf.Lerp(combat.pulseChargeSquash, combat.pulseReleaseStretch, releaseT);
                targetStretchXZ = Mathf.Lerp(1f / combat.pulseChargeSquash, 1f / combat.pulseReleaseStretch, releaseT);
                targetRotationEuler.x = combat.pulseArmSpread * 0.1f * releaseT * intensity;
            }
            else
            {
                // Recover
                float recoverT = (t - 0.5f) / 0.5f;
                targetSquashY = Mathf.Lerp(combat.pulseReleaseStretch, 1f, recoverT);
                targetStretchXZ = Mathf.Lerp(1f / combat.pulseReleaseStretch, 1f, recoverT);
            }
        }

        void AnimateThreadLash(float t)
        {
            float intensity = toneIntensityMultiplier;

            if (t < 0.25f)
            {
                // Wind up
                float windupT = t / 0.25f;
                targetRotationEuler.y = -combat.threadLashWindup * windupT * intensity;
                targetRotationEuler.z = -5f * windupT * intensity;
            }
            else if (t < 0.5f)
            {
                // Lash
                float lashT = (t - 0.25f) / 0.25f;
                targetRotationEuler.y = Mathf.Lerp(-combat.threadLashWindup, combat.threadLashArc, lashT) * intensity;
                targetRotationEuler.z = 5f * lashT * intensity;
                targetStretchXZ = 1f + 0.1f * Mathf.Sin(lashT * Mathf.PI);
            }
            else
            {
                // Recover
                float recoverT = (t - 0.5f) / 0.5f;
                targetRotationEuler.y = Mathf.Lerp(combat.threadLashArc, 0f, recoverT) * intensity;
                targetRotationEuler.z = Mathf.Lerp(5f, 0f, recoverT) * intensity;
            }
        }

        void AnimateRadiantHold(float t)
        {
            float intensity = toneIntensityMultiplier;

            // Stable, protective stance with subtle pulse
            targetSquashY = combat.radiantHoldSquash;
            float pulsePhase = t * Mathf.PI * 4f;
            targetOffset.y = Mathf.Sin(pulsePhase) * combat.radiantHoldPulse * intensity;
            targetRotationEuler.z = Mathf.Sin(pulsePhase * 0.5f) * combat.radiantHoldSway * intensity;
        }

        void AnimateEdgeClaim(float t)
        {
            float intensity = toneIntensityMultiplier;

            if (t < 0.4f)
            {
                // Crouch and claim
                float claimT = t / 0.4f;
                targetSquashY = Mathf.Lerp(1f, combat.edgeClaimSquash, claimT);
                targetStretchXZ = Mathf.Lerp(1f, 1.15f, claimT);
                targetRotationEuler.x = combat.edgeClaimStance * claimT * intensity;
            }
            else
            {
                // Hold and stabilize
                float holdT = (t - 0.4f) / 0.6f;
                targetSquashY = combat.edgeClaimSquash;
                targetStretchXZ = 1.15f;
                float shimmer = Mathf.Sin(holdT * Mathf.PI * 8f) * 0.02f;
                targetOffset.y = shimmer * intensity;
            }
        }

        void AnimateReTune(float t)
        {
            float intensity = toneIntensityMultiplier;

            // Circular motion with scale pulse
            float spinAngle = t * combat.retuneSpinSpeed * intensity;
            targetRotationEuler.y = spinAngle;

            // Pulsing scale
            float pulsePhase = t * Mathf.PI * 4f;
            float pulseAmount = (combat.retunePulseScale - 1f) * Mathf.Abs(Mathf.Sin(pulsePhase));
            targetSquashY = 1f + pulseAmount;
            targetStretchXZ = 1f + pulseAmount * 0.5f;

            // Rising motion
            targetOffset.y = Mathf.Sin(t * Mathf.PI) * 0.1f * intensity;
        }

        #endregion

        #region Windprint Effects

        void HandleWindprintChange(WindprintMode from, WindprintMode to)
        {
            currentWindprintMode = to;

            // Flash on mode change
            if (from != to)
            {
                AnimationEvents.ScreenEffect(ScreenEffectType.Flash, windprint.transitionFlash, 0.15f);
            }
        }

        void ApplyWindprintEffect()
        {
            switch (currentWindprintMode)
            {
                case WindprintMode.Cushion:
                    ApplyCushionEffect();
                    break;
                case WindprintMode.Guard:
                    ApplyGuardEffect();
                    break;
            }
        }

        void ApplyCushionEffect()
        {
            // Softer, larger, more relaxed
            float cushionScale = windprint.cushionScaleBase;
            float pulsePhase = animTime * windprint.cushionPulseSpeed * Mathf.PI * 2f;
            cushionScale += Mathf.Sin(pulsePhase) * windprint.cushionPulseAmount;

            targetSquashY *= cushionScale;
            targetStretchXZ *= cushionScale;
        }

        void ApplyGuardEffect()
        {
            // Tighter, more grounded
            float guardScale = windprint.guardScaleBase;
            targetSquashY *= guardScale;

            // Stance angle
            targetRotationEuler.x += windprint.guardStanceAngle;

            // Subtle shimmer
            float shimmerPhase = animTime * 15f;
            targetOffset.x += Mathf.Sin(shimmerPhase) * windprint.guardShimmer;
        }

        #endregion

        #region Reactions

        void HandlePlayerAction(PlayerAction action)
        {
            switch (action)
            {
                case PlayerAction.Jump:
                    TriggerReaction(ReactionType.Jump);
                    break;
                case PlayerAction.DoubleJump:
                    TriggerReaction(ReactionType.DoubleJump);
                    break;
                case PlayerAction.AirDash:
                case PlayerAction.DoubleAirDash:
                    TriggerReaction(ReactionType.AirDash);
                    break;
                case PlayerAction.WallKick:
                    TriggerReaction(ReactionType.WallKick);
                    break;
                case PlayerAction.Land:
                    TriggerReaction(ReactionType.Land);
                    break;
                case PlayerAction.LandHard:
                    TriggerReaction(ReactionType.HardLand);
                    break;
            }
        }

        void HandleDamage(DamageType type, float amount)
        {
            TriggerReaction(ReactionType.Damage);
            AnimationEvents.ScreenEffect(ScreenEffectType.Shake, amount, reactions.damageRecoveryTime);
        }

        void HandleCollect(CollectibleType type)
        {
            if (type == CollectibleType.PrincipleModule)
                TriggerReaction(ReactionType.PrincipleCollect);
            else
                TriggerReaction(ReactionType.Collect);
        }

        void HandleDeath(DeathType type)
        {
            TriggerReaction(ReactionType.Death);
        }

        void HandleRespawn()
        {
            isReacting = false;
            currentSquashY = 1f;
            currentStretchXZ = 1f;
            currentOffset = Vector3.zero;
            currentRotationEuler = Vector3.zero;
        }

        void TriggerReaction(ReactionType type)
        {
            isReacting = true;
            reactionTimer = 0f;
            currentReaction = type;

            // Initial squash/stretch values
            switch (type)
            {
                case ReactionType.Jump:
                    targetSquashY = aerial.jumpStretch;
                    targetStretchXZ = aerial.jumpSquash;
                    break;
                case ReactionType.DoubleJump:
                    targetSquashY = aerial.jumpStretch * 1.1f;
                    targetStretchXZ = aerial.jumpSquash * 0.9f;
                    break;
                case ReactionType.Land:
                    targetSquashY = aerial.landSquash;
                    targetStretchXZ = aerial.landSpread;
                    break;
                case ReactionType.HardLand:
                    targetSquashY = aerial.hardLandSquash;
                    targetStretchXZ = aerial.landSpread * 1.2f;
                    AnimationEvents.ScreenEffect(ScreenEffectType.Shake, 0.3f, 0.2f);
                    break;
                case ReactionType.Damage:
                    targetSquashY = reactions.damageSquash;
                    break;
                case ReactionType.Collect:
                    targetSquashY = reactions.collectPop;
                    targetStretchXZ = reactions.collectPop;
                    break;
                case ReactionType.AirDash:
                    targetSquashY = 1f / aerial.dashStretch;
                    targetStretchXZ = aerial.dashStretch;
                    break;
            }
        }

        void UpdateReaction(float dt)
        {
            reactionTimer += dt;
            float intensity = toneIntensityMultiplier;

            float duration = GetReactionDuration(currentReaction);
            float t = Mathf.Clamp01(reactionTimer / duration);

            switch (currentReaction)
            {
                case ReactionType.Jump:
                case ReactionType.DoubleJump:
                    AnimateJumpReaction(t);
                    break;
                case ReactionType.Land:
                case ReactionType.HardLand:
                    AnimateLandReaction(t);
                    break;
                case ReactionType.Damage:
                    AnimateDamageReaction(t);
                    break;
                case ReactionType.Collect:
                    AnimateCollectReaction(t);
                    break;
                case ReactionType.PrincipleCollect:
                    AnimatePrincipleCollectReaction(t);
                    break;
                case ReactionType.Death:
                    AnimateDeathReaction(t);
                    return; // Death doesn't end
                case ReactionType.AirDash:
                    AnimateAirDashReaction(t);
                    break;
                case ReactionType.WallKick:
                    AnimateWallKickReaction(t);
                    break;
            }

            if (t >= 1f)
            {
                isReacting = false;
                targetSquashY = 1f;
                targetStretchXZ = 1f;
            }
        }

        float GetReactionDuration(ReactionType type)
        {
            return type switch
            {
                ReactionType.Jump => aerial.jumpAnticipation,
                ReactionType.DoubleJump => aerial.jumpAnticipation * 0.8f,
                ReactionType.Land => aerial.landRecoveryTime,
                ReactionType.HardLand => aerial.landRecoveryTime * 1.5f,
                ReactionType.Damage => reactions.damageRecoveryTime,
                ReactionType.Collect => reactions.collectDuration,
                ReactionType.PrincipleCollect => reactions.principleDuration,
                ReactionType.Death => reactions.deathDuration,
                ReactionType.AirDash => 0.2f,
                ReactionType.WallKick => wall.wallKickDuration,
                _ => 0.2f
            };
        }

        void AnimateJumpReaction(float t)
        {
            float intensity = toneIntensityMultiplier;

            if (t < 0.3f)
            {
                // Quick anticipation squat
                float squatT = t / 0.3f;
                targetSquashY = Mathf.Lerp(1f, aerial.jumpSquash * 0.95f, squatT);
                targetOffset.y = -aerial.jumpAnticipation * squatT * intensity;
            }
            else
            {
                // Launch
                float launchT = (t - 0.3f) / 0.7f;
                targetSquashY = Mathf.Lerp(aerial.jumpSquash, aerial.jumpStretch, launchT);
                targetStretchXZ = Mathf.Lerp(1.05f, aerial.jumpSquash, launchT);
                targetRotationEuler.x = -aerial.jumpTuckAngle * launchT * intensity;
            }
        }

        void AnimateLandReaction(float t)
        {
            float intensity = toneIntensityMultiplier;

            // Squash then recover
            float squashT = Mathf.Sin(t * Mathf.PI);
            float landSquash = currentReaction == ReactionType.HardLand ? aerial.hardLandSquash : aerial.landSquash;
            targetSquashY = Mathf.Lerp(landSquash, 1f, t);
            targetStretchXZ = Mathf.Lerp(aerial.landSpread, 1f, t);
            targetOffset.y = -0.05f * (1f - t) * intensity;
        }

        void AnimateDamageReaction(float t)
        {
            float intensity = toneIntensityMultiplier;

            // Recoil and shake
            float recoilT = 1f - t;
            targetSquashY = Mathf.Lerp(reactions.damageSquash, 1f, t);
            targetRotationEuler.x = -reactions.damageRecoilAngle * recoilT * intensity;

            // Shake
            float shake = Mathf.Sin(t * 50f) * reactions.damageShakeAmount * recoilT * intensity;
            targetOffset.x = shake;
            targetOffset.z = shake * 0.5f;
        }

        void AnimateCollectReaction(float t)
        {
            float intensity = toneIntensityMultiplier;

            // Pop and spin
            float popT = Mathf.Sin(t * Mathf.PI);
            targetSquashY = 1f + (reactions.collectPop - 1f) * popT;
            targetStretchXZ = 1f + (reactions.collectPop - 1f) * popT * 0.5f;
            targetRotationEuler.y = reactions.collectSpinSpeed * t * intensity;
        }

        void AnimatePrincipleCollectReaction(float t)
        {
            float intensity = toneIntensityMultiplier;

            // Glowing expansion
            float glowT = Mathf.Sin(t * Mathf.PI);
            targetSquashY = 1f + (reactions.principleGlow - 1f) * glowT;
            targetStretchXZ = 1f + (reactions.principleGlow - 1f) * glowT;
            targetOffset.y = 0.1f * glowT * intensity;

            // VFX
            if (t < 0.1f)
            {
                AnimationEvents.VFXRequest(VFXType.PrincipleGlow, transform.position, Quaternion.identity, 1.5f);
            }
        }

        void AnimateDeathReaction(float t)
        {
            float curveValue = reactions.deathFadeCurve.Evaluate(t);
            targetSquashY = curveValue;
            targetStretchXZ = 2f - curveValue;
            targetRotationEuler.x = reactions.deathFallAngle * t;
            targetOffset.y = -0.5f * t;
        }

        void AnimateAirDashReaction(float t)
        {
            float stretch = Mathf.Lerp(aerial.dashStretch, 1f, t);
            targetSquashY = 1f / stretch;
            targetStretchXZ = stretch;
            targetRotationEuler.x = aerial.dashAngle * (1f - t);
        }

        void AnimateWallKickReaction(float t)
        {
            float intensity = toneIntensityMultiplier;

            targetRotationEuler.y = wall.wallKickSpin * t * intensity;
            targetSquashY = Mathf.Lerp(wall.wallKickTuck, 1f, t);
        }

        #endregion

        #region Emotional Response

        void HandleStoryBeat(int chapter, EmotionalTone tone)
        {
            if (!emotional.respondToStoryBeats) return;

            currentTone = tone;
            targetToneIntensity = GetIntensityForTone(tone);
            targetToneSpeed = GetSpeedForTone(tone);
        }

        float GetIntensityForTone(EmotionalTone tone)
        {
            return tone switch
            {
                EmotionalTone.Gentle => emotional.gentleMultiplier,
                EmotionalTone.Hopeful => emotional.hopefulMultiplier,
                EmotionalTone.Melancholic => emotional.melancholicMultiplier,
                EmotionalTone.Grounded => emotional.groundedMultiplier,
                EmotionalTone.Tender => emotional.tenderMultiplier,
                _ => 1f
            };
        }

        float GetSpeedForTone(EmotionalTone tone)
        {
            return tone switch
            {
                EmotionalTone.Gentle => emotional.gentleSpeedMult,
                EmotionalTone.Hopeful => emotional.hopefulSpeedMult,
                EmotionalTone.Melancholic => emotional.melancholicSpeedMult,
                EmotionalTone.Grounded => emotional.groundedSpeedMult,
                _ => 1f
            };
        }

        void UpdateEmotionalState(float dt)
        {
            float transitionSpeed = dt / emotional.toneTransitionTime;
            toneIntensityMultiplier = Mathf.MoveTowards(toneIntensityMultiplier, targetToneIntensity, transitionSpeed);
            toneSpeedMultiplier = Mathf.MoveTowards(toneSpeedMultiplier, targetToneSpeed, transitionSpeed);
        }

        #endregion

        #region Final Application

        void UpdateSquashStretch(float dt)
        {
            float speed = squashStretch.recoverySpeed * dt;
            currentSquashY = Mathf.MoveTowards(currentSquashY, targetSquashY, speed);
            currentStretchXZ = Mathf.MoveTowards(currentStretchXZ, targetStretchXZ, speed);

            // Clamp
            currentSquashY = Mathf.Clamp(currentSquashY, squashStretch.minSquash, squashStretch.maxStretch);
            currentStretchXZ = Mathf.Clamp(currentStretchXZ, squashStretch.minSquash, squashStretch.maxStretch);

            // Reset targets toward 1
            if (!isReacting && !isPerformingVerb)
            {
                targetSquashY = Mathf.MoveTowards(targetSquashY, 1f, speed * 0.5f);
                targetStretchXZ = Mathf.MoveTowards(targetStretchXZ, 1f, speed * 0.5f);
            }
        }

        void SmoothAndApply(float dt)
        {
            // Smooth position
            currentOffset = Vector3.Lerp(currentOffset, targetOffset, smoothing.positionSmoothing * dt);

            // Smooth rotation
            currentRotationEuler = Vector3.Lerp(currentRotationEuler, targetRotationEuler, smoothing.rotationSmoothing * dt);

            // Calculate final scale with volume preservation
            Vector3 finalScale = baseScale;
            if (squashStretch.preserveVolume)
            {
                float volumeCorrection = 1f / Mathf.Sqrt(currentSquashY * currentStretchXZ * currentStretchXZ);
                finalScale.y *= currentSquashY;
                finalScale.x *= currentStretchXZ * volumeCorrection;
                finalScale.z *= currentStretchXZ * volumeCorrection;
            }
            else
            {
                finalScale.y *= currentSquashY;
                finalScale.x *= currentStretchXZ;
                finalScale.z *= currentStretchXZ;
            }

            // Apply
            visualTarget.localPosition = baseLocalPos + currentOffset;
            visualTarget.localRotation = baseLocalRot * Quaternion.Euler(currentRotationEuler);
            visualTarget.localScale = Vector3.Lerp(visualTarget.localScale, finalScale, smoothing.scaleSmoothing * dt);
        }

        #endregion

        #region Debug

        void OnGUI()
        {
            if (!showDebugInfo) return;

            GUILayout.BeginArea(new Rect(10, 10, 300, 400));
            GUILayout.Label($"<b>Translator Animation</b>");
            GUILayout.Label($"Speed: {normalizedSpeed:F2} | Grounded: {isGrounded}");
            GUILayout.Label($"Tone: {currentTone} (Intensity: {toneIntensityMultiplier:F2})");
            GUILayout.Label($"Squash: {currentSquashY:F2} | Stretch: {currentStretchXZ:F2}");
            GUILayout.Label($"Windprint: {currentWindprintMode}");
            if (isReacting) GUILayout.Label($"Reaction: {currentReaction}");
            if (isPerformingVerb) GUILayout.Label($"Verb: {currentVerb}");
            GUILayout.EndArea();
        }

        #endregion
    }
}
