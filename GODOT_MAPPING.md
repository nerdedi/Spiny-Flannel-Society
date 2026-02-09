# Spiny Flannel Society — Godot 4 Mapping Guide

How the Python prototype maps to Godot 4 concepts.
Each system has a clear 1:1 translation path.

---

## Core Architecture

| Python Prototype | Godot 4 Concept | Notes |
|---|---|---|
| `DefaultsRegistry` | **Autoload singleton** (`DefaultsRegistry.gd`) | Autoloads persist across scenes. Every node can call `DefaultsRegistry.get("timing_window")`. |
| `Default` dataclass | **`Resource` subclass** (`DefaultResource.tres`) | Godot Resources are serialisable, inspector-visible, and can be saved/loaded. Each default becomes a `.tres` file. |
| `DefaultCategory` enum | **`enum DefaultCategory`** in GDScript | Enums work identically. |
| `EventBus` | **Godot signals** (global signal bus autoload) | Create `SignalBus.gd` autoload with typed signals. Nodes connect/disconnect freely. |
| `GameState` | **Autoload singleton** (`GameState.gd`) | Same pattern as DefaultsRegistry. |

### Example: DefaultsRegistry.gd

```gdscript
extends Node

# Autoload: Project Settings → Autoload → DefaultsRegistry

var _defaults: Dictionary = {}

func _ready():
    _register_all()

func get_value(key: String) -> float:
    if _defaults.has(key):
        return _defaults[key].current_value
    return 0.0

func read_default(key: String) -> String:
    var d = _defaults.get(key)
    if d:
        d.is_read = true
        return d.get_description()
    return ""

func rewrite_default(key: String) -> bool:
    var d = _defaults.get(key)
    if d and d.is_read:
        d.is_rewritten = true
        d.current_value = d.rewritten_value
        SignalBus.default_rewritten.emit(key, d.current_value)
        return true
    return false
```

---

## Systems Mapping

### Windprint Rig

| Python | Godot 4 |
|---|---|
| `WindprintRigSystem` class | **Node** attached to Player scene |
| `WindprintMode` enum | `enum WindprintMode { CUSHION, GUARD }` |
| `activate_cushion()` / `activate_guard()` | Methods on the node; emit signals |
| `get_timing_multiplier()` | Reads `DefaultsRegistry.get_value("timing_window")` × mode multiplier |
| `CUSHION_COSTS` / `GUARD_COSTS` | `@export` Resource arrays on the WindprintRig node |

### Movement

| Python | Godot 4 |
|---|---|
| `MovementRules` static class | **`CharacterBody3D` script** methods |
| `TraversalState` dataclass | Instance variables on CharacterBody3D |
| `can_jump()` | Inline check in `_physics_process()` |
| `coyote_time` default | `DefaultsRegistry.get_value("coyote_time")` in jump logic |
| `apply_cushion_modifiers()` | Called when `WindprintRig.is_cushion_active()` |

### Example: Jump with Defaults

```gdscript
# In CharacterBody3D script
func _physics_process(delta):
    var coyote = DefaultsRegistry.get_value("coyote_time")
    var jump_buffer = DefaultsRegistry.get_value("jump_buffer")

    if not is_on_floor():
        coyote_timer -= delta

    if Input.is_action_just_pressed("jump"):
        jump_buffer_timer = jump_buffer

    if (is_on_floor() or coyote_timer > 0) and jump_buffer_timer > 0:
        velocity.y = JUMP_FORCE
        if WindprintRig.is_cushion_active():
            velocity.y *= 1.3  # Cushion widens jump
```

### Combat (Pattern Intervention)

| Python | Godot 4 |
|---|---|
| `AntagPattern` class | **`Node3D` with script** per pattern type |
| `PatternType` enum | `enum PatternType` in GDScript |
| `Verb` enum | `enum Verb` — mapped to input actions |
| `receive_verb()` | Method on pattern node; plays animation + emits signal |
| `Encounter` class | **Scene** that instantiates pattern nodes |
| Timing window affect | Vulnerability phase duration = `DefaultsRegistry.get_value("timing_window") * 2` |

### Signals (Read/Rewrite)

| Python | Godot 4 |
|---|---|
| `TranslationVerbs` class | **Autoload** or Player child node |
| `read_default()` | Calls `DefaultsRegistry.read_default()` + emits `SignalBus.default_read` |
| `rewrite_default()` | Calls `DefaultsRegistry.rewrite_default()` + emits `SignalBus.default_rewritten` |
| `scan_area()` | Raycasts or Area3D overlap checks for `Readable` group nodes |

---

## World Systems

### Districts

| Python | Godot 4 |
|---|---|
| `District` dataclass | **Scene per district** with a `DistrictData` Resource attached |
| `DISTRICTS` dict | `DistrictData.tres` Resource files loaded by autoload |
| `drift_level` | Property on DistrictData; affects shader parameters |
| `reduce_drift()` | Method on DistrictData; triggers signal for visual update |
| `defaults_present` | Array of string keys; used by `TranslationVerbs.scan_area()` |

### Drift Visualisation

```gdscript
# Shader uniform driven by drift_level
# Attached to district root MeshInstance3D or Environment
@export var district_data: DistrictData

func _process(_delta):
    var drift = district_data.drift_level
    material.set_shader_parameter("drift_intensity", drift)
    # Drift affects: colour desaturation, particle noise, wind turbulence
```

### Distortions

| Python | Godot 4 |
|---|---|
| `Distortion` class | **Scene** spawned by district based on drift_level |
| `DistortionType` enum | `enum DistortionType` |
| `linked_default` | String key → when that default is rewritten, distortion resolves |

---

## Accessibility

### Presets

| Python | Godot 4 |
|---|---|
| `Preset` dataclass | **`Resource` subclass** (`PresetResource.tres`) |
| `PRESETS` dict | Preset resources loaded at startup |
| `overrides` dict | Dictionary applied to DefaultsRegistry on selection |
| Applying a preset | Loop over `overrides`, call `DefaultsRegistry.set_value(key, value)` |

### Sensory Rules

| Python | Godot 4 |
|---|---|
| `SensoryProfile` | Computed from DefaultsRegistry values |
| `visual_density` | Controls particle system emission rate, environment fog density |
| `audio_layers` | Controls AudioBus volumes (duck non-primary layers) |
| `screen_shake` | Multiplier on camera shake amplitude |
| `motion_intensity` | Controls AnimationPlayer speed scales, particle speeds |

---

## Signal Bus Pattern

```gdscript
# SignalBus.gd — Autoload
extends Node

signal default_read(key: String)
signal default_rewritten(key: String, new_value: float)
signal verb_used(verb_name: String)
signal chapter_advanced(chapter_number: int)
signal preset_applied(preset_id: String)
signal distortion_resolved(distortion_id: String)
signal encounter_started(encounter_id: String)
signal encounter_resolved(encounter_id: String)
signal civic_rule_restored(rule_id: String)
```

Any node connects to signals it cares about:

```gdscript
func _ready():
    SignalBus.default_rewritten.connect(_on_default_rewritten)

func _on_default_rewritten(key: String, new_value: float):
    if key == "timing_window":
        _update_gear_speed(new_value)
```

---

## Scene Tree Recommendation

```
Main (Node)
├── GameState (Autoload)
├── DefaultsRegistry (Autoload)
├── SignalBus (Autoload)
├── WindprintRig (Autoload or Player child)
├── World (Node3D)
│   ├── WindgapAcademy (scene)
│   │   ├── DistrictData (Resource)
│   │   ├── Platforms (Node3D)
│   │   ├── Distortions (Node3D)
│   │   ├── ConsensusGates (Node3D)
│   │   └── NPCs (Node3D)
│   ├── VeilMarket (scene)
│   └── ... other districts
├── Player (CharacterBody3D)
│   ├── Camera3D
│   ├── WindprintRig (Node)
│   ├── TranslationVerbs (Node)
│   └── CombatSystem (Node)
├── UI (CanvasLayer)
│   ├── DefaultReadOverlay
│   ├── PresetComposer
│   └── HUD
└── NarrativeSystem (Node)
    ├── ChapterManager
    ├── DialogueSystem
    └── CharacterManager
```

---

## Migration Priority

1. **DefaultsRegistry + SignalBus** — the spine. Build and test these first.
2. **Player CharacterBody3D** — movement with defaults-driven coyote/buffer.
3. **One district (Windgap Academy)** — geometry, platforms, one readable default.
4. **Read/Rewrite interaction** — the first-minute sequence.
5. **Windprint Rig** — Cushion/Guard modes with cost feedback.
6. **Combat patterns** — one encounter type to prove the verb system.
7. **Presets + sensory rules** — shader/audio integration.
8. **Remaining districts, chapters, NPCs** — content expansion.

---

## Key Godot Features to Leverage

- **`@export`**: Expose default values in Inspector for designer tuning.
- **`Resource`**: Saveable/loadable data — perfect for defaults, presets, districts.
- **`AnimationPlayer`**: Drive Read/Rewrite visual transitions.
- **`ShaderMaterial`**: Drift visualisation via uniform parameters.
- **`Area3D`**: Detect when player enters a "readable" zone.
- **`AudioBus`**: Layer management for sensory profile.
- **`Tween`**: Smooth transitions when defaults are rewritten.
- **`InputMap`**: Map verbs to configurable inputs.
