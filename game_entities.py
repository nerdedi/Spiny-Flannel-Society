"""
Spiny Flannel Society - Core Game Entities
Defines the main game objects: Player (Translator), Environment, Districts, and Systems

A neuroaffirming indie game where accessibility is world law.
"""

import math
from typing import List, Dict, Tuple, Optional, Set
from dataclasses import dataclass, field
from enum import Enum, auto
from game_config import *


# =============================================================================
# VECTOR MATHEMATICS
# =============================================================================

class Vector3:
    """Simple 3D vector for positions and movements"""

    def __init__(self, x: float = 0, y: float = 0, z: float = 0):
        self.x = x
        self.y = y
        self.z = z

    def magnitude(self) -> float:
        return math.sqrt(self.x**2 + self.y**2 + self.z**2)

    def normalize(self) -> 'Vector3':
        mag = self.magnitude()
        if mag > 0:
            return Vector3(self.x/mag, self.y/mag, self.z/mag)
        return Vector3()

    def __add__(self, other: 'Vector3') -> 'Vector3':
        return Vector3(self.x + other.x, self.y + other.y, self.z + other.z)

    def __sub__(self, other: 'Vector3') -> 'Vector3':
        return Vector3(self.x - other.x, self.y - other.y, self.z - other.z)

    def __mul__(self, scalar: float) -> 'Vector3':
        return Vector3(self.x * scalar, self.y * scalar, self.z * scalar)

    def __repr__(self) -> str:
        return f"Vector3({self.x:.2f}, {self.y:.2f}, {self.z:.2f})"

    def distance_to(self, other: 'Vector3') -> float:
        return (self - other).magnitude()


# =============================================================================
# HIDDEN ASSUMPTIONS
# =============================================================================

class HiddenAssumption:
    """Represents a hidden assumption in a system that can be read and rewritten"""

    def __init__(self, name: str, description: str, constraint: str,
                 impact_type: str, is_visible: bool = False):
        self.name = name
        self.description = description
        self.constraint = constraint  # The actual limiting rule
        self.impact_type = impact_type  # How it affects the environment
        self.is_visible = is_visible
        self.is_rewritten = False

    def reveal(self):
        """Make the assumption visible to the player"""
        self.is_visible = True

    def rewrite(self):
        """Rewrite the assumption to remove its constraint"""
        self.is_rewritten = True

    def __repr__(self) -> str:
        status = "rewritten" if self.is_rewritten else ("visible" if self.is_visible else "hidden")
        return f"Assumption({self.name}, {status})"


# =============================================================================
# WINDPRINT SYSTEM
# =============================================================================

class WindprintRecord:
    """Records a player's interaction pattern for the finale"""

    def __init__(self):
        self.mode_preferences: Dict[str, int] = {
            WindprintModes.CUSHION: 0,
            WindprintModes.GUARD: 0
        }
        self.verb_usage: Dict[str, int] = {verb: 0 for verb in [
            CombatVerbs.PULSE, CombatVerbs.THREAD_LASH,
            CombatVerbs.RADIANT_HOLD, CombatVerbs.EDGE_CLAIM,
            CombatVerbs.RETUNE
        ]}
        self.communication_style: str = CommunicationModes.DIRECT
        self.electives_completed: List[str] = []
        self.civic_rules_restored: List[str] = []

    def record_mode_use(self, mode: str):
        """Record use of a Windprint mode"""
        if mode in self.mode_preferences:
            self.mode_preferences[mode] += 1

    def record_verb_use(self, verb: str):
        """Record use of a combat verb"""
        if verb in self.verb_usage:
            self.verb_usage[verb] += 1

    def get_preferred_mode(self) -> str:
        """Get the player's most-used mode"""
        return max(self.mode_preferences, key=self.mode_preferences.get)

    def get_signature_verb(self) -> str:
        """Get the player's most-used combat verb"""
        return max(self.verb_usage, key=self.verb_usage.get)


# =============================================================================
# TRANSLATOR (PLAYER CHARACTER)
# =============================================================================

class Translator:
    """
    The player character - a Translator who can read hidden assumptions
    and rewrite environments affected by The Drift.

    Perceives "Windprints" - how assumptions embed into space.
    """

    def __init__(self, position: Vector3 = None):
        self.position = position or Vector3(0, 0, 0)
        self.velocity = Vector3()

        # Player stats
        self.speed = PLAYER_SPEED
        self.jump_height = PLAYER_JUMP_HEIGHT
        self.wall_run_speed = PLAYER_WALL_RUN_SPEED

        # Advanced movement stats
        self.air_dash_speed = PLAYER_AIR_DASH_SPEED
        self.glide_speed = PLAYER_GLIDE_SPEED
        self.grapple_speed = PLAYER_GRAPPLE_SPEED

        # Triple hop state
        self.hop_count = 0
        self.hop_timer = 0.0

        # Translator abilities
        self.abilities = {
            TranslatorAbilities.READ_ASSUMPTIONS: True,
            TranslatorAbilities.PERCEIVE_WINDPRINTS: True,
            TranslatorAbilities.REWRITE_ENVIRONMENT: False,
            TranslatorAbilities.DECODE_SIGNALS: False,
            TranslatorAbilities.CREATE_PATHWAYS: False,
            TranslatorAbilities.CUSHION_MODE: False,
            TranslatorAbilities.GUARD_MODE: False
        }

        # Combat verbs (unlocked progressively)
        self.combat_verbs = {
            CombatVerbs.PULSE: False,
            CombatVerbs.THREAD_LASH: False,
            CombatVerbs.RADIANT_HOLD: False,
            CombatVerbs.EDGE_CLAIM: False,
            CombatVerbs.RETUNE: False
        }
        self.verb_cooldowns: Dict[str, float] = {}

        # Energy resources
        self.translation_energy = 100
        self.max_translation_energy = 100
        self.windprint_energy = WINDPRINT_ENERGY_MAX
        self.max_windprint_energy = WINDPRINT_ENERGY_MAX

        # State
        self.is_grounded = False
        self.is_wall_running = False
        self.is_gliding = False
        self.is_grappling = False
        self.rewrite_cooldown = 0.0

        # Communication preference
        self.communication_mode = CommunicationModes.DIRECT

        # Progress tracking
        self.assumptions_read: List[HiddenAssumption] = []
        self.environments_rewritten: List[HiddenAssumption] = []
        self.systems_restored = 0
        self.current_chapter = 1
        self.completed_chapters: Set[int] = set()

        # Windprint record for finale
        self.windprint_record = WindprintRecord()

    def unlock_ability(self, ability: str):
        """Unlock a new Translator ability"""
        if ability in self.abilities:
            self.abilities[ability] = True

    def unlock_verb(self, verb: str):
        """Unlock a combat verb"""
        if verb in self.combat_verbs:
            self.combat_verbs[verb] = True
            self.verb_cooldowns[verb] = 0.0

    def can_use_verb(self, verb: str) -> bool:
        """Check if a verb can be used"""
        if not self.combat_verbs.get(verb, False):
            return False
        if self.verb_cooldowns.get(verb, 0) > 0:
            return False
        stats = COMBAT_VERB_STATS.get(verb, {})
        return self.windprint_energy >= stats.get("energy_cost", 0)

    def use_verb(self, verb: str) -> bool:
        """Use a combat verb"""
        if not self.can_use_verb(verb):
            return False

        stats = COMBAT_VERB_STATS[verb]
        self.windprint_energy -= stats["energy_cost"]
        self.verb_cooldowns[verb] = stats["cooldown"]
        self.windprint_record.record_verb_use(verb)
        return True

    def read_assumption(self, assumption: HiddenAssumption) -> bool:
        """Read a hidden assumption in the environment"""
        if not self.abilities[TranslatorAbilities.READ_ASSUMPTIONS]:
            return False

        assumption.reveal()
        if assumption not in self.assumptions_read:
            self.assumptions_read.append(assumption)
        return True

    def rewrite_environment(self, assumption: HiddenAssumption) -> bool:
        """Rewrite an environment by removing a constraining assumption"""
        if not self.abilities[TranslatorAbilities.REWRITE_ENVIRONMENT]:
            return False

        if self.translation_energy < REWRITE_ENERGY_COST:
            return False

        if self.rewrite_cooldown > 0:
            return False

        if not assumption.is_visible:
            return False

        assumption.rewrite()
        self.translation_energy -= REWRITE_ENERGY_COST
        self.rewrite_cooldown = REWRITE_COOLDOWN

        if assumption not in self.environments_rewritten:
            self.environments_rewritten.append(assumption)

        return True

    def complete_chapter(self, chapter_id: int):
        """Mark a chapter as complete"""
        self.completed_chapters.add(chapter_id)
        if chapter_id < TOTAL_CHAPTERS:
            self.current_chapter = chapter_id + 1

    def set_communication_mode(self, mode: str):
        """Set preferred communication mode (all modes have equal outcomes)"""
        if mode in [CommunicationModes.DIRECT, CommunicationModes.SCRIPTED,
                    CommunicationModes.ICONS, CommunicationModes.MINIMAL]:
            self.communication_mode = mode
            self.windprint_record.communication_style = mode

    def update(self, delta_time: float):
        """Update player state"""
        # Update cooldowns
        if self.rewrite_cooldown > 0:
            self.rewrite_cooldown = max(0, self.rewrite_cooldown - delta_time)

        for verb in self.verb_cooldowns:
            if self.verb_cooldowns[verb] > 0:
                self.verb_cooldowns[verb] = max(0, self.verb_cooldowns[verb] - delta_time)

        # Update triple hop timer
        if self.hop_timer > 0:
            self.hop_timer = max(0, self.hop_timer - delta_time)
            if self.hop_timer == 0:
                self.hop_count = 0

        # Regenerate energy
        if self.translation_energy < self.max_translation_energy:
            self.translation_energy = min(
                self.max_translation_energy,
                self.translation_energy + delta_time * 5  # 5 energy per second
            )

        if self.windprint_energy < self.max_windprint_energy:
            self.windprint_energy = min(
                self.max_windprint_energy,
                self.windprint_energy + delta_time * WINDPRINT_ENERGY_REGEN
            )


# =============================================================================
# CONTRADICTORY SPACE
# =============================================================================

class ContradictorySpace:
    """
    A space affected by The Drift that contradicts itself.
    Properties change based on how rigidly the player conforms to Standard Defaults.
    """

    def __init__(self, name: str, base_form: Dict, alternate_form: Dict,
                 district: str = None):
        self.name = name
        self.base_form = base_form  # Form under Standard Defaults
        self.alternate_form = alternate_form  # True form (under Axiom)
        self.current_contradiction_level = 1.0  # 1.0 = fully contradicted
        self.assumptions: List[HiddenAssumption] = []
        self.district = district

    def add_assumption(self, assumption: HiddenAssumption):
        """Add a hidden assumption that affects this space"""
        self.assumptions.append(assumption)

    def update_contradiction(self, drift_intensity: float):
        """Update how contradicted the space is based on Drift intensity"""
        self.current_contradiction_level = drift_intensity

    def resolve_assumptions(self) -> bool:
        """Check if all assumptions have been rewritten"""
        return all(assumption.is_rewritten for assumption in self.assumptions)

    def get_current_form(self) -> Dict:
        """Get the current form based on contradiction level"""
        if self.current_contradiction_level > 0.5:
            return self.base_form
        else:
            return self.alternate_form


# =============================================================================
# CORRUPTED SIGNAL
# =============================================================================

class CorruptedSignal:
    """
    A signal or communication corrupted by The Drift.
    Must be decoded by the Translator to reveal its true meaning.
    """

    def __init__(self, original_message: str, corruption_level: float,
                 source: str = "unknown"):
        self.original_message = original_message
        self.corruption_level = corruption_level  # 0.0 = clean, 1.0 = fully corrupted
        self.corrupted_message = self._corrupt_message(original_message, corruption_level)
        self.is_decoded = False
        self.partially_decoded_message = self.corrupted_message
        self.source = source

    def _corrupt_message(self, message: str, level: float) -> str:
        """Apply corruption to a message"""
        if level < 0.1:
            return message

        # Clamp level to avoid division issues
        level = min(level, 0.99)

        corrupted = ""
        corruption_interval = max(2, int(1/level + 1))
        for i, char in enumerate(message):
            if char == ' ':
                corrupted += ' '
            elif i % corruption_interval == 0:
                corrupted += '█'  # Block character for corruption
            else:
                corrupted += char
        return corrupted

    def decode(self, translator: Translator) -> bool:
        """Attempt to decode the signal"""
        if not translator.abilities[TranslatorAbilities.DECODE_SIGNALS]:
            return False

        self.is_decoded = True
        self.partially_decoded_message = self.original_message
        return True


# =============================================================================
# PENALIZED PATHWAY
# =============================================================================

class PenalizedPathway:
    """
    A pathway that penalizes non-standard approaches due to The Drift.
    Can be made safe by creating alternative interpretations.
    """

    def __init__(self, name: str, standard_requirement: str, penalty: float,
                 district: str = None):
        self.name = name
        self.standard_requirement = standard_requirement
        self.penalty = penalty  # Damage or slowdown for non-conforming
        self.alternatives_created: List[str] = []
        self.is_safe = False
        self.district = district

    def create_alternative(self, translator: Translator, alternative_name: str) -> bool:
        """Create an alternative pathway interpretation"""
        if not translator.abilities[TranslatorAbilities.CREATE_PATHWAYS]:
            return False

        self.alternatives_created.append(alternative_name)

        # Pathway becomes safe when alternatives exist
        if len(self.alternatives_created) > 0:
            self.is_safe = True

        return True


# =============================================================================
# ANTAGONISTIC PATTERNS
# =============================================================================

class AntagonisticPattern:
    """Base class for antagonistic patterns (not villains, but misdesign consequences)"""

    def __init__(self, name: str, intensity: float, position: Vector3 = None):
        self.name = name
        self.intensity = intensity
        self.position = position or Vector3()
        self.is_resolved = False
        self.resolution_progress = 0.0

    def receive_verb(self, verb: str) -> float:
        """Receive a combat verb and return resolution progress"""
        raise NotImplementedError

    def update(self, delta_time: float):
        """Update pattern state"""
        pass


class EchoForm(AntagonisticPattern):
    """Coercive social scripts given motion"""

    def __init__(self, script_name: str, intensity: float, position: Vector3 = None):
        super().__init__(f"Echo Form: {script_name}", intensity, position)
        self.script_name = script_name
        self.loop_phase = 0.0
        self.data = ANTAGONIST_DATA["echo_form"]

    def receive_verb(self, verb: str) -> float:
        """Thread Lash is primary resolver, Pulse is secondary"""
        if verb == CombatVerbs.THREAD_LASH:
            self.resolution_progress += 0.5
        elif verb == CombatVerbs.PULSE:
            self.resolution_progress += 0.25
        elif verb == CombatVerbs.RADIANT_HOLD:
            self.resolution_progress += 0.1  # Shields provide minor progress

        if self.resolution_progress >= 1.0:
            self.is_resolved = True

        return self.resolution_progress

    def update(self, delta_time: float):
        """Update echo form loop"""
        if not self.is_resolved:
            self.loop_phase = (self.loop_phase + delta_time) % 2.0


class Distortion(AntagonisticPattern):
    """Broken rules manifested physically"""

    def __init__(self, rule_name: str, intensity: float, position: Vector3 = None):
        super().__init__(f"Distortion: {rule_name}", intensity, position)
        self.rule_name = rule_name
        self.cycle_position = 0.0
        self.data = ANTAGONIST_DATA["distortion"]

    def receive_verb(self, verb: str) -> float:
        """Pulse is primary resolver, Edge Claim is secondary"""
        if verb == CombatVerbs.PULSE:
            self.resolution_progress += 0.5
            self.reset_cycle()
        elif verb == CombatVerbs.EDGE_CLAIM:
            self.resolution_progress += 0.3
        elif verb == CombatVerbs.THREAD_LASH:
            self.resolution_progress += 0.15

        if self.resolution_progress >= 1.0:
            self.is_resolved = True

        return self.resolution_progress

    def reset_cycle(self):
        """Reset the distortion's cycle"""
        self.cycle_position = 0.0

    def update(self, delta_time: float):
        """Update distortion cycle"""
        if not self.is_resolved:
            self.cycle_position = (self.cycle_position + delta_time * 0.5) % 1.0


class NoiseBeast(AntagonisticPattern):
    """Sensory overload as weather"""

    def __init__(self, overload_type: str, intensity: float, position: Vector3 = None):
        super().__init__(f"Noise Beast: {overload_type}", intensity, position)
        self.overload_type = overload_type
        self.storm_intensity = intensity
        self.data = ANTAGONIST_DATA["noise_beast"]

    def receive_verb(self, verb: str) -> float:
        """Re-tune is primary resolver, Radiant Hold is secondary"""
        if verb == CombatVerbs.RETUNE:
            self.resolution_progress += 0.4
            self.storm_intensity *= 0.7
        elif verb == CombatVerbs.RADIANT_HOLD:
            self.resolution_progress += 0.2
        elif verb == CombatVerbs.EDGE_CLAIM:
            self.resolution_progress += 0.1

        if self.resolution_progress >= 1.0:
            self.is_resolved = True
            self.storm_intensity = 0.0

        return self.resolution_progress

    def update(self, delta_time: float):
        """Update noise beast storm"""
        if not self.is_resolved:
            # Storm intensity fluctuates
            self.storm_intensity = min(1.0, self.storm_intensity + delta_time * 0.1)


# =============================================================================
# DISTRICT
# =============================================================================

class District:
    """Represents a district of Spiny Flannel Society"""

    def __init__(self, district_id: str):
        self.id = district_id
        self.data = DISTRICT_DATA.get(district_id, {})
        self.name = self.data.get("name", district_id)
        self.description = self.data.get("description", "")
        self.color_theme = self.data.get("color_theme", "warm_amber")
        self.wind_pattern = self.data.get("wind_pattern", "stable")

        self.spaces: List[ContradictorySpace] = []
        self.signals: List[CorruptedSignal] = []
        self.pathways: List[PenalizedPathway] = []
        self.patterns: List[AntagonisticPattern] = []

        self.drift_intensity = DRIFT_INTENSITY_MAX
        self.is_restored = False

    def add_space(self, space: ContradictorySpace):
        space.district = self.id
        self.spaces.append(space)

    def add_pattern(self, pattern: AntagonisticPattern):
        self.patterns.append(pattern)

    def update_drift(self, global_drift: float):
        """Update district drift based on global state"""
        self.drift_intensity = global_drift
        for space in self.spaces:
            space.update_contradiction(global_drift)

    def get_active_patterns(self) -> List[AntagonisticPattern]:
        """Get unresolved antagonistic patterns"""
        return [p for p in self.patterns if not p.is_resolved]


# =============================================================================
# DESIGN TERMINAL
# =============================================================================

class DesignTerminal:
    """Interface for rewriting civic rules"""

    def __init__(self, terminal_id: str, rule_to_restore: str, position: Vector3 = None):
        self.id = terminal_id
        self.rule_to_restore = rule_to_restore
        self.rule_description = CIVIC_RULES.get(rule_to_restore, "Unknown rule")
        self.position = position or Vector3()
        self.is_activated = False
        self.is_complete = False
        self.options: List[str] = []

    def interact(self, translator: Translator) -> bool:
        """Interact with the terminal"""
        if translator.position.distance_to(self.position) > DESIGN_TERMINAL_INTERACTION_RANGE:
            return False

        self.is_activated = True
        return True

    def select_option(self, option: str, translator: Translator) -> bool:
        """Select a rewrite option"""
        if not self.is_activated:
            return False

        self.is_complete = True
        translator.windprint_record.civic_rules_restored.append(self.rule_to_restore)
        return True


# =============================================================================
# GAME WORLD
# =============================================================================

class GameWorld:
    """
    Represents the entire Spiny Flannel Society settlement
    """

    def __init__(self):
        self.narrative_state = NarrativeStates.THE_DRIFT
        self.drift_intensity = DRIFT_INTENSITY_MAX

        # Initialize districts
        self.districts: Dict[str, District] = {
            district_id: District(district_id)
            for district_id in [
                Districts.WINDGAP_ACADEMY, Districts.VEIL_MARKET,
                Districts.SANDSTONE_QUARTER, Districts.UMBEL_GARDENS,
                Districts.SMOKE_MARGIN, Districts.RELIQUARY_EDGE
            ]
        }

        # Legacy collections (for compatibility)
        self.contradictory_spaces: List[ContradictorySpace] = []
        self.corrupted_signals: List[CorruptedSignal] = []
        self.penalized_pathways: List[PenalizedPathway] = []

        # Design terminals
        self.design_terminals: List[DesignTerminal] = []

        # Progress
        self.wind_force = WIND_FORCE_BASE
        self.systems_restored = 0
        self.total_systems = TOTAL_CHAPTERS
        self.restored_rules: Set[str] = set()

        # Current chapter
        self.current_chapter = 1

    def get_district(self, district_id: str) -> Optional[District]:
        """Get a district by ID"""
        return self.districts.get(district_id)

    def add_contradictory_space(self, space: ContradictorySpace):
        """Add a contradictory space to the world"""
        self.contradictory_spaces.append(space)
        if space.district:
            district = self.districts.get(space.district)
            if district:
                district.add_space(space)

    def add_corrupted_signal(self, signal: CorruptedSignal):
        """Add a corrupted signal to the world"""
        self.corrupted_signals.append(signal)

    def add_penalized_pathway(self, pathway: PenalizedPathway):
        """Add a penalized pathway to the world"""
        self.penalized_pathways.append(pathway)

    def add_design_terminal(self, terminal: DesignTerminal):
        """Add a design terminal"""
        self.design_terminals.append(terminal)

    def restore_civic_rule(self, rule: str):
        """Restore a civic rule"""
        if rule not in self.restored_rules:
            self.restored_rules.add(rule)
            self.restore_system()

    def update_drift_intensity(self):
        """Update The Drift intensity based on restored systems"""
        restoration_progress = self.systems_restored / self.total_systems
        self.drift_intensity = DRIFT_INTENSITY_MAX * (1.0 - restoration_progress)

        # Update narrative state based on drift intensity
        if self.drift_intensity <= 0:
            self.narrative_state = NarrativeStates.PLURAL_COHERENCE
        elif self.drift_intensity < 0.2:
            self.narrative_state = NarrativeStates.AXIOM_RESTORING
        elif self.drift_intensity < 0.8:
            self.narrative_state = NarrativeStates.STANDARD_DEFAULTS
        else:
            self.narrative_state = NarrativeStates.THE_DRIFT

        # Update all districts
        for district in self.districts.values():
            district.update_drift(self.drift_intensity)

        # Update all contradictory spaces
        for space in self.contradictory_spaces:
            space.update_contradiction(self.drift_intensity)

    def restore_system(self):
        """Mark a system as restored"""
        self.systems_restored += 1
        self.update_drift_intensity()

    def advance_chapter(self):
        """Advance to the next chapter"""
        if self.current_chapter < TOTAL_CHAPTERS:
            self.current_chapter += 1

    def is_victory(self) -> bool:
        """Check if the player has won (Axiom restored)"""
        return self.systems_restored >= self.total_systems

    def get_chapter_data(self) -> Optional[ChapterData]:
        """Get data for the current chapter"""
        return CHAPTER_DATA.get(self.current_chapter)

    def get_current_district(self) -> Optional[District]:
        """Get the district for the current chapter"""
        chapter_data = self.get_chapter_data()
        if chapter_data:
            return self.districts.get(chapter_data.location)
        return None
