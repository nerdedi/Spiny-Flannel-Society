# Spiny Flannel Society - Technical Architecture

## System Architecture

### Core Modules

#### game_config.py
Configuration and constants for the entire game system.

**Key Components:**
- `NarrativeStates` - Tracks the current state of the Society
- `DriftManifestations` - Types of corruption from The Drift
- `TranslatorAbilities` - Player capabilities
- Game physics constants (gravity, wind, movement speeds)
- Visual palettes that shift with narrative state
- Wind patterns that reflect world corruption

#### game_entities.py
Core game objects and their behaviors.

**Key Classes:**
- `Vector3` - 3D mathematics for positions and movement
- `HiddenAssumption` - Constraints that can be revealed and rewritten
- `Translator` - Player character with translation abilities
- `ContradictorySpace` - Areas affected by The Drift
- `CorruptedSignal` - Communications degraded by standard defaults
- `PenalizedPathway` - Routes hostile to diversity
- `GameWorld` - Overall game state and progression

#### platformer_mechanics.py
3D platformer movement and interaction systems.

**Key Classes:**
- `PlatformerController` - Movement, jumping, wall-running
- `EnvironmentInteraction` - Scanning, rewriting, decoding
- `HybridGameplay` - Main game loop combining all systems

## Game Loop

```
1. Update player state
   - Regenerate translation energy
   - Update ability cooldowns

2. Process input
   - Movement (WASD/arrows)
   - Jump/wall-run
   - Translation actions

3. Apply physics
   - Gravity
   - Wind forces (unique to suspended settlement)
   - Collision detection

4. Update world state
   - Check resolved assumptions
   - Update Drift intensity
   - Modify environmental properties

5. Check win condition
   - All systems restored?
   - Axiom revived?
```

## Data Flow

```
Player Action
    ↓
PlatformerController/EnvironmentInteraction
    ↓
Translator abilities check
    ↓
Entity state modification
    ↓
World state update
    ↓
Drift intensity recalculation
    ↓
Visual/audio feedback
```

## Progression System

### Ability Gates
1. **Read Assumptions** (Starting ability)
   - Scan environments for hidden constraints
   - Reveal system biases

2. **Rewrite Environment** (Early game)
   - Remove constraints from assumptions
   - Costs translation energy

3. **Decode Signals** (Mid game)
   - Restore corrupted communications
   - Reveal story elements

4. **Create Pathways** (Late game)
   - Establish alternative routes
   - Bypass penalization systems

### System Restoration
- 12 major systems in the Society
- Each system has multiple assumptions to rewrite
- Progress tracked: 0/12 → 12/12
- Drift intensity scales inversely with progress

## Wind Physics

The suspended settlement's unique mechanic:

```python
wind_force = base_force + variance(drift_intensity)
player_velocity += wind_force * delta_time
```

Wind patterns change based on world state:
- **Drifting**: High variance, unpredictable
- **Restored**: Low variance, supportive
- **Stable**: Minimal variance, consistent

## Color Palette System

Visual representation of narrative state:

```python
if drift_intensity > 0.8:
    palette = "standard_defaults"  # Gray, lifeless
elif drift_intensity > 0.2:
    palette = "transitional"       # Warming colors
else:
    palette = "axiom_active"       # Vibrant, diverse
```

## Energy System

Translation energy powers the Translator's abilities:
- Maximum: 100 units
- Regeneration: 5 units/second
- Rewrite cost: 10 units
- Cooldown: 2 seconds between rewrites

## Extensibility

The architecture supports:
- New assumption types
- Additional Drift manifestations
- More complex space contradictions
- Expanded signal corruption patterns
- Multi-stage pathway systems
- Co-op translation (multiple Translators)

## Performance Considerations

- Entity pooling for assumptions/signals
- Spatial partitioning for scanning
- LOD for distant contradictory spaces
- Wind calculations optimized for suspended physics
