"""
Spiny Flannel Society - Demo
Demonstrates the core game mechanics and narrative elements
"""

import time
from game_entities import *
from platformer_mechanics import *
from game_config import *


def create_demo_scenario():
    """Create a demo scenario with various Drift manifestations"""
    game = HybridGameplay()
    
    # Create contradictory space: The Archive District
    archive_space = ContradictorySpace(
        name="Archive District",
        base_form={
            "layout": "rigid_grid",
            "access": "restricted",
            "color": COLOR_PALETTES["standard_defaults"]
        },
        alternate_form={
            "layout": "organic_flow",
            "access": "open",
            "color": COLOR_PALETTES["axiom_active"]
        }
    )
    
    # Add hidden assumptions to the archive
    assumption1 = HiddenAssumption(
        name="Single Path Assumption",
        description="Only one correct path through the archives",
        constraint="must_use_primary_corridor",
        impact_type=DriftManifestations.CONTRADICTORY_SPACES
    )
    
    assumption2 = HiddenAssumption(
        name="Standard Format Requirement",
        description="All data must conform to standard format",
        constraint="format_type=DEFAULT",
        impact_type=DriftManifestations.SIGNAL_CORRUPTION
    )
    
    archive_space.add_assumption(assumption1)
    archive_space.add_assumption(assumption2)
    game.world.add_contradictory_space(archive_space)
    
    # Create corrupted signal
    signal = CorruptedSignal(
        original_message="The Spiny Flannel Axiom celebrates diversity in approach and thought",
        corruption_level=0.7
    )
    game.world.add_corrupted_signal(signal)
    
    # Create penalized pathway
    pathway = PenalizedPathway(
        name="East Wind Bridge",
        standard_requirement="Use certified movement pattern Alpha-7",
        penalty=15.0
    )
    game.world.add_penalized_pathway(pathway)
    
    # Unlock initial abilities
    game.translator.unlock_ability(TranslatorAbilities.REWRITE_ENVIRONMENT)
    game.translator.unlock_ability(TranslatorAbilities.DECODE_SIGNALS)
    game.translator.unlock_ability(TranslatorAbilities.CREATE_PATHWAYS)
    
    return game, archive_space, signal, pathway


def print_divider(char='=', length=60):
    """Print a visual divider"""
    print(char * length)


def print_section(title):
    """Print a section header"""
    print_divider()
    print(f"  {title}")
    print_divider()


def demo_narrative():
    """Demonstrate the narrative elements"""
    print_section("SPINY FLANNEL SOCIETY - Narrative Demo")
    
    print("\n📖 THE SETTING:")
    print("  A living settlement suspended in permanent wind currents")
    print("  above an Australian coastline.\n")
    
    print("📖 THE SPINY FLANNEL AXIOM:")
    print("  The original principle that celebrated diversity, allowing:")
    print("  • Multiple valid paths through spaces")
    print("  • Adaptive systems responding to individual approaches")
    print("  • Signals maintaining integrity across interpretation\n")
    
    print("📖 THE STANDARD DEFAULTS:")
    print("  When rigid defaults were adopted:")
    print("  • Fixed, inflexible rules emerged")
    print("  • One 'correct' way to navigate")
    print("  • Alternative perspectives rejected\n")
    
    print("📖 THE DRIFT:")
    print("  The consequence - reality corrupts:")
    print("  • Contradictory Spaces: Rooms that change unpredictably")
    print("  • Signal Corruption: Communications degrade")
    print("  • Pathway Penalization: Routes hostile to difference\n")
    
    print("📖 YOUR ROLE - THE TRANSLATOR:")
    print("  • Read Hidden Assumptions in systems")
    print("  • Rewrite Environments to remove constraints")
    print("  • Decode Corrupted Signals")
    print("  • Create Alternative Pathways\n")


def demo_gameplay():
    """Demonstrate the gameplay mechanics"""
    print_section("GAMEPLAY DEMO")
    
    game, archive_space, signal, pathway = create_demo_scenario()
    
    print(f"\n🌍 INITIAL STATE:")
    state = game.get_game_state()
    for key, value in state.items():
        print(f"  {key}: {value}")
    
    print(f"\n🌫️  Drift Intensity: {game.world.drift_intensity:.1%}")
    print(f"  Narrative State: {game.world.narrative_state}")
    
    # Demonstrate assumption reading
    print_section("Phase 1: Reading Hidden Assumptions")
    
    nearby = game.interaction.scan_for_assumptions()
    print(f"\n🔍 Scanning for assumptions...")
    print(f"  Found {len(nearby)} hidden assumptions\n")
    
    for i, assumption in enumerate(nearby[:2], 1):
        print(f"  Assumption {i}: {assumption.name}")
        game.translator.read_assumption(assumption)
        print(f"  ✓ Revealed: {assumption.description}")
        print(f"    Constraint: {assumption.constraint}")
        print(f"    Impact: {assumption.impact_type}\n")
    
    # Demonstrate environment rewriting
    print_section("Phase 2: Rewriting Environments")
    
    print(f"\n🔧 Attempting to rewrite environments...")
    print(f"  Translation Energy: {game.translator.translation_energy}\n")
    
    for assumption in archive_space.assumptions:
        if assumption.is_visible:
            success = game.translator.rewrite_environment(assumption)
            if success:
                print(f"  ✓ Rewritten: {assumption.name}")
                print(f"    The constraint '{assumption.constraint}' is removed!")
                print(f"    Energy remaining: {game.translator.translation_energy}\n")
    
    # Check if space is resolved
    result = game.interaction.interact_with_space(archive_space)
    print(f"  🎯 {result}\n")
    
    # Demonstrate signal decoding
    print_section("Phase 3: Decoding Corrupted Signals")
    
    print(f"\n📡 Corrupted Signal Detected:")
    print(f"  Corruption Level: {signal.corruption_level:.1%}")
    print(f"  Corrupted: {signal.corrupted_message}\n")
    
    print(f"  🔓 Decoding...")
    result = game.interaction.interact_with_signal(signal)
    print(f"  {result}\n")
    
    # Demonstrate pathway creation
    print_section("Phase 4: Creating Alternative Pathways")
    
    print(f"\n🚶 Encountered: {pathway.name}")
    info = game.interaction.interact_with_pathway(pathway)
    print(f"  {info}\n")
    
    print(f"  🔨 Creating alternative pathway...")
    pathway.create_alternative(game.translator, "Wind-dancing route")
    print(f"  ✓ Alternative created: Wind-dancing route")
    print(f"  The pathway is now safe!\n")
    
    # Update game state
    print_section("Final State")
    
    # Simulate some game updates
    for _ in range(5):
        game.update(0.1)
    
    state = game.get_game_state()
    print(f"\n🌍 FINAL STATE:")
    for key, value in state.items():
        print(f"  {key}: {value}")
    
    print(f"\n🌫️  Drift Intensity: {game.world.drift_intensity:.1%}")
    print(f"  Progress: {game.world.systems_restored}/{game.world.total_systems} systems restored")
    
    if game.world.drift_intensity < 0.8:
        print(f"\n  ✨ The Drift is weakening! The Axiom begins to restore...")


def demo_platformer_mechanics():
    """Demonstrate the 3D platformer mechanics"""
    print_section("3D PLATFORMER MECHANICS DEMO")
    
    game = HybridGameplay()
    
    print(f"\n🎮 MOVEMENT SYSTEM:")
    print(f"  Player Speed: {game.translator.speed} m/s")
    print(f"  Jump Height: {game.translator.jump_height} m")
    print(f"  Wall Run Speed: {game.translator.wall_run_speed} m/s")
    
    print(f"\n🌬️  WIND-BASED PHYSICS:")
    print(f"  Current Wind Force: {game.current_wind.x:.2f}, {game.current_wind.y:.2f}, {game.current_wind.z:.2f}")
    print(f"  Gravity: {GRAVITY} m/s²")
    
    print(f"\n📍 Starting Position: {game.translator.position.x:.1f}, {game.translator.position.y:.1f}, {game.translator.position.z:.1f}")
    
    # Simulate movement
    print(f"\n🏃 Simulating movement...")
    
    # Move forward
    game.controller.move(Vector3(1, 0, 0), 0.1)
    print(f"  → Moving forward")
    print(f"    Position: {game.translator.position.x:.1f}, {game.translator.position.y:.1f}, {game.translator.position.z:.1f}")
    
    # Jump
    game.translator.is_grounded = True
    game.controller.jump()
    print(f"  ⬆️  Jumping")
    print(f"    Velocity Y: {game.translator.velocity.y:.2f}")
    
    # Simulate physics
    for i in range(10):
        game.controller.apply_gravity(0.1, game.current_wind)
        game.update(0.1)
    
    print(f"  Landing...")
    print(f"    Final Position: {game.translator.position.x:.1f}, {game.translator.position.y:.1f}, {game.translator.position.z:.1f}")
    
    # Wall run
    print(f"\n🧗 Wall Run Mechanics:")
    wall_normal = Vector3(1, 0, 0)
    game.controller.start_wall_run(wall_normal)
    print(f"  ✓ Wall run initiated")
    print(f"    Direction: {game.controller.wall_run_direction.x:.2f}, {game.controller.wall_run_direction.y:.2f}, {game.controller.wall_run_direction.z:.2f}")


def main():
    """Run the complete demo"""
    print("\n")
    print("=" * 70)
    print("  ███████╗██████╗ ██╗███╗   ██╗██╗   ██╗")
    print("  ██╔════╝██╔══██╗██║████╗  ██║╚██╗ ██╔╝")
    print("  ███████╗██████╔╝██║██╔██╗ ██║ ╚████╔╝ ")
    print("  ╚════██║██╔═══╝ ██║██║╚██╗██║  ╚██╔╝  ")
    print("  ███████║██║     ██║██║ ╚████║   ██║   ")
    print("  ╚══════╝╚═╝     ╚═╝╚═╝  ╚═══╝   ╚═╝   ")
    print("  FLANNEL SOCIETY")
    print("  A Hybrid 3D Platformer about Translation and Systems")
    print("=" * 70)
    print("\n")
    
    time.sleep(1)
    
    demo_narrative()
    print("\n")
    time.sleep(2)
    
    demo_gameplay()
    print("\n")
    time.sleep(2)
    
    demo_platformer_mechanics()
    
    print("\n")
    print_divider('=', 70)
    print("  Demo Complete!")
    print("  The Spiny Flannel Axiom awaits restoration...")
    print_divider('=', 70)
    print("\n")


if __name__ == "__main__":
    main()
