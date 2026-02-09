"""
Spiny Flannel Society — Distortions
Drift manifestations: how bias becomes physical in the environment.
"""

from typing import List, Optional
from dataclasses import dataclass
from enum import Enum, auto


class DistortionType(Enum):
    CONTRADICTORY_SPACE = auto()   # A space that changes rules mid-traversal
    SIGNAL_CORRUPTION = auto()     # Information becomes unreliable
    PATHWAY_PENALTY = auto()       # Alternative routes have hidden costs
    TIMING_LOCK = auto()           # Assumes one reaction speed
    SENSORY_FLOOD = auto()         # Excessive simultaneous stimuli


@dataclass
class Distortion:
    """
    A distortion in the environment caused by The Drift.

    Distortions are assumptions made physical.  They aren't enemies;
    they are design failures the player diagnoses and corrects
    by Reading and Rewriting the underlying Default.
    """
    id: str
    distortion_type: DistortionType
    description: str
    severity: float = 0.8          # 0‑1
    linked_default: str = ""       # Key in DefaultsRegistry
    is_resolved: bool = False

    def resolve(self):
        self.is_resolved = True
        self.severity = 0.0

    @property
    def hint(self) -> str:
        """A player-facing hint about what assumption causes this."""
        hints = {
            DistortionType.CONTRADICTORY_SPACE:
                "This space changes its own rules. Look for the timing assumption.",
            DistortionType.SIGNAL_CORRUPTION:
                "The signal is garbled. Something assumes one communication mode.",
            DistortionType.PATHWAY_PENALTY:
                "This path punishes you for not taking the 'expected' route.",
            DistortionType.TIMING_LOCK:
                "Everything here assumes you react at one speed.",
            DistortionType.SENSORY_FLOOD:
                "Too much happening at once. The density default is too high.",
        }
        return hints.get(self.distortion_type, "Something here isn't right.")
