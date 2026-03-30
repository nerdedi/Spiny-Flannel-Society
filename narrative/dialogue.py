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
    "filtration": [
        DialogueLine(
            speaker="June Corrow",
            text="Filtration isn't hiding. It's choosing what helps.",
            icon_version="🔍 Filter = choose helpful",
            minimal_version="Filter: choose helpful",
        ),
    ],
    "rest_pockets": [
        DialogueLine(
            speaker="June Corrow",
            text="Rest isn't weakness. It's maintenance. "
                 "The Society forgot that.",
            icon_version="😴 = 🔧 Rest = maintenance",
            minimal_version="Rest: maintenance",
        ),
    ],
    "belonging": [
        DialogueLine(
            speaker="June Corrow",
            text="Belonging isn't a feeling. It's architecture.",
            icon_version="🏗️ Belonging = architecture",
            minimal_version="Belonging: architecture",
        ),
    ],
    "monocultures": [
        DialogueLine(
            speaker="June Corrow",
            text="Monocultures fail. A single species of thought "
                 "collapses the moment something unexpected arrives.",
            icon_version="🌱❌ Single species = collapse",
            minimal_version="One type = fragile",
        ),
    ],
    "stored_principles": [
        DialogueLine(
            speaker="June Corrow",
            text="We stored what we couldn't protect. "
                 "Now it's time to give it back to the city.",
            icon_version="🏛️→🌱 Stored principles → replant",
            minimal_version="Replant principles",
        ),
    ],
}


WINTON_DIALOGUE: Dict[str, List[DialogueLine]] = {
    "system_status": [
        DialogueLine(
            speaker="Winton",
            text="Default engaged. Default is not neutral.",
            icon_version="⚙️ Default ≠ neutral",
            minimal_version="Defaults aren't neutral",
        ),
    ],
    "correction_engine": [
        DialogueLine(
            speaker="Winton",
            text="Correction process active. Human variance classified as deviation.",
            icon_version="⚠️ Correction running. Variance = 'error'",
            minimal_version="Correction: active. Variance = error.",
        ),
    ],
    "charter_stones": [
        DialogueLine(
            speaker="Winton",
            text="The Society does not forget. It repeats.",
            icon_version="🔄 Society repeats",
            minimal_version="Repeats, not forgets",
        ),
    ],
    "network_integrity": [
        DialogueLine(
            speaker="Winton",
            text="Network integrity declining. Node exclusion cascades.",
            icon_version="📉 Network losing nodes",
            minimal_version="Nodes disconnecting",
        ),
    ],
    "plural_coherence": [
        DialogueLine(
            speaker="Winton",
            text="Coherence achieved through plurality.",
            icon_version="✅ Coherence = plurality",
            minimal_version="Plurality = coherence",
        ),
    ],
}


ARI_DIALOGUE: Dict[str, List[DialogueLine]] = {
    "reliquary": [
        DialogueLine(
            speaker="Ari",
            text="Time to plant it back.",
            icon_version="🌱 Replant principles",
            minimal_version="Plant it back",
        ),
    ],
    "hope": [
        DialogueLine(
            speaker="Ari",
            text="Every civic rule in this vault was someone's fight. "
                 "Now it's ours to carry forward.",
            icon_version="📜→💪 Past fights → our turn",
            minimal_version="Our turn to carry forward",
        ),
    ],
}
