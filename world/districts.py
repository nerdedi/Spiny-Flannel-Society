"""
Spiny Flannel Society — Districts
The six districts of the Society and their properties.
"""

from typing import Dict, List, Optional
from dataclasses import dataclass, field


@dataclass
class District:
    """A district within Spiny Flannel Society."""
    id: str
    name: str
    description: str
    color_theme: str
    wind_pattern: str
    drift_level: float = 1.0       # Current drift corruption (0‑1)
    defaults_present: List[str] = field(default_factory=list)
    is_unlocked: bool = False

    def reduce_drift(self, amount: float):
        self.drift_level = max(0.0, self.drift_level - amount)


# ─── Canonical District Data ────────────────────────────────────────

DISTRICTS: Dict[str, District] = {
    "windgap_academy": District(
        id="windgap_academy",
        name="Windgap Academy",
        description="Learning commons, workshops, archives, simulation studios. "
                    "The Society's 'translation engine'.",
        color_theme="warm_amber",
        wind_pattern="gentle_uplift",
        defaults_present=["timing_window", "coyote_time", "jump_buffer",
                         "communication_rigidity"],
        is_unlocked=True,
    ),
    "veil_market": District(
        id="veil_market",
        name="The Veil Market",
        description="Sheltered trading lane where information, tools, and favours "
                    "exchange. Signage drifts first.",
        color_theme="dappled_green",
        wind_pattern="swirling_eddies",
        defaults_present=["visual_clutter", "audio_layering",
                         "social_script_penalty"],
    ),
    "sandstone_quarter": District(
        id="sandstone_quarter",
        name="Sandstone Quarter",
        description="Foundation terraces carved into warm rock. "
                    "The Society's charter stones live here.",
        color_theme="golden_ochre",
        wind_pattern="steady_horizontal",
        defaults_present=["route_strictness", "safe_route_visibility",
                         "failure_penalty"],
    ),
    "umbel_gardens": District(
        id="umbel_gardens",
        name="The Umbel Gardens",
        description="Suspended neighbourhoods held by clustered supports. "
                    "Community visible as structure.",
        color_theme="soft_violet",
        wind_pattern="rhythmic_pulse",
        defaults_present=["platform_rhythm", "consent_gates",
                         "opt_out_available"],
    ),
    "smoke_margin": District(
        id="smoke_margin",
        name="The Smoke Margin",
        description="Controlled-burn perimeter and repair yards. "
                    "Where obsolete rules are decommissioned.",
        color_theme="ash_grey",
        wind_pattern="turbulent_gusts",
        defaults_present=["retry_cost", "screen_shake"],
    ),
    "reliquary_edge": District(
        id="reliquary_edge",
        name="The Reliquary Edge",
        description="Seed bank vault and principle conservatory. "
                    "Preserves rare design laws and cultural practices.",
        color_theme="deep_teal",
        wind_pattern="still_and_calm",
        defaults_present=[],  # Contains principle modules, not standard defaults
    ),
}
