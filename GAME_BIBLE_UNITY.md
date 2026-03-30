<!-- markdownlint-disable MD022 MD024 MD032 MD060 -->

# Spiny Flannel Society — Complete Game Bible (Unity Production Draft)

Updated: 2026-03-30
Project: **Spiny Flannel Society**
Genre: **Hybrid 3D platformer + non-violent symbolic combat + narrative adventure**
Setting: **Floating settlement above an Australian coastline**

---

## 1) What the game is all about

**Spiny Flannel Society** is a story about redesigning systems so different kinds of people can thrive.

The world once operated under the **Spiny Flannel Axiom** (plural support, care, and adaptation), but rigid "standard defaults" replaced it. That shift created **The Drift**: architecture, routes, and social systems now force one “correct” way of moving, speaking, and processing.

You play as **The Translator** — a newcomer who can:
- **Read Defaults** (detect hidden assumptions in world systems)
- **Rewrite Defaults** (replace exclusionary rules with inclusive ones)

Core fantasy: You don’t “fix people.” You **fix the rules** that shape the environment.

---

## 2) Game aim, intention, and message

### Player’s mechanical aim
Restore 12 civic rules across 12 chapters, lower Drift intensity to zero, and recompose the Society’s operating defaults in the final chapter.

### Narrative aim
Transform the city from compliance-driven rigidity to **plural coherence**.

### Intention
Make accessibility and universal design feel like **core world logic**, not optional settings.

### Message
A society becomes resilient when it supports difference by default.

---

## 3) How progression works (what the player must do)

Each chapter follows this loop:
1. Enter district and identify Drift manifestation.
2. Traverse platforming route(s) and survive symbolic encounters.
3. Use **Read Default** on local assumptions.
4. Use Windprint verbs (**Cushion/Guard + combat verbs**) to stabilize systems.
5. Interact with the chapter **Design Terminal**.
6. Restore chapter civic rule.
7. Unlock next chapter.

Victory condition:
- All 12 civic rules restored.
- Final composition at Windcore completed.
- Society state transitions to **PLURAL_COHERENCE**.

---

## 4) Core characters (story + gameplay role)

## The Translator (Player)
- Role: Protagonist, Windprint-sensitive system translator.
- Gameplay: Traversal, Read/Rewrite, all symbolic combat verbs.

## DAZIE Vine (Mentor / Systems Ethicist)
- Teaches: Guard logic, consent gates, structural ethics.
- Tone: Calm, direct, non-patronizing.

## June Corrow (Sensory Architect)
- Teaches: Cushion logic, sensory filtration, quiet routes.
- Tone: Sparse, warm, incisive.

## Winton (Civic OS / System Ghost)
- Teaches: system state visibility, rule auditing, terminal logic.
- Tone: Blunt, precise.

## Ari (Late-game guide)
- Appears: Reliquary Edge.
- Function: Bridges preserved principles to final restoration.

---

## 5) Districts and environmental identity

1. **Windgap Academy** — learning commons, archives, simulation systems.
2. **Umbel Gardens** — clustered suspension neighborhoods, networked traversal.
3. **Smoke Margin** — correction infrastructure, decommissioned rules, turbulent zones.
4. **Sandstone Quarter** — charter stones, foundational civic inscriptions.
5. **Veil Market** — signage, sensory filtration, information economy.
6. **Reliquary Edge** — conserved principle vault and pre-finale integration.

---

## 6) Full level-by-level production script (12 chapters)

Each chapter contains:
- Level objective (player goal)
- Chapter aim (narrative purpose)
- Core mechanics
- Primary encounters
- Required dialogue script (implemented/authoritative tone)
- Unity production notes (scene, assets, animation, logic)

---

## Chapter 1 — Bract Theory
**Location:** Windgap Academy Atrium
**Civic Rule:** ACCESS_WITHOUT_PROOF
**Level Objective:** Restore supports so access does not require justification.
**Chapter Aim:** Teach Read/Rewrite and establish thesis.

### Gameplay script
- Intro traversal through welcoming but unstable atrium.
- Drift symptom: supports retract unless player conforms to narrow timing.
- First Read Default reveals timing/access assumption.
- First Rewrite (Cushion/Guard intro) stabilizes core route.
- Design Terminal interaction restores chapter rule.

### Dialogue script
- DAZIE: “Most days the place does orientation for me. Not today.”
- DAZIE: “Protection that looks like beauty.”
- WINTON: “Support withheld pending justification.”
- DAZIE: “That’s the new rule. It’s wrong. Let’s rewrite the welcome.”

### Unity implementation notes
- Scene: `SFS_Ch01_Atrium.unity`
- Prefabs: retracting ramps, adaptive signage, first terminal.
- Animations: tutorial prompts, support bloom/retract states.
- VFX: amber-to-teal rewrite pulse on successful rewrite.

---

## Chapter 2 — Felt Memory
**Location:** Archive Walk
**Civic Rule:** BUFFERS_BY_DEFAULT
**Level Objective:** Reinstate rest pockets and quiet routes.
**Chapter Aim:** Frame overload as information, not failure.

### Gameplay script
- Corridor traversal with escalating sensory density.
- Optional elective in archive logic routing.
- Radiant Hold unlock usage in pressure zones.
- Terminal restores buffer systems.

### Dialogue script
- JUNE: “They deleted quiet because it didn’t measure.”
- DAZIE: “It measures to me.”

### Unity implementation notes
- Audio buses to demonstrate layered overload before rewrite.
- Post-process profile swap for clutter reduction after rewrite.
- Spawn rest-pocket benches on rule restoration.

---

## Chapter 3 — Rayless Form
**Location:** Social Hall
**Civic Rule:** PERFORMANCE_DECOUPLED_FROM_VALUE
**Level Objective:** Break role-enforcing spotlight logic.
**Chapter Aim:** Make communication modes equally valid.

### Gameplay script
- Spotlights create “perform-to-progress” platforms.
- Thread Lash unlock introduced against Echo loops.
- Read Default reveals communication rigidity.
- Rewrite decouples platform activation from performative input.

### Dialogue script
- DAZIE: “The Social Hall stages residents into roles. Spotlights create platforms only for performers.”
- DAZIE: “You don’t have to play along.”
- WINTON: “Performance decoupled from value.”

### Unity implementation notes
- Multiple input modality triggers should all resolve same objective.
- Animator layer for spotlight hostility -> neutral ambient mode.

---

## Chapter 4 — Umbel Logic
**Location:** Umbel Gardens
**Civic Rule:** TRANSLATION_LAYERS
**Level Objective:** Reconnect excluded network nodes.
**Chapter Aim:** Show belonging as architecture.

### Gameplay script
- Grapple-thread traversal across separated node clusters.
- Node exclusion causes bridge collapse patterns.
- Guard pin used to stabilize shifting links.
- Terminal restores translation-layer redundancy.

### Dialogue script
- JUNE: “Belonging isn’t a feeling. It’s architecture.”
- WINTON: “Network integrity declining. Node exclusion detected.”
- DAZIE: “We keep asking people to adapt to a structure that doesn’t know them.”

### Unity implementation notes
- Node graph manager with failover paths.
- Procedural bridge visuals tied to graph connectivity state.

---

## Chapter 5 — Tickshape Rule
**Location:** Skybridges
**Civic Rule:** CONSENT_AS_STRUCTURE
**Level Objective:** Embed consent gates and fairness rails into traversal.
**Chapter Aim:** Reframe rules as safety from power.

### Gameplay script
- Dynamic bridge alignment hazards.
- Edge Claim unlock and boundary mechanics.
- Consent gate checkpoints require explicit player confirm.
- Terminal writes consent infrastructure into map logic.

### Dialogue script
- DAZIE: “Rules exist to protect people from power. When they protect power from people… you get this.”
- JUNE: “Boundaries are instructions for safety.”

### Unity implementation notes
- ConsentGate prefab with confirm UI in world-space canvas.
- Hazard activation blocked until consent signal received.

---

## Chapter 6 — Smoke Signal
**Location:** Smoke Margin
**Civic Rule:** ADAPTATION_RECOGNISED
**Level Objective:** Shut down correction engine and reclassify variance.
**Chapter Aim:** Replace correction with adaptation.

### Gameplay script
- Chase corridors + distortion fog.
- Retune unlock used to calm noise storms.
- Logic-tile debugging of correction decision tree.
- Terminal rewrites classifier policy.

### Dialogue script
- WINTON: “Correction process active.”
- DAZIE: “It treated human variance like a bug.”
- WINTON: “Correction retired. Adaptation recognised.”

### Unity implementation notes
- State machine for Correction Engine phases.
- VFX smoke turbulence decreases with each successful retune.

---

## Chapter 7 — Afterrain Bloom
**Location:** Rain Cliffs
**Civic Rule:** SAFE_PATH_MAIN_PATH
**Level Objective:** Promote safe route from hidden fallback to primary route.
**Chapter Aim:** Establish post-crisis care as system default.

### Gameplay script
- Rhythm traversal under rain pulses.
- Split path: risky express vs hidden safe route.
- Player rewrites routing to make safe path explicit and canonical.
- Terminal commits pacing protocol.

### Dialogue script
- JUNE: “After crisis, care. Otherwise it’s a countdown.”
- DAZIE: “We’re losing time.”
- WINTON: “Pacing protocol: inclusive by default.”

### Unity implementation notes
- Dynamic signage swaps route hierarchy labels on rewrite.
- Minimap path weighting updates after civic rule restored.

---

## Chapter 8 — Sandstone Drift
**Location:** Sandstone Quarter
**Civic Rule:** FLEXIBLE_BY_DEFAULT
**Level Objective:** Restore scratched-out charter principle in foundations.
**Chapter Aim:** Rebuild structural flexibility at civic bedrock.

### Gameplay script
- Sliding foundation blocks with drifting alignments.
- Guard stabilization used to pin navigation windows.
- Read charter inscriptions and recover deleted clauses.
- Terminal re-engraves flexible baseline into simulation mesh.

### Dialogue script
- WINTON: “The Society does not forget. It repeats.”
- WINTON: “Charter stone inscription: ‘Flexible by default.’ Status: overwritten.”

### Unity implementation notes
- World geometry blend states: rigid vs flexible topology.
- Decal/engraving swap system for charter stones.

---

## Chapter 9 — Eucalypt Veil
**Location:** Veil Canopy
**Civic Rule:** PREDICTABLE_TRANSITIONS
**Level Objective:** Restore multi-modal cueing and stable transitions.
**Chapter Aim:** Make calm an engineered outcome.

### Gameplay script
- Glide/wind-surf traversal through filtered canopy lanes.
- Sensory cue mismatches create misreads pre-rewrite.
- Player calibrates visual/audio/icon cue sync.
- Terminal commits transition protocol.

### Dialogue script
- JUNE: “Filtration isn’t hiding. It’s choosing what helps.”
- WINTON: “Predictable transitions reinstated.”

### Unity implementation notes
- Cue synchronization controller across audio/FX/UI channels.
- Trigger volumes should pre-signal transitions at configurable lead time.

---

## Chapter 10 — Clonal Echo
**Location:** Model Society Simulation
**Civic Rule:** PLURAL_ROUTES
**Level Objective:** Break monoculture loop and prove resilience through diversity.
**Chapter Aim:** Demonstrate plural systems survive disruption.

### Gameplay script
- Repeating simulation corridors collapse on deviation.
- Thread Lash combo resolves stacked echo loops.
- Player unlocks multiple simultaneous valid completions.
- Terminal writes plural routing logic.

### Dialogue script
- DAZIE: “This is what they wanted — one way.”
- JUNE: “Monocultures fail.”
- WINTON: “Resilience increased.”

### Unity implementation notes
- Simulation controller with intentionally fragile one-path model pre-rewrite.
- After rewrite: enable route set expansion and dynamic objective acceptance.

---

## Chapter 11 — Edge Reliquary
**Location:** Reliquary Edge
**Civic Rule:** PRINCIPLES_INTEGRATED
**Level Objective:** Retrieve and integrate preserved civic principle modules.
**Chapter Aim:** Move principles from archive into live governance.

### Gameplay script
- Vault traversal with principle-keyed gates.
- Optional stress-test elective validates module robustness.
- Collect principle modules and install into Windprint stack.
- Terminal activates integration.

### Dialogue script
- JUNE: “We stored what we couldn’t protect.”
- ARI: “Time to plant it back.”
- WINTON: “Rare patterns reintroduced.”

### Unity implementation notes
- Collectible module prefab with persistent unlock flags.
- Windprint UI should show newly integrated principle cards.

---

## Chapter 12 — Refound Light
**Location:** Windcore Tower
**Civic Rule:** PLURAL_COHERENCE
**Level Objective:** Defeat Standardiser Distortion and compose new societal defaults.
**Chapter Aim:** Finalize restoration through player-authored governance composition.

### Gameplay script
- Vertical final ascent combining all verbs.
- Boss pattern: Standardiser attempts to enforce old defaults.
- Encounter solved by logic restoration, not destruction.
- Final composition interface sets long-term societal defaults.

### Dialogue script
- DAZIE: “You didn’t fix us. You reminded us how to care.”
- JUNE: “This time it will remember.”
- WINTON: “Coherence achieved through plurality.”

### Unity implementation notes
- Multi-phase finale arena with state snapshots from player windprint history.
- Final composition UI should persist selected preset in save data for ending variant.

---

## 7) Symbolic combat design (non-violent)

Combat verbs are interventions on patterns, not attacks on beings:

- **Pulse** — resets distortion cycles.
- **Thread Lash** — interrupts social-script loops (Echo Forms).
- **Radiant Hold** — creates safe footholds/shields.
- **Edge Claim** — pins unstable rhythms.
- **Retune** — cleans corrupted sensory/weather signals.

Antagonistic pattern classes:
- Distortions
- Echo Forms
- Noise Beasts

Resolution language in UI should always frame actions as:
- stabilize
- restore
- retune
- unloop
- reclassify

---

## 8) Accessibility as canon (must-have implementation)

Accessibility is **diegetic** and systemic. Keep these as world rules:
- Safe routes are visible and valid main routes.
- No forced expression mode.
- No hard fail loops with punitive resets.
- Adjustable sensory load reflected in world state.
- Consent gates in high-risk interactions.
- Subtitle/communication style parity.

Do not hide these behind “assist mode” framing.

---

## 9) Unity production structure (recommended)

## Scene plan
- `SFS_Boot.unity`
- `SFS_Hub_Windgap.unity`
- `SFS_Ch01_Atrium.unity` ... `SFS_Ch12_Windcore.unity`
- `SFS_Credits.unity`

## Prefab families
- `PF_Traversal_*` (platforms, grapple nodes, rails)
- `PF_Distortion_*`
- `PF_Terminal_*`
- `PF_ConsentGate_*`
- `PF_Signage_*`
- `PF_NPC_*`

## Script domains
- Narrative (`ChapterManager`, dialogue runtime, chapter state)
- Systems (`WindprintRig`, defaults registry adapter)
- Movement (flow + precision hybrid)
- Combat (pattern intervention verbs)
- Accessibility (sensory profile + cue layers)

## Animator expectations
- Player: locomotion, wall run, glide, grapple, each combat verb.
- NPCs: idle/listen/signal/respond states per chapter beats.
- Distortion set: unstable -> stabilized state transitions.

---

## 10) Asset-to-story cohesion rules (to keep scenery/sprites/animation connected)

1. **Every major setpiece must map to a civic rule.**
2. **Every major visual change must represent a systemic rewrite.**
3. **Every chapter should visibly transform after terminal restore.**
4. **NPC animation reactions must reflect social consequence, not exposition only.**
5. **Color and audio transitions must communicate Drift reduction.**

If an asset exists without a linked rule, chapter beat, or transformation state, it should be cut or repurposed.

---

## 11) Minimal implementation checklist for “full game in Unity”

- [ ] Chapter flow from 1–12 playable end-to-end.
- [ ] Read Default + Rewrite Default interactions implemented in every chapter.
- [ ] At least 1 elective per chapter functioning (already authored conceptually).
- [ ] Dialogue trigger graph connected to chapter state.
- [ ] Windprint mode switching integrated with traversal and encounter logic.
- [ ] Distortion classes mapped to symbolic combat verbs.
- [ ] Rule restoration visibly transforms environment per chapter.
- [ ] Final preset composition determines ending variant.

---

## 12) Final one-line creative direction

**Spiny Flannel Society is a platformer where changing the world’s assumptions is the core movement mechanic, and care is engineered into architecture.**

---

## 13) Unity production task board (all departments)

Status legend: `TODO` / `IN PROGRESS` / `BLOCKED` / `DONE`

### 13.1 Core foundation sprint (shared dependencies)

| ID | Workstream | Task | Owner | Depends On | Acceptance Criteria | Status |
|---|---|---|---|---|---|---|
| CORE-01 | Narrative Systems | Implement `ChapterManager` runtime with chapter unlock/complete flow | Engineering | None | Chapter 1–12 can be progressed in-editor with debug controls | TODO |
| CORE-02 | Defaults System | Integrate Read/Rewrite interaction loop with world-state callbacks | Engineering | CORE-01 | Any level object can register a default key and react to rewrite | TODO |
| CORE-03 | Windprint | Implement Cushion/Guard mode switching + shared energy model | Engineering | CORE-02 | Mode switch updates movement, hazards, and UI in real time | TODO |
| CORE-04 | Combat | Implement non-violent verb executor (`Pulse`, `Thread Lash`, `Radiant Hold`, `Edge Claim`, `Retune`) | Engineering | CORE-03 | All verbs trigger effects + cooldown + VFX hooks | TODO |
| CORE-05 | Accessibility | Build sensory profile controller (visual clutter, audio layering, shake, subtitle style) | Engineering + UI | CORE-02 | Runtime sliders and presets affect active scene without reload | TODO |
| CORE-06 | Save/Progression | Persistent save for chapter state, restored rules, and final preset | Engineering | CORE-01 | Load returns player to exact chapter state and chosen defaults | TODO |
| CORE-07 | Tools | Chapter debug panel (jump chapter, complete objectives, spawn distortions) | Engineering | CORE-01 | Designers can validate chapter beats without code changes | TODO |

### 13.2 Chapter implementation board (design + code + art + animation + audio)

| Chapter | Level Build | Gameplay/Code | Narrative/UI | Art/Environment | Animation | Audio | QA Gate |
|---|---|---|---|---|---|---|---|
| Ch01 Bract Theory | Atrium blockout + retracting supports | Read/Rewrite tutorial + first terminal | Intro dialogue trigger chain | Warm-amber atrium kit + signage states | Support bloom/retract + mentor idle | Intro wind motif + rewrite sting | Tutorial completable in < 6 min |
| Ch02 Felt Memory | Archive corridor + quiet pockets | Radiant Hold foothold zones | June/DAZIE archive beats | Felt wall set + clutter variants | NPC overload->relief micro performances | Layered overload -> filtered mix snapshot | Sensory rewrite is visibly/audibly measurable |
| Ch03 Rayless Form | Social hall spotlight arena | Thread Lash loop interruption logic | Multi-mode comms prompts | Spotlight rig + expression nodes | Echo loop + spotlight hostility collapse | Spotlight hum -> neutral ambience | All communication modes pass chapter equally |
| Ch04 Umbel Logic | Node garden + suspended links | Grapple-thread routing + node failover | Network integrity story beats | Umbel cluster modules + bridge growth state | Bridge knit animation + node pulse states | Pulse rhythm linked to node health | Alternate node route passes without softlock |
| Ch05 Tickshape Rule | Dynamic skybridge set | Consent gate + Edge Claim integration | Boundary ethics beats | Bridge anchor V-mark props + fair rails | Consent gate open/confirm/decline cycles | Risk cue + safe rail confirmation tone | No hazard starts before consent prompt |
| Ch06 Smoke Signal | Smoke margin chase lanes | Retune + correction engine logic tree | Reclassification terminal copy | Correction engine machinery + smoke shaders | Engine phase transitions + smoke calming | Turbulent to clean-wind blend | Engine can be retired through play, not cutscene-only |
| Ch07 Afterrain Bloom | Rain cliffs + route split | Safe path promotion + pacing rewrite | Post-crisis care beats | Rain blossom platforms + signage hierarchy | Route signage transitions | Rain rhythm system + care motif | Safe route becomes explicit primary route |
| Ch08 Sandstone Drift | Sliding foundation chamber | Guard pin + foundation stabilization | Charter recovery beats | Sandstone inscription states | Engraving restore + foundation lock states | Stone resonance + stability cue | Multiple valid routes verified in same scene |
| Ch09 Eucalypt Veil | Canopy glide lanes | Predictable transition cue sync | Filtration + composition setup | Veil canopy + cue marker variants | Glide states + cue pulses | Multi-modal cue sync tests | Cue lead times pass accessibility checks |
| Ch10 Clonal Echo | Simulation loop map | Plural-route unlock + loop break logic | Monoculture failure beats | Sterile sim -> layered color transform | Echo collapse + route branch reveal | Monotone -> harmonic spread mix | At least 3 valid solution routes tracked |
| Ch11 Edge Reliquary | Reliquary vault + principle gates | Principle module collection + stress test room | Ari + integration beats | Vault props + principle artifacts | Module pickup + install animation | Reliquary reverberant motif | Principle modules persist into finale |
| Ch12 Refound Light | Windcore vertical finale | Standardiser multi-phase logic + final composition UI | Finale dialogue + ending state switch | Windcore geometry variants by player profile | Full verb combo states + finale transitions | Finale suite + ending cadence | Ending changes based on composed preset |

### 13.3 Department-specific backlog (ready to assign)

#### Engineering
- ENG-01: Build reusable `DesignTerminalController` with chapter-specific data injection.
- ENG-02: Build `DistortionSpawner` profile asset per chapter.
- ENG-03: Add `CommunicationModeParityTests` (all modes produce equal objective outcomes).
- ENG-04: Add `SafeRouteVisibilityTests` (safe route cannot be hidden after chapter rewrite).
- ENG-05: Implement ending state resolver from `ComposedPreset` values.

#### Art / Environment
- ART-01: Drift state material variants (`rigid`, `transitional`, `plural`).
- ART-02: District prop kits with before/after rewrite variants.
- ART-03: Terminal visual language pass (consistent affordance silhouettes).
- ART-04: Rule-restoration “world relax” decal and lighting overlays.

#### Animation
- ANM-01: Player locomotion full set (idle/run/jump/triple-hop/wall-run/glide/grapple).
- ANM-02: Verb set (Pulse, Thread Lash, Radiant Hold, Edge Claim, Retune).
- ANM-03: NPC reaction library per emotional state (`tense`, `relieved`, `curious`, `resolved`).
- ANM-04: Distortion state machine animations (`unstable`, `contained`, `resolved`).

#### Audio
- AUD-01: Chapter motifs and rewrite stingers.
- AUD-02: Drift corruption layers per district.
- AUD-03: Post-rewrite calm mix snapshots per chapter.
- AUD-04: Accessibility mix presets (low-layer, no-shake cue emphasis).

#### QA / Design Verification
- QA-01: Chapter completion without electives.
- QA-02: Chapter completion with all electives.
- QA-03: No forced timer blockers on main path.
- QA-04: No hard fail-state that erases chapter progress.
- QA-05: Finale outcome consistency with saved preset.

---

## 14) Full animation + cinematic shot list (Unity-ready)

Naming convention:
- Cinematic timeline shots: `SFS_CIN_CH##_S###`
- Gameplay setpiece shots: `SFS_GM_CH##_S###`
- Player clips: `PLY_*`
- NPC clips: `NPC_<NAME>_*`
- Distortion clips: `DST_*`

### 14.1 Global player animation clips (required across all chapters)

- `PLY_Idle_Default`
- `PLY_Run_Flow`
- `PLY_TripleHop_1_Short`
- `PLY_TripleHop_2_Long`
- `PLY_TripleHop_3_Float`
- `PLY_AirDash_Forward`
- `PLY_WallRun_Left`
- `PLY_WallRun_Right`
- `PLY_WallKick_Exit`
- `PLY_Grapple_Enter`
- `PLY_Grapple_Swing`
- `PLY_Grapple_Exit`
- `PLY_Glide_Enter`
- `PLY_Glide_Loop`
- `PLY_Glide_Exit`
- `PLY_Verb_Pulse`
- `PLY_Verb_ThreadLash`
- `PLY_Verb_RadiantHold`
- `PLY_Verb_EdgeClaim`
- `PLY_Verb_Retune`
- `PLY_ReadDefault_Inspect`
- `PLY_RewriteDefault_Apply`

### 14.2 NPC performance clip library

- `NPC_DAZIE_Idle_Calm`
- `NPC_DAZIE_Gesture_Explain`
- `NPC_DAZIE_Gesture_Protective`
- `NPC_JUNE_Idle_Observant`
- `NPC_JUNE_Gesture_Precise`
- `NPC_JUNE_Gesture_Warm`
- `NPC_WINTON_Holo_Idle`
- `NPC_WINTON_Holo_Audit`
- `NPC_WINTON_Holo_Confirm`
- `NPC_ARI_Idle_Hopeful`
- `NPC_ARI_Gesture_Invite`

### 14.3 Distortion / system animation clip library

- `DST_EchoLoop_Idle`
- `DST_EchoLoop_Break`
- `DST_Distortion_Cycle`
- `DST_Distortion_Reset`
- `DST_NoiseStorm_Rise`
- `DST_NoiseStorm_Calm`
- `SYS_Support_Bloom`
- `SYS_Support_Retract`
- `SYS_ConsentGate_Open`
- `SYS_ConsentGate_Close`
- `SYS_Charter_Engrave_Restore`
- `SYS_Terminal_Activate`
- `SYS_Terminal_RewritePulse`

### 14.4 Chapter cinematic + gameplay shot plan

#### Chapter 1 — Bract Theory
- `SFS_CIN_CH01_S001` — Wide aerial of Windgap atrium, supports stuttering.
- `SFS_CIN_CH01_S002` — Medium on DAZIE introducing unstable welcome.
- `SFS_GM_CH01_S003` — Player approaches retracting support lane.
- `SFS_GM_CH01_S004` — Read Default close-up overlay on timing mechanism.
- `SFS_GM_CH01_S005` — Rewrite pulse; supports bloom; NPC crosses newly stable route.

#### Chapter 2 — Felt Memory
- `SFS_CIN_CH02_S001` — Archive corridor with overwhelming layered stimuli.
- `SFS_GM_CH02_S002` — Radiant Hold deployment creates calm foothold.
- `SFS_GM_CH02_S003` — Quiet route reveal after rewrite.
- `SFS_CIN_CH02_S004` — June reaction: tension to relief micro-performance.
- `SFS_GM_CH02_S005` — Bench/rest pocket appears as systemic response.

#### Chapter 3 — Rayless Form
- `SFS_CIN_CH03_S001` — Social hall spotlights assign “performer” lanes.
- `SFS_GM_CH03_S002` — Thread Lash breaks first Echo loop.
- `SFS_GM_CH03_S003` — Multi-mode communication interaction montage.
- `SFS_CIN_CH03_S004` — Spotlights dim, hall equalizes.
- `SFS_GM_CH03_S005` — Objective complete through non-performative route.

#### Chapter 4 — Umbel Logic
- `SFS_CIN_CH04_S001` — Umbel neighborhood clusters drifting apart.
- `SFS_GM_CH04_S002` — Grapple-thread transit between disconnected nodes.
- `SFS_GM_CH04_S003` — Guard pin stabilizes oscillating bridge.
- `SFS_CIN_CH04_S004` — Network map overlay reconnects in real-time.
- `SFS_GM_CH04_S005` — Restored translation layer opens alternate route.

#### Chapter 5 — Tickshape Rule
- `SFS_CIN_CH05_S001` — Skybridge anchors misalign under coercive rhythm.
- `SFS_GM_CH05_S002` — Consent gate prompt before hazard corridor.
- `SFS_GM_CH05_S003` — Edge Claim pins rhythm lane for safe crossing.
- `SFS_CIN_CH05_S004` — DAZIE line read with protective framing.
- `SFS_GM_CH05_S005` — Fairness rail manifests after terminal rewrite.

#### Chapter 6 — Smoke Signal
- `SFS_CIN_CH06_S001` — Correction engine wake-up, smoke margin surge.
- `SFS_GM_CH06_S002` — Chase run through correction funnels.
- `SFS_GM_CH06_S003` — Retune clears storm pocket.
- `SFS_GM_CH06_S004` — Logic-tree node disable sequence.
- `SFS_CIN_CH06_S005` — Engine powers down; clear wind returns.

#### Chapter 7 — Afterrain Bloom
- `SFS_CIN_CH07_S001` — Rain cliffs with hidden safe route signage.
- `SFS_GM_CH07_S002` — Rhythm traversal on bloom platforms.
- `SFS_GM_CH07_S003` — Route hierarchy rewrite interaction.
- `SFS_CIN_CH07_S004` — Signage flips: safe route now primary.
- `SFS_GM_CH07_S005` — NPCs choose main safe route naturally.

#### Chapter 8 — Sandstone Drift
- `SFS_CIN_CH08_S001` — Charter stone reveal, inscription damage close-up.
- `SFS_GM_CH08_S002` — Sliding foundation navigation setpiece.
- `SFS_GM_CH08_S003` — Guard stabilization holds moving geometry.
- `SFS_CIN_CH08_S004` — Re-engraving cinematic insert.
- `SFS_GM_CH08_S005` — Multiple valid routes open simultaneously.

#### Chapter 9 — Eucalypt Veil
- `SFS_CIN_CH09_S001` — Canopy lanes with desynced transition cues.
- `SFS_GM_CH09_S002` — Glide run through filtration gauntlet.
- `SFS_GM_CH09_S003` — Cue sync calibration interaction.
- `SFS_CIN_CH09_S004` — Layered cue harmony pass (audio/icon/light).
- `SFS_GM_CH09_S005` — Composition terminal unlock teaser.

#### Chapter 10 — Clonal Echo
- `SFS_CIN_CH10_S001` — Sterile sim repeats identical route loop.
- `SFS_GM_CH10_S002` — Thread Lash combo on stacked Echo forms.
- `SFS_GM_CH10_S003` — Route branch reveal after plural logic write.
- `SFS_CIN_CH10_S004` — Simulation recolors from mono to layered palette.
- `SFS_GM_CH10_S005` — Three-path completion showcase.

#### Chapter 11 — Edge Reliquary
- `SFS_CIN_CH11_S001` — Reliquary vault reveal, dormant principle modules.
- `SFS_GM_CH11_S002` — Principle-keyed gate traversal.
- `SFS_GM_CH11_S003` — Module pickup and Windprint integration.
- `SFS_CIN_CH11_S004` — Ari line delivery at vault threshold.
- `SFS_GM_CH11_S005` — Stress test elective completion beat.

#### Chapter 12 — Refound Light
- `SFS_CIN_CH12_S001` — Windcore ascent establishing shot.
- `SFS_GM_CH12_S002` — Standardiser phase 1 pattern lock.
- `SFS_GM_CH12_S003` — Full-verb combo traversal/combat montage.
- `SFS_GM_CH12_S004` — Final composition UI interaction.
- `SFS_CIN_CH12_S005` — Ending tableau variant by composed preset.

### 14.5 Timeline track layout template (apply to each chapter timeline)

- Track 01: Master camera
- Track 02: Player animation override
- Track 03: NPC performance clips
- Track 04: Distortion/system clips
- Track 05: VFX events
- Track 06: Dialogue/VO cues
- Track 07: Music state transitions
- Track 08: Accessibility-safe camera alternatives (no shake / reduced motion)

### 14.6 Shot completion criteria (definition of done)

- Camera readable at gameplay speed (no silhouette loss).
- Player/NPC eye-lines and interaction focus are clear.
- Dialogue subtitle timing synchronized to VO + idle holds.
- Motion-safe alternative passes accessibility checks.
- Rewrites are visually legible in under 1.5 seconds.
- No shot depends on a single communication mode to read meaning.

---

## 15) Chapter-by-chapter Unity sprint plan (2-week milestones)

Cadence: **2 weeks per sprint**
Total baseline: **12 sprints (24 weeks)**
Priority levels:
- **P0** = must ship this sprint
- **P1** = should ship this sprint
- **P2** = nice-to-have / polish if capacity allows

### Sprint 0 (Pre-production hardening) — Weeks 1-2

**Goal:** lock foundation and unblock all chapter teams.

#### Scope
- P0: core runtime scaffolding, save data schema, chapter loading framework
- P0: Read/Rewrite interaction loop integrated with debug logging
- P1: content pipeline conventions (scene/prefab/animation naming)

#### File targets (exact)
- `Assets/_SFS/Scenes/SFS_Boot.unity`
- `Assets/_SFS/Scenes/SFS_Hub_Windgap.unity`
- `Assets/_SFS/Scripts/Core/ChapterRuntimeController.cs`
- `Assets/_SFS/Scripts/Core/SaveGameState.cs`
- `Assets/_SFS/Scripts/Interaction/DesignTerminalController.cs`
- `Assets/_SFS/Scripts/Player/WindprintRigBridge.cs`
- `Assets/_SFS/Scripts/UI/Debug/ChapterDebugPanel.cs`

#### Exit criteria
- Enter Play Mode, jump to any chapter, complete it via debug path, and persist state.

---

### Sprint 1 (Chapter 1: Bract Theory) — Weeks 3-4

**Goal:** ship full vertical slice quality for chapter 1.

#### Scope
- P0: traversal tutorial + first Read/Rewrite + first terminal
- P0: DAZIE/WINTON chapter dialogue trigger graph
- P1: atrium transformation pass (before/after rewrite)

#### File targets
- `Assets/_SFS/Scenes/SFS_Ch01_Atrium.unity`
- `Assets/_SFS/Prefabs/World/PF_Ch01_RetractingSupport.prefab`
- `Assets/_SFS/Prefabs/Interaction/PF_DesignTerminal_Ch01.prefab`
- `Assets/_SFS/Scripts/Narrative/Chapter01Flow.cs`
- `Assets/_SFS/Scripts/Animation/Chapter01AnimationHooks.cs`
- `Assets/_SFS/Timeline/Ch01_Intro.playable`

#### Exit criteria
- First-time player can complete Ch01 without external guidance in < 6 minutes.

---

### Sprint 2 (Chapter 2: Felt Memory) — Weeks 5-6

**Goal:** deliver sensory systems proof-point.

#### Scope
- P0: sensory density escalation and post-rewrite relief
- P0: Radiant Hold encounter use-case
- P1: quiet route elective room

#### File targets
- `Assets/_SFS/Scenes/SFS_Ch02_ArchiveWalk.unity`
- `Assets/_SFS/Scripts/Narrative/Chapter02Flow.cs`
- `Assets/_SFS/Scripts/Audio/SensoryLayerMixerController.cs`
- `Assets/_SFS/Scripts/Visual/ClutterProfileController.cs`
- `Assets/_SFS/Prefabs/World/PF_RestPocket_Bench.prefab`
- `Assets/_SFS/Timeline/Ch02_JuneMoment.playable`

#### Exit criteria
- A/B capture shows clear measurable sensory relief after rewrite.

---

### Sprint 3 (Chapters 3-4: Rayless Form + Umbel Logic) — Weeks 7-8

**Goal:** complete expression parity and networked traversal systems.

#### Scope
- P0: communication mode parity interactions (no objective bias)
- P0: thread-lash encounter flow in Social Hall
- P0: grapple-thread node network with failover routes
- P1: Umbel bridge growth animation states

#### File targets
- `Assets/_SFS/Scenes/SFS_Ch03_SocialHall.unity`
- `Assets/_SFS/Scenes/SFS_Ch04_UmbelGardens.unity`
- `Assets/_SFS/Scripts/Narrative/Chapter03Flow.cs`
- `Assets/_SFS/Scripts/Narrative/Chapter04Flow.cs`
- `Assets/_SFS/Scripts/Interaction/CommunicationParityResolver.cs`
- `Assets/_SFS/Scripts/World/UmbelNodeGraphController.cs`
- `Assets/_SFS/Timeline/Ch03_SpotlightCollapse.playable`
- `Assets/_SFS/Timeline/Ch04_NodeReconnect.playable`

#### Exit criteria
- All communication modes complete Ch03 with identical objective outcomes.

---

### Sprint 4 (Chapters 5-6: Tickshape Rule + Smoke Signal) — Weeks 9-10

**Goal:** consent architecture + correction-engine decommissioning.

#### Scope
- P0: consent gates and edge-claim interactions
- P0: Smoke Margin chase + retune + correction logic-tree rewrite
- P1: fairness rail visual system and smoke calming sequence

#### File targets
- `Assets/_SFS/Scenes/SFS_Ch05_Skybridges.unity`
- `Assets/_SFS/Scenes/SFS_Ch06_SmokeMargin.unity`
- `Assets/_SFS/Scripts/Narrative/Chapter05Flow.cs`
- `Assets/_SFS/Scripts/Narrative/Chapter06Flow.cs`
- `Assets/_SFS/Scripts/Interaction/ConsentGateController.cs`
- `Assets/_SFS/Scripts/World/CorrectionEngineController.cs`
- `Assets/_SFS/Prefabs/Interaction/PF_ConsentGate.prefab`
- `Assets/_SFS/Timeline/Ch06_EngineShutdown.playable`

#### Exit criteria
- No hazard corridor in Ch05 starts without explicit consent event.

---

### Sprint 5 (Chapters 7-8: Afterrain Bloom + Sandstone Drift) — Weeks 11-12

**Goal:** routing fairness and foundational civic rewrite.

#### Scope
- P0: safe route promotion from hidden to canonical
- P0: sliding foundation gameplay + guard stabilization
- P1: charter inscription restoration cinematics

#### File targets
- `Assets/_SFS/Scenes/SFS_Ch07_RainCliffs.unity`
- `Assets/_SFS/Scenes/SFS_Ch08_SandstoneQuarter.unity`
- `Assets/_SFS/Scripts/Narrative/Chapter07Flow.cs`
- `Assets/_SFS/Scripts/Narrative/Chapter08Flow.cs`
- `Assets/_SFS/Scripts/UI/RouteHierarchySignageController.cs`
- `Assets/_SFS/Scripts/World/FoundationDriftController.cs`
- `Assets/_SFS/Timeline/Ch08_CharterRestore.playable`

#### Exit criteria
- Route telemetry confirms most players naturally select safe-main route after rewrite.

---

### Sprint 6 (Chapters 9-10: Eucalypt Veil + Clonal Echo) — Weeks 13-14

**Goal:** cue predictability and plural-route resilience systems.

#### Scope
- P0: multi-modal cue sync controller (audio/icon/light)
- P0: simulation loop break + multiple valid completion branches
- P1: composition-terminal pre-finale setup

#### File targets
- `Assets/_SFS/Scenes/SFS_Ch09_VeilCanopy.unity`
- `Assets/_SFS/Scenes/SFS_Ch10_ModelSociety.unity`
- `Assets/_SFS/Scripts/Narrative/Chapter09Flow.cs`
- `Assets/_SFS/Scripts/Narrative/Chapter10Flow.cs`
- `Assets/_SFS/Scripts/Signals/CueSynchronizationController.cs`
- `Assets/_SFS/Scripts/World/PluralRouteResolver.cs`
- `Assets/_SFS/Timeline/Ch10_SimRecolor.playable`

#### Exit criteria
- Ch10 completes with at least 3 valid tracked route solutions.

---

### Sprint 7 (Chapters 11-12: Edge Reliquary + Refound Light) — Weeks 15-16

**Goal:** finish playable story arc and branching ending logic.

#### Scope
- P0: principle module collection + integration pipeline
- P0: Standardiser multi-phase finale and final composition UI
- P1: ending variants bound to composed preset

#### File targets
- `Assets/_SFS/Scenes/SFS_Ch11_ReliquaryEdge.unity`
- `Assets/_SFS/Scenes/SFS_Ch12_Windcore.unity`
- `Assets/_SFS/Scripts/Narrative/Chapter11Flow.cs`
- `Assets/_SFS/Scripts/Narrative/Chapter12Flow.cs`
- `Assets/_SFS/Scripts/Core/EndingStateResolver.cs`
- `Assets/_SFS/Scripts/UI/FinalCompositionPanel.cs`
- `Assets/_SFS/Timeline/Ch12_FinalTableau.playable`

#### Exit criteria
- End-to-end playthrough from Ch01 to Ch12 works from clean save.

---

### Sprint 8 (Animation integration pass) — Weeks 17-18

**Goal:** replace placeholders with final clip set and ensure timing integrity.

#### Scope
- P0: player locomotion + verb full set connected to gameplay events
- P0: chapter setpiece shots from Section 14 implemented on timelines
- P1: NPC reaction micro-performances on chapter completion

#### File targets
- `Assets/_SFS/Animations/Player/*.anim`
- `Assets/_SFS/Animations/NPC/*.anim`
- `Assets/_SFS/Animations/Systems/*.anim`
- `Assets/_SFS/Scripts/Animation/PlayerAnimationStateRouter.cs`
- `Assets/_SFS/Scripts/Animation/NPCReactionDirector.cs`

#### Exit criteria
- No gameplay-critical action uses placeholder animation clips.

---

### Sprint 9 (Audio + accessibility compliance pass) — Weeks 19-20

**Goal:** certify audio legibility and accessibility behavior across all chapters.

#### Scope
- P0: chapter motifs, rewrite stingers, and post-rewrite calm snapshots
- P0: subtitle timing + communication mode parity verification
- P1: motion-safe camera alternatives for all setpiece shots

#### File targets
- `Assets/_SFS/Audio/Mixers/SFS_Master.mixer`
- `Assets/_SFS/Scripts/Audio/ChapterMusicStateController.cs`
- `Assets/_SFS/Scripts/UI/SubtitleTimingController.cs`
- `Assets/_SFS/Scripts/Camera/MotionSafeCameraProfile.cs`

#### Exit criteria
- Accessibility regression test suite passes on all 12 chapters.

---

### Sprint 10 (QA stabilization + optimization) — Weeks 21-22

**Goal:** eliminate blockers, optimize runtime, and lock release candidate.

#### Scope
- P0: softlock, save corruption, and progression blocker fixes
- P0: frame-time and memory optimization on target hardware
- P1: polish pass on VFX readability for rewrites

#### File targets
- `Assets/_SFS/Tests/PlayMode/ChapterProgressionTests.cs`
- `Assets/_SFS/Tests/PlayMode/AccessibilityParityTests.cs`
- `Assets/_SFS/Tests/PlayMode/EndingVariantTests.cs`
- `Assets/_SFS/Scripts/Utility/PerformanceBudgetOverlay.cs`

#### Exit criteria
- Zero P0/P1 bugs open; all progression tests green.

---

### Sprint 11 (Gold master prep) — Weeks 23-24

**Goal:** content lock, certification checklist, launch package.

#### Scope
- P0: lock content, strings, and ending logic
- P0: final smoke test build and launch candidate
- P1: post-launch backlog grooming

#### File targets
- `Assets/_SFS/Scenes/SFS_Credits.unity`
- `Assets/_SFS/Scripts/Core/BuildMetadata.cs`
- `Assets/_SFS/Docs/ReleaseChecklist.asset`

#### Exit criteria
- Gold candidate approved by design, engineering, and QA leads.

---

## 16) Priority matrix (quick reference)

### P0 (Never slip)
- Chapter progression integrity
- Read/Rewrite functional in all chapters
- Communication/accessibility parity
- Save/load correctness
- Finale completion + ending resolution

### P1 (Can slip only with mitigation)
- Elective depth
- Cinematic polish details
- Non-critical visual variants

### P2 (Post-launch candidate)
- Extra chapter variants
- Expanded elective catalog
- Additional cosmetic response layers

---

## 17) Suggested ownership map

- **Narrative Designer:** Chapter flows, dialogue timing, ending logic sign-off
- **Gameplay Engineer:** verbs, movement, progression, save state
- **Tech Designer:** terminals, triggers, route logic, distortion profiles
- **Environment Artist:** district kits, rewrite states, readability pass
- **Animator:** player verbs, NPC reactions, system state transitions
- **Audio Designer:** chapter motifs, cue legibility, accessibility mixes
- **QA Lead:** progression matrix, parity tests, performance gates
