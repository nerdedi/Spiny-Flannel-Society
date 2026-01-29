# Spiny Flannel Society

A hybrid 3D platformer about translation, systems, and the power of diversity.

## Overview

Spiny Flannel Society is a living settlement suspended in a permanent wind current above an Australian coastline. When the Society adopted rigid "standard defaults" as its operating principle, the Spiny Flannel Axiom withdrew. The result is "The Drift": spaces contradict themselves, signals corrupt, and pathways penalize difference.

You are a **Translator** - someone who can read hidden assumptions in systems and rewrite environments to restore the Axiom.

## Core Concept

### The Spiny Flannel Axiom
The original guiding principle that celebrated diversity, allowing multiple valid paths, adaptive systems, and signals that maintain integrity across interpretation.

### The Standard Defaults
Rigid, inflexible rules that emerged when diversity was rejected - imposing one "correct" way to navigate and approach problems.

### The Drift
The consequence of abandoning the Axiom:
- **Contradictory Spaces**: Rooms and areas that change unpredictably
- **Signal Corruption**: Communications and data that degrade
- **Pathway Penalization**: Routes that become hostile to non-standard approaches

## Gameplay

As a Translator, you can:
- **Read Hidden Assumptions** - Identify underlying biases in systems
- **Rewrite Environments** - Change how spaces interpret and respond
- **Decode Corrupted Signals** - Find meaning in degraded communications
- **Create Alternative Pathways** - Establish routes that honor difference

Combine 3D platformer mechanics (jumping, wall-running, momentum-based movement) with environmental hacking to restore the Society's systems and end The Drift.

## Getting Started

### Run the Demo

```bash
python3 demo.py
```

The demo showcases:
- Core narrative elements
- Translation mechanics (reading assumptions, rewriting environments)
- Signal decoding
- Pathway creation
- 3D platformer physics with wind-based mechanics

### Project Structure

- `GAME_DESIGN.md` - Comprehensive game design document
- `game_config.py` - Game constants and configuration
- `game_entities.py` - Core game entities (Translator, spaces, signals, pathways)
- `platformer_mechanics.py` - 3D platformer movement and interaction systems
- `demo.py` - Interactive demonstration of game mechanics

## Game Systems

### Translator (Player)
- Movement: 5.0 m/s speed, 2.5m jump height, 4.0 m/s wall run
- Translation Energy: Powers environment rewriting
- Abilities: Unlock new translation capabilities as you progress

### The World
- 12 core systems to restore
- 5 districts in the suspended settlement
- Dynamic wind patterns that affect movement
- Drift intensity decreases as systems are restored

### Victory Condition
Restore enough core systems (12/12) to revive the Spiny Flannel Axiom and end The Drift.

## Technical Details

Built with Python, demonstrating:
- 3D vector mathematics for platformer physics
- State management for narrative progression
- Environmental interaction systems
- Ability-gated progression

## License

This project is a game design implementation demonstrating the Spiny Flannel Society concept.
