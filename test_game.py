"""
Spiny Flannel Society - Unit Tests
Comprehensive tests for core game mechanics, Windprint Rig, combat system, and chapters.
"""

import sys
from game_entities import *
from platformer_mechanics import *
from game_config import *
from windprint_rig import WindprintRig, WindprintModes
from combat_system import (
    CombatSystem, CombatEncounter, EncounterType,
    create_echo_form_encounter, create_distortion_encounter, create_mixed_encounter
)
from chapters import ChapterManager, get_chapter_intro_dialogue, Elective


def test_vector3():
    """Test 3D vector operations"""
    print("Testing Vector3 operations...")

    v1 = Vector3(3, 4, 0)
    assert abs(v1.magnitude() - 5.0) < 0.01, "Magnitude calculation failed"

    v2 = v1.normalize()
    assert abs(v2.magnitude() - 1.0) < 0.01, "Normalization failed"

    v3 = Vector3(1, 1, 1) + Vector3(2, 2, 2)
    assert v3.x == 3 and v3.y == 3 and v3.z == 3, "Addition failed"

    v4 = Vector3(2, 3, 4) * 2
    assert v4.x == 4 and v4.y == 6 and v4.z == 8, "Scalar multiplication failed"

    print("  ✓ Vector3 tests passed")


def test_hidden_assumption():
    """Test hidden assumption mechanics"""
    print("Testing HiddenAssumption...")

    assumption = HiddenAssumption(
        "Test Assumption",
        "Test description",
        "test_constraint",
        DriftManifestations.CONTRADICTORY_SPACES
    )

    assert not assumption.is_visible, "Should start hidden"
    assert not assumption.is_rewritten, "Should start not rewritten"

    assumption.reveal()
    assert assumption.is_visible, "Should be visible after reveal"

    assumption.rewrite()
    assert assumption.is_rewritten, "Should be rewritten"

    print("  ✓ HiddenAssumption tests passed")


def test_translator_abilities():
    """Test Translator ability system"""
    print("Testing Translator abilities...")

    translator = Translator(Vector3(0, 0, 0))

    # Test initial state
    assert translator.abilities[TranslatorAbilities.READ_ASSUMPTIONS], "Should start with read ability"
    assert not translator.abilities[TranslatorAbilities.REWRITE_ENVIRONMENT], "Should not start with rewrite"

    # Test ability unlocking
    translator.unlock_ability(TranslatorAbilities.REWRITE_ENVIRONMENT)
    assert translator.abilities[TranslatorAbilities.REWRITE_ENVIRONMENT], "Should have rewrite after unlock"

    # Test reading assumptions
    assumption = HiddenAssumption("Test", "Desc", "constraint", "type")
    result = translator.read_assumption(assumption)
    assert result, "Should successfully read assumption"
    assert assumption.is_visible, "Assumption should be visible"
    assert len(translator.assumptions_read) == 1, "Should track read assumptions"

    print("  ✓ Translator ability tests passed")


def test_environment_rewriting():
    """Test environment rewriting mechanics"""
    print("Testing environment rewriting...")

    translator = Translator(Vector3(0, 0, 0))
    translator.unlock_ability(TranslatorAbilities.REWRITE_ENVIRONMENT)

    assumption = HiddenAssumption("Test", "Desc", "constraint", "type")
    assumption.reveal()

    initial_energy = translator.translation_energy
    result = translator.rewrite_environment(assumption)

    assert result, "Should successfully rewrite"
    assert assumption.is_rewritten, "Assumption should be rewritten"
    assert translator.translation_energy == initial_energy - REWRITE_ENERGY_COST, "Should consume energy"
    assert translator.rewrite_cooldown > 0, "Should have cooldown"

    print("  ✓ Environment rewriting tests passed")


def test_contradictory_space():
    """Test contradictory space mechanics"""
    print("Testing ContradictorySpace...")

    space = ContradictorySpace(
        "Test Space",
        {"prop": "default"},
        {"prop": "axiom"}
    )

    assumption1 = HiddenAssumption("A1", "D1", "C1", "T1")
    assumption2 = HiddenAssumption("A2", "D2", "C2", "T2")

    space.add_assumption(assumption1)
    space.add_assumption(assumption2)

    assert not space.resolve_assumptions(), "Should not be resolved initially"

    assumption1.reveal()
    assumption1.rewrite()
    assert not space.resolve_assumptions(), "Should not be resolved with one assumption"

    assumption2.reveal()
    assumption2.rewrite()
    assert space.resolve_assumptions(), "Should be resolved when all assumptions rewritten"

    print("  ✓ ContradictorySpace tests passed")


def test_corrupted_signal():
    """Test signal corruption and decoding"""
    print("Testing CorruptedSignal...")

    original = "Test message"
    signal = CorruptedSignal(original, 0.5)

    assert not signal.is_decoded, "Should start not decoded"
    assert signal.corrupted_message != original, "Should be corrupted"

    translator = Translator(Vector3(0, 0, 0))
    translator.unlock_ability(TranslatorAbilities.DECODE_SIGNALS)

    result = signal.decode(translator)
    assert result, "Should successfully decode"
    assert signal.is_decoded, "Should be marked as decoded"
    assert signal.partially_decoded_message == original, "Should reveal original message"

    print("  ✓ CorruptedSignal tests passed")


def test_penalized_pathway():
    """Test pathway penalization mechanics"""
    print("Testing PenalizedPathway...")

    pathway = PenalizedPathway("Test Path", "Standard requirement", 10.0)

    assert not pathway.is_safe, "Should start unsafe"

    translator = Translator(Vector3(0, 0, 0))
    translator.unlock_ability(TranslatorAbilities.CREATE_PATHWAYS)

    result = pathway.create_alternative(translator, "Alternative route")
    assert result, "Should create alternative"
    assert pathway.is_safe, "Should become safe"
    assert len(pathway.alternatives_created) == 1, "Should track alternatives"

    print("  ✓ PenalizedPathway tests passed")


def test_game_world():
    """Test game world and drift mechanics"""
    print("Testing GameWorld...")

    world = GameWorld()

    assert world.narrative_state == NarrativeStates.THE_DRIFT, "Should start in The Drift"
    assert world.drift_intensity == DRIFT_INTENSITY_MAX, "Should start at max drift"
    assert world.systems_restored == 0, "Should start with no systems restored"

    # Restore some systems
    world.restore_system()
    assert world.systems_restored == 1, "Should track restored systems"
    assert world.drift_intensity < DRIFT_INTENSITY_MAX, "Drift should decrease"

    # Restore all systems
    for _ in range(SYSTEMS_TO_RESTORE - 1):
        world.restore_system()

    assert world.is_victory(), "Should be victory when all systems restored"
    assert world.drift_intensity < 0.2, "Drift should be minimal"

    print("  ✓ GameWorld tests passed")


def test_platformer_controller():
    """Test platformer movement mechanics"""
    print("Testing PlatformerController...")

    translator = Translator(Vector3(0, 0, 0))
    controller = PlatformerController(translator)

    # Test movement
    initial_x = translator.position.x
    controller.move(Vector3(1, 0, 0), 0.1)
    assert translator.position.x > initial_x, "Should move forward"

    # Test jumping
    translator.is_grounded = True
    result = controller.jump()
    assert result, "Should be able to jump when grounded"
    assert translator.velocity.y > 0, "Should have upward velocity"

    print("  ✓ PlatformerController tests passed")


def test_hybrid_gameplay():
    """Test integrated gameplay system"""
    print("Testing HybridGameplay integration...")

    game = HybridGameplay()

    assert game.translator is not None, "Should have translator"
    assert game.world is not None, "Should have world"
    assert game.controller is not None, "Should have controller"

    # Test game update
    initial_time = game.game_time
    game.update(0.1)
    assert game.game_time > initial_time, "Time should progress"

    # Test state retrieval
    state = game.get_game_state()
    assert "drift_intensity" in state, "Should have drift intensity"
    assert "systems_restored" in state, "Should have systems restored"

    print("  ✓ HybridGameplay tests passed")


def test_windprint_rig():
    """Test Windprint Rig system"""
    print("Testing WindprintRig...")

    translator = Translator(Vector3(0, 0, 0))
    rig = WindprintRig(translator)

    # Test initial state
    assert rig.current_mode is None, "Should start with no mode"
    assert rig.energy == rig.max_energy, "Should start with full energy"

    # Test Cushion Mode
    initial_energy = rig.energy
    rig.activate_cushion()
    assert rig.current_mode == WindprintModes.CUSHION, "Should be in cushion mode"
    assert rig.get_timing_multiplier() > 1.0, "Timing should be wider"
    assert rig.get_hazard_multiplier() < 1.0, "Hazards should be slower"

    # Mode switch consumes energy
    assert rig.energy < initial_energy, "Should consume energy on mode switch"

    # Test Guard Mode
    rig.activate_guard()
    assert rig.current_mode == WindprintModes.GUARD, "Should be in guard mode"

    # Test consent gate
    rig.guard.create_consent_gate(Vector3(0, 0, 0), 5.0)
    gates = rig.guard.consent_gates
    assert len(gates) > 0, "Should have consent gate"

    # Test edge claim
    rig.guard.claim_edge(Vector3(1, 0, 0), Vector3(3, 0, 0))
    edges = rig.guard.claimed_edges
    assert len(edges) > 0, "Should have claimed edge"

    # Test deactivation
    rig.deactivate()
    assert rig.current_mode is None, "Should be None after deactivate"

    print("  ✓ WindprintRig tests passed")


def test_combat_verbs():
    """Test non-violent combat verb system"""
    print("Testing CombatVerbs...")

    translator = Translator(Vector3(0, 0, 0))
    rig = WindprintRig(translator)
    combat = CombatSystem(translator, rig)

    # Test verb unlocking via translator
    combat.unlock_verb(CombatVerbs.PULSE)
    assert CombatVerbs.PULSE in translator.combat_verbs, "Should have Pulse"

    combat.unlock_verb(CombatVerbs.THREAD_LASH)
    assert CombatVerbs.THREAD_LASH in translator.combat_verbs, "Should have Thread Lash"

    # Test verb recommendations
    echo = EchoForm(
        script_name="Test script",
        intensity=1.0,
        position=Vector3(5, 0, 5)
    )

    recommended = combat.get_recommended_verb(echo)
    assert recommended == CombatVerbs.THREAD_LASH, "Thread Lash should be recommended for Echo Forms"

    distortion = Distortion(
        rule_name="Test rule",
        intensity=1.0,
        position=Vector3(5, 0, 5)
    )

    recommended = combat.get_recommended_verb(distortion)
    assert recommended in [CombatVerbs.PULSE, CombatVerbs.EDGE_CLAIM], "Pulse/Edge Claim for Distortions"

    print("  ✓ CombatVerbs tests passed")


def test_combat_encounters():
    """Test combat encounter system"""
    print("Testing CombatEncounters...")

    translator = Translator(Vector3(0, 0, 0))
    rig = WindprintRig(translator)
    combat = CombatSystem(translator, rig)

    # Unlock all verbs for testing
    for verb in [CombatVerbs.PULSE, CombatVerbs.THREAD_LASH,
                 CombatVerbs.RADIANT_HOLD, CombatVerbs.EDGE_CLAIM,
                 CombatVerbs.RETUNE]:
        combat.unlock_verb(verb)

    # Create encounter
    encounter = create_echo_form_encounter(
        name="Test Encounter",
        position=Vector3(10, 0, 10),
        scripts=["Script 1", "Script 2"]
    )

    assert encounter.encounter_type == EncounterType.TRAVERSAL_ARENA, "Should be Traversal Arena encounter"
    assert len(encounter.patterns) == 2, "Should have 2 patterns"

    # Start encounter
    combat.start_encounter(encounter)
    assert len(combat.active_encounters) > 0, "Should have active encounters"
    assert combat.get_active_encounter() == encounter, "Should have active encounter"

    # Use verb
    result = combat.use_verb(CombatVerbs.THREAD_LASH, Vector3(10, 0, 10))
    assert result.success, "Verb should succeed"

    print("  ✓ CombatEncounters tests passed")


def test_chapter_manager():
    """Test chapter progression system"""
    print("Testing ChapterManager...")

    world = GameWorld()
    translator = Translator(Vector3(0, 0, 0))
    manager = ChapterManager(world, translator)

    # Test initial state (starts at chapter 1)
    assert manager.current_chapter_id == 1, "Should start at chapter 1"

    # Get chapter 1 summary
    summary = manager.get_chapter_summary(1)
    assert summary is not None, "Should have summary"
    assert summary['name'] == "Bract Theory", "Should be Bract Theory"
    # Location is returned as district name string, not enum
    assert "Windgap" in summary['location'], "Should be Windgap Academy"

    # Start chapter 1
    result = manager.start_chapter(1)
    assert result, "Should start chapter"

    # Get intro dialogue
    dialogue = get_chapter_intro_dialogue(1)
    assert len(dialogue) > 0, "Should have dialogue"

    print("  ✓ ChapterManager tests passed")


def test_electives():
    """Test elective system (stealth learning)"""
    print("Testing Electives...")

    # Elective is a dataclass, create with proper fields
    elective = Elective(
        id="test_elective",
        name="Wind Reading",
        subject=ElectiveSubjects.LOGIC,
        description="Understanding wind currents",
        difficulty=1,
        rewards=["lore_test"]
    )

    assert not elective.is_completed, "Should start incomplete"

    # Mark as completed
    elective.is_completed = True
    assert elective.is_completed, "Should be completed when marked"

    print("  ✓ Electives tests passed")


def test_antagonistic_patterns():
    """Test antagonistic pattern types"""
    print("Testing AntagonisticPatterns...")

    # Test Echo Form
    echo = EchoForm(
        script_name="Must shake hands",
        intensity=1.0,
        position=Vector3(0, 0, 0)
    )
    assert echo.script_name == "Must shake hands", "Should store script"
    assert not echo.is_resolved, "Should not be resolved initially"

    # Test Distortion
    distortion = Distortion(
        rule_name="Ramp retracts on pause",
        intensity=1.0,
        position=Vector3(0, 0, 0)
    )
    assert distortion.rule_name == "Ramp retracts on pause", "Should store rule"

    # Test Noise Beast
    noise = NoiseBeast(
        overload_type="Audio cacophony",
        intensity=1.0,
        position=Vector3(0, 0, 0)
    )
    assert noise.overload_type == "Audio cacophony", "Should store overload type"

    # Test resolution via receive_verb
    initial_progress = echo.resolution_progress
    echo.receive_verb(CombatVerbs.THREAD_LASH)
    assert echo.resolution_progress > initial_progress, "Should increase progress"

    # Continue until resolved
    for _ in range(5):
        echo.receive_verb(CombatVerbs.THREAD_LASH)
    assert echo.is_resolved, "Should be resolved after multiple interventions"

    print("  ✓ AntagonisticPatterns tests passed")


def test_districts():
    """Test district system"""
    print("Testing Districts...")

    # Test district data exists
    assert Districts.WINDGAP_ACADEMY in DISTRICT_DATA, "Should have Windgap Academy"
    assert Districts.VEIL_MARKET in DISTRICT_DATA, "Should have Veil Market"

    # Test district properties
    windgap = DISTRICT_DATA[Districts.WINDGAP_ACADEMY]
    assert "name" in windgap, "Should have name"
    assert "wind_pattern" in windgap, "Should have wind pattern"
    assert "description" in windgap, "Should have description"

    # Test District entity
    district = District(Districts.WINDGAP_ACADEMY)
    assert district.drift_intensity == DRIFT_INTENSITY_MAX, "Should start at max drift"

    # Test drift update
    district.update_drift(0.7)
    assert district.drift_intensity == 0.7, "Should update drift"

    print("  ✓ Districts tests passed")


def test_civic_rules():
    """Test civic rule restoration"""
    print("Testing CivicRules...")

    # Test rules exist
    assert len(CIVIC_RULES) == 12, "Should have 12 civic rules"

    # Test specific rules
    rule1 = CIVIC_RULES.get("ACCESS_WITHOUT_PROOF")
    assert rule1 is not None, "Should have ACCESS_WITHOUT_PROOF"
    assert "support" in rule1.lower() or "proof" in rule1.lower() or "justification" in rule1.lower(), "Should describe support rule"

    print("  ✓ CivicRules tests passed")


def run_all_tests():
    """Run all unit tests"""
    print("=" * 60)
    print("  SPINY FLANNEL SOCIETY - Unit Tests")
    print("=" * 60)
    print()

    tests = [
        # Core game entities
        test_vector3,
        test_hidden_assumption,
        test_translator_abilities,
        test_environment_rewriting,
        test_contradictory_space,
        test_corrupted_signal,
        test_penalized_pathway,
        test_game_world,
        # Platformer mechanics
        test_platformer_controller,
        test_hybrid_gameplay,
        # New systems
        test_windprint_rig,
        test_combat_verbs,
        test_combat_encounters,
        test_chapter_manager,
        test_electives,
        test_antagonistic_patterns,
        test_districts,
        test_civic_rules,
    ]

    passed = 0
    failed = 0

    for test in tests:
        try:
            test()
            passed += 1
        except AssertionError as e:
            print(f"  ✗ Test failed: {e}")
            failed += 1
        except Exception as e:
            print(f"  ✗ Test error: {e}")
            failed += 1

    print()
    print("=" * 60)
    print(f"  Results: {passed} passed, {failed} failed")
    print("=" * 60)
    print()

    return failed == 0


if __name__ == "__main__":
    success = run_all_tests()
    sys.exit(0 if success else 1)
