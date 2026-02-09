# 🌿 Spiny Flannel Society - Unity Quick Start

## 🚀 START HERE (One Click Setup)

Open Unity and go to:
```
Menu → SFS → Animation → Setup Wizard
```

Click **"Run Setup Wizard"** to create:
- ✅ All folder structure and prefabs
- ✅ VFX Manager for particle effects
- ✅ Player with full animation system
- ✅ NPCs (DAZIE, June, Winton, companions)
- ✅ Living architecture prefabs
- ✅ Antagonistic pattern prefabs
- ✅ Demo scene ready to play

**Then just press Play!**

---

## 🎬 NEW: Comprehensive Animation Package

See **ANIMATION_PACKAGE.md** for full documentation.

### Key Features
- **Procedural Animation**: No animation clips needed - pure code-driven
- **Event-Driven**: Decoupled architecture with central event bus
- **Story-Aware**: Animations respond to emotional tones and story beats
- **Combat Verb System**: Full support for all 5 combat verbs
- **Windprint Rig**: Cushion and Guard mode animations
- **Living Architecture**: Platforms, ramps, bridges that breathe and respond
- **Antagonistic Patterns**: Echo Form, Distortion, Noise Beast animations
- **VFX System**: Pooled particle effects for all game actions

### Quick Integration

```csharp
// Trigger player actions
AnimationEvents.PlayerAction(PlayerAction.Jump);

// Combat verbs
AnimationEvents.CombatVerbUsed(CombatVerb.Pulse, position, direction, target);

// Windprint mode
AnimationEvents.WindprintModeChanged(WindprintMode.None, WindprintMode.Cushion);

// Story/emotion
AnimationEvents.EmotionalToneChanged(EmotionalTone.Gentle, EmotionalTone.Hopeful);
```

---

## 📁 Project Structure

```
Assets/_SFS/
├── Animations/
│   ├── Clips/              # Animation clips
│   ├── PlayerAnimator.controller
│   └── NPCAnimator.controller
├── Materials/              # Game materials
├── Prefabs/
│   ├── Animation/          # Core animation prefabs
│   ├── Characters/         # Player, NPCs
│   ├── Environment/        # Architecture prefabs
│   └── VFX/                # VFX Manager
├── Scenes/
│   └── SFS_DemoScene.unity
├── Scripts/
│   ├── Animation/          # ★ NEW: Complete animation package
│   │   ├── Core/           # Events, state machine
│   │   ├── Player/         # Translator animation
│   │   ├── NPC/            # All NPC animators
│   │   ├── Patterns/       # Antagonistic patterns
│   │   ├── Environment/    # Living architecture
│   │   ├── VFX/            # Effect system
│   │   ├── Camera/         # Cinematic camera
│   │   └── Editor/         # Setup wizard
│   ├── Camera/             # Third-person camera
│   ├── Core/               # Game managers, events
│   ├── Editor/             # Unity editor tools
│   ├── Narrative/          # Story/cutscene scripts
│   ├── Player/             # Player controller & animation
│   ├── UI/                 # Pause, menus
│   └── World/              # Collectibles, checkpoints
└── UI/                     # UI assets
```

---

## 🎬 Animation System

### TranslatorAnimationController (Player)

Add to your player character for complete animation:
- Idle breathing and sway
- Walk/run with bob and lean
- Jump squash/stretch
- All aerial moves (double jump, air dash, wall run, glide)
- Combat verb animations
- Windprint Rig visual modes
- Damage/collect reactions
- Story beat emotional response

### NPC Animators

| Script | Character |
|--------|-----------|
| `DAZIEAnimator` | Mentor - precise, measured |
| `JuneAnimator` | Sensory architect - warm, sparse |
| `WintonAnimator` | System ghost - ethereal, glitchy |
| `SocietyMemberAnimator` | Generic NPC with personality |
| `CompanionAnimator` | Following companion |

### Living Architecture

Add `LivingArchitectureAnimator` to environmental objects:
- Platforms that bob and breathe
- Ramps that bloom when you approach
- Bridges with umbel-style sway
- Signage affected by Drift
- Safe pockets that warm up

### Antagonistic Patterns

Add `AntagonisticPatternAnimator` to pattern enemies:
- **Echo Form**: Looping coercive motion
- **Distortion**: Glitch and phase shift
- **Noise Beast**: Sensory chaos

---

## 🎮 Player Controller

### EnhancedPlayerController

Full-featured 3D platformer controller:

| Feature | Description |
|---------|-------------|
| **Smooth Movement** | Acceleration/deceleration, air control |
| **Coyote Time** | Jump grace period after leaving ground |
| **Jump Buffer** | Early jump input remembered |
| **Jump Cut** | Release early for short hop |
| **Apex Gravity** | Snappy feel at jump peak |
| **Camera-Relative** | Movement relative to camera |

**Key Settings:**
```csharp
moveSpeed = 7f;       // Max movement speed
jumpHeight = 2f;      // Jump height in units
coyoteTime = 0.12f;   // Timing forgiveness
jumpBuffer = 0.15f;   // Input buffering
airControl = 0.6f;    // Air maneuverability
```

### Controls

| Input | Action |
|-------|--------|
| WASD / Left Stick | Move |
| Space / A Button | Jump |
| Mouse / Right Stick | Camera |
| Escape | Pause |

---

## 📷 Camera System

### EnhancedThirdPersonCamera

| Feature | Description |
|---------|-------------|
| **Orbit Camera** | Mouse/stick orbit around player |
| **Zoom** | Scroll wheel / triggers |
| **Collision** | Camera doesn't clip through walls |
| **Auto-Rotate** | Optional follow behind player |
| **Cinematic Mode** | Story beat camera overrides |
| **Shake** | Impact feedback |

**Key Methods:**
```csharp
camera.SetDistance(8f);          // Zoom
camera.Shake(0.3f, 0.2f);        // Screen shake
camera.EnterCinematicMode(...);  // Cutscene control
```

---

## 🧩 Key Components

### Core Managers

| Script | Purpose |
|--------|---------|
| `StoryBeatManager` | Tracks narrative progression |
| `SettingsManager` | Accessibility settings |
| `GameEvents` | Global event bus |

### Animation

| Script | Purpose |
|--------|---------|
| `AdvancedProceduralAnimator` | Code-based player animation |
| `NPCProceduralAnimator` | NPC idle/reaction animation |
| `PlayerAnimatorDriver` | Unity Animator bridge |

### World

| Script | Purpose |
|--------|---------|
| `Collectible` | Pickupable items |
| `Checkpoint` | Respawn points |
| `RestZone` | Accessibility rest areas |

---

## 🎯 Adding New Content

### New Character

1. Create capsule/model
2. Add `CharacterController`
3. Add `EnhancedPlayerController`
4. Add `AdvancedProceduralAnimator` to visual child
5. Tag as "Player"

### New NPC

1. Create capsule/model
2. Add `CapsuleCollider`
3. Add `NPCProceduralAnimator` to visual child
4. Optionally add dialogue/interaction scripts

### New Collectible

1. Create object with trigger collider
2. Add `Collectible` script
3. Add `SimpleSpinner` for visual flair

---

## 🔧 Editor Tools

Access via `SFS` menu:

| Menu Item | Action |
|-----------|--------|
| **🚀 Project Wizard** | Complete setup UI |
| **Setup > Complete Setup** | One-click all steps |
| **Setup > Generate Animations** | Create placeholder clips |
| **Setup > Assign Animations** | Link clips to controllers |
| **Setup > Create Player Animator** | Rebuild player controller |

---

## 🐛 Troubleshooting

### "Player doesn't move"
- Check `EnhancedPlayerController` is attached
- Check Input settings exist (Edit → Project Settings → Input Manager)
- Check `CharacterController` radius isn't too small

### "No animations"
- Add `AdvancedProceduralAnimator` to the visual child object
- Or run: SFS → Setup → Generate All Placeholder Animations

### "Camera doesn't follow"
- Assign player to camera's `Target` field
- Or tag player as "Player" for auto-detection

### "Animations too subtle/extreme"
- Adjust values in `AdvancedProceduralAnimator` inspector
- `IdleSettings`, `LocomotionSettings`, etc.

### "Compiler errors about missing references"
- Make sure assembly definitions reference each other
- Check `SFS.asmdef` includes `SFS.Animation`

---

## 📊 Performance Notes

- Procedural animation is **more performant** than clip animation
- Uses no blend trees or state machine overhead
- Both systems are optimized for mobile
- Disable `microMovements` if targeting low-end devices

---

## 🎨 Customization

### Color Palette

Edit materials in `Assets/_SFS/Materials/`:
- `PlayerMaterial` - Main character color
- `NPCMaterial` - NPC color
- `GroundMaterial` - Environment base
- `CollectibleMaterial` - Pickups

### Animation Feel

Adjust in `AdvancedProceduralAnimator`:
- `idle.breatheSpeed` / `breatheAmount` - Idle intensity
- `locomotion.walkBobHeight` - Step bounce
- `airborne.jumpSquash` / `jumpStretch` - Cartoon squash
- `easing.squashRecoverySpeed` - Snap vs smooth

### Accessibility

Adjust in `EnhancedPlayerController`:
- `coyoteTime` - Longer = more forgiving
- `jumpBuffer` - Longer = easier timing
- `airControl` - Higher = more control in air

---

## 📝 Next Steps

1. ✅ Run the Project Wizard
2. ✅ Press Play and test movement
3. Replace placeholder capsules with your character models
4. Add story beat triggers to your levels
5. Customize animation settings to match your art style
6. Build your world!

**Happy developing!** 🌿
