# Spiny Flannel Society - Technical Architecture

## Overview

This document describes the technical architecture for implementing Spiny Flannel Society, a hybrid 3D platformer with non-violent symbolic combat and universal design principles.

---

## System Architecture

### Module Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                        GAME LOOP                                │
│                    (platformer_mechanics.py)                    │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────────────┐  │
│  │   PLAYER     │  │   WORLD      │  │     NARRATIVE        │  │
│  │              │  │              │  │                      │  │
│  │ • Translator │  │ • Districts  │  │ • Chapters           │  │
│  │ • Windprint  │  │ • Drift      │  │ • Characters         │  │
│  │   Rig        │  │ • Spaces     │  │ • Dialogue           │  │
│  └──────────────┘  └──────────────┘  └──────────────────────┘  │
│         │                 │                    │                │
│         └────────────┬────┴────────────────────┘                │
│                      │                                          │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │                    COMBAT SYSTEM                          │  │
│  │  Pulse | Thread Lash | Radiant Hold | Edge Claim | Re-tune│  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## Core Modules

### game_config.py

Configuration and constants for the entire game system.

**Key Components:**
- `NarrativeStates` — Tracks the current state of the Society
- `DriftManifestations` — Types of corruption (Distortions, Echo Forms, Noise Beasts)
- `TranslatorAbilities` — Player capabilities (read, rewrite, decode, create)
- `CombatVerbs` — Non-violent combat actions
- `WindprintModes` — Cushion and Guard mode constants
- `Districts` — World locations
- `Chapters` — 12-chapter progression data
- Physics constants, visual palettes, wind patterns

### game_entities.py

Core game objects and their behaviours.

**Key Classes:**
- `Vector3` — 3D mathematics for positions and movement
- `HiddenAssumption` — Constraints that can be revealed and rewritten
- `Translator` — Player character with translation abilities
- `ContradictorySpace` — Areas affected by The Drift
- `CorruptedSignal` — Communications degraded by standard defaults
- `PenalizedPathway` — Routes hostile to diversity
- `District` — World regions with unique properties
- `GameWorld` — Overall game state and progression

### windprint_rig.py

The Translator's signature dual-mode tool.

**Classes:**
- `WindprintRig` — Main rig controller
- `CushionMode` — Softness and accessibility effects
- `GuardMode` — Protection and boundary effects

**Key Mechanics:**
```python
# Cushion Mode effects
- timing_window_multiplier: 1.5  # Wider timing windows
- safe_pocket_spawn_rate: 0.3
- clutter_reduction: 0.6
- hazard_slowdown: 0.5

# Guard Mode effects
- rhythm_pin_strength: 0.8
- jitter_stabilisation: 0.9
- consent_gate_active: True
- edge_claim_range: 3.0
```

### combat_system.py

Non-violent symbolic combat through pattern intervention.

**Combat Verbs:**

| Verb | Class | Function |
|------|-------|----------|
| Pulse | `PulseVerb` | Clears/resets cycles; breaks Distortion loops |
| Thread Lash | `ThreadLashVerb` | Interrupts Echo Form patterns |
| Radiant Hold | `RadiantHoldVerb` | Creates shields and safe footholds |
| Edge Claim | `EdgeClaimVerb` | Pins rhythms; stabilises platform timing |
| Re-tune | `RetuneVerb` | Cleans signal corruption; calms Noise Beasts |

**Antagonistic Patterns:**
- `EchoForm` — Coercive social scripts given motion
- `Distortion` — Broken rules manifested physically
- `NoiseBeast` — Sensory overload as weather

### chapters.py

12-chapter narrative progression system.

**Structure:**
```python
class Chapter:
    id: int
    name: str
    location: str  # District
    theme: str
    civic_rule: str  # Rule restored
    primary_mechanic: str
    npcs: List[str]
    distortions: List[str]
    electives: List[Elective]
```

**Chapter Flow:**
```
Chapter Start
    │
    ├─► Cinematic/Dialogue
    │
    ├─► Traversal Section (platforming)
    │
    ├─► Combat Encounter (pattern intervention)
    │
    ├─► Optional Elective(s)
    │
    ├─► Design Terminal (rewrite rule)
    │
    └─► Chapter Complete → Drift Intensity Reduced
```

### characters.py

NPC characters with dialogue and arc progression.

**Key Characters:**
- `DAZIEVine` — Mentor, systems ethicist
- `JuneCorrow` — Sensory architect
- `Winton` — Civic OS, system-ghost

**Dialogue System:**
```python
class DialogueNode:
    speaker: str
    text: str
    communication_mode: str  # "direct", "scripted", "icons", "minimal"
    responses: List[DialogueResponse]
```

### platformer_mechanics.py

3D platformer movement and interaction systems.

**Movement Actions:**

| Action | Description | Input |
|--------|-------------|-------|
| Move | Directional movement | WASD/Stick |
| Triple Hop | Short → Long → Float | Jump x3 |
| Air Dash | Horizontal burst (upgrades to double) | Dash in air |
| Wall Run | Vertical wall traversal | Move + Wall |
| Wall Kick | Launch from wall | Jump on Wall |
| Grapple Thread | Connect to Botanical Nodes | Grapple button |
| Glide | Wind surf across canopy | Hold Jump |
| Pulse Slam | Ground interaction | Down + Attack |

**Key Classes:**
- `PlatformerController` — Movement, jumping, wall-running
- `EnvironmentInteraction` — Scanning, rewriting, decoding
- `HybridGameplay` — Main game loop combining all systems

---

## Data Flow

```
Player Input
    │
    ▼
┌───────────────────────┐
│ PlatformerController  │
│ • Movement processing │
│ • Jump/dash/wall run  │
└───────────┬───────────┘
            │
            ▼
┌───────────────────────┐
│    WindprintRig       │
│ • Mode application    │
│ • Timing modification │
└───────────┬───────────┘
            │
            ▼
┌───────────────────────┐
│    CombatSystem       │  ◄── If combat encounter
│ • Verb execution      │
│ • Pattern resolution  │
└───────────┬───────────┘
            │
            ▼
┌───────────────────────┐
│     GameWorld         │
│ • Drift recalculation │
│ • State updates       │
└───────────┬───────────┘
            │
            ▼
┌───────────────────────┐
│   ChapterProgression  │
│ • Rule restoration    │
│ • Narrative advance   │
└───────────────────────┘
```

---

## Game Loop

```python
def game_loop(delta_time):
    # 1. Update player state
    translator.update(delta_time)
    windprint_rig.update(delta_time)

    # 2. Process input
    handle_movement_input()
    handle_ability_input()
    handle_combat_input()

    # 3. Apply Windprint effects
    if windprint_rig.mode == CushionMode:
        apply_cushion_effects()
    elif windprint_rig.mode == GuardMode:
        apply_guard_effects()

    # 4. Apply physics
    apply_gravity(wind_modified=True)
    handle_collisions()

    # 5. Update world state
    world.update_drift_intensity()
    update_contradictory_spaces()
    update_antagonistic_patterns()

    # 6. Check progression
    if chapter.is_complete():
        advance_to_next_chapter()

    # 7. Check victory
    if world.axiom_restored():
        trigger_finale()
```

---

## Windprint Rig System

The player's signature mechanic bridging accessibility and gameplay.

### Mode Switching

```python
class WindprintRig:
    def __init__(self):
        self.current_mode = None
        self.cushion = CushionMode()
        self.guard = GuardMode()
        self.energy = 100
        self.max_energy = 100

    def activate_cushion(self):
        """Softness mode - widens timing, reduces clutter"""
        self.current_mode = self.cushion

    def activate_guard(self):
        """Protection mode - pins rhythms, creates boundaries"""
        self.current_mode = self.guard

    def get_timing_multiplier(self) -> float:
        if self.current_mode == self.cushion:
            return 1.5  # 50% wider timing windows
        return 1.0
```

### Cushion Mode Effects

| Effect | Value | Description |
|--------|-------|-------------|
| Timing Windows | ×1.5 | Platform timing more forgiving |
| Safe Pockets | 30% spawn | Rest areas appear during hazards |
| Clutter Reduction | 60% | Visual noise reduced |
| Hazard Slowdown | 50% | Moving hazards slower |

### Guard Mode Effects

| Effect | Value | Description |
|--------|-------|-------------|
| Rhythm Pin | 80% strength | Stabilise erratic platform patterns |
| Jitter Stabilisation | 90% | Reduce environmental shake |
| Consent Gates | Active | Require confirmation before danger |
| Edge Claim | 3.0m range | Pin boundaries in place |

---

## Combat System

### Verb Mechanics

```python
class CombatVerb:
    name: str
    energy_cost: int
    cooldown: float
    effect_radius: float

    def execute(self, target: AntagonisticPattern) -> bool:
        """Execute verb against target pattern"""
        pass

class PulseVerb(CombatVerb):
    """Clears/resets cycles"""
    name = "Pulse"
    energy_cost = 10
    cooldown = 1.0
    effect_radius = 5.0

    def execute(self, target):
        if isinstance(target, Distortion):
            target.reset_cycle()
            return True
        return False
```

### Antagonistic Pattern Resolution

| Pattern | Primary Verb | Secondary | Resolution |
|---------|--------------|-----------|------------|
| Echo Form | Thread Lash | Pulse | Loop interrupted, script dissolves |
| Distortion | Pulse | Edge Claim | Cycle reset, rule corrected |
| Noise Beast | Re-tune | Radiant Hold | Corruption cleaned, calm restored |

---

## Chapter Progression

### Chapter Data Structure

```python
CHAPTERS = {
    1: Chapter(
        id=1,
        name="Bract Theory",
        location="Windgap Academy",
        theme="Supports without proof",
        civic_rule="ACCESS_WITHOUT_PROOF",
        primary_mechanic="cushion_guard_toggle",
        npcs=["DAZIE", "Winton"],
        distortions=["looping_door"],
        electives=[Elective("orientation_decode", "literacy")]
    ),
    # ... chapters 2-12
}
```

### Progression System

```python
def complete_chapter(chapter_id: int):
    chapter = CHAPTERS[chapter_id]

    # Restore civic rule
    world.restore_civic_rule(chapter.civic_rule)

    # Reduce drift
    world.reduce_drift_intensity(1/12)

    # Unlock abilities if applicable
    check_ability_unlocks(chapter_id)

    # Trigger next chapter
    if chapter_id < 12:
        load_chapter(chapter_id + 1)
    else:
        trigger_finale()
```

---

## Elective System (Stealth Learning)

### Structure

```python
class Elective:
    name: str
    subject: str  # "logic", "literacy", "numeracy", "language", "digital"
    puzzle_type: str
    difficulty: int
    rewards: List[str]

    def complete(self, translator):
        grant_rewards(translator, self.rewards)
        # Never gates story progress
```

### Subject Mechanics

| Subject | Mechanic | Example |
|---------|----------|---------|
| Logic | IF/THEN tiles | Conditional platform activation |
| Literacy | Signal decoding | Read signage to reveal routes |
| Numeracy | Ratio calibration | Balance bridge weights |
| Language | Sequence building | Arrange communication symbols |
| Digital | System debugging | Navigate data flow puzzles |

---

## Wind Physics

The suspended settlement's signature mechanic:

```python
def calculate_wind_force(drift_intensity: float) -> Vector3:
    pattern = get_wind_pattern(drift_intensity)

    base = Vector3(WIND_FORCE_BASE, 0, 0)
    variance = pattern["variance"]

    return Vector3(
        base.x + random.uniform(-variance, variance),
        random.uniform(-variance * 0.3, variance * 0.3),
        random.uniform(-variance, variance)
    )

def apply_wind(translator: Translator, wind: Vector3, delta_time: float):
    # Wind affects movement
    translator.velocity += wind * delta_time * 0.5

    # Cushion mode reduces wind impact
    if translator.windprint_rig.mode == CushionMode:
        translator.velocity *= 0.8  # Softer wind effect
```

---

## Accessibility Architecture

Accessibility is implemented as world mechanics, not hidden settings:

```python
class AccessibilitySettings:
    """In-world tuning rather than settings menu"""

    # Sensory
    motion_intensity: float = 1.0
    brightness_level: float = 1.0
    audio_layers: Dict[str, float]  # Per-category volumes

    # Visual
    clutter_reduction: float = 0.0
    subtitle_style: str = "standard"

    # Timing
    no_forced_timers: bool = True
    safe_routes_main: bool = True

    # Communication
    communication_mode: str = "direct"  # direct, scripted, icons, minimal
```

---

## State Management

### World State

```python
class WorldState:
    narrative_state: NarrativeStates
    drift_intensity: float  # 0.0 (restored) to 1.0 (maximum drift)

    current_chapter: int
    completed_chapters: List[int]

    restored_rules: List[str]
    collected_principles: List[str]

    districts: Dict[str, DistrictState]
```

### Save System

```python
class SaveData:
    translator_state: TranslatorState
    world_state: WorldState
    chapter_progress: ChapterProgress
    elective_completions: List[str]
    windprint_recordings: List[WindprintRecord]  # For finale
```

---

## Performance Considerations

- Entity pooling for Distortions/Echo Forms/Noise Beasts
- Spatial partitioning for assumption scanning
- LOD for distant contradictory spaces
- Wind calculations optimised for suspended physics
- Async loading for chapter transitions

---

## Extensibility

The architecture supports:
- New antagonistic pattern types
- Additional combat verbs
- More chapters/districts
- Expanded elective subjects
- Multiplayer translation (future)
- Modding support for custom civic rules
