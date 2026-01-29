"""
Spiny Flannel Society - Platformer Mechanics
Core 3D platformer movement and interaction systems
"""

import math
import random
from typing import Optional, Tuple
from game_entities import *
from game_config import *


class PlatformerController:
    """Handles 3D platformer movement mechanics for the Translator"""
    
    def __init__(self, translator: Translator):
        self.translator = translator
        self.momentum = Vector3()
        self.wall_run_direction = Vector3()
        self.jump_buffer = 0.0  # Jump buffering for better feel
        self.coyote_time = 0.0  # Coyote time for forgiving platforming
    
    def move(self, direction: Vector3, delta_time: float):
        """Handle player movement input"""
        if not direction or direction.magnitude() < 0.001:
            return
        
        # Normalize direction
        normalized = direction.normalize()
        
        # Apply speed
        target_velocity = normalized * self.translator.speed
        
        # Smooth acceleration
        self.translator.velocity.x += (target_velocity.x - self.translator.velocity.x) * 0.2
        self.translator.velocity.z += (target_velocity.z - self.translator.velocity.z) * 0.2
        
        # Update position
        self.translator.position = self.translator.position + (
            self.translator.velocity * delta_time
        )
    
    def jump(self) -> bool:
        """Execute jump"""
        can_jump = (self.translator.is_grounded or 
                   self.coyote_time > 0 or 
                   self.translator.is_wall_running)
        
        if can_jump:
            self.translator.velocity.y = math.sqrt(
                2 * abs(GRAVITY) * self.translator.jump_height
            )
            self.translator.is_grounded = False
            self.translator.is_wall_running = False
            return True
        
        return False
    
    def start_wall_run(self, wall_normal: Vector3):
        """Start wall running"""
        self.translator.is_wall_running = True
        
        # Calculate wall run direction (perpendicular to wall normal and up)
        up = Vector3(0, 1, 0)
        self.wall_run_direction = Vector3(
            -wall_normal.z,
            0,
            wall_normal.x
        ).normalize()
        
        # Apply wall run speed
        self.translator.velocity = (
            self.wall_run_direction * self.translator.wall_run_speed
        )
    
    def end_wall_run(self):
        """End wall running"""
        if self.translator.is_wall_running:
            self.translator.is_wall_running = False
            # Kick off wall
            self.translator.velocity.y += 1.0
    
    def apply_gravity(self, delta_time: float, wind_force: Vector3 = None):
        """Apply gravity and wind forces"""
        if not self.translator.is_grounded and not self.translator.is_wall_running:
            self.translator.velocity.y += GRAVITY * delta_time
            
            # Apply wind force (key to the suspended settlement)
            if wind_force:
                self.translator.velocity = self.translator.velocity + (
                    wind_force * delta_time * 0.5
                )
    
    def update(self, delta_time: float):
        """Update platformer state"""
        # Update timers
        if self.jump_buffer > 0:
            self.jump_buffer = max(0, self.jump_buffer - delta_time)
        
        if self.coyote_time > 0 and not self.translator.is_grounded:
            self.coyote_time = max(0, self.coyote_time - delta_time)
        
        # Reset coyote time when grounded
        if self.translator.is_grounded:
            self.coyote_time = 0.15  # 150ms of grace period


class EnvironmentInteraction:
    """Handles interaction with environment objects and systems"""
    
    def __init__(self, translator: Translator, world: GameWorld):
        self.translator = translator
        self.world = world
    
    def scan_for_assumptions(self, radius: float = ASSUMPTION_SCAN_RADIUS) -> List[HiddenAssumption]:
        """Scan nearby environment for hidden assumptions"""
        if not self.translator.abilities[TranslatorAbilities.READ_ASSUMPTIONS]:
            return []
        
        nearby_assumptions = []
        
        # Check all contradictory spaces
        for space in self.world.contradictory_spaces:
            for assumption in space.assumptions:
                if not assumption.is_visible:
                    # In a real game, would check distance
                    nearby_assumptions.append(assumption)
        
        return nearby_assumptions
    
    def interact_with_space(self, space: ContradictorySpace) -> Optional[str]:
        """Interact with a contradictory space"""
        if space.resolve_assumptions():
            # All assumptions resolved, restore the system
            self.world.restore_system()
            self.translator.systems_restored += 1
            return f"System '{space.name}' restored! The Drift weakens."
        else:
            unresolved = [a for a in space.assumptions if not a.is_rewritten]
            return f"Space still contradicted. {len(unresolved)} assumptions remain."
    
    def interact_with_signal(self, signal: CorruptedSignal) -> Optional[str]:
        """Interact with a corrupted signal"""
        if signal.is_decoded:
            return signal.original_message
        
        if self.translator.abilities[TranslatorAbilities.DECODE_SIGNALS]:
            if signal.decode(self.translator):
                return f"Decoded: {signal.original_message}"
        
        return f"Corrupted signal: {signal.corrupted_message}"
    
    def interact_with_pathway(self, pathway: PenalizedPathway) -> Optional[str]:
        """Interact with a penalized pathway"""
        if pathway.is_safe:
            return f"Pathway '{pathway.name}' is safe to traverse."
        
        if self.translator.abilities[TranslatorAbilities.CREATE_PATHWAYS]:
            return f"Standard requirement: {pathway.standard_requirement}. Create alternative?"
        
        return f"Dangerous pathway. Penalty: {pathway.penalty}"


class HybridGameplay:
    """
    Combines platformer mechanics with environmental hacking/translation mechanics
    """
    
    def __init__(self):
        self.world = GameWorld()
        self.translator = Translator(Vector3(0, 10, 0))  # Start suspended
        self.controller = PlatformerController(self.translator)
        self.interaction = EnvironmentInteraction(self.translator, self.world)
        
        self.current_wind = Vector3(1.0, 0.2, 0.5)  # Simulated wind
        self.game_time = 0.0
    
    def update(self, delta_time: float):
        """Main game update loop"""
        self.game_time += delta_time
        
        # Update player
        self.translator.update(delta_time)
        self.controller.update(delta_time)
        
        # Apply physics
        self.controller.apply_gravity(delta_time, self.current_wind)
        
        # Update wind (varies with Drift intensity)
        self.update_wind()
        
        # Check victory
        if self.world.is_victory():
            return "VICTORY: The Spiny Flannel Axiom is restored!"
        
        return None
    
    def update_wind(self):
        """Update wind patterns based on world state"""
        pattern_key = "drifting" if self.world.drift_intensity > 0.5 else "restored"
        pattern = WIND_PATTERNS.get(pattern_key, WIND_PATTERNS["drifting"])
        
        # Simulate varying wind with some randomness
        variance = pattern["variance"]
        self.current_wind = Vector3(
            WIND_FORCE_BASE + random.uniform(-variance, variance),
            random.uniform(-variance * 0.3, variance * 0.3),
            random.uniform(-variance, variance)
        )
    
    def get_game_state(self) -> Dict:
        """Get current game state for display/debugging"""
        return {
            "narrative_state": self.world.narrative_state,
            "drift_intensity": self.world.drift_intensity,
            "systems_restored": f"{self.world.systems_restored}/{self.world.total_systems}",
            "player_position": f"({self.translator.position.x:.1f}, {self.translator.position.y:.1f}, {self.translator.position.z:.1f})",
            "translation_energy": f"{self.translator.translation_energy}/{self.translator.max_translation_energy}",
            "assumptions_read": len(self.translator.assumptions_read),
            "environments_rewritten": len(self.translator.environments_rewritten),
            "wind": f"({self.current_wind.x:.2f}, {self.current_wind.y:.2f}, {self.current_wind.z:.2f})"
        }
