"""
Spiny Flannel Society - Demo
Demonstrates the expanded game mechanics and narrative systems.

Features demonstrated:
- Windprint Rig (Cushion/Guard modes)
- Non-violent symbolic combat verbs
- Chapter progression system
- Character dialogue
- District exploration
- Accessibility as world law
"""

import time
from typing import Dict, List

from game_entities import (
    Vector3, Translator, GameWorld, HiddenAssumption,
    ContradictorySpace, CorruptedSignal, PenalizedPathway,
    EchoForm, Distortion, NoiseBeast
)
from platformer_mechanics import HybridGameplay
from windprint_rig import WindprintRig, WindprintModes
from combat_system import (
    CombatSystem, CombatEncounter, EncounterType,
    create_echo_form_encounter, create_distortion_encounter,
    create_mixed_encounter
)
from chapters import ChapterManager, get_chapter_intro_dialogue
from game_config import (
    GAME_TITLE, VERSION, GAME_DESCRIPTION,
    COLOR_PALETTES, DriftManifestations, CombatVerbs,
    Districts, DISTRICT_DATA, CHAPTER_DATA, CIVIC_RULES,
    Characters, CHARACTER_DATA, NarrativeStates
)


def create_demo_scenario():
    """Create a comprehensive demo scenario"""
    game = HybridGameplay()

    # Create combat system
    combat = CombatSystem(game.translator, game.windprint_rig)

    # Create chapter manager
    chapters = ChapterManager(game.world, game.translator)

    # Unlock all abilities for demo
    for verb in [CombatVerbs.PULSE, CombatVerbs.THREAD_LASH,
                 CombatVerbs.RADIANT_HOLD, CombatVerbs.EDGE_CLAIM,
                 CombatVerbs.RETUNE]:
        combat.unlock_verb(verb)

    # Create demo space: Windgap Academy Atrium
    atrium_space = ContradictorySpace(
        name="Windgap Academy Atrium",
        base_form={
            "layout": "rigid_grid",
            "access": "conditional",
            "message": "WELCOME / YOU WILL FIT",
            "color": COLOR_PALETTES["standard_defaults"]
        },
        alternate_form={
            "layout": "organic_flow",
            "access": "unconditional",
            "message": "WELCOME / YOU FIT",
            "color": COLOR_PALETTES["axiom_active"]
        },
        district=Districts.WINDGAP_ACADEMY
    )

    # Add assumptions
    assumption1 = HiddenAssumption(
        name="Proof Requirement",
        description="Support must be justified before given",
        constraint="requires_justification=True",
        impact_type=DriftManifestations.DISTORTIONS
    )

    assumption2 = HiddenAssumption(
        name="Standard Pace",
        description="All movement must match standard tempo",
        constraint="pace=STANDARD_ONLY",
        impact_type=DriftManifestations.ECHO_FORMS
    )

    atrium_space.add_assumption(assumption1)
    atrium_space.add_assumption(assumption2)
    game.world.add_contradictory_space(atrium_space)

    # Create corrupted signal
    signal = CorruptedSignal(
        original_message="The Spiny Flannel Axiom: supports appear without proof, translation over correction, consent as structure",
        corruption_level=0.6,
        source="Charter Stone Echo"
    )
    game.world.add_corrupted_signal(signal)

    # Create combat encounter
    encounter = create_mixed_encounter(
        name="Atrium Drift Manifestation",
        position=Vector3(5, 0, 5),
        echo_scripts=["Must perform greeting ritual", "Eye contact required"],
        broken_rules=["Ramp retraction on pause"],
        overload_types=["Announcement cacophony"]
    )
    combat.start_encounter(encounter)

    return game, combat, chapters, atrium_space, signal, encounter


def print_divider(char='=', length=70):
    """Print a visual divider"""
    print(char * length)


def print_section(title, char='='):
    """Print a section header"""
    print()
    print_divider(char)
    print(f"  {title}")
    print_divider(char)


def print_subsection(title):
    """Print a subsection header"""
    print(f"\n  ▸ {title}")
    print("  " + "-" * 40)


def demo_title_screen():
    """Display the title screen"""
    print("\n")
    print_divider('═')
    print("""
  ███████╗██████╗ ██╗███╗   ██╗██╗   ██╗
  ██╔════╝██╔══██╗██║████╗  ██║╚██╗ ██╔╝
  ███████╗██████╔╝██║██╔██╗ ██║ ╚████╔╝
  ╚════██║██╔═══╝ ██║██║╚██╗██║  ╚██╔╝
  ███████║██║     ██║██║ ╚████║   ██║
  ╚══════╝╚═╝     ╚═╝╚═╝  ╚═══╝   ╚═╝
  ╔═╗╦  ╔═╗╔╗╔╔╗╔╔═╗╦    ╔═╗╔═╗╔═╗╦╔═╗╔╦╗╦ ╦
  ╠╣ ║  ╠═╣║║║║║║║╣ ║    ╚═╗║ ║║  ║║╣  ║ ╚╦╝
  ╚  ╩═╝╩ ╩╝╚╝╝╚╝╚═╝╩═╝  ╚═╝╚═╝╚═╝╩╚═╝ ╩  ╩
""")
    print(f"  Version {VERSION}")
    print()
    print("  A hybrid 3D platformer about translation, systems, and diversity")
    print("  Neuroaffirming | Non-violent combat | Universal design as world law")
    print_divider('═')
    print()


def demo_narrative():
    """Demonstrate the narrative elements"""
    print_section("NARRATIVE FOUNDATION")

    print("""
  📖 THE SETTING
  ─────────────
  Spiny Flannel Society is a living settlement suspended in permanent
  wind currents above an Australian coastline. It is not a school—
  it is a place people live.

  Windgap Academy is one precinct inside: a learning commons, workshop
  district, and navigation hub—the Society's "translation engine."
""")

    print("""
  📖 THE SPINY FLANNEL AXIOM
  ──────────────────────────
  The original principle that celebrated diversity:
  • Multiple valid paths through spaces
  • Adaptive systems responding to individuals
  • Signals maintaining integrity across interpretation
  • Environment supports people, not corrects them
""")

    print("""
  📖 THE DRIFT
  ────────────
  When rigid "standard defaults" were adopted, the Axiom withdrew.
  The Drift is systemic bias made physical:

  • Distortions — Glitched rules (ramps retract if you pause)
  • Echo Forms — Social scripts given motion (coercive routines)
  • Noise Beasts — Sensory overload as weather
""")

    print("""
  📖 YOUR ROLE: THE TRANSLATOR
  ─────────────────────────────
  You perceive "Windprints"—how assumptions embed into space.
  Your mission: make the Society fit people again.

  Communication modes (all equal outcomes):
  • Direct speech    • Scripted responses
  • Icon-based       • Minimal speech
""")


def demo_windprint_rig(game: HybridGameplay):
    """Demonstrate the Windprint Rig system"""
    print_section("WINDPRINT RIG SYSTEM")

    rig = game.windprint_rig

    print("""
  The Windprint Rig is your signature tool—a dual-mode system that
  embodies the game's philosophy: softness + protection = coherence.
""")

    print_subsection("Cushion Mode — Softness & Accessibility")

    rig.activate_cushion()
    print(f"  ☁️  Mode activated: {rig.current_mode}")
    print(f"  ⏱️  Timing windows: {rig.get_timing_multiplier()}x wider")
    print(f"  🐢 Hazard speed: {rig.get_hazard_multiplier()}x (slower)")
    print(f"  🧹 Clutter reduction: {rig.cushion.get_clutter_reduction()*100:.0f}%")
    print(f"  🌬️  Wind impact: {rig.cushion.get_wind_reduction()*100:.0f}% of normal")

    print_subsection("Guard Mode — Protection & Boundaries")

    rig.activate_guard()
    print(f"  🛡️  Mode activated: {rig.current_mode}")
    print(f"  📍 Rhythm pin strength: {rig.guard.get_rhythm_pin_strength()*100:.0f}%")
    print(f"  📐 Jitter stabilisation: {rig.guard.get_jitter_stabilisation()*100:.0f}%")
    print(f"  🚪 Consent gates: {'Active' if rig.guard.are_consent_gates_active() else 'Inactive'}")
    print(f"  📏 Edge claim range: {rig.guard.get_edge_claim_range()}m")

    print(f"\n  💡 Both modes together make the Society coherent.")
    print(f"  ⚡ Energy: {rig.energy:.0f}/{rig.max_energy}")


def demo_combat_system(combat: CombatSystem, game: HybridGameplay):
    """Demonstrate the non-violent combat system"""
    print_section("NON-VIOLENT SYMBOLIC COMBAT")

    print("""
  Combat is pattern intervention, not violence.
  Threats are misaligned patterns—consequences of misdesign.
  Resolution comes through understanding and restoration.
""")

    print_subsection("Combat Verbs")

    verbs_info = [
        ("⚡ PULSE", "Clears/resets cycles", "Distortions"),
        ("🧵 THREAD LASH", "Interrupts loops", "Echo Forms"),
        ("✨ RADIANT HOLD", "Shields, safe footholds", "All (defense)"),
        ("📌 EDGE CLAIM", "Pins rhythms", "Distortions"),
        ("🎵 RE-TUNE", "Cleans corruption", "Noise Beasts")
    ]

    for verb, effect, best_against in verbs_info:
        print(f"  {verb}")
        print(f"     Effect: {effect}")
        print(f"     Best against: {best_against}")
        print()

    # Demo combat encounter
    encounter = combat.get_active_encounter()
    if encounter:
        print_subsection(f"Active Encounter: {encounter.name}")
        print(f"  Type: {encounter.encounter_type.name}")
        print(f"  Patterns to resolve: {len(encounter.get_active_patterns())}")

        for pattern in encounter.get_active_patterns():
            recommended = combat.get_recommended_verb(pattern)
            print(f"    • {pattern.name}")
            print(f"      Recommended verb: {recommended}")

        # Execute some verbs
        print("\n  🎮 Resolving patterns...")

        result = combat.use_verb(CombatVerbs.THREAD_LASH, Vector3(5, 0, 5))
        if result.success:
            print(f"    → {result.message}")

        result = combat.use_verb(CombatVerbs.PULSE, Vector3(5, 0, 8))
        if result.success:
            print(f"    → {result.message}")

        result = combat.use_verb(CombatVerbs.RETUNE, Vector3(5, 2, 5))
        if result.success:
            print(f"    → {result.message}")

        print(f"\n  📊 Encounter progress: {encounter.get_resolution_progress()*100:.0f}%")


def demo_chapter_system(chapters: ChapterManager):
    """Demonstrate the chapter progression system"""
    print_section("12-CHAPTER NARRATIVE")

    print("""
  Each chapter focuses on a district, theme, and civic rule to restore.
  Completing chapters reduces The Drift and restores the Axiom.
""")

    print_subsection("Chapter Overview")

    for i in range(1, 7):  # Show first 6 chapters
        ch = CHAPTER_DATA.get(i)
        if ch:
            district_name = DISTRICT_DATA.get(ch.location, {}).get("name", ch.location)
            rule_desc = CIVIC_RULES.get(ch.civic_rule, "")[:40]
            print(f"  {i:2}. {ch.name}")
            print(f"      📍 {district_name}")
            print(f"      🎯 {ch.theme}")
            print(f"      📜 {rule_desc}...")
            print()

    print("  ... and 6 more chapters leading to the finale at Windcore Tower")

    # Start chapter 1
    print_subsection("Chapter 1: Bract Theory")

    chapters.start_chapter(1)
    summary = chapters.get_chapter_summary(1)

    print(f"  📍 Location: {summary['location']}")
    print(f"  🎯 Theme: {summary['theme']}")
    print(f"  📜 Civic Rule: {summary['civic_rule']}")
    print(f"  📖 \"{summary['civic_rule_description']}\"")
    print(f"  👥 NPCs: {', '.join(summary['npcs'])}")

    # Show intro dialogue
    print_subsection("Scene: Arrival at Windgap Academy")

    dialogue = get_chapter_intro_dialogue(1)
    for line in dialogue:
        speaker_data = CHARACTER_DATA.get(line.speaker, {})
        speaker_name = speaker_data.get("name", line.speaker)
        print(f"\n  {speaker_name}:")
        print(f"    \"{line.text}\"")


def demo_districts():
    """Demonstrate the district system"""
    print_section("DISTRICTS OF SPINY FLANNEL SOCIETY")

    for district_id, data in DISTRICT_DATA.items():
        print(f"\n  🏛️  {data['name']}")
        print(f"      {data['description']}")
        print(f"      🌬️  Wind: {data['wind_pattern'].replace('_', ' ').title()}")


def demo_accessibility():
    """Demonstrate accessibility as world law"""
    print_section("ACCESSIBILITY AS WORLD LAW")

    print("""
  In Spiny Flannel Society, accessibility isn't a settings menu—
  it's how the world works. Universal design is canon.
""")

    features = [
        ("🎚️  Sensory sliders", "Adjust motion, brightness, audio layers"),
        ("🧹 Clutter reduction", "Reduce visual noise in-world"),
        ("⏱️  No forced timers", "Take the time you need"),
        ("🛤️  Safe routes = main routes", "Accessibility isn't a side path"),
        ("🗣️  Communication modes", "Direct, scripted, icons, minimal—all equal"),
        ("💬 Subtitle styles", "Standard, high contrast, dyslexia-friendly"),
        ("🚫 No hard fail states", "Learn without punishment")
    ]

    for icon_name, description in features:
        print(f"  {icon_name}")
        print(f"     {description}")
        print()

    print("  These aren't accommodations. They're how the Society should work.")
    print("  The Drift broke them. You're restoring them.")


def demo_gameplay_loop(game: HybridGameplay, space: ContradictorySpace):
    """Demonstrate core gameplay"""
    print_section("GAMEPLAY DEMONSTRATION")

    print_subsection("Reading Hidden Assumptions")

    nearby = game.interaction.scan_for_assumptions()
    print(f"  🔍 Scanning for assumptions...")
    print(f"     Found {len(nearby)} hidden assumptions\n")

    for assumption in nearby:
        print(f"  📌 {assumption.name}")
        game.translator.read_assumption(assumption)
        print(f"     Revealed: {assumption.description}")
        print(f"     Constraint: {assumption.constraint}")
        print()

    print_subsection("Rewriting the Environment")

    print(f"  ⚡ Translation Energy: {game.translator.translation_energy}")

    for assumption in space.assumptions:
        if assumption.is_visible:
            success = game.translator.rewrite_environment(assumption)
            if success:
                print(f"  ✓ Rewritten: {assumption.name}")
                print(f"    Constraint '{assumption.constraint}' removed!")
                print(f"    Energy remaining: {game.translator.translation_energy}")

    result = game.interaction.interact_with_space(space)
    print(f"\n  🎯 {result}")

    print_subsection("Game State")

    state = game.get_game_state()
    for key, value in state.items():
        print(f"  {key}: {value}")


def demo_finale_preview():
    """Preview the finale mechanics"""
    print_section("FINALE PREVIEW: REFOUND LIGHT")

    print("""
  In Chapter 12, you ascend the Windcore Tower. The Standardiser—
  a massive Distortion—attempts to reinstall old defaults.

  Using every verb you've learned, you dismantle it by fixing its logic.
  Then, at the Design Interface, you compose new societal defaults:
""")

    defaults = [
        "✓ Supports by default",
        "✓ Translation culture",
        "✓ Sensory baselines",
        "✓ Consent gates",
        "✓ Plural success metrics"
    ]

    for default in defaults:
        print(f"    {default}")

    print("""

  The Society stabilises into plural coherence.

  DAZIE: "You didn't fix us. You reminded us how to care."
  JUNE:  "This time it will remember."
  WINTON: "Coherence achieved through plurality."

  END
""")


def main():
    """Run the complete demo"""
    demo_title_screen()
    time.sleep(1)

    # Create demo scenario
    game, combat, chapters, space, signal, encounter = create_demo_scenario()

    demo_narrative()
    input("\n  Press Enter to continue...")

    demo_windprint_rig(game)
    input("\n  Press Enter to continue...")

    demo_combat_system(combat, game)
    input("\n  Press Enter to continue...")

    demo_chapter_system(chapters)
    input("\n  Press Enter to continue...")

    demo_districts()
    input("\n  Press Enter to continue...")

    demo_accessibility()
    input("\n  Press Enter to continue...")

    demo_gameplay_loop(game, space)
    input("\n  Press Enter to continue...")

    demo_finale_preview()

    print_section("DEMO COMPLETE")
    print("""
  The Spiny Flannel Axiom awaits restoration...

  Core Promise:
  A world that becomes coherent not by fixing individuals,
  but by redesigning society so many kinds of minds can thrive.
""")
    print_divider('═')
    print()


if __name__ == "__main__":
    main()
