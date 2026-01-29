"""
Spiny Flannel Society - Combat System
Non-violent symbolic combat through pattern intervention.

Combat is pattern intervention, not violence:
- Pulse: Clears/resets cycles
- Thread Lash: Interrupts loops
- Radiant Hold: Shields, creates safe footholds
- Edge Claim: Pins a rhythm
- Re-tune: Cleans signal corruption

Antagonistic patterns are not villains - they are embodied consequences of misdesign.
"""

from typing import List, Dict, Optional, Tuple
from dataclasses import dataclass
from enum import Enum, auto
import math

from game_config import (
    CombatVerbs, COMBAT_VERB_STATS, ANTAGONIST_DATA
)
from game_entities import (
    Vector3, Translator, AntagonisticPattern, EchoForm, Distortion, NoiseBeast
)
from windprint_rig import WindprintRig


# =============================================================================
# COMBAT VERBS
# =============================================================================

class CombatVerb:
    """Base class for combat verbs - non-violent pattern intervention actions"""

    def __init__(self, verb_id: str):
        self.id = verb_id
        self.stats = COMBAT_VERB_STATS.get(verb_id, {})
        self.name = verb_id.replace("_", " ").title()
        self.energy_cost = self.stats.get("energy_cost", 10)
        self.cooldown = self.stats.get("cooldown", 1.0)
        self.effect_radius = self.stats.get("effect_radius", 5.0)
        self.description = self.stats.get("description", "")
        self.duration = self.stats.get("duration", 0.0)

    def can_execute(self, translator: Translator) -> bool:
        """Check if verb can be executed"""
        return translator.can_use_verb(self.id)

    def execute(self, translator: Translator, target_position: Vector3,
                targets: List[AntagonisticPattern]) -> 'VerbResult':
        """Execute the verb against targets"""
        if not translator.use_verb(self.id):
            return VerbResult(success=False, message="Cannot use verb")

        affected = []
        total_progress = 0.0

        for target in targets:
            if target_position.distance_to(target.position) <= self.effect_radius:
                progress = target.receive_verb(self.id)
                affected.append(target)
                total_progress += progress

        return VerbResult(
            success=True,
            message=f"{self.name} affected {len(affected)} patterns",
            affected_patterns=affected,
            total_resolution_progress=total_progress
        )


class PulseVerb(CombatVerb):
    """
    Pulse - Clears/resets cycles

    Primary resolver for Distortions.
    Sends out a wave that resets glitched rule cycles to their starting state.
    """

    def __init__(self):
        super().__init__(CombatVerbs.PULSE)

    def execute(self, translator: Translator, target_position: Vector3,
                targets: List[AntagonisticPattern]) -> 'VerbResult':
        """Execute Pulse - best against Distortions"""
        result = super().execute(translator, target_position, targets)

        if result.success:
            # Additional Pulse effect: reset cycles on all nearby Distortions
            for pattern in result.affected_patterns:
                if isinstance(pattern, Distortion):
                    pattern.reset_cycle()

            result.message = f"Pulse wave cleared {len(result.affected_patterns)} pattern cycles"

        return result


class ThreadLashVerb(CombatVerb):
    """
    Thread Lash - Interrupts loops

    Primary resolver for Echo Forms.
    Snaps the repeating social script loops that Echo Forms enact.
    """

    def __init__(self):
        super().__init__(CombatVerbs.THREAD_LASH)

    def execute(self, translator: Translator, target_position: Vector3,
                targets: List[AntagonisticPattern]) -> 'VerbResult':
        """Execute Thread Lash - best against Echo Forms"""
        result = super().execute(translator, target_position, targets)

        if result.success:
            echo_forms_snapped = sum(
                1 for p in result.affected_patterns if isinstance(p, EchoForm)
            )
            result.message = f"Thread Lash snapped {echo_forms_snapped} script loops"

        return result


class RadiantHoldVerb(CombatVerb):
    """
    Radiant Hold - Shields, creates safe footholds

    Secondary resolver, creates protective spaces.
    Generates a radiant barrier that provides safe ground during encounters.
    """

    def __init__(self):
        super().__init__(CombatVerbs.RADIANT_HOLD)
        self.shield_positions: List[Vector3] = []

    def execute(self, translator: Translator, target_position: Vector3,
                targets: List[AntagonisticPattern]) -> 'VerbResult':
        """Execute Radiant Hold - creates shields and safe footholds"""
        result = super().execute(translator, target_position, targets)

        if result.success:
            # Create a safe foothold at the target position
            self.shield_positions.append(target_position)
            result.message = f"Radiant Hold created safe foothold, affected {len(result.affected_patterns)} patterns"
            result.created_shield = target_position

        return result

    def is_position_shielded(self, position: Vector3) -> bool:
        """Check if a position is within a Radiant Hold shield"""
        for shield_pos in self.shield_positions:
            if position.distance_to(shield_pos) <= self.effect_radius:
                return True
        return False


class EdgeClaimVerb(CombatVerb):
    """
    Edge Claim - Pins a rhythm

    Secondary resolver for Distortions.
    Stabilises erratic platform timing by claiming and pinning rhythm patterns.
    """

    def __init__(self):
        super().__init__(CombatVerbs.EDGE_CLAIM)
        self.claimed_rhythms: Dict[str, float] = {}

    def execute(self, translator: Translator, target_position: Vector3,
                targets: List[AntagonisticPattern]) -> 'VerbResult':
        """Execute Edge Claim - pins rhythms"""
        result = super().execute(translator, target_position, targets)

        if result.success:
            # Claim rhythms from Distortions
            for pattern in result.affected_patterns:
                if isinstance(pattern, Distortion):
                    self.claimed_rhythms[pattern.name] = pattern.cycle_position

            result.message = f"Edge Claim pinned {len(self.claimed_rhythms)} rhythms"

        return result


class RetuneVerb(CombatVerb):
    """
    Re-tune - Cleans signal corruption

    Primary resolver for Noise Beasts.
    Harmonises corrupted sensory signals, calming overload storms.
    """

    def __init__(self):
        super().__init__(CombatVerbs.RETUNE)

    def execute(self, translator: Translator, target_position: Vector3,
                targets: List[AntagonisticPattern]) -> 'VerbResult':
        """Execute Re-tune - best against Noise Beasts"""
        result = super().execute(translator, target_position, targets)

        if result.success:
            noise_beasts_calmed = sum(
                1 for p in result.affected_patterns if isinstance(p, NoiseBeast)
            )
            # Calculate total storm reduction
            total_reduction = sum(
                (1.0 - p.storm_intensity) for p in result.affected_patterns
                if isinstance(p, NoiseBeast)
            )
            result.message = f"Re-tune calmed {noise_beasts_calmed} noise storms"
            result.storm_reduction = total_reduction

        return result


# =============================================================================
# VERB RESULT
# =============================================================================

@dataclass
class VerbResult:
    """Result of executing a combat verb"""
    success: bool
    message: str
    affected_patterns: List[AntagonisticPattern] = None
    total_resolution_progress: float = 0.0
    created_shield: Vector3 = None
    storm_reduction: float = 0.0

    def __post_init__(self):
        if self.affected_patterns is None:
            self.affected_patterns = []


# =============================================================================
# COMBAT ENCOUNTER
# =============================================================================

class EncounterType(Enum):
    """Types of combat encounters"""
    TRAVERSAL_ARENA = auto()  # Combat while platforming
    CHASE_CORRIDOR = auto()  # Escape while resolving patterns
    SYSTEM_DEBUG = auto()  # Puzzle-focused pattern resolution


@dataclass
class CombatEncounter:
    """
    A combat encounter - pattern intervention scenario.

    Encounters are not about defeating enemies, but about
    restoring misaligned patterns to healthy states.
    """
    name: str
    encounter_type: EncounterType
    patterns: List[AntagonisticPattern]
    position: Vector3
    radius: float = 15.0
    is_resolved: bool = False
    resolution_threshold: float = 1.0  # All patterns must be resolved

    def get_active_patterns(self) -> List[AntagonisticPattern]:
        """Get patterns that haven't been resolved yet"""
        return [p for p in self.patterns if not p.is_resolved]

    def get_resolution_progress(self) -> float:
        """Get overall encounter resolution progress"""
        if not self.patterns:
            return 1.0
        resolved = sum(1 for p in self.patterns if p.is_resolved)
        return resolved / len(self.patterns)

    def check_resolution(self) -> bool:
        """Check if encounter is fully resolved"""
        progress = self.get_resolution_progress()
        if progress >= self.resolution_threshold:
            self.is_resolved = True
        return self.is_resolved

    def update(self, delta_time: float):
        """Update all patterns in the encounter"""
        for pattern in self.patterns:
            pattern.update(delta_time)


# =============================================================================
# COMBAT SYSTEM
# =============================================================================

class CombatSystem:
    """
    Manages non-violent symbolic combat through pattern intervention.

    The combat system embodies the game's core philosophy:
    - Threats are misaligned patterns, not enemies
    - Resolution comes through understanding and restoration
    - Violence is never the answer
    """

    def __init__(self, translator: Translator, windprint_rig: WindprintRig = None):
        self.translator = translator
        self.windprint_rig = windprint_rig

        # Initialize verbs
        self.verbs: Dict[str, CombatVerb] = {
            CombatVerbs.PULSE: PulseVerb(),
            CombatVerbs.THREAD_LASH: ThreadLashVerb(),
            CombatVerbs.RADIANT_HOLD: RadiantHoldVerb(),
            CombatVerbs.EDGE_CLAIM: EdgeClaimVerb(),
            CombatVerbs.RETUNE: RetuneVerb()
        }

        # Active encounters
        self.active_encounters: List[CombatEncounter] = []

        # Combat statistics
        self.patterns_resolved = 0
        self.encounters_completed = 0
        self.verbs_used: Dict[str, int] = {v: 0 for v in self.verbs}

    def unlock_verb(self, verb_id: str):
        """Unlock a combat verb for the translator"""
        self.translator.unlock_verb(verb_id)

    def use_verb(self, verb_id: str, target_position: Vector3 = None) -> VerbResult:
        """Use a combat verb at a target position"""
        if verb_id not in self.verbs:
            return VerbResult(success=False, message=f"Unknown verb: {verb_id}")

        verb = self.verbs[verb_id]

        if not verb.can_execute(self.translator):
            cooldown = self.translator.verb_cooldowns.get(verb_id, 0)
            if cooldown > 0:
                return VerbResult(success=False, message=f"{verb.name} on cooldown: {cooldown:.1f}s")
            return VerbResult(success=False, message=f"Cannot use {verb.name}")

        # Default to player position if no target specified
        if target_position is None:
            target_position = self.translator.position

        # Gather all patterns in range from active encounters
        targets = []
        for encounter in self.active_encounters:
            if not encounter.is_resolved:
                for pattern in encounter.get_active_patterns():
                    targets.append(pattern)

        # Execute verb
        result = verb.execute(self.translator, target_position, targets)

        if result.success:
            self.verbs_used[verb_id] += 1

            # Check for resolved patterns
            for pattern in result.affected_patterns:
                if pattern.is_resolved:
                    self.patterns_resolved += 1

            # Check for completed encounters
            for encounter in self.active_encounters:
                if encounter.check_resolution():
                    self.encounters_completed += 1

        return result

    def start_encounter(self, encounter: CombatEncounter):
        """Start a new combat encounter"""
        self.active_encounters.append(encounter)

    def get_active_encounter(self) -> Optional[CombatEncounter]:
        """Get the current active encounter (if any)"""
        for encounter in self.active_encounters:
            if not encounter.is_resolved:
                return encounter
        return None

    def get_nearby_patterns(self, position: Vector3, radius: float) -> List[AntagonisticPattern]:
        """Get all unresolved patterns within radius"""
        patterns = []
        for encounter in self.active_encounters:
            for pattern in encounter.get_active_patterns():
                if position.distance_to(pattern.position) <= radius:
                    patterns.append(pattern)
        return patterns

    def get_recommended_verb(self, pattern: AntagonisticPattern) -> str:
        """Get the recommended verb for a pattern type"""
        if isinstance(pattern, EchoForm):
            return CombatVerbs.THREAD_LASH
        elif isinstance(pattern, Distortion):
            return CombatVerbs.PULSE
        elif isinstance(pattern, NoiseBeast):
            return CombatVerbs.RETUNE
        return CombatVerbs.PULSE  # Default

    def update(self, delta_time: float):
        """Update combat system state"""
        # Update all active encounters
        for encounter in self.active_encounters:
            encounter.update(delta_time)

        # Remove fully resolved encounters (keep for history)
        # In a real game, might archive them instead

    def get_statistics(self) -> Dict:
        """Get combat statistics"""
        return {
            "patterns_resolved": self.patterns_resolved,
            "encounters_completed": self.encounters_completed,
            "verbs_used": self.verbs_used.copy(),
            "active_encounters": len([e for e in self.active_encounters if not e.is_resolved]),
            "favorite_verb": max(self.verbs_used, key=self.verbs_used.get) if any(self.verbs_used.values()) else "None"
        }


# =============================================================================
# ENCOUNTER FACTORY
# =============================================================================

def create_echo_form_encounter(name: str, scripts: List[str],
                                position: Vector3) -> CombatEncounter:
    """Create an encounter focused on Echo Forms (social script patterns)"""
    patterns = [
        EchoForm(script, intensity=0.7, position=position + Vector3(i * 3, 0, 0))
        for i, script in enumerate(scripts)
    ]
    return CombatEncounter(
        name=name,
        encounter_type=EncounterType.TRAVERSAL_ARENA,
        patterns=patterns,
        position=position
    )


def create_distortion_encounter(name: str, rules: List[str],
                                 position: Vector3) -> CombatEncounter:
    """Create an encounter focused on Distortions (broken rules)"""
    patterns = [
        Distortion(rule, intensity=0.8, position=position + Vector3(0, 0, i * 3))
        for i, rule in enumerate(rules)
    ]
    return CombatEncounter(
        name=name,
        encounter_type=EncounterType.SYSTEM_DEBUG,
        patterns=patterns,
        position=position
    )


def create_noise_beast_encounter(name: str, overload_types: List[str],
                                  position: Vector3) -> CombatEncounter:
    """Create an encounter focused on Noise Beasts (sensory overload)"""
    patterns = [
        NoiseBeast(overload_type, intensity=0.9, position=position + Vector3(i * 2, i, 0))
        for i, overload_type in enumerate(overload_types)
    ]
    return CombatEncounter(
        name=name,
        encounter_type=EncounterType.CHASE_CORRIDOR,
        patterns=patterns,
        position=position
    )


def create_mixed_encounter(name: str, position: Vector3,
                           echo_scripts: List[str] = None,
                           broken_rules: List[str] = None,
                           overload_types: List[str] = None) -> CombatEncounter:
    """Create a mixed encounter with multiple pattern types"""
    patterns = []
    offset = 0

    if echo_scripts:
        for script in echo_scripts:
            patterns.append(EchoForm(script, 0.7, position + Vector3(offset, 0, 0)))
            offset += 3

    if broken_rules:
        for rule in broken_rules:
            patterns.append(Distortion(rule, 0.8, position + Vector3(offset, 0, 3)))
            offset += 3

    if overload_types:
        for overload in overload_types:
            patterns.append(NoiseBeast(overload, 0.9, position + Vector3(offset, 2, 0)))
            offset += 3

    return CombatEncounter(
        name=name,
        encounter_type=EncounterType.TRAVERSAL_ARENA,
        patterns=patterns,
        position=position
    )
