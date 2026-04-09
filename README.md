# Spiny Flannel Society

> **A hybrid 3D platformer about translation, systems, and the power of designing for diversity.**

![Indie 3D Platformer](https://img.shields.io/badge/Genre-3D%20Platformer-blue)
![Neuroaffirming](https://img.shields.io/badge/Design-Neuroaffirming-green)
![Non-Violent](https://img.shields.io/badge/Combat-Non--Violent%20Symbolic-purple)

> **Note:** This repository contains a **playable systems prototype** demonstrating mechanics, narrative structure, and accessibility-first design logic. It is intended as a **reference implementation and vertical-slice simulator**, not a final engine. The Python codebase models game systems, validates design assumptions, and serves as the authoritative design document for engine re-implementation (Godot 4 or Unity).

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

| Mode | Effect | Trade-off |
|------|--------|-----------|
| **Cushion** | Widens timing, spawns safe pockets, reduces clutter, slows hazards | Increases entropy in adjacent areas; unexplored platforms may drift |
| **Guard** | Pins rhythms, stabilises jitter, creates consent gates, claims edges | Locks out some alternative routes; narrows exploration while active |

These costs are not punishments — they mean **accommodation requires intention**, not just toggling.

### 🔍 Read Default → Rewrite Default
Your core interaction loop:

1. **Read Default** — scan the environment to reveal a hidden assumption (e.g. "timing window assumes 200 ms reactions")
2. **Rewrite Default** — replace it with an inclusive alternative (e.g. widen to 500 ms)

Every system in the Society queries the **Defaults Registry**. When you Rewrite a default, the whole world responds. See [DEFAULTS.md](DEFAULTS.md) for the full table.

### ⚡ Non-Violent Symbolic Combat
Combat is pattern intervention, not violence:

- **Pulse** — Clears/resets cycles
- **Thread Lash** — Interrupts loops
- **Radiant Hold** — Shields, creates safe footholds
- **Edge Claim** — Pins a rhythm
- **Re-tune** — Cleans signal corruption

### 📚 Optional Electives (Stealth Learning)
Challenge rooms embedding logic, literacy, numeracy, language, and digital literacy as physical puzzles. **Never gates story.**

**Explicit rewards:**
- 🎭 New traversal expressions (different animation styles, not power upgrades)
- 🌬️ Alternate Windprint behaviours (variant Cushion/Guard effects)
- 🌍 World state changes (signage clarity, NPC behaviour shifts, new dialogue)
- 📜 Lore fragments and cosmetic customisation

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

### Generate NDIS Quote (Participant Summary of Supports)

```bash
python3 ndis_quote_generator.py
python3 test_ndis_quote_generator.py
```

Optional region selector (uses regional rate columns when available):

```bash
python3 ndis_quote_generator.py --region national
python3 ndis_quote_generator.py --region remote
python3 ndis_quote_generator.py --region very_remote
```

This writes a quote HTML file to:

- `output/participant_summary_of_supports.html`

Data sources are file-based so you can swap in your real pricing and category rules:

- `ndis_data/price_guide.2025-26.json` (preferred when present)
- `ndis_data/price_guide.sample.json`
- `ndis_data/claiming_categories.sample.json`

Supporting extraction artifacts generated from the uploaded PDF:

- `ndis_data/NDIS Pricing Arrangements and Price Limits 2025-26 PDF.pdf`
- `ndis_data/ndis_price_guide_extract.txt`
- `ndis_data/ndis_code_line_candidates.txt`

Governance source extracts used to guide quote/report/review/letter content:

- `ndis_data/reasonable_and_necessary_extract.txt`
- `ndis_data/ndis_act_2013_extract.txt`
- `ndis_data/practice_standards_extract.txt`

Guidance module:

- `ndis_compliance_guidance.py`

Example usage in Python:

```python
from ndis_compliance_guidance import get_document_guidance

quote_guidance = get_document_guidance("quote")
report_guidance = get_document_guidance("report")
review_guidance = get_document_guidance("review")
letter_guidance = get_document_guidance("letter")
```

### Generate governance-guided Reports / Reviews / Letters

```bash
python3 ndis_document_generator.py --type report --participant "Sample Participant"
python3 ndis_document_generator.py --type review --participant "Sample Participant" --period-start "01/01/2026" --period-end "31/03/2026"
python3 ndis_document_generator.py --type letter --participant "Sample Participant" --purpose "Request for review consideration"
python3 test_ndis_document_generator.py
```

Progress note upload support (for feature/adaptation recommendations):

```bash
python3 ndis_document_generator.py \
	--type report \
	--participant "Sample Participant" \
	--progress-note "ndis_data/sample_progress_note.txt"

python3 test_ndis_progress_notes.py
```

When progress notes are provided, generated drafts include:

- progress-note analysis summary
- recommended features/adaptations with priority and confidence
- evidence lines from uploaded notes

Outputs are written to `output/` as markdown drafts with:

- compliance checklist
- suggested structure
- participant-specific key points
- governance references and source snippets

### Professional web app (quotes + notes + docs)

```bash
python app.py
```

Features include:

- Flask-based UI with modern tabs, dropdowns, and interactive widgets
- Windgap-inspired color styling used discreetly across the interface
- quote builder with regional pricing and HTML download
- progress note upload and adaptation recommendations
- governance-guided report/review/letter generation with file download
- SharePoint deployment guidance for online and downloadable access

Health check endpoint:

```bash
curl http://127.0.0.1:8000/health
```

Deployment docs:

- `DEPLOY_SHAREPOINT.md`
- `Dockerfile`

The generator validates each quote line against:

- known support item codes
- known claiming categories
- item/category compatibility

---

## Project Structure

```
Spiny-Flannel-Society/
│
├── GAME_DESIGN.md           # Full game design document
├── ARCHITECTURE.md          # Technical architecture
├── DEFAULTS.md              # Defaults Registry philosophy & reference
├── IMPLEMENTATION_SUMMARY.md # Implementation notes
├── README.md                # This file
│
├── core/                    # State, events, and the Defaults Registry
│   ├── state.py             # Game state, chapter flow, drift tracking
│   ├── events.py            # Signal/pulse/translation event bus
│   └── defaults_registry.py # ★ The thematic heart — all rewritable defaults
│
├── systems/                 # Gameplay systems (engine-agnostic rules)
│   ├── movement.py          # Abstract traversal rules (not a physics engine)
│   ├── windprint.py         # Windprint Rig: Cushion/Guard with trade-off costs
│   ├── combat.py            # Non-violent symbolic pattern intervention
│   └── signals.py           # Read Default / Rewrite Default verbs
│
├── world/                   # World data and spatial logic
│   ├── districts.py         # Six districts of the Society
│   ├── distortions.py       # Drift manifestations (bias made physical)
│   └── routes.py            # Safe routes, alternative paths, visibility
│
├── narrative/               # Story and characters
│   ├── chapters_data.py     # 12-chapter progression with linked defaults
│   ├── characters.py        # NPC definitions (DAZIE, June, Winton, Ari)
│   └── dialogue.py          # Dialogue trees and mentor conversations
│
├── accessibility/           # Presets and sensory configuration
│   ├── presets.py            # Named profiles (Gentle, Focused, etc.)
│   ├── sensory_rules.py     # Sensory output derived from defaults
│   └── defaults.py          # Quick reference table (documentation)
│
├── game_config.py           # Legacy: original flat configuration
├── game_entities.py         # Legacy: original entity definitions
├── platformer_mechanics.py  # Legacy: original movement systems
├── windprint_rig.py         # Legacy: original Windprint Rig
├── combat_system.py         # Legacy: original combat system
├── chapters.py              # Legacy: original chapter system
│
├── demo.py                  # Interactive demonstration
├── test_game.py             # Unit tests
└── requirements.txt         # Python dependencies
```

### Legacy vs New Structure
The original flat files (`game_config.py`, `game_entities.py`, etc.) are preserved for backward compatibility. The new `core/`, `systems/`, `world/`, `narrative/`, and `accessibility/` packages represent the recommended architecture for engine re-implementation.

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

This is a **systems prototype** built in Python, demonstrating:
- Defaults Registry — every rewritable assumption in one queryable structure
- Read Default / Rewrite Default verb pair — the core interaction loop
- Windprint Rig with ethical trade-off costs
- Non-violent symbolic combat verbs
- Event bus for decoupled system communication
- 12-chapter narrative with linked defaults per chapter
- Accessibility presets (not difficulty levels)
- Engine-agnostic movement rules (ready for Godot/Unity port)

### Engine Migration Path

This Python repo is the **design authority**. When porting:
1. Keep this repo as the reference for mechanics and defaults
2. Re-implement against the engine — don't redesign
3. Start with one movement loop + one Windprint interaction + one "aha" moment
4. Recommended engine: **Godot 4** (philosophically aligned, open source, excellent signals system)

---

## License

This project is licensed under the MIT License.

---

*"You didn't fix us. You reminded us how to care."* — DAZIE Vine
