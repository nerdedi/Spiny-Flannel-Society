"""
Spiny Flannel Society — Characters
NPC definitions and dialogue stubs.
"""

from typing import Dict, List
from dataclasses import dataclass, field
from enum import Enum, auto


class CommunicationStyle(Enum):
    """How dialogue is presented based on player preference."""
    DIRECT = "direct"       # Full sentences
    SCRIPTED = "scripted"   # Formal, structured
    ICONS = "icons"         # Visual symbols, minimal text
    MINIMAL = "minimal"     # Key words only


@dataclass
class DialogueLine:
    """A single line of NPC dialogue, with accessibility variants."""
    speaker: str
    text: str
    icon_version: str = ""
    minimal_version: str = ""
    emotion: str = "neutral"

    def for_style(self, style: CommunicationStyle) -> str:
        if style == CommunicationStyle.ICONS and self.icon_version:
            return self.icon_version
        if style == CommunicationStyle.MINIMAL and self.minimal_version:
            return self.minimal_version
        return self.text


@dataclass
class Character:
    """An NPC in the Society."""
    id: str
    name: str
    role: str
    description: str
    voice: str
    introduces: List[str] = field(default_factory=list)


# ─── Canonical Characters ───────────────────────────────────────────

CHARACTERS: Dict[str, Character] = {
    "translator": Character(
        id="translator",
        name="The Translator",
        role="Player Character",
        description="Newcomer with the rare capacity to perceive Windprints.",
        voice="Player-defined communication mode",
    ),
    "dazie": Character(
        id="dazie",
        name="DAZIE Vine",
        role="Mentor / Systems Ethicist",
        description="Orientation lead at Windgap Academy. "
                    "Teaches Guard Mode, Edge Claim, consent gates.",
        voice="Calm, precise, non-patronising",
        introduces=["Guard Mode", "Edge Claim", "Consent gates"],
    ),
    "june": Character(
        id="june",
        name="June Corrow",
        role="Sensory Architect / Biodesign Maker",
        description="Designed the Society's quiet infrastructure. "
                    "Teaches Cushion Mode, filtration, veil traversal.",
        voice="Sparse, warm, incisive",
        introduces=["Cushion Mode", "Filtration mechanics", "Veil traversal"],
    ),
    "winton": Character(
        id="winton",
        name="Winton",
        role="Civic OS / System-Ghost",
        description="The Society's operating interface made audible. "
                    "Provides system states, design terminals, Windprint recording.",
        voice="Blunt, ethically focused, occasionally dry",
        introduces=["System states", "Design terminals", "Windprint recording"],
    ),
    "ari": Character(
        id="ari",
        name="Ari",
        role="Resident / Guide",
        description="Appears in Chapter 11 at the Reliquary Edge.",
        voice="Hopeful, practical",
    ),
}
