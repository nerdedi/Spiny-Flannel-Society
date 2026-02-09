"""
Spiny Flannel Society — Non-Violent Symbolic Combat
Pattern intervention, not violence.

Antagonistic patterns are embodied consequences of misdesign,
not villains. Resolution means restoring coherent systems.
"""

from typing import List, Dict, Optional
from dataclasses import dataclass, field
from enum import Enum, auto


# ─── Combat Verbs ────────────────────────────────────────────────────

class Verb(Enum):
    PULSE = "pulse"               # Clears / resets cycles
    THREAD_LASH = "thread_lash"   # Interrupts loops
    RADIANT_HOLD = "radiant_hold" # Shields, creates safe footholds
    EDGE_CLAIM = "edge_claim"     # Pins a rhythm
    RETUNE = "retune"             # Cleans signal corruption


@dataclass
class VerbStats:
    energy_cost: int
    cooldown: float
    effect_radius: float
    description: str
    duration: float = 0.0


VERB_STATS: Dict[Verb, VerbStats] = {
    Verb.PULSE: VerbStats(10, 1.0, 5.0, "Clears/resets cycles; breaks Distortion loops"),
    Verb.THREAD_LASH: VerbStats(15, 1.5, 8.0, "Interrupts Echo Form patterns"),
    Verb.RADIANT_HOLD: VerbStats(20, 2.0, 3.0, "Creates shields and safe footholds", 3.0),
    Verb.EDGE_CLAIM: VerbStats(15, 1.5, 4.0, "Pins rhythms; stabilises platform timing", 5.0),
    Verb.RETUNE: VerbStats(25, 3.0, 6.0, "Cleans signal corruption; calms Noise Beasts"),
}


# ─── Antagonistic Patterns ──────────────────────────────────────────

class PatternType(Enum):
    ECHO_FORM = auto()    # Social scripts given motion
    DISTORTION = auto()   # Broken rules manifested physically
    NOISE_BEAST = auto()  # Sensory overload as weather


@dataclass
class AntagPattern:
    """An antagonistic pattern to be resolved (not killed)."""
    pattern_type: PatternType
    name: str
    intensity: float = 0.8         # 0‑1
    resolution_progress: float = 0.0
    primary_verb: Verb = Verb.PULSE
    secondary_verb: Verb = Verb.EDGE_CLAIM

    @property
    def is_resolved(self) -> bool:
        return self.resolution_progress >= 1.0

    def receive_verb(self, verb: Verb) -> float:
        """
        Apply a verb to this pattern.
        Returns progress gained (0‑1).
        """
        if verb == self.primary_verb:
            gain = 0.4
        elif verb == self.secondary_verb:
            gain = 0.25
        else:
            gain = 0.1
        self.resolution_progress = min(1.0, self.resolution_progress + gain)
        return gain


# ─── Encounter ───────────────────────────────────────────────────────

@dataclass
class Encounter:
    """A combat encounter the player enters."""
    patterns: List[AntagPattern] = field(default_factory=list)
    is_active: bool = True

    @property
    def is_complete(self) -> bool:
        return all(p.is_resolved for p in self.patterns)

    def resolve_check(self) -> bool:
        if self.is_complete:
            self.is_active = False
        return self.is_complete


# ─── Signal System ───────────────────────────────────────────────────

@dataclass
class Signal:
    """A corrupted or clean signal in the environment."""
    id: str
    message: str
    corruption: float = 0.0      # 0 = clean, 1 = fully corrupted
    decoded: bool = False

    @property
    def is_corrupted(self) -> bool:
        return self.corruption > 0.5

    def decode(self) -> str:
        self.decoded = True
        self.corruption = 0.0
        return self.message
