# Spiny Flannel Society — DEFAULTS.md

> **The Defaults Registry is the thematic heart of the codebase.**
> Every system queries it. When the player rewrites a Default,
> every system that reads it responds automatically.
>
> This turns the game's theme into architecture.

---

## Philosophy

The Society failed because it imposed **Standard Defaults** — rigid assumptions about how all people interact with space, time, communication, and risk. These defaults penalise anyone who doesn't match the assumed norm.

The player's role is to:

1. **Read Defaults** — reveal the hidden assumption.
2. **Rewrite Defaults** — replace it with an inclusive alternative.

When a Default is rewritten, systems across the entire Society shift. Timing windows widen. Alternative routes appear. Consent gates materialise. This isn't a settings menu — it's the gameplay loop.

---

## Default Categories

| Category | What it governs | Example |
|----------|----------------|---------|
| **Timing** | Speed expectations, window widths, grace periods | Assumes 200 ms reactions → rewritten to 500 ms |
| **Sensory** | Visual density, audio layering, motion intensity | Full particle clutter → reduced to 40% |
| **Routing** | Path strictness, alternative routes, visibility | One valid path → multiple valid routes |
| **Social** | Communication norms, expression modes, NPC responses | Only spoken dialogue accepted → all modes equal |
| **Failure** | Penalty severity, retry costs | Full progress reset → gentle repositioning |
| **Consent** | Gate presence, opt-in / opt-out availability | No warning before danger → consent gates present |

---

## Complete Defaults Table

| Key | Label | Rigid (Drift) | Rewritten (Restored) | Category |
|-----|-------|:-------------:|:--------------------:|----------|
| `timing_window` | Timing Window Width | 0.2 (200 ms) | 0.5 (500 ms) | TIMING |
| `platform_rhythm` | Platform Rhythm | 1.0 (full speed) | 0.6 (60%) | TIMING |
| `coyote_time` | Coyote Time | 0.0 (none) | 0.2 (200 ms) | TIMING |
| `jump_buffer` | Jump Buffer Window | 0.0 (none) | 0.15 (150 ms) | TIMING |
| `visual_clutter` | Visual Density | 1.0 (maximum) | 0.4 (breathable) | SENSORY |
| `audio_layering` | Audio Layering | 1.0 (all at once) | 0.5 (ducked) | SENSORY |
| `screen_shake` | Screen Shake | 1.0 (full) | 0.0 (none) | SENSORY |
| `route_strictness` | Route Strictness | 1.0 (one path) | 0.3 (many routes) | ROUTING |
| `safe_route_visibility` | Safe Route Visibility | 0.0 (hidden) | 1.0 (main path) | ROUTING |
| `communication_rigidity` | Communication Mode | 1.0 (one mode) | 0.0 (all equal) | SOCIAL |
| `social_script_penalty` | Social Script Penalty | 1.0 (punished) | 0.0 (accepted) | SOCIAL |
| `failure_penalty` | Failure Penalty | 1.0 (full reset) | 0.1 (gentle) | FAILURE |
| `retry_cost` | Retry Cost | 1.0 (costly) | 0.0 (free) | FAILURE |
| `consent_gates` | Consent Gates | 0.0 (none) | 1.0 (present) | CONSENT |
| `opt_out_available` | Opt-Out Availability | 0.0 (locked in) | 1.0 (can leave) | CONSENT |

---

## How Systems Use Defaults

Any system can query the registry:

```python
from core.defaults_registry import DefaultsRegistry

registry = DefaultsRegistry()

# A platformer module asks: "how wide should timing windows be?"
window = registry.get("timing_window")   # → 0.2 under Drift, 0.5 after rewrite

# A rendering module asks: "how much visual clutter?"
clutter = registry.get("visual_clutter") # → 1.0 under Drift, 0.4 after rewrite
```

When the player Reads and Rewrites a default, the value changes
and every system that queries it automatically responds.

---

## Player-Facing Verbs

### Read Default
- Highlights the hidden assumption in the environment
- Reveals what the default assumes and who it penalises
- Must be done **before** Rewrite is available
- Gives the player a **learning loop** — understanding before action

### Rewrite Default
- Changes the default to its inclusive value
- Can only be used after Reading
- Applies through Cushion (softness) or Guard (protection) intent
- Gives the player a **clear fantasy** — you are reprogramming society

---

## Windprint Rig Trade-offs

Cushion and Guard are not free mode switches. Each has an **ethical cost**:

### Cushion Costs
| Cost | Description | Magnitude |
|------|-------------|:---------:|
| **Entropy Bleed** | Visual noise increases in adjacent, un-cushioned areas | 0.3 |
| **Platform Drift** | Unexplored platforms shift while Cushion stabilises here | 0.2 |

### Guard Costs
| Cost | Description | Magnitude |
|------|-------------|:---------:|
| **Route Lock** | Some alternative routes become inaccessible while Guard pins the current path | 0.4 |
| **Exploration Narrowing** | Peripheral areas are less responsive during Guard | 0.25 |

These trade-offs reinforce the game's ethics: **accommodation requires thought, not just toggling.**

---

## Accessibility Presets

Presets set multiple defaults at once. They are **not difficulty levels** —
every preset leads to the same story outcome and full content access.

| Preset | Name | Focus |
|--------|------|-------|
| `default` | Society Standard | Balanced starting point |
| `gentle` | Gentle Current | Wider timing, less clutter, no shake |
| `focused` | Focused Flow | Minimal noise, strong routing cues |
| `challenge` | Sharp Edge | Tighter timing for precision players |
| `low_motion` | Still Air | Reduced motion, vestibular comfort |

---

## For Engine Developers

The Defaults Registry is **engine-agnostic**. When porting to Godot or Unity:

1. Import `core/defaults_registry.py` as your settings authority
2. Bind each default key to the equivalent engine parameter
3. The Read/Rewrite verbs become in-game interactions that call `registry.read()` and `registry.rewrite()`
4. Windprint costs should modify district-level parameters in the engine's world system

The defaults are the **bridge between design intent and engine reality**.

---

*"You didn't fix us. You reminded us how to care."* — DAZIE Vine
