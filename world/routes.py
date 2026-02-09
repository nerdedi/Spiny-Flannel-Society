"""
Spiny Flannel Society — Routes
Route logic: safe routes, alternative routes, route visibility.
"""

from typing import List, Optional
from dataclasses import dataclass, field


@dataclass
class RouteSegment:
    """A segment of a traversal route through a district."""
    id: str
    name: str
    difficulty: float = 0.5        # 0 = effortless, 1 = demanding
    is_safe_route: bool = False    # Safe routes are main routes
    is_alternative: bool = False   # True = optional harder path
    is_visible: bool = True        # Affected by safe_route_visibility default
    requires_guard: bool = False   # Needs Guard to stabilise
    requires_cushion: bool = False # Benefits from Cushion


@dataclass
class Route:
    """A complete route through a game area."""
    id: str
    name: str
    segments: List[RouteSegment] = field(default_factory=list)
    district_id: str = ""

    @property
    def is_safe(self) -> bool:
        return all(s.is_safe_route for s in self.segments)

    @property
    def average_difficulty(self) -> float:
        if not self.segments:
            return 0.0
        return sum(s.difficulty for s in self.segments) / len(self.segments)

    def accessible_segments(self, guard_active: bool, cushion_active: bool) -> List[RouteSegment]:
        """Which segments are accessible given current Windprint state."""
        result = []
        for s in self.segments:
            if s.requires_guard and not guard_active:
                continue
            if s.requires_cushion and not cushion_active:
                # Cushion segments are always *traversable*, just harder without
                result.append(s)
            else:
                result.append(s)
        return result
