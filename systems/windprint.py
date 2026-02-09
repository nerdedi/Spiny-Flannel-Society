"""
Spiny Flannel Society — Windprint Rig
Dual-mode tool: Cushion (softness) vs Guard (boundaries).

KEY DESIGN INSIGHT (from feedback):
    Cushion and Guard are not just "mode switches".
    Each has a COST that reinforces the game's ethics:

    • Cushion widens timing and reduces clutter
      → but INCREASES entropy elsewhere (visual noise in adjacent areas,
        platform drift in unexplored zones)

    • Guard pins rhythms and creates consent gates
      → but REDUCES flexibility (locks out some alternative routes,
        narrows exploration options while active)

    This makes the system feel *ethical*, not just *tactical*.
    The player must weigh softness vs stability — the same tension
    the Society faces.
"""

from typing import Dict, List, Optional
from dataclasses import dataclass, field

from core.defaults_registry import DefaultsRegistry


# ─── Mode Costs ──────────────────────────────────────────────────────

@dataclass
class ModeCost:
    """
    Trade-off incurred while a Windprint mode is active.
    These are the ethical weights that prevent mode-switching
    from being a free optimisation.
    """
    label: str
    description: str
    magnitude: float         # 0‑1 severity scale


CUSHION_COSTS = [
    ModeCost(
        label="Entropy Bleed",
        description="Visual noise increases in adjacent, un-cushioned areas. "
                    "Softening here displaces friction elsewhere.",
        magnitude=0.3,
    ),
    ModeCost(
        label="Platform Drift",
        description="Unexplored platforms may shift unpredictably while "
                    "Cushion stabilises the current zone.",
        magnitude=0.2,
    ),
]

GUARD_COSTS = [
    ModeCost(
        label="Route Lock",
        description="Some alternative routes become inaccessible while Guard "
                    "pins the current path in place.",
        magnitude=0.4,
    ),
    ModeCost(
        label="Exploration Narrowing",
        description="Guard's stability comes at the expense of open-ended "
                    "discovery; peripheral areas are less responsive.",
        magnitude=0.25,
    ),
]


# ─── Windprint Rig ───────────────────────────────────────────────────

class WindprintMode:
    CUSHION = "cushion"
    GUARD = "guard"


@dataclass
class WindprintState:
    """Current state of the Windprint Rig."""
    active_mode: Optional[str] = None
    energy: float = 100.0
    energy_max: float = 100.0
    energy_regen: float = 5.0
    mode_switch_cost: float = 5.0


class WindprintRigSystem:
    """
    The player's signature tool.

    Systems query defaults_registry for base values, then layer
    Windprint modifications on top.
    """

    def __init__(self, registry: DefaultsRegistry):
        self.registry = registry
        self.state = WindprintState()

    # ── Mode control ─────────────────────────────────────────────

    def activate_cushion(self) -> Dict:
        """
        Activate Cushion mode.
        Returns description of effects AND costs.
        """
        cost = self.state.mode_switch_cost if self.state.active_mode == WindprintMode.GUARD else 0
        if self.state.energy < cost:
            return {"success": False, "reason": "Insufficient energy"}

        self.state.energy -= cost
        self.state.active_mode = WindprintMode.CUSHION

        return {
            "success": True,
            "mode": WindprintMode.CUSHION,
            "effects": {
                "timing_multiplier": 1.5,
                "clutter_reduction": 0.6,
                "hazard_slowdown": 0.5,
                "safe_pocket_rate": 0.3,
            },
            "costs": [
                {"label": c.label, "description": c.description, "magnitude": c.magnitude}
                for c in CUSHION_COSTS
            ],
        }

    def activate_guard(self) -> Dict:
        """
        Activate Guard mode.
        Returns description of effects AND costs.
        """
        cost = self.state.mode_switch_cost if self.state.active_mode == WindprintMode.CUSHION else 0
        if self.state.energy < cost:
            return {"success": False, "reason": "Insufficient energy"}

        self.state.energy -= cost
        self.state.active_mode = WindprintMode.GUARD

        return {
            "success": True,
            "mode": WindprintMode.GUARD,
            "effects": {
                "rhythm_pin_strength": 0.8,
                "jitter_stabilisation": 0.9,
                "consent_gates": True,
                "edge_claim_range": 3.0,
            },
            "costs": [
                {"label": c.label, "description": c.description, "magnitude": c.magnitude}
                for c in GUARD_COSTS
            ],
        }

    def deactivate(self):
        self.state.active_mode = None

    # ── Queries ──────────────────────────────────────────────────

    def is_cushion_active(self) -> bool:
        return self.state.active_mode == WindprintMode.CUSHION

    def is_guard_active(self) -> bool:
        return self.state.active_mode == WindprintMode.GUARD

    def get_timing_multiplier(self) -> float:
        """How much to widen timing windows right now."""
        base = self.registry.get("timing_window") or 0.2
        if self.is_cushion_active():
            return base * 1.5
        return base

    def get_route_flexibility(self) -> float:
        """How many alternative routes are available right now."""
        base = 1.0 - (self.registry.get("route_strictness") or 1.0)
        if self.is_guard_active():
            return base * 0.5   # Guard restricts alternatives
        return base

    # ── Energy ───────────────────────────────────────────────────

    def update(self, delta_time: float):
        """Tick energy regen."""
        if self.state.active_mode is None:
            self.state.energy = min(
                self.state.energy_max,
                self.state.energy + self.state.energy_regen * delta_time
            )
