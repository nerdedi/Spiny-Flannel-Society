"""
Spiny Flannel Society — Chapters
12-chapter narrative progression through the Society.

Each chapter restores a civic rule and reduces The Drift.
Chapters never gate accessibility — safe routes are main routes.
"""

from typing import List, Dict, Optional
from dataclasses import dataclass, field


@dataclass
class Chapter:
    """One of the 12 story chapters."""
    number: int
    name: str
    location: str
    theme: str
    civic_rule_id: str
    primary_mechanic: str
    npcs: List[str] = field(default_factory=list)
    defaults_to_rewrite: List[str] = field(default_factory=list)
    is_complete: bool = False


# ─── Chapter Data ────────────────────────────────────────────────────

CHAPTERS: List[Chapter] = [
    Chapter(1,  "Bract Theory",      "windgap_academy",   "Supports without proof",
            "ACCESS_WITHOUT_PROOF",       "cushion_guard_toggle",
            ["DAZIE", "Winton"],          ["timing_window", "coyote_time"]),

    Chapter(2,  "Felt Memory",       "windgap_academy",   "Overload as information",
            "BUFFERS_BY_DEFAULT",         "radiant_hold",
            ["June", "DAZIE"],            ["visual_clutter", "audio_layering"]),

    Chapter(3,  "Rayless Form",      "windgap_academy",   "Equal expression modes",
            "PERFORMANCE_DECOUPLED",      "thread_lash",
            ["DAZIE", "Winton"],          ["communication_rigidity"]),

    Chapter(4,  "Umbel Logic",       "umbel_gardens",     "Community as architecture",
            "TRANSLATION_LAYERS",         "grapple_thread",
            ["June", "Winton"],           ["platform_rhythm"]),

    Chapter(5,  "Tickshape Rule",    "umbel_gardens",     "Consent gates",
            "CONSENT_AS_STRUCTURE",       "edge_claim",
            ["DAZIE", "June"],            ["consent_gates", "opt_out_available"]),

    Chapter(6,  "Smoke Signal",      "smoke_margin",      "Difference as adaptation",
            "ADAPTATION_RECOGNISED",      "retune",
            ["DAZIE", "Winton"],          ["retry_cost"]),

    Chapter(7,  "Afterrain Bloom",   "smoke_margin",      "Safe path = main path",
            "SAFE_PATH_MAIN_PATH",        "rhythm_traversal",
            ["June", "DAZIE"],            ["safe_route_visibility"]),

    Chapter(8,  "Sandstone Drift",   "sandstone_quarter", "Multiple valid routes",
            "FLEXIBLE_BY_DEFAULT",        "guard_pin",
            ["Winton"],                   ["route_strictness"]),

    Chapter(9,  "Eucalypt Veil",     "veil_market",       "Engineered calm",
            "PREDICTABLE_TRANSITIONS",    "glide_surf",
            ["June", "Winton"],           ["screen_shake"]),

    Chapter(10, "Clonal Echo",       "veil_market",       "Diversity = resilience",
            "PLURAL_ROUTES",              "thread_lash_combo",
            ["DAZIE", "June"],            ["social_script_penalty"]),

    Chapter(11, "Edge Reliquary",    "reliquary_edge",    "Principle modules",
            "PRINCIPLES_INTEGRATED",      "principle_collection",
            ["June", "Ari"],              ["failure_penalty"]),

    Chapter(12, "Refound Light",     "reliquary_edge",    "Compose new defaults",
            "PLURAL_COHERENCE",           "finale_all_verbs",
            ["DAZIE", "June", "Winton"],  ["jump_buffer"]),
]
