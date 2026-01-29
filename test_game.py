"""
Spiny Flannel Society - Unit Tests
Basic tests for core game mechanics
"""

import sys
from game_entities import *
from platformer_mechanics import *
from game_config import *


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
    
    translator = Translator()
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
    
    translator = Translator()
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
    
    translator = Translator()
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


def run_all_tests():
    """Run all unit tests"""
    print("=" * 60)
    print("  SPINY FLANNEL SOCIETY - Unit Tests")
    print("=" * 60)
    print()
    
    tests = [
        test_vector3,
        test_hidden_assumption,
        test_translator_abilities,
        test_environment_rewriting,
        test_contradictory_space,
        test_corrupted_signal,
        test_penalized_pathway,
        test_game_world,
        test_platformer_controller,
        test_hybrid_gameplay
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
