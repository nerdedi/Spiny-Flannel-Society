"""
Spiny Flannel Society — Event System
Signal/pulse/translation events.

Decouples systems: any module can emit/listen without direct imports.
"""

from typing import Callable, Dict, List, Any
from dataclasses import dataclass, field


# ─── Event Types ─────────────────────────────────────────────────────

class EventType:
    """Well-known event names used throughout the prototype."""

    # Player actions
    ASSUMPTION_REVEALED = "assumption_revealed"
    ASSUMPTION_REWRITTEN = "assumption_rewritten"
    DEFAULT_READ = "default_read"
    DEFAULT_REWRITTEN = "default_rewritten"

    # Windprint Rig
    MODE_SWITCHED = "mode_switched"
    CUSHION_ACTIVATED = "cushion_activated"
    GUARD_ACTIVATED = "guard_activated"
    SAFE_POCKET_SPAWNED = "safe_pocket_spawned"
    CONSENT_GATE_CREATED = "consent_gate_created"

    # Combat / Pattern Intervention
    VERB_USED = "verb_used"
    PATTERN_RESOLVED = "pattern_resolved"
    ENCOUNTER_STARTED = "encounter_started"
    ENCOUNTER_COMPLETED = "encounter_completed"

    # Narrative
    CHAPTER_ADVANCED = "chapter_advanced"
    CIVIC_RULE_RESTORED = "civic_rule_restored"
    ELECTIVE_COMPLETED = "elective_completed"
    DIALOGUE_STARTED = "dialogue_started"

    # World
    DRIFT_REDUCED = "drift_reduced"
    DISTRICT_ENTERED = "district_entered"

    # Accessibility / Defaults
    DEFAULTS_CHANGED = "defaults_changed"


@dataclass
class Event:
    """A single event instance."""
    type: str
    data: Dict[str, Any] = field(default_factory=dict)
    source: str = ""


# ─── Event Bus ───────────────────────────────────────────────────────

class EventBus:
    """
    Simple pub/sub event bus.

    Usage:
        bus = EventBus()
        bus.subscribe(EventType.VERB_USED, my_handler)
        bus.emit(Event(EventType.VERB_USED, {"verb": "pulse"}))
    """

    def __init__(self):
        self._listeners: Dict[str, List[Callable[[Event], None]]] = {}
        self._history: List[Event] = []

    def subscribe(self, event_type: str, handler: Callable[[Event], None]):
        self._listeners.setdefault(event_type, []).append(handler)

    def unsubscribe(self, event_type: str, handler: Callable[[Event], None]):
        if event_type in self._listeners:
            self._listeners[event_type] = [
                h for h in self._listeners[event_type] if h is not handler
            ]

    def emit(self, event: Event):
        self._history.append(event)
        for handler in self._listeners.get(event.type, []):
            handler(event)

    @property
    def history(self) -> List[Event]:
        return list(self._history)

    def clear_history(self):
        self._history.clear()
