using UnityEngine;

public class FUNDING_SUMMARY : MonoBehaviour
{
}# Spiny Flannel Society — Project Summary

**A neuroaffirming 3D platformer where accessibility is the core mechanic, not a menu.**

---

## One-Line Pitch

*A 3D platformer where every accessibility setting is a puzzle to find and rewrite — and every rewrite makes the Society better for everyone.*

---

## The Problem

Games treat accessibility as a settings menu — a checklist of toggles separate from the game's design. This teaches players that accommodation is peripheral. Spiny Flannel Society makes accommodation the central mechanic: the thing you *do* is find, read, and rewrite the assumptions built into the world.

---

## Core Mechanic: Read / Rewrite

Players explore the Spiny Flannel Society, a windborne settlement built on rigid defaults — timing windows too narrow, sensory loads too high, safe routes hidden, failure punished. The Society is failing (a state called "the Drift") because these defaults excluded the people who needed them most.

The player uses two verbs:

- **Read Default**: Reveal the assumption embedded in a game mechanic (e.g., "This platform assumes you react within 200 ms").
- **Rewrite Default**: Change that assumption, widening it for everyone.

Every rewrite propagates: rewriting the timing window makes *all* timing-gated obstacles easier, changes NPC behaviour, shifts the district's visual drift, and moves the Society toward recovery.

---

## What Makes It Different

| Traditional Approach | Spiny Flannel Society |
|---|---|
| Accessibility is a settings menu | Accessibility is the gameplay |
| "Easy mode" is hidden or stigmatised | Generous defaults are the *goal* |
| Difficulty settings are a slider | Each default is a discoverable puzzle |
| Accommodation is personal/private | Every rewrite improves the world for NPCs too |
| Theme and mechanics are separate | The theme *is* the architecture |

---

## Technical Foundation

### Systems Prototype (Complete)

A Python reference implementation (~2,400 lines) proving the architecture works:

- **Defaults Registry**: 15 rewritable defaults across 6 categories (Timing, Sensory, Routing, Social, Failure, Consent). Every system queries this registry.
- **Event Bus**: Decoupled publish/subscribe system. When a default is rewritten, all dependent systems respond automatically.
- **Windprint Rig**: The player's traversal tool with two modes — Cushion (widens tolerances, costs entropy) and Guard (pins safe paths, costs exploration range). Ethical trade-offs built into the tool.
- **Non-Violent Combat**: Antagonists are "patterns" (embodied consequences of bad design), not enemies. Five intervention verbs resolve encounters symbolically.
- **12-Chapter Narrative**: Each chapter introduces specific defaults and links rewrites to civic rules being restored.
- **5 Accessibility Presets**: Named sensory profiles (not difficulty levels) that set multiple defaults at once.
- **Stress-tested**: One default traced through all systems end-to-end, proving propagation works.

### Engine Target: Godot 4

Full mapping guide written, covering:
- Autoload singletons for DefaultsRegistry and GameState
- Godot Resource subclasses for each Default
- Signal Bus pattern for decoupled communication
- Scene tree architecture for districts, NPCs, and UI
- Shader-driven drift visualisation
- Migration priority (8 phases from spine to content)

---

## Content Scope

- **6 Districts**: Windgap Academy, The Veil Market, Sandstone Quarter, The Umbel Gardens, The Smoke Margin, The Reliquary Edge
- **12 Chapters**: Linear narrative with branching player expression
- **15 Rewritable Defaults**: Each a discoverable puzzle with visible consequences
- **5 NPCs**: DAZIE (warm mentor), Winton (blunt civic OS), June (sensory architect), Ari (community voice), and ambient residents
- **Elective Rooms**: Optional spaces exploring one default deeply (no score penalty for skipping)
- **4 Communication Styles**: Direct, Scripted, Icons, Minimal — all equally valid throughout

---

## Design Principles

1. **Accommodation is architecture, not afterthought.** Every accessibility feature is a first-class game mechanic.
2. **No punishment for needing support.** Presets, routes, and timing are generous by design.
3. **Social consequences are visible.** NPCs respond to your rewrites — you see the Society change.
4. **Trade-offs are ethical, not punitive.** Cushion costs entropy (slight unpredictability), Guard costs exploration range — both are honest.
5. **Neurodivergent experience as source material.** Processing differences, sensory needs, and communication variation are the *content*, not edge cases.

---

## Target Audience

- Neurodivergent players and allies (primary)
- Educators and disability advocates (secondary)
- 3D platformer players who want a game that *says something* (tertiary)
- Age: 10+ (gentle difficulty, no violence, meaningful themes)

---

## Comparable Works

- **Celeste** (accessibility + platforming as metaphor)
- **A Short Hike** (gentle exploration, community focus)
- **Unpacking** (mechanics-as-meaning, quiet discovery)
- **Chicory** (creative tools as core mechanic)

Unlike these, SFS makes the accessibility *system itself* the puzzle — not just a metaphor or a mood.

---

## Current Status

| Milestone | Status |
|---|---|
| Game design document | ✅ Complete |
| Architecture document | ✅ Complete |
| Python systems prototype | ✅ Complete (~2,400 lines) |
| Defaults Registry (15 defaults) | ✅ Complete + stress-tested |
| Event system + verb mechanics | ✅ Complete |
| Windprint Rig (Cushion/Guard) | ✅ Complete with ethical costs |
| Non-violent combat system | ✅ Complete |
| 12-chapter narrative structure | ✅ Complete |
| Accessibility presets (5 profiles) | ✅ Complete |
| Worked examples per category | ✅ Complete (6 walkthroughs) |
| First Playable Minute design | ✅ Complete |
| Godot 4 mapping guide | ✅ Complete |
| Godot 4 engine build | 🔲 Next phase |
| Art direction / visual targets | 🔲 Needed |
| Audio design | 🔲 Needed |
| Playtesting | 🔲 After engine prototype |

---

## What's Needed Next

1. **Godot 4 vertical slice** — Windgap Academy, one chapter, full Read/Rewrite loop. (~3 months with 1 developer)
2. **Art direction** — Australian brutalist-organic aesthetic, warm colour palettes, drift-as-visual-corruption.
3. **Audio design** — Layered soundscape that responds to sensory defaults. Wind as emotional language.
4. **Playtesting with neurodivergent players** — The core audience must shape the final experience.
5. **Community feedback** — Open development, lived-experience advisory.

---

## Team

- **Lead Design + Systems Architecture**: Demonstrated through complete prototype
- **Seeking**: Godot developer, environment artist, audio designer, ND advisory panel

---

## Why Fund This

Games are the dominant cultural medium for young people. The message that "accessibility is a checkbox" teaches an entire generation that accommodation is peripheral. Spiny Flannel Society teaches the opposite: that redesigning systems to be inclusive is heroic, skilled, and beautiful.

This isn't an accessibility game. It's a game *about* accessibility — the first to make the *act of designing for difference* into gameplay. It has the potential to shift how an entire medium thinks about inclusion.

The prototype proves the architecture works. What's needed now is the visual, spatial, and sonic craft to make it *felt*.
