"""
Spiny Flannel Society - Windprint Rig System
The Translator's signature dual-mode tool for environmental interaction.

Cushion Mode: Softness and accessibility - widens timing, reduces clutter
Guard Mode: Protection and boundaries - pins rhythms, creates consent gates

The Society becomes coherent when both are used: softness + protection.
"""

from typing import Optional, Dict, List, Callable
from dataclasses import dataclass
from enum import Enum, auto

from game_config import (
    WindprintModes, CUSHION_MODE_EFFECTS, GUARD_MODE_EFFECTS,
    WINDPRINT_ENERGY_MAX, WINDPRINT_ENERGY_REGEN, WINDPRINT_MODE_SWITCH_COST
)
from game_entities import Vector3, Translator


# =============================================================================
# MODE EFFECTS
# =============================================================================

@dataclass
class ModeEffect:
    """Represents an active effect from a Windprint mode"""
    name: str
    value: float
    duration: float  # -1 for permanent while mode active
    remaining: float

    def update(self, delta_time: float) -> bool:
        """Update effect, return True if still active"""
        if self.duration < 0:
            return True
        self.remaining -= delta_time
        return self.remaining > 0


class CushionMode:
    """
    Cushion Mode - Softness and accessibility

    Effects:
    - Widens timing windows (50% more forgiving)
    - Spawns safe pockets during hazards
    - Reduces visual clutter (60%)
    - Slows hazards (50% speed)
    - Reduces wind impact
    """

    MODE_NAME = WindprintModes.CUSHION

    def __init__(self):
        self.is_active = False
        self.effects = CUSHION_MODE_EFFECTS.copy()
        self.active_safe_pockets: List[Vector3] = []
        self.timing_multiplier = self.effects["timing_window_multiplier"]

    def activate(self):
        """Activate cushion mode"""
        self.is_active = True
        self.active_safe_pockets.clear()

    def deactivate(self):
        """Deactivate cushion mode"""
        self.is_active = False
        self.active_safe_pockets.clear()

    def get_timing_multiplier(self) -> float:
        """Get the timing window multiplier"""
        return self.timing_multiplier if self.is_active else 1.0

    def get_hazard_speed_multiplier(self) -> float:
        """Get hazard speed multiplier (lower = slower hazards)"""
        return self.effects["hazard_slowdown"] if self.is_active else 1.0

    def get_clutter_reduction(self) -> float:
        """Get visual clutter reduction amount"""
        return self.effects["clutter_reduction"] if self.is_active else 0.0

    def get_wind_reduction(self) -> float:
        """Get wind impact reduction"""
        return self.effects["wind_impact_reduction"] if self.is_active else 1.0

    def should_spawn_safe_pocket(self) -> bool:
        """Check if a safe pocket should spawn (based on spawn rate)"""
        if not self.is_active:
            return False
        import random
        return random.random() < self.effects["safe_pocket_spawn_rate"]

    def spawn_safe_pocket(self, position: Vector3):
        """Spawn a safe pocket at the given position"""
        if self.is_active:
            self.active_safe_pockets.append(position)

    def is_in_safe_pocket(self, position: Vector3, pocket_radius: float = 2.0) -> bool:
        """Check if a position is within a safe pocket"""
        for pocket in self.active_safe_pockets:
            if position.distance_to(pocket) <= pocket_radius:
                return True
        return False

    def update(self, delta_time: float):
        """Update cushion mode state"""
        # Safe pockets could decay over time if needed
        pass


class GuardMode:
    """
    Guard Mode - Protection and boundaries

    Effects:
    - Pins platform rhythms (stabilises erratic timing)
    - Stabilises environmental jitter (90% reduction)
    - Creates consent gates (require confirmation before danger)
    - Claims boundary edges (pins them in place)
    """

    MODE_NAME = WindprintModes.GUARD

    def __init__(self):
        self.is_active = False
        self.effects = GUARD_MODE_EFFECTS.copy()
        self.pinned_rhythms: Dict[str, float] = {}  # entity_id -> pinned_phase
        self.consent_gates: List['ConsentGate'] = []
        self.claimed_edges: List['ClaimedEdge'] = []

    def activate(self):
        """Activate guard mode"""
        self.is_active = True

    def deactivate(self):
        """Deactivate guard mode"""
        self.is_active = False
        self.pinned_rhythms.clear()
        self.consent_gates.clear()
        self.claimed_edges.clear()

    def get_rhythm_pin_strength(self) -> float:
        """Get rhythm stabilisation strength"""
        return self.effects["rhythm_pin_strength"] if self.is_active else 0.0

    def get_jitter_stabilisation(self) -> float:
        """Get environmental jitter reduction"""
        return self.effects["jitter_stabilisation"] if self.is_active else 0.0

    def are_consent_gates_active(self) -> bool:
        """Check if consent gates are active"""
        return self.is_active and self.effects["consent_gate_active"]

    def get_edge_claim_range(self) -> float:
        """Get range for edge claiming"""
        return self.effects["edge_claim_range"] if self.is_active else 0.0

    def pin_rhythm(self, entity_id: str, current_phase: float):
        """Pin an entity's rhythm at its current phase"""
        if self.is_active:
            self.pinned_rhythms[entity_id] = current_phase

    def get_pinned_rhythm(self, entity_id: str) -> Optional[float]:
        """Get the pinned rhythm for an entity"""
        return self.pinned_rhythms.get(entity_id)

    def create_consent_gate(self, position: Vector3, danger_description: str):
        """Create a consent gate before a dangerous area"""
        if self.is_active:
            gate = ConsentGate(position, danger_description)
            self.consent_gates.append(gate)
            return gate
        return None

    def claim_edge(self, position: Vector3, edge_direction: Vector3):
        """Claim and stabilise a boundary edge"""
        if self.is_active:
            edge = ClaimedEdge(
                position,
                edge_direction,
                self.effects["boundary_hold_duration"]
            )
            self.claimed_edges.append(edge)
            return edge
        return None

    def update(self, delta_time: float):
        """Update guard mode state"""
        # Update claimed edges (they decay over time)
        self.claimed_edges = [
            edge for edge in self.claimed_edges
            if edge.update(delta_time)
        ]


# =============================================================================
# SUPPORT STRUCTURES
# =============================================================================

class ConsentGate:
    """
    A gate that requires player confirmation before proceeding into danger.
    Embodies the principle: boundaries are instructions for safety.
    """

    def __init__(self, position: Vector3, danger_description: str):
        self.position = position
        self.danger_description = danger_description
        self.is_confirmed = False
        self.confirmation_options = [
            "Proceed (I understand the risk)",
            "Show alternative route",
            "Wait (let me prepare)"
        ]

    def confirm(self, option_index: int = 0) -> str:
        """Confirm passage through the gate"""
        self.is_confirmed = True
        return self.confirmation_options[min(option_index, len(self.confirmation_options) - 1)]

    def can_pass(self) -> bool:
        """Check if the player can pass through"""
        return self.is_confirmed


class ClaimedEdge:
    """
    A boundary edge that has been claimed and stabilised.
    Prevents the environment from shifting unexpectedly.
    """

    def __init__(self, position: Vector3, direction: Vector3, duration: float):
        self.position = position
        self.direction = direction.normalize()
        self.duration = duration
        self.remaining = duration
        self.stability = 1.0

    def update(self, delta_time: float) -> bool:
        """Update edge state, return True if still active"""
        self.remaining -= delta_time
        self.stability = max(0, self.remaining / self.duration)
        return self.remaining > 0

    def is_stable(self) -> bool:
        """Check if edge is still stable"""
        return self.stability > 0.5


# =============================================================================
# WINDPRINT RIG
# =============================================================================

class WindprintRig:
    """
    The Translator's signature dual-mode tool.

    Cushion Mode: Softness and accessibility
    Guard Mode: Protection and boundaries

    The Society becomes coherent when both are used together.
    """

    def __init__(self, translator: Translator):
        self.translator = translator
        self.cushion = CushionMode()
        self.guard = GuardMode()
        self.current_mode: Optional[str] = None

        # Energy system
        self.energy = WINDPRINT_ENERGY_MAX
        self.max_energy = WINDPRINT_ENERGY_MAX
        self.energy_regen_rate = WINDPRINT_ENERGY_REGEN

        # Mode usage tracking (for Windprint recording)
        self.mode_activations = {
            WindprintModes.CUSHION: 0,
            WindprintModes.GUARD: 0
        }

    def can_switch_mode(self) -> bool:
        """Check if mode can be switched"""
        return self.energy >= WINDPRINT_MODE_SWITCH_COST

    def activate_cushion(self) -> bool:
        """Activate Cushion Mode"""
        if not self.translator.abilities.get('cushion_mode', False):
            # Allow if not explicitly locked
            pass

        if self.current_mode == WindprintModes.CUSHION:
            return True  # Already active

        if not self.can_switch_mode():
            return False

        # Deactivate other mode
        if self.current_mode == WindprintModes.GUARD:
            self.guard.deactivate()

        self.current_mode = WindprintModes.CUSHION
        self.cushion.activate()
        self.energy -= WINDPRINT_MODE_SWITCH_COST
        self.mode_activations[WindprintModes.CUSHION] += 1

        # Record for Windprint
        self.translator.windprint_record.record_mode_use(WindprintModes.CUSHION)

        return True

    def activate_guard(self) -> bool:
        """Activate Guard Mode"""
        if not self.translator.abilities.get('guard_mode', False):
            # Allow if not explicitly locked
            pass

        if self.current_mode == WindprintModes.GUARD:
            return True  # Already active

        if not self.can_switch_mode():
            return False

        # Deactivate other mode
        if self.current_mode == WindprintModes.CUSHION:
            self.cushion.deactivate()

        self.current_mode = WindprintModes.GUARD
        self.guard.activate()
        self.energy -= WINDPRINT_MODE_SWITCH_COST
        self.mode_activations[WindprintModes.GUARD] += 1

        # Record for Windprint
        self.translator.windprint_record.record_mode_use(WindprintModes.GUARD)

        return True

    def deactivate(self):
        """Deactivate all modes"""
        if self.current_mode == WindprintModes.CUSHION:
            self.cushion.deactivate()
        elif self.current_mode == WindprintModes.GUARD:
            self.guard.deactivate()
        self.current_mode = None

    def toggle_mode(self) -> str:
        """Toggle between modes"""
        if self.current_mode == WindprintModes.CUSHION:
            self.activate_guard()
            return WindprintModes.GUARD
        else:
            self.activate_cushion()
            return WindprintModes.CUSHION

    def get_timing_multiplier(self) -> float:
        """Get current timing window multiplier"""
        if self.current_mode == WindprintModes.CUSHION:
            return self.cushion.get_timing_multiplier()
        return 1.0

    def get_hazard_multiplier(self) -> float:
        """Get current hazard speed multiplier"""
        if self.current_mode == WindprintModes.CUSHION:
            return self.cushion.get_hazard_speed_multiplier()
        return 1.0

    def get_jitter_reduction(self) -> float:
        """Get current jitter reduction"""
        if self.current_mode == WindprintModes.GUARD:
            return self.guard.get_jitter_stabilisation()
        return 0.0

    def is_cushion_active(self) -> bool:
        """Check if Cushion Mode is active"""
        return self.current_mode == WindprintModes.CUSHION

    def is_guard_active(self) -> bool:
        """Check if Guard Mode is active"""
        return self.current_mode == WindprintModes.GUARD

    def update(self, delta_time: float):
        """Update rig state"""
        # Regenerate energy when not at max
        if self.energy < self.max_energy:
            self.energy = min(
                self.max_energy,
                self.energy + self.energy_regen_rate * delta_time
            )

        # Update active mode
        if self.current_mode == WindprintModes.CUSHION:
            self.cushion.update(delta_time)
        elif self.current_mode == WindprintModes.GUARD:
            self.guard.update(delta_time)

    def get_status(self) -> Dict:
        """Get current rig status"""
        return {
            "current_mode": self.current_mode or "None",
            "energy": f"{self.energy:.0f}/{self.max_energy}",
            "cushion_activations": self.mode_activations[WindprintModes.CUSHION],
            "guard_activations": self.mode_activations[WindprintModes.GUARD],
            "timing_multiplier": self.get_timing_multiplier(),
            "hazard_multiplier": self.get_hazard_multiplier(),
            "jitter_reduction": self.get_jitter_reduction()
        }


# =============================================================================
# HELPER FUNCTIONS
# =============================================================================

def apply_cushion_to_timing(base_timing: float, rig: WindprintRig) -> float:
    """Apply Cushion Mode timing extension to a timing window"""
    return base_timing * rig.get_timing_multiplier()


def apply_guard_to_rhythm(base_rhythm: float, rig: WindprintRig, entity_id: str) -> float:
    """Apply Guard Mode rhythm pinning"""
    if rig.is_guard_active():
        pinned = rig.guard.get_pinned_rhythm(entity_id)
        if pinned is not None:
            # Blend between base and pinned based on pin strength
            strength = rig.guard.get_rhythm_pin_strength()
            return base_rhythm * (1 - strength) + pinned * strength
    return base_rhythm


def create_safe_environment(rig: WindprintRig, position: Vector3) -> Dict:
    """
    Create a safe environment configuration using the Windprint Rig.
    Combines Cushion softness with Guard protection.
    """
    env = {
        "timing_window": 1.0,
        "hazard_speed": 1.0,
        "clutter_level": 1.0,
        "jitter_level": 1.0,
        "consent_required": False,
        "safe_pockets": []
    }

    if rig.is_cushion_active():
        env["timing_window"] = rig.cushion.get_timing_multiplier()
        env["hazard_speed"] = rig.cushion.get_hazard_speed_multiplier()
        env["clutter_level"] = 1.0 - rig.cushion.get_clutter_reduction()
        if rig.cushion.should_spawn_safe_pocket():
            rig.cushion.spawn_safe_pocket(position)
        env["safe_pockets"] = rig.cushion.active_safe_pockets.copy()

    if rig.is_guard_active():
        env["jitter_level"] = 1.0 - rig.guard.get_jitter_stabilisation()
        env["consent_required"] = rig.guard.are_consent_gates_active()

    return env
