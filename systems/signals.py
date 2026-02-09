"""
Spiny Flannel Society — Signals System
Translation events: Read Default → Rewrite Default loop.

This module implements the explicit early-game verb pair
recommended in the design feedback:

  1. **Read Default** — highlights assumptions in geometry, timing,
     UI, or routes.  Gives the player a learning loop.
  2. **Rewrite Default** — applies Cushion or Guard only AFTER reading.
     Gives a clear player fantasy and teachable interaction.
"""

from typing import List, Optional, Dict
from dataclasses import dataclass

from core.defaults_registry import DefaultsRegistry, Default, DefaultCategory
from core.events import EventBus, Event, EventType


# ─── Translation Verbs ───────────────────────────────────────────────

class TranslationVerbs:
    """
    Player-facing verbs for interacting with the Defaults system.
    """

    def __init__(self, registry: DefaultsRegistry, event_bus: EventBus):
        self.registry = registry
        self.bus = event_bus

    def read_default(self, key: str) -> Optional[str]:
        """
        *Read Default* — highlights the assumption behind a default.

        The player must Read before they can Rewrite.
        Returns a readable description, or None if the key is unknown.
        """
        result = self.registry.read(key)
        if result:
            self.bus.emit(Event(
                EventType.DEFAULT_READ,
                {"key": key, "description": result},
                source="translation_verbs",
            ))
        return result

    def rewrite_default(self, key: str, via_mode: str = "cushion") -> bool:
        """
        *Rewrite Default* — changes the default to its inclusive value.

        Can only be called after Read Default on the same key.
        `via_mode` records whether the rewrite used Cushion or Guard intent.
        """
        success = self.registry.rewrite(key)
        if success:
            self.bus.emit(Event(
                EventType.DEFAULT_REWRITTEN,
                {"key": key, "mode": via_mode},
                source="translation_verbs",
            ))
        return success

    def scan_area(self, area_defaults: List[str]) -> List[Dict]:
        """
        Scan a game area for readable defaults.
        Returns a list of summaries the player can inspect.
        """
        results = []
        for key in area_defaults:
            d = self.registry.get_default(key)
            if d and not d.is_read:
                results.append({
                    "key": d.key,
                    "label": d.label,
                    "category": d.category.name,
                    "hint": d.description[:60] + "…" if len(d.description) > 60 else d.description,
                })
        return results
