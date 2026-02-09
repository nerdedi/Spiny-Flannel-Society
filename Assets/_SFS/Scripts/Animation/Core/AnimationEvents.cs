using System;
using UnityEngine;

namespace SFS.Animation
{
    /// <summary>
    /// Centralized animation event system for the Spiny Flannel Society.
    /// Coordinates all animation triggers across player, NPCs, environment, and VFX.
    /// </summary>
    public static class AnimationEvents
    {
        #region Player Animation Events

        /// <summary>Fired when player movement state changes (speed, grounded, direction)</summary>
        public static event Action<MovementState> OnPlayerMovementChanged;

        /// <summary>Fired when player performs any action (jump, dash, wall run, etc.)</summary>
        public static event Action<PlayerAction> OnPlayerAction;

        /// <summary>Fired when player uses a combat verb (Pulse, Thread Lash, etc.)</summary>
        public static event Action<CombatVerb, Vector3> OnCombatVerbUsed;

        /// <summary>Fired when Windprint Rig mode changes (Cushion/Guard)</summary>
        public static event Action<WindprintMode, WindprintMode> OnWindprintModeChanged;

        /// <summary>Fired when player takes damage or is affected by Drift</summary>
        public static event Action<DamageType, float> OnPlayerDamaged;

        /// <summary>Fired when player collects something</summary>
        public static event Action<CollectibleType> OnPlayerCollected;

        /// <summary>Fired when player dies</summary>
        public static event Action<DeathType> OnPlayerDeath;

        /// <summary>Fired when player respawns</summary>
        public static event Action OnPlayerRespawn;

        #endregion

        #region NPC Animation Events

        /// <summary>Fired when an NPC acknowledges the player</summary>
        public static event Action<Transform, AcknowledgeType> OnNPCAcknowledge;

        /// <summary>Fired when NPCs should sync their animations (for groups)</summary>
        public static event Action<Transform, float> OnNPCGroupSync;

        /// <summary>Fired when an NPC transitions to a new emotional state</summary>
        public static event Action<Transform, EmotionalTone> OnNPCEmotionChanged;

        /// <summary>Fired when an NPC enters belonging state (finale)</summary>
        public static event Action<Transform> OnNPCBelongingReached;

        #endregion

        #region Environmental Animation Events

        /// <summary>Fired when Drift intensity changes</summary>
        public static event Action<float, float> OnDriftIntensityChanged;

        /// <summary>Fired when architecture responds to player (ramps bloom, bridges knit)</summary>
        public static event Action<Transform, ArchitectureResponse> OnArchitectureRespond;

        /// <summary>Fired when environmental hazard state changes</summary>
        public static event Action<Transform, HazardState> OnHazardStateChanged;

        /// <summary>Fired during wind current events</summary>
        public static event Action<Vector3, float> OnWindPulse;

        #endregion

        #region Story Beat Events

        /// <summary>Fired when the current story beat changes</summary>
        public static event Action<int, EmotionalTone> OnStoryBeatAnimation;

        /// <summary>Fired when entering a chapter's cinematic sequence</summary>
        public static event Action<int> OnCinematicBegin;

        /// <summary>Fired when exiting a cinematic sequence</summary>
        public static event Action OnCinematicEnd;

        #endregion

        #region VFX Events

        /// <summary>Request particle effect spawn</summary>
        public static event Action<VFXType, Vector3, Quaternion, float> OnVFXRequest;

        /// <summary>Request screen effect (flash, shake, distortion)</summary>
        public static event Action<ScreenEffectType, float, float> OnScreenEffect;

        /// <summary>Request trail effect on transform</summary>
        public static event Action<Transform, TrailType, float> OnTrailRequest;

        #endregion

        #region Event Triggers

        // Player triggers
        public static void PlayerMovementChanged(MovementState state)
            => OnPlayerMovementChanged?.Invoke(state);

        public static void PlayerAction(PlayerAction action)
            => OnPlayerAction?.Invoke(action);

        public static void CombatVerbUsed(CombatVerb verb, Vector3 targetPos)
            => OnCombatVerbUsed?.Invoke(verb, targetPos);

        public static void WindprintModeChanged(WindprintMode from, WindprintMode to)
            => OnWindprintModeChanged?.Invoke(from, to);

        public static void PlayerDamaged(DamageType type, float amount)
            => OnPlayerDamaged?.Invoke(type, amount);

        public static void PlayerCollected(CollectibleType type)
            => OnPlayerCollected?.Invoke(type);

        public static void PlayerDeath(DeathType type)
            => OnPlayerDeath?.Invoke(type);

        public static void PlayerRespawn()
            => OnPlayerRespawn?.Invoke();

        // NPC triggers
        public static void NPCAcknowledge(Transform npc, AcknowledgeType type)
            => OnNPCAcknowledge?.Invoke(npc, type);

        public static void NPCGroupSync(Transform leader, float phase)
            => OnNPCGroupSync?.Invoke(leader, phase);

        public static void NPCEmotionChanged(Transform npc, EmotionalTone tone)
            => OnNPCEmotionChanged?.Invoke(npc, tone);

        public static void NPCBelongingReached(Transform npc)
            => OnNPCBelongingReached?.Invoke(npc);

        // Environmental triggers
        public static void DriftIntensityChanged(float previous, float current)
            => OnDriftIntensityChanged?.Invoke(previous, current);

        public static void ArchitectureRespond(Transform architecture, ArchitectureResponse response)
            => OnArchitectureRespond?.Invoke(architecture, response);

        public static void HazardStateChanged(Transform hazard, HazardState state)
            => OnHazardStateChanged?.Invoke(hazard, state);

        public static void WindPulse(Vector3 direction, float strength)
            => OnWindPulse?.Invoke(direction, strength);

        // Story triggers
        public static void StoryBeatAnimation(int chapter, EmotionalTone tone)
            => OnStoryBeatAnimation?.Invoke(chapter, tone);

        public static void CinematicBegin(int chapter)
            => OnCinematicBegin?.Invoke(chapter);

        public static void CinematicEnd()
            => OnCinematicEnd?.Invoke();

        // VFX triggers
        public static void VFXRequest(VFXType type, Vector3 position, Quaternion rotation, float scale = 1f)
            => OnVFXRequest?.Invoke(type, position, rotation, scale);

        public static void ScreenEffect(ScreenEffectType type, float intensity, float duration)
            => OnScreenEffect?.Invoke(type, intensity, duration);

        public static void TrailRequest(Transform target, TrailType type, float duration)
            => OnTrailRequest?.Invoke(target, type, duration);

        #endregion
    }

    #region Enums

    /// <summary>Player movement state data</summary>
    public struct MovementState
    {
        public float Speed;
        public float NormalizedSpeed;
        public bool IsGrounded;
        public bool IsWallRunning;
        public bool IsGliding;
        public float YVelocity;
        public Vector3 MoveDirection;
        public Vector3 FacingDirection;
    }

    /// <summary>Player actions that trigger animations</summary>
    public enum PlayerAction
    {
        Jump,
        DoubleJump,
        AirDash,
        DoubleAirDash,
        WallRun,
        WallKick,
        GrappleThread,
        GlideStart,
        GlideEnd,
        PulseSlam,
        Land,
        LandHard,
        TripleHopShort,
        TripleHopLong,
        TripleHopFloat
    }

    /// <summary>Combat verbs from the design document</summary>
    public enum CombatVerb
    {
        Pulse,
        ThreadLash,
        RadiantHold,
        EdgeClaim,
        ReTune
    }

    /// <summary>Windprint Rig modes</summary>
    public enum WindprintMode
    {
        None,
        Cushion,
        Guard
    }

    /// <summary>Types of damage/drift effects</summary>
    public enum DamageType
    {
        Distortion,
        EchoForm,
        NoiseBeast,
        Environmental,
        Drift
    }

    /// <summary>Types of collectibles</summary>
    public enum CollectibleType
    {
        Standard,
        PrincipleModule,
        Lore,
        Cosmetic,
        WindprintPerk
    }

    /// <summary>Death types for different animations</summary>
    public enum DeathType
    {
        Fall,
        Drift,
        NoiseBeast,
        Environmental
    }

    /// <summary>NPC acknowledgment types</summary>
    public enum AcknowledgeType
    {
        Nod,
        Wave,
        Bow,
        Gesture,
        Verbal
    }

    /// <summary>Architecture response types</summary>
    public enum ArchitectureResponse
    {
        RampBloom,
        PlatformKnit,
        SignageRender,
        DoorOpen,
        PathReveal,
        BufferExpand,
        SafePocketSpawn
    }

    /// <summary>Hazard states</summary>
    public enum HazardState
    {
        Active,
        Warning,
        Paused,
        Disabled,
        CushionSlowed
    }

    /// <summary>VFX types</summary>
    public enum VFXType
    {
        // Windprint
        CushionActivate,
        CushionPulse,
        GuardActivate,
        GuardShield,

        // Combat Verbs
        PulseWave,
        ThreadLashArc,
        RadiantHoldDome,
        EdgeClaimPin,
        ReTuneWave,

        // Movement
        JumpDust,
        LandImpact,
        DashTrail,
        WallRunDust,
        GlideWind,

        // Collectibles
        CollectSparkle,
        PrincipleGlow,
        LoreReveal,

        // Environmental
        DriftDistortion,
        WindCurrent,
        ArchitectureBloom,

        // Story
        BeatTransition,
        ChapterTitle,
        AxiomRestore
    }

    /// <summary>Screen effect types</summary>
    public enum ScreenEffectType
    {
        Flash,
        Shake,
        Distortion,
        ChromaticAberration,
        Vignette,
        RadialBlur,
        DriftPulse
    }

    /// <summary>Trail effect types</summary>
    public enum TrailType
    {
        Movement,
        Dash,
        Windprint,
        Combat,
        Glide
    }

    /// <summary>Emotional tones matching the design document</summary>
    public enum EmotionalTone
    {
        Gentle = 0,
        Hopeful = 1,
        Melancholic = 2,
        Grounded = 3,
        Tender = 4
    }

    #endregion
}
