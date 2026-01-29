"""
Spiny Flannel Society - Core Game Entities
Defines the main game objects: Player (Translator), Environment, and Systems
"""

import math
from typing import List, Dict, Tuple, Optional
from game_config import *


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
    
    def __mul__(self, scalar: float) -> 'Vector3':
        return Vector3(self.x * scalar, self.y * scalar, self.z * scalar)


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


class Translator:
    """
    The player character - a Translator who can read hidden assumptions
    and rewrite environments affected by The Drift
    """
    def __init__(self, position: Vector3 = None):
        self.position = position or Vector3(0, 0, 0)
        self.velocity = Vector3()
        
        # Player stats
        self.speed = PLAYER_SPEED
        self.jump_height = PLAYER_JUMP_HEIGHT
        self.wall_run_speed = PLAYER_WALL_RUN_SPEED
        
        # Translator abilities
        self.abilities = {
            TranslatorAbilities.READ_ASSUMPTIONS: True,
            TranslatorAbilities.REWRITE_ENVIRONMENT: False,
            TranslatorAbilities.DECODE_SIGNALS: False,
            TranslatorAbilities.CREATE_PATHWAYS: False
        }
        
        # Resources
        self.translation_energy = 100
        self.max_translation_energy = 100
        
        # State
        self.is_grounded = False
        self.is_wall_running = False
        self.rewrite_cooldown = 0.0
        
        # Progress tracking
        self.assumptions_read = []
        self.environments_rewritten = []
        self.systems_restored = 0
    
    def unlock_ability(self, ability: str):
        """Unlock a new Translator ability"""
        if ability in self.abilities:
            self.abilities[ability] = True
    
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
    
    def update(self, delta_time: float):
        """Update player state"""
        # Update cooldowns
        if self.rewrite_cooldown > 0:
            self.rewrite_cooldown = max(0, self.rewrite_cooldown - delta_time)
        
        # Regenerate translation energy
        if self.translation_energy < self.max_translation_energy:
            self.translation_energy = min(
                self.max_translation_energy,
                self.translation_energy + delta_time * 5  # 5 energy per second
            )


class ContradictorySpace:
    """
    A space affected by The Drift that contradicts itself.
    Properties change based on how rigidly the player conforms to Standard Defaults
    """
    def __init__(self, name: str, base_form: Dict, alternate_form: Dict):
        self.name = name
        self.base_form = base_form  # Form under Standard Defaults
        self.alternate_form = alternate_form  # True form (under Axiom)
        self.current_contradiction_level = 1.0  # 1.0 = fully contradicted
        self.assumptions = []
    
    def add_assumption(self, assumption: HiddenAssumption):
        """Add a hidden assumption that affects this space"""
        self.assumptions.append(assumption)
    
    def update_contradiction(self, drift_intensity: float):
        """Update how contradicted the space is based on Drift intensity"""
        self.current_contradiction_level = drift_intensity
    
    def resolve_assumptions(self):
        """Check if all assumptions have been rewritten"""
        return all(assumption.is_rewritten for assumption in self.assumptions)


class CorruptedSignal:
    """
    A signal or communication corrupted by The Drift.
    Must be decoded by the Translator to reveal its true meaning
    """
    def __init__(self, original_message: str, corruption_level: float):
        self.original_message = original_message
        self.corruption_level = corruption_level  # 0.0 = clean, 1.0 = fully corrupted
        self.corrupted_message = self._corrupt_message(original_message, corruption_level)
        self.is_decoded = False
        self.partially_decoded_message = self.corrupted_message
    
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


class PenalizedPathway:
    """
    A pathway that penalizes non-standard approaches due to The Drift.
    Can be made safe by creating alternative interpretations
    """
    def __init__(self, name: str, standard_requirement: str, penalty: float):
        self.name = name
        self.standard_requirement = standard_requirement
        self.penalty = penalty  # Damage or slowdown for non-conforming
        self.alternatives_created = []
        self.is_safe = False
    
    def create_alternative(self, translator: Translator, alternative_name: str) -> bool:
        """Create an alternative pathway interpretation"""
        if not translator.abilities[TranslatorAbilities.CREATE_PATHWAYS]:
            return False
        
        self.alternatives_created.append(alternative_name)
        
        # Pathway becomes safe when alternatives exist
        if len(self.alternatives_created) > 0:
            self.is_safe = True
        
        return True


class GameWorld:
    """
    Represents the entire Spiny Flannel Society settlement
    """
    def __init__(self):
        self.narrative_state = NarrativeStates.THE_DRIFT
        self.drift_intensity = DRIFT_INTENSITY_MAX
        
        self.contradictory_spaces: List[ContradictorySpace] = []
        self.corrupted_signals: List[CorruptedSignal] = []
        self.penalized_pathways: List[PenalizedPathway] = []
        
        self.wind_force = WIND_FORCE_BASE
        self.systems_restored = 0
        self.total_systems = SYSTEMS_TO_RESTORE
    
    def add_contradictory_space(self, space: ContradictorySpace):
        """Add a contradictory space to the world"""
        self.contradictory_spaces.append(space)
    
    def add_corrupted_signal(self, signal: CorruptedSignal):
        """Add a corrupted signal to the world"""
        self.corrupted_signals.append(signal)
    
    def add_penalized_pathway(self, pathway: PenalizedPathway):
        """Add a penalized pathway to the world"""
        self.penalized_pathways.append(pathway)
    
    def update_drift_intensity(self):
        """Update The Drift intensity based on restored systems"""
        restoration_progress = self.systems_restored / self.total_systems
        self.drift_intensity = DRIFT_INTENSITY_MAX * (1.0 - restoration_progress)
        
        # Update narrative state based on drift intensity
        if self.drift_intensity < 0.2:
            self.narrative_state = NarrativeStates.AXIOM_RESTORING
        elif self.drift_intensity < 0.8:
            self.narrative_state = NarrativeStates.STANDARD_DEFAULTS
        else:
            self.narrative_state = NarrativeStates.THE_DRIFT
        
        # Update all contradictory spaces
        for space in self.contradictory_spaces:
            space.update_contradiction(self.drift_intensity)
    
    def restore_system(self):
        """Mark a system as restored"""
        self.systems_restored += 1
        self.update_drift_intensity()
    
    def is_victory(self) -> bool:
        """Check if the player has won (Axiom restored)"""
        return self.systems_restored >= self.total_systems
