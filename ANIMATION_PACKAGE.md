# Spiny Flannel Society - Animation Package

## Overview

This is a comprehensive, market-ready animation package for Spiny Flannel Society, a neuroaffirming 3D platformer about a Translator navigating Flannel City's living architecture and discovering belonging.

The animation system is designed to be:
- **Story-driven**: Animations respond to emotional tones and story beats
- **Event-based**: Decoupled architecture using C# events
- **Procedural**: No animation clips required - pure code-driven animation
- **Accessible**: Supports the game's neuroaffirming design principles
- **Market-ready**: Sophisticated, polished quality matching 2025/2026 standards

---

## Quick Start

### One-Click Setup

1. Open **SFS > Animation > Setup Wizard** from Unity's menu
2. Select desired components
3. Click **Run Setup Wizard**
4. The wizard creates all prefabs and configures the scene

### Manual Setup

1. Add `VFXManager` to your scene (one instance)
2. Add `TranslatorAnimationController` to your player character
3. Add `NPCAnimationController` variants to NPCs
4. Add `LivingArchitectureAnimator` to environmental objects
5. Add `CinematicCameraController` to your main camera

---

## Architecture

```
SFS.Animation/
├── Core/
│   ├── AnimationEvents.cs       # Central event system
│   └── AnimationStateMachine.cs # Layer-based state machine
├── Player/
│   └── TranslatorAnimationController.cs  # Complete player animation
├── NPC/
│   └── NPCAnimationController.cs   # All NPC types
├── Patterns/
│   └── AntagonisticPatternAnimator.cs  # Echo Form, Distortion, Noise Beast
├── Environment/
│   └── LivingArchitectureAnimator.cs   # Living architecture types
├── VFX/
│   └── VFXManager.cs   # Particle and effect coordination
├── Camera/
│   └── CinematicCameraController.cs  # Cinematic camera system
└── Editor/
    └── SFSAnimationWizard.cs   # Setup wizard
```

---

## Core Systems

### AnimationEvents (Event Bus)

All animation communication goes through `AnimationEvents`. This decouples systems and allows any component to react to game events.

```csharp
// Triggering events
AnimationEvents.PlayerAction(PlayerAction.Jump);
AnimationEvents.CombatVerbUsed(CombatVerb.Pulse, position, direction, target);
AnimationEvents.StoryBeatChanged(oldBeat, newBeat);
AnimationEvents.EmotionalToneChanged(EmotionalTone.Gentle, EmotionalTone.Hopeful);

// Listening to events
void Start() {
    AnimationEvents.OnPlayerAction += HandlePlayerAction;
    AnimationEvents.OnCombatVerbUsed += HandleCombatVerb;
}

void OnDestroy() {
    AnimationEvents.OnPlayerAction -= HandlePlayerAction;
    AnimationEvents.OnCombatVerbUsed -= HandleCombatVerb;
}
```

#### Key Events

| Event | Description |
|-------|-------------|
| `OnMovementStateChanged` | Movement state transitions |
| `OnPlayerAction` | Player actions (jump, dash, land, etc.) |
| `OnCombatVerbUsed` | Combat verb activation |
| `OnWindprintModeChanged` | Windprint Rig mode changes |
| `OnStoryBeatChanged` | Story progression |
| `OnEmotionalToneChanged` | Tone shifts |
| `OnDriftIntensityChanged` | Drift corruption level |
| `OnCollectibleCollected` | Collectible pickup |
| `OnPlayerDamaged` | Damage received |
| `OnVFXRequest` | VFX spawn request |
| `OnScreenEffect` | Screen overlay effect |

---

### TranslatorAnimationController (Player)

Complete player animation with these systems:

#### Idle Animation
- Breathing cycle with chest rise
- Subtle weight shifts
- Micro-movements for life
- Head bob variations

#### Locomotion
- Walk/run cycle with bob and lean
- Direction-responsive tilting
- Smooth acceleration curves

#### Aerial Movement
- Jump squash/stretch (volume preserving)
- Double jump spin
- Air dash forward lean
- Fall posture
- Glide arms
- Land impact

#### Combat Verbs

| Verb | Animation |
|------|-----------|
| Pulse | Charge crouch → explosive stretch |
| Thread Lash | Arm extension → follow through |
| Radiant Hold | Centering → sustained glow pose |
| Edge Claim | Grounding stance → boundary push |
| Re-tune | Breathing rhythm → harmonic pose |

#### Windprint Rig

| Mode | Visual |
|------|--------|
| Cushion | Soft pulsing, arms slightly out |
| Guard | Protective stance, grounded |

#### Emotional Response
Each emotional tone modifies animation speed and intensity:
- **Gentle**: 0.9x speed, 0.95x intensity
- **Hopeful**: 1.05x speed, 1.1x intensity
- **Melancholic**: 0.85x speed, 1.0x intensity
- **Grounded**: 1.0x speed, 1.05x intensity
- **Tender**: 0.92x speed, 0.9x intensity

---

### NPC Animation Controllers

#### NPCAnimationController (Base)
- Idle variants (breathe, shift, look around)
- Player acknowledgment (nod, wave, gesture)
- Group synchronization
- Belonging state for finale

#### DAZIEAnimator (Mentor)
- Precise, measured movements
- Teaching gestures
- Contemplative pauses

#### JuneAnimator (Sensory Architect)
- Warm but sparse movement
- Environmental sensitivity
- Stillness moments

#### WintonAnimator (System Ghost)
- Ethereal hover (slight float)
- Digital glitch effects
- Processing flicker
- Transparency pulse

#### SocietyMemberAnimator
- Personality variance system
- Activity states (idle, walking, working, conversing, resting)

#### CompanionAnimator
- Player following behavior
- Alert state when player is in danger
- Support gestures

---

### AntagonisticPatternAnimator

Animates the non-villain antagonists (embodied consequences of poorly-designed systems):

#### Echo Form (Coercive Scripts)
- Looping motion representing repeated expectations
- Jerkiness in movement
- Hesitation moments
- Dissolved by Thread Lash or Pulse

#### Distortion (Broken Rules)
- Glitch movement effects
- Phase shift wobble
- Reality tear jitter
- Corrected by Pulse or Edge Claim

#### Noise Beast (Sensory Overload)
- Chaotic movement
- Overload scaling (grows with intensity)
- Storm particle effect
- Calmed by Re-tune or Radiant Hold

---

### LivingArchitectureAnimator

Animates Flannel City's responsive architecture:

| Type | Behavior |
|------|----------|
| Platform | Rhythm bob, knitting animation |
| Ramp | Bloom/retract based on player proximity |
| Bridge | Umbel-style cluster sway |
| Signage | Multi-modal render, drift corruption |
| Canopy | Light filtering, dappling |
| SafePocket | Warm protective pulse |
| ConsentGate | Ready pulse, solid when activated |
| DesignTerminal | Processing shimmer, player attention |

#### Drift Response
When Drift corruption is high, architecture:
- Jitters and twists
- Loses scale
- Rejects the player (tilts away)
- Ramps may retract

#### Windprint Response
- **Cushion Mode**: Architecture blooms and softens
- **Guard Mode**: Architecture stabilizes and locks rhythm

---

### VFXManager

Coordinates all visual effects with object pooling for performance.

#### Combat Verb VFX
- **Pulse**: Expanding sphere burst
- **Thread Lash**: Line trail with segments
- **Radiant Hold**: Multiple ring expansion
- **Edge Claim**: Box boundary expansion
- **Re-tune**: Wave pulses with gradient

#### Movement VFX
- Footstep dust
- Land impact particles
- Air dash trails
- Wall run sparks
- Double jump ring

#### Screen Effects
- Damage flash
- Story transition fade
- Drift corruption (aberration, vignette, distortion)
- Belonging glow

---

### CinematicCameraController

Story-aware camera system:

#### Following
- Smooth follow with look-ahead
- Configurable offset and zoom
- Emotional tone modifiers

#### Story Transitions
- **Arrival Sweep**: Opening chapter sweep
- **Belonging Pullback**: Final chapter wide shot
- **Conversation Framing**: Two-shot framing for dialogue

#### Combat Camera
- Verb-specific camera movements
- Focus on combat points
- Dynamic shake

#### Drift Effects
- Camera jitter
- Tilt corruption
- Rejection push

---

## Enums Reference

### PlayerAction
```
None, Jump, DoubleJump, AirDash, WallRun, WallKick,
Land, LandHard, Glide, GlideEnd, Crouch, Slide,
EnterRest, ExitRest, Interact, Die, Respawn
```

### CombatVerb
```
None, Pulse, ThreadLash, RadiantHold, EdgeClaim, ReTune
```

### WindprintMode
```
None, Cushion, Guard
```

### EmotionalTone
```
Gentle, Hopeful, Melancholic, Grounded, Tender
```

### ArchitectureType
```
Platform, Ramp, Bridge, Signage, Canopy,
SafePocket, ConsentGate, DesignTerminal
```

### PatternType
```
EchoForm, Distortion, NoiseBeast
```

---

## Integration Guide

### With Existing PlayerController

```csharp
// In your PlayerController, trigger animation events:

void OnJump()
{
    AnimationEvents.PlayerAction(PlayerAction.Jump);
}

void OnCombat(CombatVerb verb)
{
    AnimationEvents.CombatVerbUsed(verb, transform.position, transform.forward, target);
}

void OnWindprintChange(WindprintMode mode)
{
    AnimationEvents.WindprintModeChanged(currentMode, mode);
}
```

### With Story System

```csharp
// In your story manager:
void OnBeatChange(StoryBeat newBeat)
{
    AnimationEvents.StoryBeatChanged(previousBeat, newBeat);
    AnimationEvents.EmotionalToneChanged(previousTone, newBeat.emotionalTone);
}
```

### With Drift System

```csharp
// When drift level changes:
AnimationEvents.DriftIntensityChanged(previousLevel, newLevel);
```

---

## Performance Considerations

- VFX uses object pooling (configurable pool size)
- Animation calculations are lightweight procedural transforms
- Events use standard C# delegates (no reflection)
- Update loops optimized with cached references

---

## Customization

### Creating Custom NPC Types

```csharp
public class MyCustomNPC : NPCAnimationController
{
    [Header("Custom Settings")]
    public float customParameter = 1f;

    protected override void CalculateIdleAnimation()
    {
        base.CalculateIdleAnimation();
        // Add custom idle behavior
    }

    protected override void CalculateAcknowledgment()
    {
        // Custom acknowledgment animation
    }
}
```

### Adding New VFX Types

1. Add to `VFXType` enum in `AnimationEvents.cs`
2. Create spawn method in `VFXManager.cs`
3. Add to pool initialization

---

## Story Beat Reference

The animation system is designed around the 12-chapter structure:

| Chapter | Tone | Animation Notes |
|---------|------|-----------------|
| 1: Arrival | Gentle | Slow, observant movements |
| 2: First Echoes | Hopeful → Uncertain | Building to pattern encounters |
| 3: The Drift | Melancholic | Architecture reaction animations |
| 4: Hidden Gardens | Grounded | Peaceful, organic movement |
| 5: Noise | Tense | Intense, protective animations |
| 6: June's Archive | Tender | Warm, quiet interactions |
| 7: Fault Lines | Melancholic | Heavy, weighted movement |
| 8: Consent | Grounded | Clear, deliberate actions |
| 9: System Access | Hopeful | Discovery and revelation |
| 10: The Rewrite | Hopeful | Triumphant, expansive |
| 11: Bloom | Grounded | Flourishing, growth |
| 12: Belonging | Tender → Gentle | Community, acceptance |

---

## Troubleshooting

### Animations Not Playing
- Ensure `AnimationEvents` is being called
- Check that components have `visualTarget` assigned
- Verify `Time.timeScale` is not 0

### VFX Not Appearing
- Confirm `VFXManager` exists in scene
- Check pool settings aren't too small
- Verify shader compatibility

### Camera Jittering
- Adjust smoothing values
- Reduce drift camera intensity
- Check for conflicting camera controllers

---

## Version History

### 1.0.0 (2025)
- Complete animation package
- All character types
- VFX system
- Editor wizard
- Documentation

---

## Credits

Designed for Spiny Flannel Society, a neuroaffirming 3D platformer about recognition, belonging, and systemic change.

---
