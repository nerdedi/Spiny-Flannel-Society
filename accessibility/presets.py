"""
Spiny Flannel Society — Accessibility Presets
Named preset configurations that set multiple defaults at once.

These are NOT difficulty levels — they are sensory and pacing profiles.
Every preset leads to the same story outcome and full content access.
"""

from typing import Dict
from dataclasses import dataclass, field


@dataclass
class Preset:
    """A named accessibility preset."""
    id: str
    name: str
    description: str
    overrides: Dict[str, float] = field(default_factory=dict)


# ─── Built-in Presets ────────────────────────────────────────────────

PRESETS: Dict[str, Preset] = {
    "default": Preset(
        id="default",
        name="Society Standard",
        description="The Society as-designed: balanced pacing, standard sensory load. "
                    "A good starting point.",
        overrides={},  # Uses DefaultsRegistry as-is
    ),
    "gentle": Preset(
        id="gentle",
        name="Gentle Current",
        description="Wider timing, reduced clutter, softer feedback. "
                    "More room to observe before acting.",
        overrides={
            "timing_window": 0.6,
            "visual_clutter": 0.3,
            "audio_layering": 0.4,
            "screen_shake": 0.0,
            "failure_penalty": 0.0,
            "platform_rhythm": 0.5,
        },
    ),
    "focused": Preset(
        id="focused",
        name="Focused Flow",
        description="Minimal visual noise, strong routing cues, predictable transitions. "
                    "Optimised for sensory clarity.",
        overrides={
            "visual_clutter": 0.2,
            "audio_layering": 0.3,
            "screen_shake": 0.0,
            "route_strictness": 0.2,
            "safe_route_visibility": 1.0,
        },
    ),
    "challenge": Preset(
        id="challenge",
        name="Sharp Edge",
        description="Tighter timing, full density. For players who want the "
                    "precision platformer experience. Still non-punishing.",
        overrides={
            "timing_window": 0.25,
            "platform_rhythm": 0.9,
            "coyote_time": 0.08,
            "failure_penalty": 0.3,  # Still mild
        },
    ),
    "low_motion": Preset(
        id="low_motion",
        name="Still Air",
        description="Reduced motion, no screen shake, slower camera. "
                    "For vestibular comfort.",
        overrides={
            "screen_shake": 0.0,
            "visual_clutter": 0.3,
            "platform_rhythm": 0.5,
        },
    ),
}
