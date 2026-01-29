"""
Spiny Flannel Society - Chapter & Narrative System
12-chapter narrative progression through the Society.

Each chapter restores a civic rule and reduces The Drift.
Chapters never gate accessibility - safe routes are main routes.
"""

from typing import List, Dict, Optional, Callable
from dataclasses import dataclass, field
from enum import Enum, auto

from game_config import (
    CHAPTER_DATA, ChapterData, TOTAL_CHAPTERS, CIVIC_RULES,
    Districts, DISTRICT_DATA, Characters, CHARACTER_DATA,
    CombatVerbs, TranslatorAbilities, ElectiveSubjects
)
from game_entities import (
    Vector3, Translator, GameWorld, District, DesignTerminal,
    ContradictorySpace, CorruptedSignal, EchoForm, Distortion, NoiseBeast
)


# =============================================================================
# DIALOGUE SYSTEM
# =============================================================================

class CommunicationStyle(Enum):
    """How dialogue is presented based on player preference"""
    DIRECT = "direct"  # Full sentences
    SCRIPTED = "scripted"  # Formal, structured
    ICONS = "icons"  # Visual symbols with minimal text
    MINIMAL = "minimal"  # Key words only


@dataclass
class DialogueLine:
    """A single line of dialogue"""
    speaker: str
    text: str
    icon_version: str = ""  # For icon communication mode
    minimal_version: str = ""  # For minimal speech mode
    emotion: str = "neutral"

    def get_text(self, style: CommunicationStyle) -> str:
        """Get text appropriate for communication style"""
        if style == CommunicationStyle.ICONS and self.icon_version:
            return self.icon_version
        elif style == CommunicationStyle.MINIMAL and self.minimal_version:
            return self.minimal_version
        return self.text


@dataclass
class DialogueNode:
    """A node in a dialogue tree"""
    id: str
    lines: List[DialogueLine]
    responses: List['DialogueResponse'] = field(default_factory=list)
    next_node: str = None  # Auto-advance to this node if no responses
    triggers_event: str = None  # Event to trigger after node


@dataclass
class DialogueResponse:
    """A player response option"""
    text: str
    icon_version: str = ""
    next_node: str = None
    requires_ability: str = None


# =============================================================================
# CHARACTER DEFINITIONS
# =============================================================================

class Character:
    """An NPC character in the game"""

    def __init__(self, character_id: str):
        self.id = character_id
        self.data = CHARACTER_DATA.get(character_id, {})
        self.name = self.data.get("name", character_id)
        self.role = self.data.get("role", "")
        self.description = self.data.get("description", "")
        self.voice = self.data.get("voice", "")
        self.introduces = self.data.get("introduces", [])

        # Character state
        self.current_location: str = None
        self.dialogue_state: Dict[str, bool] = {}
        self.relationship_level = 0

    def get_greeting(self, chapter: int) -> str:
        """Get chapter-appropriate greeting"""
        # Could be expanded with chapter-specific dialogue
        return f"{self.name} acknowledges your presence."


class DAZIEVine(Character):
    """DAZIE Vine - Mentor / Systems Ethicist"""

    def __init__(self):
        super().__init__(Characters.DAZIE)
        self.mentor_lessons_given: List[str] = []

    def get_mentor_dialogue(self, topic: str) -> List[DialogueLine]:
        """Get mentorship dialogue on a topic"""
        dialogues = {
            "guard_mode": [
                DialogueLine(
                    speaker=self.name,
                    text="Guard Mode isn't about keeping others out. It's about keeping harmful patterns from getting in.",
                    icon_version="🛡️ Guard = protect self from harm",
                    minimal_version="Guard: self-protection"
                ),
                DialogueLine(
                    speaker=self.name,
                    text="Rules exist to protect people from power. Guard helps you enforce that.",
                    icon_version="📜➡️🛡️ Rules protect. Guard enforces.",
                    minimal_version="Rules protect. Guard enforces."
                )
            ],
            "consent_gates": [
                DialogueLine(
                    speaker=self.name,
                    text="A consent gate asks: 'Do you want to proceed?' That question should never be optional.",
                    icon_version="🚪❓ Always ask before danger",
                    minimal_version="Always ask first"
                )
            ],
            "systems_ethics": [
                DialogueLine(
                    speaker=self.name,
                    text="The Society didn't break because people were different. It broke because it stopped accommodating difference.",
                    icon_version="🏛️💔 ≠ 👥 different. = 🏛️ stopped adapting",
                    minimal_version="Society broke by rejecting difference"
                )
            ]
        }
        return dialogues.get(topic, [])


class JuneCorrow(Character):
    """June Corrow - Sensory Architect / Biodesign Maker"""

    def __init__(self):
        super().__init__(Characters.JUNE)
        self.quiet_routes_revealed: List[str] = []

    def get_architect_dialogue(self, topic: str) -> List[DialogueLine]:
        """Get dialogue about sensory architecture"""
        dialogues = {
            "cushion_mode": [
                DialogueLine(
                    speaker=self.name,
                    text="Cushion Mode creates softness. The world doesn't have to be hard.",
                    icon_version="☁️ Cushion = soft world",
                    minimal_version="Cushion: softness"
                ),
                DialogueLine(
                    speaker=self.name,
                    text="I designed quiet routes years ago. They called them 'non-essential'. They were wrong.",
                    icon_version="🤫🛤️ Quiet routes = essential",
                    minimal_version="Quiet routes: essential"
                )
            ],
            "filtration": [
                DialogueLine(
                    speaker=self.name,
                    text="Filtration isn't hiding. It's choosing what helps.",
                    icon_version="🔍 Filter = choose helpful",
                    minimal_version="Filter: choose helpful"
                )
            ],
            "rest_pockets": [
                DialogueLine(
                    speaker=self.name,
                    text="Rest isn't weakness. It's maintenance. The Society forgot that.",
                    icon_version="😴 = 🔧 Rest = maintenance",
                    minimal_version="Rest: maintenance"
                )
            ]
        }
        return dialogues.get(topic, [])


class Winton(Character):
    """Winton - Civic OS / System-Ghost"""

    def __init__(self):
        super().__init__(Characters.WINTON)
        self.system_states_reported: List[str] = []

    def report_system_state(self, system: str, state: str) -> DialogueLine:
        """Report on a system's state"""
        self.system_states_reported.append(system)
        return DialogueLine(
            speaker=self.name,
            text=f"System status: {system}. Current state: {state}.",
            icon_version=f"⚙️ {system}: {state}",
            minimal_version=f"{system}: {state}",
            emotion="neutral"
        )

    def get_audit_dialogue(self, rule: str) -> List[DialogueLine]:
        """Get dialogue auditing a civic rule"""
        rule_desc = CIVIC_RULES.get(rule, "Unknown rule")
        return [
            DialogueLine(
                speaker=self.name,
                text=f"Civic rule detected: {rule}. Status: Pending restoration.",
                icon_version=f"📋 {rule}: ⏳",
                minimal_version=f"Rule {rule}: pending"
            ),
            DialogueLine(
                speaker=self.name,
                text=f"Rule definition: {rule_desc}",
                icon_version=f"📖 {rule_desc}",
                minimal_version=rule_desc
            )
        ]


# =============================================================================
# ELECTIVE SYSTEM
# =============================================================================

@dataclass
class Elective:
    """
    An optional challenge room embedding educational content.
    Electives never gate story - they reward lore, cosmetics, shortcuts.
    """
    id: str
    name: str
    subject: str  # ElectiveSubjects value
    description: str
    difficulty: int  # 1-3
    rewards: List[str]
    is_completed: bool = False

    def get_subject_icon(self) -> str:
        """Get icon for the subject"""
        icons = {
            ElectiveSubjects.LOGIC: "🧩",
            ElectiveSubjects.LITERACY: "📚",
            ElectiveSubjects.NUMERACY: "🔢",
            ElectiveSubjects.LANGUAGE: "💬",
            ElectiveSubjects.DIGITAL: "💻"
        }
        return icons.get(self.subject, "📝")


# =============================================================================
# CHAPTER SYSTEM
# =============================================================================

class ChapterState(Enum):
    """State of a chapter"""
    LOCKED = auto()
    AVAILABLE = auto()
    IN_PROGRESS = auto()
    COMPLETED = auto()


@dataclass
class ChapterProgress:
    """Tracks progress within a chapter"""
    assumptions_revealed: int = 0
    assumptions_rewritten: int = 0
    patterns_resolved: int = 0
    design_terminal_used: bool = False
    electives_completed: List[str] = field(default_factory=list)
    civic_rule_restored: bool = False


class Chapter:
    """
    A chapter in the 12-chapter narrative.
    Each chapter focuses on a district, theme, and civic rule to restore.
    """

    def __init__(self, chapter_id: int):
        self.id = chapter_id
        self.data = CHAPTER_DATA.get(chapter_id)

        if self.data:
            self.name = self.data.name
            self.location = self.data.location
            self.theme = self.data.theme
            self.civic_rule = self.data.civic_rule
            self.primary_mechanic = self.data.primary_mechanic
            self.npcs = self.data.npcs
        else:
            self.name = f"Chapter {chapter_id}"
            self.location = Districts.WINDGAP_ACADEMY
            self.theme = "Unknown"
            self.civic_rule = "UNKNOWN"
            self.primary_mechanic = "traversal"
            self.npcs = []

        self.state = ChapterState.LOCKED if chapter_id > 1 else ChapterState.AVAILABLE
        self.progress = ChapterProgress()
        self.electives: List[Elective] = []
        self.design_terminal: DesignTerminal = None

        # Chapter-specific content
        self.intro_dialogue: List[DialogueNode] = []
        self.outro_dialogue: List[DialogueNode] = []

    def start(self):
        """Start this chapter"""
        self.state = ChapterState.IN_PROGRESS

    def complete(self):
        """Complete this chapter"""
        self.state = ChapterState.COMPLETED
        self.progress.civic_rule_restored = True

    def is_complete(self) -> bool:
        """Check if chapter completion requirements are met"""
        return self.progress.design_terminal_used and self.progress.civic_rule_restored

    def get_completion_percentage(self) -> float:
        """Get chapter completion percentage"""
        total_objectives = 3  # Minimum: reveal assumptions, resolve patterns, use terminal
        completed = 0

        if self.progress.assumptions_revealed > 0:
            completed += 1
        if self.progress.patterns_resolved > 0:
            completed += 1
        if self.progress.design_terminal_used:
            completed += 1

        return (completed / total_objectives) * 100


# =============================================================================
# CHAPTER MANAGER
# =============================================================================

class ChapterManager:
    """
    Manages the 12-chapter narrative progression.
    """

    def __init__(self, world: GameWorld, translator: Translator):
        self.world = world
        self.translator = translator

        # Initialize all chapters
        self.chapters: Dict[int, Chapter] = {
            i: Chapter(i) for i in range(1, TOTAL_CHAPTERS + 1)
        }

        # Initialize characters
        self.characters: Dict[str, Character] = {
            Characters.DAZIE: DAZIEVine(),
            Characters.JUNE: JuneCorrow(),
            Characters.WINTON: Winton()
        }

        self.current_chapter_id = 1
        self._setup_chapter_content()

    def _setup_chapter_content(self):
        """Set up content for each chapter"""
        # Chapter 1: Bract Theory
        ch1 = self.chapters[1]
        ch1.design_terminal = DesignTerminal(
            "terminal_ch1",
            "ACCESS_WITHOUT_PROOF",
            Vector3(0, 0, 0)
        )
        ch1.electives.append(Elective(
            id="elective_ch1_literacy",
            name="Orientation Decode",
            subject=ElectiveSubjects.LITERACY,
            description="Decode the welcome banners to reveal their true meaning",
            difficulty=1,
            rewards=["lore_bract_history", "cosmetic_bract_pattern"]
        ))

        # Chapter 2: Felt Memory
        ch2 = self.chapters[2]
        ch2.design_terminal = DesignTerminal(
            "terminal_ch2",
            "BUFFERS_BY_DEFAULT",
            Vector3(10, 0, 0)
        )
        ch2.electives.append(Elective(
            id="elective_ch2_logic",
            name="Archive Pathfinding",
            subject=ElectiveSubjects.LOGIC,
            description="Navigate the archive using IF/THEN logic tiles",
            difficulty=1,
            rewards=["lore_quiet_routes", "shortcut_archive"]
        ))

        # Add design terminals for remaining chapters
        for i in range(3, TOTAL_CHAPTERS + 1):
            ch = self.chapters[i]
            ch_data = CHAPTER_DATA.get(i)
            if ch_data:
                ch.design_terminal = DesignTerminal(
                    f"terminal_ch{i}",
                    ch_data.civic_rule,
                    Vector3(i * 10, 0, 0)
                )

    @property
    def current_chapter(self) -> Chapter:
        """Get the current chapter"""
        return self.chapters[self.current_chapter_id]

    def start_chapter(self, chapter_id: int) -> bool:
        """Start a specific chapter"""
        if chapter_id not in self.chapters:
            return False

        chapter = self.chapters[chapter_id]

        # Check if chapter is available
        if chapter.state == ChapterState.LOCKED:
            # Unlock if previous chapter is complete
            if chapter_id > 1:
                prev = self.chapters[chapter_id - 1]
                if prev.state != ChapterState.COMPLETED:
                    return False
            chapter.state = ChapterState.AVAILABLE

        chapter.start()
        self.current_chapter_id = chapter_id
        self.world.current_chapter = chapter_id

        # Unlock chapter-specific abilities
        self._unlock_chapter_abilities(chapter_id)

        return True

    def _unlock_chapter_abilities(self, chapter_id: int):
        """Unlock abilities introduced in a chapter"""
        unlocks = {
            1: [TranslatorAbilities.CUSHION_MODE, TranslatorAbilities.GUARD_MODE,
                CombatVerbs.PULSE],
            2: [CombatVerbs.RADIANT_HOLD],
            3: [CombatVerbs.THREAD_LASH],
            5: [CombatVerbs.EDGE_CLAIM],
            6: [CombatVerbs.RETUNE, TranslatorAbilities.CREATE_PATHWAYS]
        }

        for unlock in unlocks.get(chapter_id, []):
            if unlock in [v for v in dir(CombatVerbs) if not v.startswith('_')]:
                self.translator.unlock_verb(unlock)
            else:
                self.translator.unlock_ability(unlock)

    def complete_chapter(self, chapter_id: int = None) -> bool:
        """Complete a chapter"""
        chapter_id = chapter_id or self.current_chapter_id
        chapter = self.chapters.get(chapter_id)

        if not chapter or chapter.state != ChapterState.IN_PROGRESS:
            return False

        chapter.complete()

        # Restore civic rule in world
        self.world.restore_civic_rule(chapter.civic_rule)

        # Record in translator's windprint
        self.translator.windprint_record.civic_rules_restored.append(chapter.civic_rule)
        self.translator.complete_chapter(chapter_id)

        # Unlock next chapter
        if chapter_id < TOTAL_CHAPTERS:
            next_chapter = self.chapters[chapter_id + 1]
            next_chapter.state = ChapterState.AVAILABLE

        return True

    def get_chapter_summary(self, chapter_id: int = None) -> Dict:
        """Get summary of a chapter's status"""
        chapter_id = chapter_id or self.current_chapter_id
        chapter = self.chapters.get(chapter_id)

        if not chapter:
            return {}

        return {
            "id": chapter.id,
            "name": chapter.name,
            "location": DISTRICT_DATA.get(chapter.location, {}).get("name", chapter.location),
            "theme": chapter.theme,
            "civic_rule": chapter.civic_rule,
            "civic_rule_description": CIVIC_RULES.get(chapter.civic_rule, ""),
            "state": chapter.state.name,
            "completion": f"{chapter.get_completion_percentage():.0f}%",
            "npcs": chapter.npcs,
            "electives_completed": len(chapter.progress.electives_completed),
            "electives_total": len(chapter.electives)
        }

    def get_narrative_state(self) -> Dict:
        """Get overall narrative state"""
        completed = sum(1 for ch in self.chapters.values() if ch.state == ChapterState.COMPLETED)

        return {
            "current_chapter": self.current_chapter_id,
            "chapters_completed": completed,
            "total_chapters": TOTAL_CHAPTERS,
            "progress_percentage": (completed / TOTAL_CHAPTERS) * 100,
            "drift_intensity": self.world.drift_intensity,
            "narrative_state": self.world.narrative_state,
            "restored_rules": list(self.world.restored_rules)
        }

    def complete_elective(self, elective_id: str) -> bool:
        """Mark an elective as completed"""
        for chapter in self.chapters.values():
            for elective in chapter.electives:
                if elective.id == elective_id:
                    elective.is_completed = True
                    chapter.progress.electives_completed.append(elective_id)
                    self.translator.windprint_record.electives_completed.append(elective_id)
                    return True
        return False


# =============================================================================
# CHAPTER DIALOGUE SCRIPTS
# =============================================================================

def get_chapter_intro_dialogue(chapter_id: int) -> List[DialogueLine]:
    """Get intro dialogue for a chapter"""
    dialogues = {
        1: [
            DialogueLine(
                Characters.DAZIE,
                "Most days the place does orientation for me. Not today.",
                "🏛️ Usually auto-orientation. Not today.",
                "Orientation broken today."
            ),
            DialogueLine(
                Characters.DAZIE,
                "Protection that looks like beauty.",
                "🛡️ = 🌸 Protection = beauty",
                "Protection as beauty."
            ),
            DialogueLine(
                Characters.WINTON,
                "Support withheld pending justification.",
                "⚠️ Support needs proof",
                "Support: needs proof"
            ),
            DialogueLine(
                Characters.DAZIE,
                "That's the new rule. It's wrong. Let's rewrite the welcome.",
                "❌ Bad rule. ✏️ Rewrite.",
                "Wrong rule. Rewrite it."
            )
        ],
        2: [
            DialogueLine(
                Characters.JUNE,
                "They deleted quiet because it didn't measure.",
                "🤫❌ Quiet deleted - not measured",
                "Quiet deleted. Unmeasurable."
            ),
            DialogueLine(
                Characters.DAZIE,
                "It measures to me.",
                "📊 Quiet measures to me.",
                "Quiet matters."
            )
        ],
        12: [
            DialogueLine(
                Characters.DAZIE,
                "You didn't fix us. You reminded us how to care.",
                "💝 Not fixed - remembered care",
                "Reminded us to care."
            ),
            DialogueLine(
                Characters.JUNE,
                "This time it will remember.",
                "🧠 Will remember this time",
                "Will remember."
            ),
            DialogueLine(
                Characters.WINTON,
                "Coherence achieved through plurality.",
                "✅ Coherence = plurality",
                "Plurality = coherence."
            )
        ]
    }
    return dialogues.get(chapter_id, [])
