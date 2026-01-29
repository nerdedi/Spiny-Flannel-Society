"""
Spiny Flannel Society - Game Configuration
Core constants and settings for the hybrid 3D platformer

A neuroaffirming indie game where accessibility is world law.
"""

from enum import Enum, auto
from typing import Dict, List, Tuple, NamedTuple
from dataclasses import dataclass

# =============================================================================
# GAME METADATA
# =============================================================================

GAME_TITLE = "Spiny Flannel Society"
VERSION = "0.2.0"
GAME_TYPE = "Hybrid 3D Platformer"
GAME_DESCRIPTION = """
A living settlement suspended in wind currents above an Australian coastline.
Neuroaffirming | Non-violent symbolic combat | Universal design as world law
"""

# =============================================================================
# WORLD SETTINGS
# =============================================================================

WORLD_SETTING = "Living settlement suspended in wind currents above Australian coastline"
GRAVITY = -9.8  # Wind-modified gravity
WIND_FORCE_BASE = 2.5  # Base wind current force

# =============================================================================
# NARRATIVE STATES
# =============================================================================

class NarrativeStates:
    """Represents the state of the Society"""
    AXIOM_ACTIVE = "spiny_flannel_axiom_active"
    STANDARD_DEFAULTS = "standard_defaults_dominant"
    THE_DRIFT = "the_drift_active"
    AXIOM_RESTORING = "axiom_restoring"
    PLURAL_COHERENCE = "plural_coherence_achieved"  # Victory state


class DriftManifestations:
    """Types of corruption caused by The Drift"""
    DISTORTIONS = "distortions"  # Glitched rules
    ECHO_FORMS = "echo_forms"  # Social scripts given motion
    NOISE_BEASTS = "noise_beasts"  # Sensory overload as weather
    # Legacy (kept for compatibility)
    CONTRADICTORY_SPACES = "contradictory_spaces"
    SIGNAL_CORRUPTION = "signal_corruption"
    PATHWAY_PENALIZATION = "pathway_penalization"


# =============================================================================
# PLAYER ABILITIES
# =============================================================================

class TranslatorAbilities:
    """Abilities available to the Translator player"""
    # Core abilities (always available)
    READ_ASSUMPTIONS = "read_hidden_assumptions"
    PERCEIVE_WINDPRINTS = "perceive_windprints"

    # Unlockable abilities
    REWRITE_ENVIRONMENT = "rewrite_environment"
    DECODE_SIGNALS = "decode_corrupted_signals"
    CREATE_PATHWAYS = "create_alternative_pathways"

    # Windprint Rig modes
    CUSHION_MODE = "cushion_mode"
    GUARD_MODE = "guard_mode"


class CombatVerbs:
    """Non-violent symbolic combat verbs"""
    PULSE = "pulse"  # Clears/resets cycles
    THREAD_LASH = "thread_lash"  # Interrupts loops
    RADIANT_HOLD = "radiant_hold"  # Shields, creates safe footholds
    EDGE_CLAIM = "edge_claim"  # Pins a rhythm
    RETUNE = "retune"  # Cleans signal corruption


# Combat verb stats
COMBAT_VERB_STATS = {
    CombatVerbs.PULSE: {
        "energy_cost": 10,
        "cooldown": 1.0,
        "effect_radius": 5.0,
        "description": "Clears/resets cycles; breaks Distortion loops"
    },
    CombatVerbs.THREAD_LASH: {
        "energy_cost": 15,
        "cooldown": 1.5,
        "effect_radius": 8.0,
        "description": "Interrupts Echo Form patterns"
    },
    CombatVerbs.RADIANT_HOLD: {
        "energy_cost": 20,
        "cooldown": 2.0,
        "effect_radius": 3.0,
        "duration": 3.0,
        "description": "Creates shields and safe footholds"
    },
    CombatVerbs.EDGE_CLAIM: {
        "energy_cost": 15,
        "cooldown": 1.5,
        "effect_radius": 4.0,
        "duration": 5.0,
        "description": "Pins rhythms; stabilises platform timing"
    },
    CombatVerbs.RETUNE: {
        "energy_cost": 25,
        "cooldown": 3.0,
        "effect_radius": 6.0,
        "description": "Cleans signal corruption; calms Noise Beasts"
    }
}


# =============================================================================
# WINDPRINT RIG SETTINGS
# =============================================================================

class WindprintModes:
    """Windprint Rig operational modes"""
    CUSHION = "cushion"
    GUARD = "guard"


# Cushion Mode - Softness and accessibility
CUSHION_MODE_EFFECTS = {
    "timing_window_multiplier": 1.5,  # 50% wider timing windows
    "safe_pocket_spawn_rate": 0.3,
    "clutter_reduction": 0.6,  # 60% visual noise reduction
    "hazard_slowdown": 0.5,  # Hazards move at 50% speed
    "wind_impact_reduction": 0.8,  # Softer wind effect
}

# Guard Mode - Protection and boundaries
GUARD_MODE_EFFECTS = {
    "rhythm_pin_strength": 0.8,
    "jitter_stabilisation": 0.9,  # 90% shake reduction
    "consent_gate_active": True,
    "edge_claim_range": 3.0,  # meters
    "boundary_hold_duration": 5.0,  # seconds
}

WINDPRINT_ENERGY_MAX = 100
WINDPRINT_ENERGY_REGEN = 5  # per second
WINDPRINT_MODE_SWITCH_COST = 5


# =============================================================================
# PLAYER STATS
# =============================================================================

# Base movement
PLAYER_SPEED = 5.0  # m/s
PLAYER_JUMP_HEIGHT = 2.5  # meters
PLAYER_WALL_RUN_SPEED = 4.0  # m/s

# Advanced movement
PLAYER_AIR_DASH_SPEED = 8.0  # m/s
PLAYER_AIR_DASH_DISTANCE = 4.0  # meters
PLAYER_GLIDE_SPEED = 3.0  # m/s
PLAYER_GRAPPLE_SPEED = 10.0  # m/s

# Triple hop
TRIPLE_HOP_HEIGHTS = (1.5, 2.5, 3.5)  # Short, Long, Float
TRIPLE_HOP_WINDOW = 0.5  # seconds to chain hops

# Coyote time and jump buffering
COYOTE_TIME = 0.15  # seconds
JUMP_BUFFER_TIME = 0.1  # seconds


# =============================================================================
# DRIFT MECHANICS
# =============================================================================

DRIFT_INTENSITY_MIN = 0.0  # Axiom fully restored
DRIFT_INTENSITY_MAX = 1.0  # Standard Defaults fully dominant

# How much each chapter completion reduces drift
DRIFT_REDUCTION_PER_CHAPTER = 1.0 / 12.0  # 12 chapters

# Space Contradiction Settings
SPACE_CONTRADICTION_THRESHOLD = 0.6
SPACE_CHANGE_RATE = 0.3

# Signal Corruption
SIGNAL_CORRUPTION_RATE = 0.4
SIGNAL_INTEGRITY_THRESHOLD = 0.5

# Pathway Penalization
PENALIZATION_MULTIPLIER = 1.5
STANDARD_PATH_BONUS = 0.8


# =============================================================================
# ENVIRONMENT REWRITING
# =============================================================================

REWRITE_ENERGY_COST = 10
REWRITE_COOLDOWN = 2.0  # seconds
ASSUMPTION_SCAN_RADIUS = 5.0  # meters

# Design Terminal settings
DESIGN_TERMINAL_INTERACTION_RANGE = 2.0  # meters


# =============================================================================
# GAME PROGRESSION
# =============================================================================

TOTAL_CHAPTERS = 12
TOTAL_DISTRICTS = 6


# =============================================================================
# DISTRICTS
# =============================================================================

class Districts:
    """Key districts of Spiny Flannel Society"""
    WINDGAP_ACADEMY = "windgap_academy"
    VEIL_MARKET = "veil_market"
    SANDSTONE_QUARTER = "sandstone_quarter"
    UMBEL_GARDENS = "umbel_gardens"
    SMOKE_MARGIN = "smoke_margin"
    RELIQUARY_EDGE = "reliquary_edge"


DISTRICT_DATA = {
    Districts.WINDGAP_ACADEMY: {
        "name": "Windgap Academy",
        "description": "Learning commons, workshops, archives, simulation studios; the Society's 'translation engine'",
        "color_theme": "warm_amber",
        "wind_pattern": "gentle_uplift"
    },
    Districts.VEIL_MARKET: {
        "name": "The Veil Market",
        "description": "Sheltered trading lane where information, tools, and favours exchange; signage drifts first",
        "color_theme": "dappled_green",
        "wind_pattern": "swirling_eddies"
    },
    Districts.SANDSTONE_QUARTER: {
        "name": "Sandstone Quarter",
        "description": "Foundation terraces carved into warm rock; the Society's charter stones live here",
        "color_theme": "golden_ochre",
        "wind_pattern": "steady_horizontal"
    },
    Districts.UMBEL_GARDENS: {
        "name": "The Umbel Gardens",
        "description": "Suspended neighbourhoods held by clustered supports; community visible as structure",
        "color_theme": "soft_violet",
        "wind_pattern": "rhythmic_pulse"
    },
    Districts.SMOKE_MARGIN: {
        "name": "The Smoke Margin",
        "description": "Controlled-burn perimeter and repair yards; where obsolete rules are decommissioned",
        "color_theme": "ash_grey",
        "wind_pattern": "turbulent_gusts"
    },
    Districts.RELIQUARY_EDGE: {
        "name": "The Reliquary Edge",
        "description": "Seed bank vault and principle conservatory; preserves rare design laws and cultural practices",
        "color_theme": "deep_teal",
        "wind_pattern": "still_and_calm"
    }
}


# =============================================================================
# CHAPTERS
# =============================================================================

@dataclass
class ChapterData:
    """Data structure for chapter information"""
    id: int
    name: str
    location: str
    theme: str
    civic_rule: str
    primary_mechanic: str
    npcs: List[str]


CHAPTER_DATA = {
    1: ChapterData(
        id=1,
        name="Bract Theory",
        location=Districts.WINDGAP_ACADEMY,
        theme="Supports without proof",
        civic_rule="ACCESS_WITHOUT_PROOF",
        primary_mechanic="cushion_guard_toggle",
        npcs=["DAZIE", "Winton"]
    ),
    2: ChapterData(
        id=2,
        name="Felt Memory",
        location=Districts.WINDGAP_ACADEMY,
        theme="Overload as information",
        civic_rule="BUFFERS_BY_DEFAULT",
        primary_mechanic="radiant_hold",
        npcs=["June", "DAZIE"]
    ),
    3: ChapterData(
        id=3,
        name="Rayless Form",
        location=Districts.WINDGAP_ACADEMY,
        theme="Equal expression modes",
        civic_rule="PERFORMANCE_DECOUPLED_FROM_VALUE",
        primary_mechanic="thread_lash",
        npcs=["DAZIE", "Winton"]
    ),
    4: ChapterData(
        id=4,
        name="Umbel Logic",
        location=Districts.UMBEL_GARDENS,
        theme="Community as architecture",
        civic_rule="TRANSLATION_LAYERS",
        primary_mechanic="grapple_thread",
        npcs=["June", "Winton"]
    ),
    5: ChapterData(
        id=5,
        name="Tickshape Rule",
        location=Districts.UMBEL_GARDENS,
        theme="Consent gates",
        civic_rule="CONSENT_AS_STRUCTURE",
        primary_mechanic="edge_claim",
        npcs=["DAZIE", "June"]
    ),
    6: ChapterData(
        id=6,
        name="Smoke Signal",
        location=Districts.SMOKE_MARGIN,
        theme="Difference as adaptation",
        civic_rule="ADAPTATION_RECOGNISED",
        primary_mechanic="retune",
        npcs=["DAZIE", "Winton"]
    ),
    7: ChapterData(
        id=7,
        name="Afterrain Bloom",
        location=Districts.SMOKE_MARGIN,
        theme="Safe path = main path",
        civic_rule="SAFE_PATH_MAIN_PATH",
        primary_mechanic="rhythm_traversal",
        npcs=["June", "DAZIE"]
    ),
    8: ChapterData(
        id=8,
        name="Sandstone Drift",
        location=Districts.SANDSTONE_QUARTER,
        theme="Multiple valid routes",
        civic_rule="FLEXIBLE_BY_DEFAULT",
        primary_mechanic="guard_pin",
        npcs=["Winton"]
    ),
    9: ChapterData(
        id=9,
        name="Eucalypt Veil",
        location=Districts.VEIL_MARKET,
        theme="Engineered calm",
        civic_rule="PREDICTABLE_TRANSITIONS",
        primary_mechanic="glide_surf",
        npcs=["June", "Winton"]
    ),
    10: ChapterData(
        id=10,
        name="Clonal Echo",
        location=Districts.VEIL_MARKET,
        theme="Diversity = resilience",
        civic_rule="PLURAL_ROUTES",
        primary_mechanic="thread_lash_combo",
        npcs=["DAZIE", "June"]
    ),
    11: ChapterData(
        id=11,
        name="Edge Reliquary",
        location=Districts.RELIQUARY_EDGE,
        theme="Principle modules",
        civic_rule="PRINCIPLES_INTEGRATED",
        primary_mechanic="principle_collection",
        npcs=["June", "Ari"]
    ),
    12: ChapterData(
        id=12,
        name="Refound Light",
        location=Districts.RELIQUARY_EDGE,
        theme="Compose new defaults",
        civic_rule="PLURAL_COHERENCE",
        primary_mechanic="finale_all_verbs",
        npcs=["DAZIE", "June", "Winton"]
    )
}


# =============================================================================
# CHARACTERS
# =============================================================================

class Characters:
    """Character identifiers"""
    TRANSLATOR = "translator"  # Player
    DAZIE = "dazie_vine"
    JUNE = "june_corrow"
    WINTON = "winton"
    ARI = "ari"  # Minor character in Chapter 11


CHARACTER_DATA = {
    Characters.TRANSLATOR: {
        "name": "The Translator",
        "role": "Player Character",
        "description": "Newcomer with the rare capacity to perceive Windprints",
        "voice": "Player-defined communication mode"
    },
    Characters.DAZIE: {
        "name": "DAZIE Vine",
        "role": "Mentor / Systems Ethicist",
        "description": "Resident of Spiny Flannel Society and orientation lead at Windgap Academy",
        "voice": "Calm, precise, non-patronising",
        "introduces": ["Guard Mode", "Edge Claim", "Consent gates"]
    },
    Characters.JUNE: {
        "name": "June Corrow",
        "role": "Sensory Architect / Biodesign Maker",
        "description": "Designed the Society's quiet infrastructure",
        "voice": "Sparse, warm, incisive",
        "introduces": ["Cushion Mode", "Filtration mechanics", "Veil traversal"]
    },
    Characters.WINTON: {
        "name": "Winton",
        "role": "Civic OS / System-Ghost",
        "description": "The Society's operating interface made audible",
        "voice": "Blunt, ethically focused, occasionally dry",
        "introduces": ["System states", "Design terminals", "Windprint recording"]
    },
    Characters.ARI: {
        "name": "Ari",
        "role": "Resident / Guide",
        "description": "Appears in Chapter 11 at the Reliquary Edge",
        "voice": "Hopeful, practical"
    }
}


# =============================================================================
# COMMUNICATION MODES
# =============================================================================

class CommunicationModes:
    """Player communication style options (all equal outcomes)"""
    DIRECT = "direct"
    SCRIPTED = "scripted"
    ICONS = "icons"
    MINIMAL = "minimal_speech"


# =============================================================================
# ELECTIVE SUBJECTS
# =============================================================================

class ElectiveSubjects:
    """Stealth learning subject categories"""
    LOGIC = "logic"  # IF/THEN tiles
    LITERACY = "literacy"  # Signal decoding
    NUMERACY = "numeracy"  # Ratio calibration
    LANGUAGE = "language"  # Sequence building
    DIGITAL = "digital_literacy"  # System debugging


ELECTIVE_REWARD_TYPES = [
    "lore",
    "cosmetic",
    "shortcut",
    "windprint_perk"
]


# =============================================================================
# VISUAL/ATMOSPHERE
# =============================================================================

COLOR_PALETTES = {
    "standard_defaults": {
        "primary": (128, 128, 128),  # Gray
        "secondary": (96, 96, 96),   # Darker gray
        "accent": (160, 160, 160)    # Light gray
    },
    "axiom_active": {
        "primary": (255, 140, 60),   # Warm orange (flannel-inspired)
        "secondary": (100, 180, 220), # Sky blue
        "accent": (220, 100, 180)    # Vibrant pink
    },
    "transitional": {
        "primary": (192, 134, 94),   # Warming gray
        "secondary": (98, 138, 168), # Hint of blue
        "accent": (170, 98, 148)     # Emerging color
    },
    "plural_coherence": {
        "primary": (255, 200, 100),  # Golden warmth
        "secondary": (120, 200, 180), # Teal harmony
        "accent": (200, 150, 220)    # Soft violet
    }
}

# District-specific color themes
DISTRICT_COLOR_THEMES = {
    "warm_amber": ((255, 180, 100), (200, 140, 80)),
    "dappled_green": ((100, 180, 120), (80, 140, 100)),
    "golden_ochre": ((220, 180, 100), (180, 140, 80)),
    "soft_violet": ((180, 140, 200), (140, 100, 160)),
    "ash_grey": ((160, 160, 160), (120, 120, 120)),
    "deep_teal": ((60, 140, 140), (40, 100, 100))
}


# =============================================================================
# WIND PATTERNS
# =============================================================================

WIND_PATTERNS = {
    "stable": {"variance": 0.1, "direction_change": 0.05},
    "drifting": {"variance": 0.4, "direction_change": 0.3},
    "restored": {"variance": 0.2, "direction_change": 0.1},
    # District-specific patterns
    "gentle_uplift": {"variance": 0.15, "direction_change": 0.08, "vertical_bias": 0.3},
    "swirling_eddies": {"variance": 0.35, "direction_change": 0.25, "rotation": 0.5},
    "steady_horizontal": {"variance": 0.1, "direction_change": 0.05, "horizontal_bias": 0.8},
    "rhythmic_pulse": {"variance": 0.2, "direction_change": 0.1, "pulse_frequency": 2.0},
    "turbulent_gusts": {"variance": 0.5, "direction_change": 0.4, "gust_intensity": 1.5},
    "still_and_calm": {"variance": 0.05, "direction_change": 0.02}
}


# =============================================================================
# ANTAGONISTIC PATTERNS
# =============================================================================

@dataclass
class AntagonistData:
    """Data for antagonistic pattern types"""
    name: str
    description: str
    resolution_verb: str
    secondary_verb: str
    base_intensity: float


ANTAGONIST_DATA = {
    "echo_form": AntagonistData(
        name="Echo Form",
        description="Coercive social scripts given motion",
        resolution_verb=CombatVerbs.THREAD_LASH,
        secondary_verb=CombatVerbs.PULSE,
        base_intensity=0.7
    ),
    "distortion": AntagonistData(
        name="Distortion",
        description="Broken rules manifested physically",
        resolution_verb=CombatVerbs.PULSE,
        secondary_verb=CombatVerbs.EDGE_CLAIM,
        base_intensity=0.8
    ),
    "noise_beast": AntagonistData(
        name="Noise Beast",
        description="Sensory overload as weather",
        resolution_verb=CombatVerbs.RETUNE,
        secondary_verb=CombatVerbs.RADIANT_HOLD,
        base_intensity=0.9
    )
}


# =============================================================================
# CIVIC RULES (Restorable)
# =============================================================================

CIVIC_RULES = {
    "ACCESS_WITHOUT_PROOF": "Supports appear without justification required",
    "BUFFERS_BY_DEFAULT": "Rest pockets and quiet routes are standard infrastructure",
    "PERFORMANCE_DECOUPLED_FROM_VALUE": "Expression mode does not determine worth",
    "TRANSLATION_LAYERS": "Systems accommodate different speeds and communication styles",
    "CONSENT_AS_STRUCTURE": "Boundaries are encoded into environmental design",
    "ADAPTATION_RECOGNISED": "Difference is classified as adaptation, not deviation",
    "SAFE_PATH_MAIN_PATH": "Accessible routes are the primary routes",
    "FLEXIBLE_BY_DEFAULT": "Multiple valid approaches are structurally supported",
    "PREDICTABLE_TRANSITIONS": "Environmental changes are signalled in multiple modalities",
    "PLURAL_ROUTES": "Diversity increases system resilience",
    "PRINCIPLES_INTEGRATED": "Rare civic principles are restored to active use",
    "PLURAL_COHERENCE": "The Society achieves coherence through plurality"
}


# =============================================================================
# ACCESSIBILITY SETTINGS (World mechanics, not hidden options)
# =============================================================================

ACCESSIBILITY_DEFAULTS = {
    "motion_intensity": 1.0,
    "brightness_level": 1.0,
    "clutter_reduction": 0.0,
    "subtitle_style": "standard",
    "no_forced_timers": True,
    "safe_routes_main": True,
    "communication_mode": CommunicationModes.DIRECT
}

SUBTITLE_STYLES = ["standard", "high_contrast", "dyslexia_friendly", "minimal"]


# =============================================================================
# LEGACY COMPATIBILITY
# =============================================================================

# Keep old constant names working
SYSTEMS_TO_RESTORE = TOTAL_CHAPTERS
DISTRICTS_COUNT = TOTAL_DISTRICTS
