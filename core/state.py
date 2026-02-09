"""
Spiny Flannel Society — Game State
Central state management: chapter flow, drift intensity, civic rules, narrative gates.

This is the single source of truth for the game's macro state.
"""

from typing import Dict, List, Optional, Set
from dataclasses import dataclass, field
from enum import Enum, auto


# ─── Narrative States ────────────────────────────────────────────────

class Phase(Enum):
    """Macro state of the Society"""
    AXIOM_ACTIVE = auto()          # Before the game's timeline
    STANDARD_DEFAULTS = auto()     # The crisis state; game begins here
    THE_DRIFT = auto()             # Active corruption
    AXIOM_RESTORING = auto()       # Player is making progress
    PLURAL_COHERENCE = auto()      # Victory


class DriftLevel(Enum):
    """Qualitative drift severity (maps to float 0‑1)"""
    NONE = 0.0
    LOW = 0.25
    MODERATE = 0.5
    HIGH = 0.75
    CRITICAL = 1.0


# ─── Game State ──────────────────────────────────────────────────────

@dataclass
class GameState:
    """
    Top‑level mutable state for a single playthrough.

    Every system should *read* GameState rather than keeping its own
    parallel variables.
    """

    # Narrative
    current_chapter: int = 1
    total_chapters: int = 12
    phase: Phase = Phase.THE_DRIFT
    drift_intensity: float = 1.0       # 1.0 = full drift, 0.0 = resolved

    # Civic rules restored so far
    civic_rules_restored: Set[str] = field(default_factory=set)

    # Player progress
    electives_completed: List[str] = field(default_factory=list)
    assumptions_revealed: int = 0
    assumptions_rewritten: int = 0

    # District unlock state
    districts_visited: Set[str] = field(default_factory=set)

    # Windprint record (for finale personalisation)
    cushion_uses: int = 0
    guard_uses: int = 0
    verb_usage: Dict[str, int] = field(default_factory=dict)

    # ── Chapter progression ──────────────────────────────────────────

    def advance_chapter(self) -> bool:
        """Advance to the next chapter. Returns False if already at the end."""
        if self.current_chapter >= self.total_chapters:
            return False
        self.current_chapter += 1
        self._reduce_drift()
        self._update_phase()
        return True

    def restore_civic_rule(self, rule_id: str):
        """Mark a civic rule as restored."""
        self.civic_rules_restored.add(rule_id)
        self._reduce_drift()

    # ── Drift ────────────────────────────────────────────────────────

    DRIFT_REDUCTION_PER_CHAPTER = 1.0 / 12.0

    def _reduce_drift(self):
        self.drift_intensity = max(
            0.0,
            self.drift_intensity - self.DRIFT_REDUCTION_PER_CHAPTER
        )

    def _update_phase(self):
        if self.drift_intensity <= 0.0:
            self.phase = Phase.PLURAL_COHERENCE
        elif self.drift_intensity < 0.5:
            self.phase = Phase.AXIOM_RESTORING
        else:
            self.phase = Phase.THE_DRIFT

    # ── Queries ──────────────────────────────────────────────────────

    @property
    def progress_fraction(self) -> float:
        return (self.current_chapter - 1) / self.total_chapters

    @property
    def is_victory(self) -> bool:
        return self.phase == Phase.PLURAL_COHERENCE

    def preferred_mode(self) -> str:
        """Return 'cushion' or 'guard' depending on playstyle."""
        return "cushion" if self.cushion_uses >= self.guard_uses else "guard"
