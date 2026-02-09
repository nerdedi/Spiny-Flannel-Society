"""
Spiny Flannel Society — Defaults Registry
The thematic heart of the codebase.

Every system queries this registry for its baseline values.
When the player rewrites a Default, the registry is what changes —
and every system that queries it responds automatically.

THIS TURNS THE GAME'S THEME INTO ARCHITECTURE:
    "The Society failed because it imposed rigid defaults.
     You restore it by rewriting them."
"""

from typing import Any, Dict, Optional, Set
from dataclasses import dataclass, field
from enum import Enum, auto


# ─── Default Categories ─────────────────────────────────────────────

class DefaultCategory(Enum):
    """Which aspect of the Society a default governs."""
    TIMING = auto()         # Speed expectations, window widths
    SENSORY = auto()        # Visual density, audio load, clutter
    ROUTING = auto()        # Path strictness, alternatives
    SOCIAL = auto()         # Communication norms, expression modes
    FAILURE = auto()        # Penalty severity, retry cost
    CONSENT = auto()        # Gate presence, opt-in/opt-out


# ─── Individual Default ─────────────────────────────────────────────

@dataclass
class Default:
    """
    A single rewritable default.

    Attributes:
        key:          Machine-readable identifier.
        label:        Human-friendly name (shown during Read Default).
        description:  What this default assumes and who it penalises.
        category:     Which aspect of society it governs.
        rigid_value:  The value under Standard Defaults (The Drift).
        rewritten_value: The value after the player rewrites it.
        current_value: Starts at rigid_value; becomes rewritten_value.
        is_read:      Has the player used Read Default on this?
        is_rewritten: Has the player used Rewrite Default on this?
    """
    key: str
    label: str
    description: str
    category: DefaultCategory

    rigid_value: Any
    rewritten_value: Any
    current_value: Any = None

    is_read: bool = False
    is_rewritten: bool = False

    def __post_init__(self):
        if self.current_value is None:
            self.current_value = self.rigid_value

    def read(self) -> str:
        """
        Player uses *Read Default*.
        Reveals the assumption and returns a description.
        """
        self.is_read = True
        return (
            f"[{self.label}]\n"
            f"  Assumption: {self.description}\n"
            f"  Current value: {self.current_value}\n"
            f"  Rigid default: {self.rigid_value}"
        )

    def rewrite(self) -> bool:
        """
        Player uses *Rewrite Default*.
        Can only rewrite after reading.
        Returns True if the rewrite succeeds.
        """
        if not self.is_read:
            return False
        self.is_rewritten = True
        self.current_value = self.rewritten_value
        return True


# ─── The Registry ────────────────────────────────────────────────────

class DefaultsRegistry:
    """
    Central registry of all rewritable defaults in the Society.

    Any system can query:
        registry.get("timing_window")
    and automatically receive the correct value for the current game state.
    """

    def __init__(self):
        self._defaults: Dict[str, Default] = {}
        self._register_all()

    # ── Public API ───────────────────────────────────────────────────

    def get(self, key: str) -> Any:
        """Get the current value of a default. Systems call this."""
        d = self._defaults.get(key)
        return d.current_value if d else None

    def read(self, key: str) -> Optional[str]:
        """Player uses Read Default on a specific default."""
        d = self._defaults.get(key)
        return d.read() if d else None

    def rewrite(self, key: str) -> bool:
        """Player uses Rewrite Default on a specific default."""
        d = self._defaults.get(key)
        return d.rewrite() if d else False

    def list_readable(self) -> list:
        """Defaults the player can currently Read (not yet read)."""
        return [d for d in self._defaults.values() if not d.is_read]

    def list_rewritable(self) -> list:
        """Defaults the player has Read but not yet Rewritten."""
        return [d for d in self._defaults.values() if d.is_read and not d.is_rewritten]

    def all_rewritten(self) -> bool:
        """Victory check: are all defaults rewritten?"""
        return all(d.is_rewritten for d in self._defaults.values())

    @property
    def progress(self) -> float:
        """Fraction of defaults rewritten (0.0 → 1.0)."""
        total = len(self._defaults)
        return sum(1 for d in self._defaults.values() if d.is_rewritten) / total if total else 0.0

    @property
    def read_count(self) -> int:
        return sum(1 for d in self._defaults.values() if d.is_read)

    @property
    def rewritten_count(self) -> int:
        return sum(1 for d in self._defaults.values() if d.is_rewritten)

    def by_category(self, category: DefaultCategory) -> list:
        """All defaults in a given category."""
        return [d for d in self._defaults.values() if d.category == category]

    def get_default(self, key: str) -> Optional[Default]:
        """Get the full Default object (for inspection / UI)."""
        return self._defaults.get(key)

    # ── Registration ─────────────────────────────────────────────────

    def _register(self, default: Default):
        self._defaults[default.key] = default

    def _register_all(self):
        """
        Pre-populate every rewritable default in the Society.
        This is the dataset that makes the theme playable.
        """

        # ── TIMING ───────────────────────────────────────────────
        self._register(Default(
            key="timing_window",
            label="Timing Window Width",
            description="Assumes all players react within 200 ms. "
                        "Penalises slower processing speeds.",
            category=DefaultCategory.TIMING,
            rigid_value=0.2,        # 200 ms — punishing
            rewritten_value=0.5,    # 500 ms — generous
        ))

        self._register(Default(
            key="platform_rhythm",
            label="Platform Rhythm",
            description="Platforms move at one fixed tempo. "
                        "No accommodation for observation before action.",
            category=DefaultCategory.TIMING,
            rigid_value=1.0,        # Full speed
            rewritten_value=0.6,    # 60 % speed — room to read
        ))

        self._register(Default(
            key="coyote_time",
            label="Coyote Time",
            description="Zero grace period after leaving a ledge. "
                        "Assumes instant spatial awareness.",
            category=DefaultCategory.TIMING,
            rigid_value=0.0,        # None
            rewritten_value=0.2,    # 200 ms grace
        ))

        self._register(Default(
            key="jump_buffer",
            label="Jump Buffer Window",
            description="No input buffering. "
                        "Requires frame-perfect timing.",
            category=DefaultCategory.TIMING,
            rigid_value=0.0,
            rewritten_value=0.15,
        ))

        # ── SENSORY ──────────────────────────────────────────────
        self._register(Default(
            key="visual_clutter",
            label="Visual Density",
            description="All particle effects, decorations, and ambient motion "
                        "rendered simultaneously. Assumes high sensory filtering.",
            category=DefaultCategory.SENSORY,
            rigid_value=1.0,        # Maximum
            rewritten_value=0.4,    # 40 % — breathable
        ))

        self._register(Default(
            key="audio_layering",
            label="Audio Layering",
            description="Multiple concurrent audio streams with no ducking. "
                        "Assumes ability to parse layered sound.",
            category=DefaultCategory.SENSORY,
            rigid_value=1.0,
            rewritten_value=0.5,
        ))

        self._register(Default(
            key="screen_shake",
            label="Screen Shake Intensity",
            description="Full camera shake on impacts. "
                        "Assumes vestibular comfort.",
            category=DefaultCategory.SENSORY,
            rigid_value=1.0,
            rewritten_value=0.0,
        ))

        # ── ROUTING ──────────────────────────────────────────────
        self._register(Default(
            key="route_strictness",
            label="Route Strictness",
            description="Single valid path through each area. "
                        "Penalises alternative approaches.",
            category=DefaultCategory.ROUTING,
            rigid_value=1.0,        # One path only
            rewritten_value=0.3,    # Multiple valid routes
        ))

        self._register(Default(
            key="safe_route_visibility",
            label="Safe Route Visibility",
            description="Accessible routes are hidden behind harder paths. "
                        "Assumes safe routes are 'easy mode'.",
            category=DefaultCategory.ROUTING,
            rigid_value=0.0,        # Hidden
            rewritten_value=1.0,    # Fully visible, main path
        ))

        # ── SOCIAL ───────────────────────────────────────────────
        self._register(Default(
            key="communication_rigidity",
            label="Communication Mode",
            description="Only one expression style is accepted. "
                        "Penalises non-verbal or icon-based communication.",
            category=DefaultCategory.SOCIAL,
            rigid_value=1.0,        # Single mode
            rewritten_value=0.0,    # All modes equal
        ))

        self._register(Default(
            key="social_script_penalty",
            label="Social Script Penalty",
            description="NPCs penalise 'unexpected' dialogue responses. "
                        "Assumes one correct conversational flow.",
            category=DefaultCategory.SOCIAL,
            rigid_value=1.0,
            rewritten_value=0.0,
        ))

        # ── FAILURE ──────────────────────────────────────────────
        self._register(Default(
            key="failure_penalty",
            label="Failure Penalty",
            description="Falling or missing a jump resets significant progress. "
                        "Assumes failure is deviation, not information.",
            category=DefaultCategory.FAILURE,
            rigid_value=1.0,        # Full reset
            rewritten_value=0.1,    # Gentle re-position
        ))

        self._register(Default(
            key="retry_cost",
            label="Retry Cost",
            description="Retrying a section costs resources. "
                        "Assumes learning happens on the first attempt.",
            category=DefaultCategory.FAILURE,
            rigid_value=1.0,
            rewritten_value=0.0,    # Free retries
        ))

        # ── CONSENT ──────────────────────────────────────────────
        self._register(Default(
            key="consent_gates",
            label="Consent Gates",
            description="No confirmation before danger zones. "
                        "Assumes willingness to proceed.",
            category=DefaultCategory.CONSENT,
            rigid_value=0.0,        # No gates
            rewritten_value=1.0,    # Gates present
        ))

        self._register(Default(
            key="opt_out_available",
            label="Opt-Out Availability",
            description="No way to leave an encounter once started. "
                        "Assumes commitment is always free.",
            category=DefaultCategory.CONSENT,
            rigid_value=0.0,
            rewritten_value=1.0,
        ))
