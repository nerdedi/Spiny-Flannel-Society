"""
Spiny Flannel Society - Platformer Mechanics
Core 3D platformer movement and interaction systems.

Movement: Flow + Precision
- Triple hop (short → long → float)
- Air dash (upgrades to double)
- Wall run/kick
- Grapple thread to Botanical Nodes
- Glide/wind surf across canopy lines
- Pulse slam to interact with mechanisms
"""

import math
import random
from typing import Optional, Tuple, Dict, List

from game_entities import (
    Vector3, Translator, GameWorld, HiddenAssumption,
    ContradictorySpace, CorruptedSignal, PenalizedPathway,
    AntagonisticPattern
)
from game_config import (
    GRAVITY, WIND_FORCE_BASE, WIND_PATTERNS,
    ASSUMPTION_SCAN_RADIUS, TranslatorAbilities,
    TRIPLE_HOP_HEIGHTS, TRIPLE_HOP_WINDOW,
    COYOTE_TIME, JUMP_BUFFER_TIME,
    PLAYER_AIR_DASH_DISTANCE, PLAYER_GLIDE_SPEED
)
from windprint_rig import WindprintRig, apply_cushion_to_timing


# =============================================================================
# PLATFORMER CONTROLLER
# =============================================================================

class PlatformerController:
    """
    Handles 3D platformer movement mechanics for the Translator.

    Movement emphasises flow on main routes, with precision
    available for optional Elective challenges.
    """

    def __init__(self, translator: Translator, windprint_rig: WindprintRig = None):
        self.translator = translator
        self.windprint_rig = windprint_rig

        # Momentum and physics
        self.momentum = Vector3()
        self.wall_run_direction = Vector3()

        # Timing buffers (for accessible platforming)
        self.jump_buffer = 0.0
        self.coyote_time = 0.0

        # Triple hop state
        self.current_hop = 0  # 0 = none, 1-3 = hop number
        self.hop_timer = 0.0

        # Air dash state
        self.air_dashes_remaining = 1
        self.max_air_dashes = 1  # Upgrades to 2

        # Glide state
        self.is_gliding = False
        self.glide_stamina = 100.0

        # Grapple state
        self.grapple_target: Optional[Vector3] = None
        self.is_grappling = False

    def move(self, direction: Vector3, delta_time: float):
        """Handle player movement input"""
        if not direction or direction.magnitude() < 0.001:
            return

        # Normalize direction
        normalized = direction.normalize()

        # Apply speed (modified by Cushion mode if active)
        speed = self.translator.speed
        if self.windprint_rig and self.windprint_rig.is_cushion_active():
            # Cushion mode provides smoother movement
            speed *= 0.9

        target_velocity = normalized * speed

        # Smooth acceleration
        lerp_factor = 0.2 if self.translator.is_grounded else 0.1
        self.translator.velocity.x += (target_velocity.x - self.translator.velocity.x) * lerp_factor
        self.translator.velocity.z += (target_velocity.z - self.translator.velocity.z) * lerp_factor

        # Update position
        self.translator.position = self.translator.position + (
            self.translator.velocity * delta_time
        )

    def jump(self) -> bool:
        """Execute jump - supports triple hop mechanic"""
        can_jump = (
            self.translator.is_grounded or
            self.coyote_time > 0 or
            self.translator.is_wall_running
        )

        # Check for buffered jump
        if not can_jump and self.jump_buffer > 0:
            return False

        if can_jump:
            # Determine hop height
            if self.translator.is_grounded and self.current_hop < 3:
                self.current_hop += 1
                self.hop_timer = TRIPLE_HOP_WINDOW
            else:
                self.current_hop = 1
                self.hop_timer = TRIPLE_HOP_WINDOW

            # Get hop height (modified by Cushion if active)
            hop_height = TRIPLE_HOP_HEIGHTS[min(self.current_hop - 1, 2)]

            if self.windprint_rig and self.windprint_rig.is_cushion_active():
                # Cushion mode gives slightly more forgiving jumps
                hop_height *= 1.1

            # Calculate jump velocity
            self.translator.velocity.y = math.sqrt(2 * abs(GRAVITY) * hop_height)

            # Handle wall kick
            if self.translator.is_wall_running:
                # Add horizontal kick
                kick_direction = self.wall_run_direction * -1
                self.translator.velocity = self.translator.velocity + (kick_direction * 3.0)

            self.translator.is_grounded = False
            self.translator.is_wall_running = False
            self.coyote_time = 0

            # Reset air dashes when jumping from ground
            if not self.translator.is_wall_running:
                self.air_dashes_remaining = self.max_air_dashes

            return True

        # Buffer the jump input
        self.jump_buffer = JUMP_BUFFER_TIME
        return False

    def air_dash(self, direction: Vector3) -> bool:
        """Execute air dash"""
        if self.translator.is_grounded or self.air_dashes_remaining <= 0:
            return False

        if direction.magnitude() < 0.001:
            direction = Vector3(1, 0, 0)  # Default forward

        normalized = direction.normalize()

        # Apply dash
        dash_speed = self.translator.air_dash_speed
        self.translator.velocity = normalized * dash_speed
        self.translator.velocity.y = 0  # Horizontal dash

        self.air_dashes_remaining -= 1
        self.translator.is_gliding = False

        return True

    def start_wall_run(self, wall_normal: Vector3):
        """Start wall running"""
        self.translator.is_wall_running = True
        self.translator.is_gliding = False

        # Calculate wall run direction (perpendicular to wall normal and up)
        self.wall_run_direction = Vector3(
            -wall_normal.z,
            0,
            wall_normal.x
        ).normalize()

        # Apply wall run speed
        self.translator.velocity = (
            self.wall_run_direction * self.translator.wall_run_speed
        )
        # Slight upward movement
        self.translator.velocity.y = 1.0

    def end_wall_run(self):
        """End wall running"""
        if self.translator.is_wall_running:
            self.translator.is_wall_running = False

    def start_glide(self) -> bool:
        """Start gliding/wind surfing"""
        if self.translator.is_grounded or self.glide_stamina <= 0:
            return False

        self.translator.is_gliding = True
        self.is_gliding = True
        return True

    def end_glide(self):
        """End gliding"""
        self.translator.is_gliding = False
        self.is_gliding = False

    def start_grapple(self, target: Vector3) -> bool:
        """Start grappling to a Botanical Node"""
        if self.is_grappling:
            return False

        self.grapple_target = target
        self.is_grappling = True
        self.translator.is_grappling = True

        # Calculate grapple direction
        direction = (target - self.translator.position).normalize()
        self.translator.velocity = direction * self.translator.grapple_speed

        return True

    def update_grapple(self, delta_time: float):
        """Update grapple state"""
        if not self.is_grappling or not self.grapple_target:
            return

        distance = self.translator.position.distance_to(self.grapple_target)

        if distance < 1.0:
            # Reached target
            self.end_grapple()

    def end_grapple(self):
        """End grappling"""
        self.is_grappling = False
        self.translator.is_grappling = False
        self.grapple_target = None
        # Preserve some momentum
        self.translator.velocity = self.translator.velocity * 0.5

    def apply_gravity(self, delta_time: float, wind_force: Vector3 = None):
        """Apply gravity and wind forces"""
        if self.translator.is_grounded:
            return

        if self.translator.is_wall_running:
            # Reduced gravity while wall running
            self.translator.velocity.y += GRAVITY * delta_time * 0.3
            return

        if self.translator.is_gliding and self.glide_stamina > 0:
            # Gliding: very slow fall
            self.translator.velocity.y = max(
                self.translator.velocity.y + GRAVITY * delta_time * 0.1,
                -2.0  # Terminal glide velocity
            )
            self.glide_stamina -= delta_time * 20
        else:
            # Normal gravity
            self.translator.velocity.y += GRAVITY * delta_time

        # Apply wind force
        if wind_force:
            wind_multiplier = 0.5

            # Cushion mode reduces wind impact
            if self.windprint_rig and self.windprint_rig.is_cushion_active():
                wind_multiplier *= self.windprint_rig.cushion.get_wind_reduction()

            self.translator.velocity = self.translator.velocity + (
                wind_force * delta_time * wind_multiplier
            )

    def update(self, delta_time: float):
        """Update platformer state"""
        # Update jump buffer
        if self.jump_buffer > 0:
            self.jump_buffer = max(0, self.jump_buffer - delta_time)
            # Check if we can execute buffered jump
            if self.translator.is_grounded:
                self.jump()

        # Update coyote time
        if self.coyote_time > 0 and not self.translator.is_grounded:
            self.coyote_time = max(0, self.coyote_time - delta_time)

        # Reset coyote time when grounded
        if self.translator.is_grounded:
            coyote_duration = COYOTE_TIME
            # Cushion mode extends coyote time
            if self.windprint_rig and self.windprint_rig.is_cushion_active():
                coyote_duration *= self.windprint_rig.get_timing_multiplier()
            self.coyote_time = coyote_duration
            self.air_dashes_remaining = self.max_air_dashes
            self.glide_stamina = min(100, self.glide_stamina + delta_time * 30)

        # Update triple hop timer
        if self.hop_timer > 0:
            hop_window = TRIPLE_HOP_WINDOW
            if self.windprint_rig and self.windprint_rig.is_cushion_active():
                hop_window *= self.windprint_rig.get_timing_multiplier()
            self.hop_timer = max(0, self.hop_timer - delta_time)
            if self.hop_timer == 0:
                self.current_hop = 0

        # Update grapple
        if self.is_grappling:
            self.update_grapple(delta_time)


# =============================================================================
# ENVIRONMENT INTERACTION
# =============================================================================

class EnvironmentInteraction:
    """Handles interaction with environment objects and systems"""

    def __init__(self, translator: Translator, world: GameWorld,
                 windprint_rig: WindprintRig = None):
        self.translator = translator
        self.world = world
        self.windprint_rig = windprint_rig

    def scan_for_assumptions(self, radius: float = ASSUMPTION_SCAN_RADIUS) -> List[HiddenAssumption]:
        """Scan nearby environment for hidden assumptions"""
        if not self.translator.abilities[TranslatorAbilities.READ_ASSUMPTIONS]:
            return []

        nearby_assumptions = []

        # Check all contradictory spaces
        for space in self.world.contradictory_spaces:
            for assumption in space.assumptions:
                if not assumption.is_visible:
                    nearby_assumptions.append(assumption)

        # Check current district
        current_district = self.world.get_current_district()
        if current_district:
            for space in current_district.spaces:
                for assumption in space.assumptions:
                    if not assumption.is_visible:
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

    def interact_with_design_terminal(self, terminal) -> Optional[str]:
        """Interact with a design terminal"""
        if terminal.interact(self.translator):
            return f"Design Terminal activated. Rule to restore: {terminal.rule_description}"
        return "Cannot access terminal from here."


# =============================================================================
# HYBRID GAMEPLAY
# =============================================================================

class HybridGameplay:
    """
    Combines platformer mechanics with environmental hacking/translation mechanics.
    Main game controller that ties all systems together.
    """

    def __init__(self):
        self.world = GameWorld()
        self.translator = Translator(Vector3(0, 10, 0))  # Start suspended

        # Initialize Windprint Rig
        self.windprint_rig = WindprintRig(self.translator)

        # Initialize systems with rig
        self.controller = PlatformerController(self.translator, self.windprint_rig)
        self.interaction = EnvironmentInteraction(
            self.translator, self.world, self.windprint_rig
        )

        # Wind simulation
        self.current_wind = Vector3(1.0, 0.2, 0.5)
        self.game_time = 0.0

        # Unlock initial abilities
        self.translator.unlock_ability(TranslatorAbilities.REWRITE_ENVIRONMENT)
        self.translator.unlock_ability(TranslatorAbilities.CUSHION_MODE)
        self.translator.unlock_ability(TranslatorAbilities.GUARD_MODE)

    def update(self, delta_time: float):
        """Main game update loop"""
        self.game_time += delta_time

        # Update player
        self.translator.update(delta_time)
        self.windprint_rig.update(delta_time)
        self.controller.update(delta_time)

        # Apply physics
        self.controller.apply_gravity(delta_time, self.current_wind)

        # Update wind (varies with Drift intensity)
        self.update_wind()

        # Check victory
        if self.world.is_victory():
            return "VICTORY: The Spiny Flannel Axiom is restored! Plural coherence achieved."

        return None

    def update_wind(self):
        """Update wind patterns based on world state"""
        pattern_key = "drifting" if self.world.drift_intensity > 0.5 else "restored"
        pattern = WIND_PATTERNS.get(pattern_key, WIND_PATTERNS["drifting"])

        # Get district-specific wind if available
        current_district = self.world.get_current_district()
        if current_district:
            district_pattern = WIND_PATTERNS.get(
                current_district.wind_pattern, pattern
            )
            pattern = district_pattern

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
            "drift_intensity": f"{self.world.drift_intensity:.1%}",
            "current_chapter": self.world.current_chapter,
            "systems_restored": f"{self.world.systems_restored}/{self.world.total_systems}",
            "player_position": str(self.translator.position),
            "translation_energy": f"{self.translator.translation_energy:.0f}/{self.translator.max_translation_energy}",
            "windprint_energy": f"{self.windprint_rig.energy:.0f}/{self.windprint_rig.max_energy}",
            "windprint_mode": self.windprint_rig.current_mode or "None",
            "assumptions_read": len(self.translator.assumptions_read),
            "environments_rewritten": len(self.translator.environments_rewritten),
            "wind": f"({self.current_wind.x:.2f}, {self.current_wind.y:.2f}, {self.current_wind.z:.2f})"
        }
