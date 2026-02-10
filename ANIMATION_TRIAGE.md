# Animation Triage & Wiring Checklist

**For anyone opening the Unity project and finding "nothing happens."**

---

## Part A — Fast Triage: Why Nothing Animates

Work through these in order. You'll find the culprit in 5–10 minutes.

### 1. Animator isn't driving the visible model

**Symptoms:** Parameters change in Inspector, but mesh doesn't move.

**Check:**
- [ ] Animator component is on the same GameObject as SkinnedMeshRenderer (or rig root)
- [ ] Animator → Avatar targets the correct rig (not a random parent)
- [ ] In Play Mode, click the character → confirm Animator → Controller is assigned

### 2. Controller is assigned but stuck in Entry/Idle forever

**Symptoms:** Animator window shows Entry highlighted, never transitions.

**Check:**
- [ ] Open Animator window **in Play Mode** and watch the highlighted state
- [ ] Transitions either have:
  - `Has Exit Time` enabled with correct clip lengths, OR
  - Conditions that actually become true (e.g., `Speed > 0`)

### 3. Parameter names or types don't match

**Symptoms:** You call `SetTrigger("Pulse")` but the parameter is named `PULSE` or is a Bool.

**Fix:**
- [ ] Use `AnimParams.cs` hashes — never raw strings
- [ ] Confirm each parameter matches the type in the Animator Controller:
  - `Speed` → Float
  - `Grounded` → Bool
  - `Jump` → Trigger
  - etc. (full list below)

### 4. Animation clip imported but wrong rig type

**Symptoms:** Clip plays in preview but bones don't move on character.

**Check:**
- [ ] Import Settings → Rig tab → Humanoid vs Generic must match your model
- [ ] Avatar created properly (Configure → all bones green)
- [ ] For Mixamo: set Avatar to Humanoid, configure mapping

### 5. Code overwrites the Animator every frame

**Symptoms:** Animation fires but instantly snaps back to Idle.

**Common bugs:**
- [ ] Check no script sets `Speed = 0` in every `Update()`
- [ ] Check no script calls `Play("Idle")` repeatedly
- [ ] `SetLocomotion()` should be the ONLY place that feeds continuous params

### 6. Layers / Avatar Masks / Weight = 0

**Symptoms:** Upper-body animations (verbs, reactions) never show.

**Check:**
- [ ] Animator Layer weight > 0 for all active layers
- [ ] Avatar Mask includes the bones you expect (spine, arms, head)
- [ ] Base Layer = full body locomotion; Layer 1 = upper-body verbs

### 7. Root Motion fights code-driven movement

**Symptoms:** Character "slides" or movement fights animation.

**Fix:**
- [ ] For this project: **Root Motion OFF** (code-driven via `CharacterController`)
- [ ] Animation is purely visual; `PlayerMotor.cs` handles all position changes
- [ ] Uncheck `Apply Root Motion` on the Animator component

---

## Part B — The SFS Bug (Already Fixed)

**`PlayerAnimatorDriver.cs` was corrupted** — the first ~30 lines contained duplicate `using` statements and nested class definitions. This caused a compile error, meaning **nothing in the project compiled**, meaning no scripts ran, meaning "nothing happens."

**Status:** Fixed. The file now compiles cleanly.

---

## Part C — Animation Architecture Overview

```
┌──────────────────────────────────────────────────────────┐
│  INPUT (every frame)                                      │
│  PlayerMotor / PlayerController                           │
│    ├── movement → SetLocomotion(speed, grounded, yVel)    │
│    └── actions  → PlayJump(), PlayDash()                  │
│                                                           │
│  TRANSLATION VERBS (on interaction)                       │
│  TranslationVerbBridge                                    │
│    ├── BeginRead(key) → PlayReadDefault()                 │
│    ├── BeginRewriteCushion(key) → PlayRewriteCushion()    │
│    └── BeginRewriteGuard(key)  → PlayRewriteGuard()      │
│                                                           │
│  COMBAT VERBS (on input during encounters)                │
│  TranslationVerbBridge                                    │
│    ├── UsePulse()     → PlayPulse()                       │
│    ├── UseThreadLash()→ PlayThreadLash()                  │
│    └── etc.                                               │
│                                                           │
│         ▼                                                 │
│  CharacterAnimationDriver                                 │
│    Uses AnimParams hashes                                 │
│    Sets Animator parameters                               │
│         ▼                                                 │
│  Unity Animator Controller                                │
│    States, transitions, blend trees                       │
│         ▼                                                 │
│  Visual output                                            │
│    Mesh deformation, transform changes                    │
└──────────────────────────────────────────────────────────┘
```

**Rule:** Only `CharacterAnimationDriver` touches the Animator directly.
Everything else calls the driver's methods.

---

## Part D — Animator Parameter Contract

Create these parameters in your Animator Controller (or run the setup wizard):

### Locomotion (continuous)
| Parameter | Type | Fed By | Notes |
|---|---|---|---|
| `Speed` | Float | `SetLocomotion()` every frame | 0 = idle, 1 = full run |
| `Grounded` | Bool | `SetLocomotion()` every frame | |
| `VerticalVel` | Float | `SetLocomotion()` every frame | + = rising, - = falling |
| `Jump` | Trigger | `PlayJump()` once | Auto-reset by trigger |
| `Land` | Trigger | Auto on grounded transition | |
| `Dash` | Trigger | `PlayDash()` once | |
| `Grapple` | Trigger | `PlayGrapple()` once | |
| `Glide` | Bool | `SetGlide(true/false)` | Sustained state |
| `WallRun` | Bool | `SetWallRun(true/false)` | Sustained state |

### Translation Verbs (one-shot)
| Parameter | Type | Fed By | Notes |
|---|---|---|---|
| `ReadDefault` | Trigger | `PlayReadDefault()` | Subtle pause + focus gesture |
| `RewriteCushion` | Trigger | `PlayRewriteCushion()` | Open gesture + glyph |
| `RewriteGuard` | Trigger | `PlayRewriteGuard()` | Pinning gesture + structure |

### Windprint Costs (body-truth reactions)
| Parameter | Type | Fed By | Notes |
|---|---|---|---|
| `EntropyBleed` | Trigger | `PlayEntropyBleed()` | Micro-stagger, hand tremor |
| `RouteLock` | Trigger | `PlayRouteLock()` | Rigid stance, narrow scanning |

### Symbolic Combat Verbs
| Parameter | Type | Fed By | Notes |
|---|---|---|---|
| `Pulse` | Trigger | `PlayPulse()` | Area clear, cycle reset |
| `ThreadLash` | Trigger | `PlayThreadLash()` | Interrupt, arc motion |
| `RadiantHold` | Bool | `SetRadiantHold(true/false)` | Sustained shield pose |
| `EdgeClaim` | Trigger | `PlayEdgeClaim()` | Sharp pin gesture |
| `Retune` | Trigger | `PlayRetune()` | Cleaning wave gesture |

### Sensory / Emotional
| Parameter | Type | Fed By | Notes |
|---|---|---|---|
| `Overload` | Float | `SetOverload(0-1)` | Fidget, stim, breathing |
| `Calm` | Float | `SetCalm(0-1)` | Relaxed idle, slow micro-motion |
| `IdleIntensity` | Float | Story beat system | Idle animation liveliness |
| `EmotionTone` | Int | `SetEmotionTone()` | 0=Gentle, 1=Hopeful, 2=Melancholic, 3=Grounded |

### Context
| Parameter | Type | Fed By | Notes |
|---|---|---|---|
| `InRestZone` | Bool | `SetInRestZone()` | Reduces idle intensity |
| `WithCompanion` | Bool | `SetWithCompanion()` | Adjusts body language |

---

## Part E — Wiring Checklist

### Player GameObject Setup
- [ ] Has `CharacterController` component
- [ ] Has `Animator` component with a Controller assigned
- [ ] `Apply Root Motion` is **unchecked**
- [ ] Has `PlayerMotor.cs` (or `PlayerController.cs`) attached
- [ ] Has `CharacterAnimationDriver.cs` attached
- [ ] Has `TranslationVerbBridge.cs` attached
- [ ] `PlayerMotor.animDriver` → drag `CharacterAnimationDriver`
- [ ] `CharacterAnimationDriver.animator` → drag `Animator`
- [ ] `TranslationVerbBridge.animDriver` → drag `CharacterAnimationDriver`

### Animator Controller Setup
- [ ] All parameters from the contract table above are created
- [ ] Parameter types match exactly (Float/Bool/Trigger/Int)
- [ ] Default state = Idle
- [ ] Idle → Locomotion blend tree (driven by `Speed`)
- [ ] Any State → Jump (condition: `Jump` trigger)
- [ ] Jump → Fall (condition: `VerticalVel < 0`)
- [ ] Fall → Land (condition: `Land` trigger)
- [ ] Any State → ReadDefault (condition: `ReadDefault` trigger)
- [ ] Any State → RewriteCushion / RewriteGuard (conditions: respective triggers)
- [ ] Layer 1 (upper body): verb animations with Avatar Mask (spine + arms + head)

### Animation Clips
- [ ] ReadDefault clip has Animation Event calling `OnReadReveal()` at gesture focus frame
- [ ] RewriteCushion/Guard clips have Animation Event calling `OnRewriteCommit()` at gesture land frame
- [ ] EntropyBleed clip: micro-stagger, gaze flicker, hand tremor (~0.5s)
- [ ] RouteLock clip: rigid stance snap, narrow scanning (~0.5s)

### Accessibility Integration
- [ ] `Overload` / `Calm` parameters drive Blend Trees for animation intensity
- [ ] When `visual_clutter` default is rewritten → reduce camera bob, idle fidget, snap turns
- [ ] Presets (Gentle Current, Still Air) map to `Overload` / `Calm` values
- [ ] `reducedMotionScale` field on `CharacterAnimationDriver` controls max animation amplitude

---

## Part F — File Map

```
Assets/_SFS/Scripts/
├── Animation/
│   ├── Core/
│   │   ├── AnimParams.cs              ← NEW: Hash constants (prevents name bugs)
│   │   ├── AnimationEvents.cs         ← UPDATED: +ReadDefaultRevealed, +RewriteDefaultCommitted
│   │   └── AnimationStateMachine.cs   ← Existing: custom state machine
│   ├── Player/
│   │   ├── CharacterAnimationDriver.cs ← NEW: Mecanim driver (the one to use)
│   │   ├── TranslationVerbBridge.cs    ← NEW: Read/Rewrite → animation → world sync
│   │   └── TranslatorAnimationController.cs ← Existing: procedural fallback
│   └── ...
├── Player/
│   ├── PlayerMotor.cs                 ← NEW: Clean movement + animation wiring
│   ├── PlayerController.cs            ← Existing: original controller
│   ├── PlayerAnimatorDriver.cs        ← FIXED: was corrupted (duplicate usings)
│   └── SimpleProceduralAnimator.cs    ← Existing: procedural fallback
└── Core/
    ├── GameEvents.cs                  ← Existing
    └── SettingsManager.cs             ← Existing
```

---

## Part G — Quick-Start Decision

**Choose ONE path and commit to it:**

### Path A: Mecanim (clip-based animation)
Use when: You have animation clips (Mixamo, custom, store assets)
- Use `CharacterAnimationDriver.cs` + `TranslationVerbBridge.cs`
- Build Animator Controller with states/transitions
- Wire clips to parameters

### Path B: Procedural (code-driven animation)
Use when: No clips yet, want instant visual feedback
- Use `TranslatorAnimationController.cs` (already 1255 lines of procedural animation)
- Or `SimpleProceduralAnimator.cs` for minimal version
- No Animator Controller needed

### Path C: Hybrid (recommended for development)
Use when: Starting development, clips will come later
- `PlayerAnimatorDriver.cs` automatically detects if a controller is assigned
- If yes → Mecanim. If no → falls back to `SimpleProceduralAnimator`
- Add clips later without changing any game logic

---

## Questions to Answer for Precise Tailoring

If you want scripts tailored exactly to your setup, answer these:

1. **Unity version** (e.g., 2022.3 LTS?)
2. **Animation rig type**: Humanoid or Generic?
3. **What exactly is "nothing happens"**:
   - Animator parameters never change?
   - Parameters change but model doesn't move?
   - State changes but still no visible movement?
4. **Animator parameters** — screenshot or list from Animator window
5. **Animator graph** — screenshot of states + transitions
6. **Player setup**: Animator on root or child? CharacterController or Rigidbody?

If you can only provide one thing: **screenshot of the Animator window in Play Mode while pressing inputs.** That tells us immediately whether it's wiring, import, or controller logic.
