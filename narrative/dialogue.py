"""
Spiny Flannel Society — Dialogue
Placeholder dialogue trees and mentor conversations.
"""

from typing import List, Dict
from narrative.characters import DialogueLine, CommunicationStyle


# ─── Mentor Dialogue ────────────────────────────────────────────────

DAZIE_DIALOGUE: Dict[str, List[DialogueLine]] = {
    "guard_mode": [
        DialogueLine(
            speaker="DAZIE Vine",
            text="Guard Mode isn't about keeping others out. "
                 "It's about keeping harmful patterns from getting in.",
            icon_version="🛡️ Guard = protect self from harm",
            minimal_version="Guard: self-protection",
        ),
        DialogueLine(
            speaker="DAZIE Vine",
            text="Rules exist to protect people from power. "
                 "Guard helps you enforce that.",
            icon_version="📜➡️🛡️ Rules protect. Guard enforces.",
            minimal_version="Rules protect. Guard enforces.",
        ),
    ],
    "read_default": [
        DialogueLine(
            speaker="DAZIE Vine",
            text="Before you change anything, you need to understand "
                 "what it assumes.  Read the Default first.",
            icon_version="👁️ Read → then ✏️ Rewrite",
            minimal_version="Read first, then rewrite.",
        ),
    ],
    "consent_gates": [
        DialogueLine(
            speaker="DAZIE Vine",
            text="A consent gate asks: 'Do you want to proceed?' "
                 "That question should never be optional.",
            icon_version="🚪❓ Always ask before danger",
            minimal_version="Always ask first",
        ),
    ],
    "systems_ethics": [
        DialogueLine(
            speaker="DAZIE Vine",
            text="The Society didn't break because people were different. "
                 "It broke because it stopped accommodating difference.",
            icon_version="🏛️💔 ≠ 👥 different. = 🏛️ stopped adapting",
            minimal_version="Society broke by rejecting difference",
        ),
    ],
}

JUNE_DIALOGUE: Dict[str, List[DialogueLine]] = {
    "cushion_mode": [
        DialogueLine(
            speaker="June Corrow",
            text="Cushion isn't about making things easy. "
                 "It's about making space for processing.",
            icon_version="🌿 Cushion = space to think",
            minimal_version="Cushion: space to process",
        ),
    ],
    "quiet_routes": [
        DialogueLine(
            speaker="June Corrow",
            text="The quiet route isn't a shortcut. "
                 "It's the route that should have been the main one.",
            icon_version="🤫🛤️ = main path",
            minimal_version="Quiet route = main route",
        ),
    ],
}
