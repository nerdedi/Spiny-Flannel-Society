# Spiny Flannel Society

> **A hybrid 3D platformer about translation, systems, and the power of designing for diversity.**

![Indie 3D Platformer](https://img.shields.io/badge/Genre-3D%20Platformer-blue)
![Neuroaffirming](https://img.shields.io/badge/Design-Neuroaffirming-green)
![Non-Violent](https://img.shields.io/badge/Combat-Non--Violent%20Symbolic-purple)

---

## Overview

**Spiny Flannel Society** is a living settlement suspended in a permanent wind current above an Australian coastline. When the Society adopted rigid "standard defaults" (one pace, one social script, one correct way), its operating principle — the **Spiny Flannel Axiom** — withdrew.

The result is **"The Drift"**: spaces contradict themselves, signals corrupt, and pathways penalise difference.

You are a **Translator** — someone who can read hidden assumptions in systems and rewrite environments to restore the Axiom.

### Core Promise

> A world that becomes coherent not by fixing individuals, but by redesigning society so many kinds of minds can thrive.

---

## Key Features

### 🌬️ Flow + Precision Platforming
- **Triple hop** (short → long → float)
- **Air dash** with double-dash upgrade
- **Wall run/kick** for vertical traversal
- **Grapple thread** to Botanical Nodes
- **Glide/wind surf** across canopy lines
- **Pulse slam** for mechanism interaction

### 🛡️ Windprint Rig System
Your signature tool with two complementary modes:

| Mode | Effect |
|------|--------|
| **Cushion** | Widens timing, spawns safe pockets, reduces clutter, slows hazards |
| **Guard** | Pins rhythms, stabilises jitter, creates consent gates, claims edges |

### ⚡ Non-Violent Symbolic Combat
Combat is pattern intervention, not violence:

- **Pulse** — Clears/resets cycles
- **Thread Lash** — Interrupts loops
- **Radiant Hold** — Shields, creates safe footholds
- **Edge Claim** — Pins a rhythm
- **Re-tune** — Cleans signal corruption

### 📚 Optional Electives (Stealth Learning)
Challenge rooms embedding logic, literacy, numeracy, language, and digital literacy as physical puzzles. **Never gates story** — rewards lore, cosmetics, and shortcuts.

### ♿ Accessibility as World Law
Universal design is canon, not a settings menu:
- Sensory sliders and clutter reduction
- No forced timers or hard fail states
- Safe routes are main routes
- Multiple communication modes with equal outcomes

---

## The World

### Districts

| District | Description |
|----------|-------------|
| **Windgap Academy** | Learning commons and "translation engine" |
| **The Veil Market** | Trading lane where signage drifts first |
| **Sandstone Quarter** | Foundation terraces with charter stones |
| **The Umbel Gardens** | Suspended neighbourhoods showing community as structure |
| **The Smoke Margin** | Repair yards where obsolete rules are decommissioned |
| **The Reliquary Edge** | Vault preserving rare design laws |

### The Drift Manifestations

- **Distortions** — Glitched rules made physical
- **Echo Forms** — Social scripts given motion
- **Noise Beasts** — Sensory overload as weather

---

## Characters

| Character | Role |
|-----------|------|
| **The Translator** | Player; perceives Windprints and rewrites environments |
| **DAZIE Vine** | Mentor; systems ethicist at Windgap Academy |
| **June Corrow** | Sensory architect; designed quiet infrastructure |
| **Winton** | Civic OS; the Society's operating interface made audible |

---

## Getting Started

### Prerequisites

```bash
python3 >= 3.8
```

### Run the Demo

```bash
python3 demo.py
```

The demo showcases:
- Core narrative elements
- Windprint Rig mechanics (Cushion/Guard modes)
- Combat verbs (Pulse, Thread Lash, Radiant Hold, Edge Claim, Re-tune)
- Signal decoding and pathway creation
- 3D platformer physics with wind-based mechanics

### Run Tests

```bash
python3 test_game.py
```

---

## Project Structure

```
Spiny-Flannel-Society/
├── GAME_DESIGN.md           # Full game design document
├── ARCHITECTURE.md          # Technical architecture
├── IMPLEMENTATION_SUMMARY.md # Implementation notes
├── README.md                # This file
│
├── game_config.py           # Game constants and configuration
├── game_entities.py         # Core entities (Translator, spaces, signals)
├── platformer_mechanics.py  # 3D platformer movement systems
├── windprint_rig.py         # Windprint Rig (Cushion/Guard) system
├── combat_system.py         # Non-violent symbolic combat
├── chapters.py              # 12-chapter narrative progression
├── characters.py            # NPC characters and dialogue
│
├── demo.py                  # Interactive demonstration
├── test_game.py             # Unit tests
└── requirements.txt         # Python dependencies
```

---

## 12-Chapter Narrative

| # | Chapter | Location | Theme |
|---|---------|----------|-------|
| 1 | Bract Theory | Windgap Academy | Supports without proof |
| 2 | Felt Memory | Archive Walk | Overload as information |
| 3 | Rayless Form | Social Hall | Equal expression modes |
| 4 | Umbel Logic | Umbel Gardens | Community as architecture |
| 5 | Tickshape Rule | Skybridges | Consent gates |
| 6 | Smoke Signal | Smoke Margin | Difference as adaptation |
| 7 | Afterrain Bloom | Rain Cliffs | Safe path = main path |
| 8 | Sandstone Drift | Sandstone Quarter | Multiple valid routes |
| 9 | Eucalypt Veil | Eucalypt Canopy | Engineered calm |
| 10 | Clonal Echo | Model Society Sim | Diversity = resilience |
| 11 | Edge Reliquary | Reliquary Edge | Principle modules |
| 12 | Refound Light | Windcore Tower | Compose new defaults |

---

## Victory Condition

Restore the **Spiny Flannel Axiom** by composing new societal defaults in the Windcore:
- Supports by default
- Translation culture
- Sensory baselines
- Consent gates
- Plural success metrics

End **The Drift** and stabilise the Society into **plural coherence**.

---

## Technical Details

Built with Python, demonstrating:
- 3D vector mathematics for platformer physics
- Windprint Rig dual-mode system
- Non-violent symbolic combat verbs
- State management for 12-chapter narrative
- Environmental interaction systems
- Elective-based stealth learning mechanics

---

## License

This project is licensed under the MIT License.

---

*"You didn't fix us. You reminded us how to care."* — DAZIE Vine
