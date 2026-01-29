"""
Spiny Flannel Society - Game Configuration
Core constants and settings for the hybrid 3D platformer
"""

# Game Metadata
GAME_TITLE = "Spiny Flannel Society"
VERSION = "0.1.0"
GAME_TYPE = "Hybrid 3D Platformer"

# World Settings
WORLD_SETTING = "Living settlement suspended in wind currents above Australian coastline"
GRAVITY = -9.8  # Wind-modified gravity
WIND_FORCE_BASE = 2.5  # Base wind current force

# Core Narrative Constants
class NarrativeStates:
    """Represents the state of the Society"""
    AXIOM_ACTIVE = "spiny_flannel_axiom_active"
    STANDARD_DEFAULTS = "standard_defaults_dominant"
    THE_DRIFT = "the_drift_active"
    AXIOM_RESTORING = "axiom_restoring"

class DriftManifestations:
    """Types of corruption caused by The Drift"""
    CONTRADICTORY_SPACES = "contradictory_spaces"
    SIGNAL_CORRUPTION = "signal_corruption"
    PATHWAY_PENALIZATION = "pathway_penalization"

# Player Abilities
class TranslatorAbilities:
    """Abilities available to the Translator player"""
    READ_ASSUMPTIONS = "read_hidden_assumptions"
    REWRITE_ENVIRONMENT = "rewrite_environment"
    DECODE_SIGNALS = "decode_corrupted_signals"
    CREATE_PATHWAYS = "create_alternative_pathways"

# Player Stats
PLAYER_SPEED = 5.0
PLAYER_JUMP_HEIGHT = 2.5
PLAYER_WALL_RUN_SPEED = 4.0

# Drift Mechanics
DRIFT_INTENSITY_MIN = 0.0  # Axiom fully restored
DRIFT_INTENSITY_MAX = 1.0  # Standard Defaults fully dominant

# Space Contradiction Settings
SPACE_CONTRADICTION_THRESHOLD = 0.6
SPACE_CHANGE_RATE = 0.3

# Signal Corruption
SIGNAL_CORRUPTION_RATE = 0.4
SIGNAL_INTEGRITY_THRESHOLD = 0.5

# Pathway Penalization
PENALIZATION_MULTIPLIER = 1.5  # Damage multiplier for non-standard paths
STANDARD_PATH_BONUS = 0.8  # Speed bonus for conforming

# Environment Rewriting
REWRITE_ENERGY_COST = 10
REWRITE_COOLDOWN = 2.0  # seconds
ASSUMPTION_SCAN_RADIUS = 5.0  # meters

# Game Progression
SYSTEMS_TO_RESTORE = 12  # Number of core systems to rewrite
DISTRICTS_COUNT = 5

# Visual/Atmosphere
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
    }
}

# Wind Patterns
WIND_PATTERNS = {
    "stable": {"variance": 0.1, "direction_change": 0.05},
    "drifting": {"variance": 0.4, "direction_change": 0.3},
    "restored": {"variance": 0.2, "direction_change": 0.1}
}
