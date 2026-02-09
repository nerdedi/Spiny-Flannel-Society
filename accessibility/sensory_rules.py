"""
Spiny Flannel Society — Sensory Rules
Rules that govern how sensory information is presented.

These rules respond to the DefaultsRegistry, so when the player
rewrites a sensory default, these rules change automatically.
"""

from typing import Dict
from dataclasses import dataclass

from core.defaults_registry import DefaultsRegistry


@dataclass
class SensoryProfile:
    """Current sensory configuration derived from defaults."""
    visual_density: float      # 0 = minimal, 1 = maximum
    audio_layers: float        # 0 = single layer, 1 = all layers
    screen_shake: float        # 0 = none, 1 = full
    subtitle_style: str        # "standard", "high_contrast", "dyslexia_friendly", "minimal"
    motion_intensity: float    # 0 = no motion, 1 = full motion


def get_sensory_profile(registry: DefaultsRegistry) -> SensoryProfile:
    """
    Build a sensory profile from current registry values.
    Any system that renders graphics/audio should call this.
    """
    return SensoryProfile(
        visual_density=registry.get("visual_clutter") or 0.4,
        audio_layers=registry.get("audio_layering") or 0.5,
        screen_shake=registry.get("screen_shake") or 0.0,
        subtitle_style="standard",  # Player preference, not a default
        motion_intensity=registry.get("platform_rhythm") or 0.6,
    )


SUBTITLE_STYLES = ["standard", "high_contrast", "dyslexia_friendly", "minimal"]
