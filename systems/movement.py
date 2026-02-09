"""
Spiny Flannel Society — Abstract Traversal Rules
Movement verbs and traversal logic (not a physics engine).

This models *what the player can do*, not how pixels move on screen.
An engine implementation (Godot / Unity) would consume these rules.
"""

from typing import Optional
from dataclasses import dataclass


@dataclass
class TraversalState:
    """Snapshot of the player's traversal state each tick."""
    is_grounded: bool = True
    is_wall_running: bool = False
    is_gliding: bool = False
    is_grappling: bool = False
    current_hop: int = 0          # 0 = none, 1–3 = triple hop
    air_dashes_remaining: int = 1
    coyote_timer: float = 0.0
    jump_buffer: float = 0.0
    glide_stamina: float = 100.0


class MovementRules:
    """
    Pure rule layer describing movement verbs.

    Every method returns a result dict describing what *should* happen.
    An engine adapter turns these into physics / animation calls.
    """

    # Tuning — these pull from DefaultsRegistry at runtime
    BASE_SPEED = 5.0
    TRIPLE_HOP_HEIGHTS = (1.5, 2.5, 3.5)
    TRIPLE_HOP_WINDOW = 0.5       # seconds to chain hops
    AIR_DASH_DISTANCE = 4.0
    WALL_RUN_SPEED = 4.0
    GLIDE_SPEED = 3.0
    GRAPPLE_SPEED = 10.0

    @staticmethod
    def can_jump(state: TraversalState) -> bool:
        return state.is_grounded or state.coyote_timer > 0 or state.is_wall_running

    @staticmethod
    def resolve_hop(state: TraversalState) -> dict:
        """Determine hop height from triple-hop chain."""
        if state.current_hop < 3:
            hop = state.current_hop + 1
        else:
            hop = 1
        height = MovementRules.TRIPLE_HOP_HEIGHTS[min(hop - 1, 2)]
        return {"hop_number": hop, "height": height}

    @staticmethod
    def can_air_dash(state: TraversalState) -> bool:
        return not state.is_grounded and state.air_dashes_remaining > 0

    @staticmethod
    def can_wall_run(state: TraversalState) -> bool:
        return not state.is_grounded and not state.is_wall_running

    @staticmethod
    def can_glide(state: TraversalState) -> bool:
        return not state.is_grounded and state.glide_stamina > 0

    @staticmethod
    def can_grapple(state: TraversalState) -> bool:
        return not state.is_grappling

    @staticmethod
    def apply_cushion_modifiers(result: dict, cushion_active: bool) -> dict:
        """
        When Cushion mode is active, traversal becomes more forgiving.
        Trade-off: increases entropy elsewhere (see windprint.py).
        """
        if cushion_active:
            result["height"] = result.get("height", 0) * 1.1
            result["speed_multiplier"] = 0.9   # slightly slower, smoother
            result["timing_multiplier"] = 1.5  # wider windows
        return result

    @staticmethod
    def apply_guard_modifiers(result: dict, guard_active: bool) -> dict:
        """
        When Guard mode is active, traversal is more predictable.
        Trade-off: fewer alternative routes available.
        """
        if guard_active:
            result["jitter_reduction"] = 0.9
            result["route_flexibility"] = 0.5  # restricted routes
        return result
