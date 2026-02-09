"""
Spiny Flannel Society — Preset Narrative Moment
The emotional payoff for the Defaults system.

At a key narrative moment (Chapter 9: Eucalypt Veil), the player
is invited to COMPOSE a preset — choosing which defaults to
prioritise — and then watches NPCs respond to their choices
immediately.

This is the "Defaults Preset moment": the player sees that their
accumulated rewrites have social consequences, and that different
compositions of defaults produce different social textures.
"""

from typing import Dict, List, Tuple
from dataclasses import dataclass, field

from core.defaults_registry import DefaultsRegistry, DefaultCategory
from core.events import EventBus, Event, EventType
from narrative.characters import CHARACTERS


# ─── Preset Composition ─────────────────────────────────────────────

@dataclass
class ComposedPreset:
    """A player-composed preset: their personal balance of defaults."""
    name: str
    values: Dict[str, float] = field(default_factory=dict)
    player_note: str = ""    # Optional player description


@dataclass
class NPCReaction:
    """How an NPC reacts to the player's composed preset."""
    character_id: str
    emotion: str             # "relieved", "curious", "grateful", "reflective"
    dialogue: str
    icon_version: str = ""
    behavioural_change: str = ""


# ─── The Preset Moment ──────────────────────────────────────────────

class PresetMoment:
    """
    The narrative moment where the player composes a preset
    and sees the social consequences.

    Triggered in Chapter 9 (Eucalypt Veil) at the Veil Market's
    central signage hub, where the Society's information systems
    converge.

    Flow:
        1. Winton presents the Composition Terminal
        2. Player sees their rewritten defaults as sliders/cards
        3. Player arranges them into a named preset
        4. The preset is applied to the current district
        5. NPCs react in real time
        6. The player sees the social texture change
    """

    def __init__(self, registry: DefaultsRegistry, event_bus: EventBus):
        self.registry = registry
        self.bus = event_bus
        self.composed_preset: ComposedPreset = None

    def begin(self) -> Dict:
        """
        Start the preset composition moment.
        Returns the current state of all defaults for the player to arrange.
        """
        categories = {}
        for cat in DefaultCategory:
            defaults = self.registry.by_category(cat)
            categories[cat.name] = [
                {
                    "key": d.key,
                    "label": d.label,
                    "is_rewritten": d.is_rewritten,
                    "current_value": d.current_value,
                    "rigid_value": d.rigid_value,
                    "rewritten_value": d.rewritten_value,
                }
                for d in defaults
            ]

        return {
            "scene": "composition_terminal",
            "location": "Veil Market — Central Signage Hub",
            "intro_dialogue": {
                "speaker": "Winton",
                "text": (
                    "You've rewritten enough defaults to compose a preset. "
                    "This is significant. A preset isn't a settings menu — "
                    "it's a statement about what kind of society you want. "
                    "Arrange your values. Name it. Then watch what happens."
                ),
                "icon_version": "⚙️ Compose preset → 🏛️ Society changes",
            },
            "categories": categories,
            "rewrite_progress": self.registry.progress,
        }

    def compose(self, name: str, values: Dict[str, float],
                player_note: str = "") -> ComposedPreset:
        """
        Player submits their composed preset.
        """
        self.composed_preset = ComposedPreset(
            name=name,
            values=values,
            player_note=player_note,
        )

        self.bus.emit(Event(
            EventType.DEFAULTS_CHANGED,
            {"preset_name": name, "values": values},
            source="preset_moment",
        ))

        return self.composed_preset

    def get_npc_reactions(self) -> List[NPCReaction]:
        """
        Generate NPC reactions based on the composed preset.
        Different default compositions produce different social textures.
        """
        if not self.composed_preset:
            return []

        reactions = []
        values = self.composed_preset.values

        # ── DAZIE responds to structural/consent changes ─────────
        consent_value = values.get("consent_gates", 0.0)
        failure_value = values.get("failure_penalty", 1.0)

        if consent_value >= 0.8:
            reactions.append(NPCReaction(
                character_id="dazie",
                emotion="grateful",
                dialogue=(
                    "You prioritised consent gates. That's structural care. "
                    "The Society hasn't felt this safe since the Axiom was active."
                ),
                icon_version="🛡️ Consent gates → 🏛️ Safe Society",
                behavioural_change=(
                    "DAZIE moves to stand at the nearest consent gate, "
                    "demonstrating it to passing NPCs."
                ),
            ))
        elif failure_value <= 0.3:
            reactions.append(NPCReaction(
                character_id="dazie",
                emotion="reflective",
                dialogue=(
                    "Gentle failure. That's not softness — it's information "
                    "architecture. The Society can learn again."
                ),
                icon_version="🔄 Gentle failure → 📚 Learning enabled",
                behavioural_change=(
                    "DAZIE sits on a bench and writes in a journal, "
                    "modelling rest as productive."
                ),
            ))
        else:
            reactions.append(NPCReaction(
                character_id="dazie",
                emotion="curious",
                dialogue=(
                    "Interesting composition. Every preset is a hypothesis "
                    "about how a society should work. Let's see what yours does."
                ),
                icon_version="🔬 Your preset → hypothesis about society",
                behavioural_change="DAZIE observes nearby interactions with interest.",
            ))

        # ── JUNE responds to sensory/routing changes ─────────────
        clutter_value = values.get("visual_clutter", 1.0)
        route_vis = values.get("safe_route_visibility", 0.0)

        if clutter_value <= 0.4:
            reactions.append(NPCReaction(
                character_id="june",
                emotion="relieved",
                dialogue=(
                    "The filtration layers are back. I can see the architecture "
                    "again — not just the noise on top of it. Thank you."
                ),
                icon_version="🌿 Filtered → 🏗️ Architecture visible",
                behavioural_change=(
                    "June begins sketching on a wall — designing new quiet "
                    "infrastructure now that the clutter is gone."
                ),
            ))
        elif route_vis >= 0.8:
            reactions.append(NPCReaction(
                character_id="june",
                emotion="grateful",
                dialogue=(
                    "Safe routes visible. I designed them — and watched them "
                    "disappear under the Drift. Seeing them lit again… it matters."
                ),
                icon_version="🛤️ Safe routes → visible again 💡",
                behavioural_change=(
                    "June walks along a newly visible safe route, "
                    "pointing out design details to nearby residents."
                ),
            ))
        else:
            reactions.append(NPCReaction(
                character_id="june",
                emotion="curious",
                dialogue=(
                    "Every composition has a texture. I can feel yours "
                    "already — the air is different here."
                ),
                icon_version="🌬️ Preset → new texture",
                behavioural_change="June pauses and breathes deeply.",
            ))

        # ── WINTON responds to systemic completeness ─────────────
        rewrite_count = sum(
            1 for v in values.values() if v != self.registry.get_default(
                next((k for k in self.registry._defaults if self.registry._defaults[k].rigid_value == v), "")
            )
        ) if values else 0

        progress = self.registry.progress

        if progress >= 0.7:
            reactions.append(NPCReaction(
                character_id="winton",
                emotion="measured",
                dialogue=(
                    f"Preset '{self.composed_preset.name}' registered. "
                    f"Default rewrite progress: {progress*100:.0f}%. "
                    f"The Society's operating parameters are shifting. "
                    f"I can feel the Axiom returning — cautiously."
                ),
                icon_version=f"📊 {progress*100:.0f}% rewritten → Axiom returning",
                behavioural_change=(
                    "Winton's voice gains a slight warmth — as close to "
                    "emotion as a Civic OS gets."
                ),
            ))
        else:
            reactions.append(NPCReaction(
                character_id="winton",
                emotion="measured",
                dialogue=(
                    f"Preset '{self.composed_preset.name}' registered. "
                    f"Progress: {progress*100:.0f}%. Continue."
                ),
                icon_version=f"📊 {progress*100:.0f}% → continue",
                behavioural_change="Winton resumes monitoring.",
            ))

        # ── Ambient NPC reactions ────────────────────────────────
        timing = values.get("timing_window", 0.2)
        comm = values.get("communication_rigidity", 1.0)

        if timing >= 0.4:
            reactions.append(NPCReaction(
                character_id="ambient_npc_1",
                emotion="relieved",
                dialogue="[An NPC who was rushing through the market slows down, looks around, and smiles.]",
                behavioural_change="Walking speed decreases to a comfortable pace.",
            ))

        if comm <= 0.3:
            reactions.append(NPCReaction(
                character_id="ambient_npc_2",
                emotion="joyful",
                dialogue="[Two NPCs who were standing silently begin communicating — one uses icons, the other gestures. They understand each other.]",
                behavioural_change="New NPC interactions appear using diverse communication modes.",
            ))

        return reactions

    def narrate_outcome(self) -> str:
        """
        Produce a narrative summary of the preset moment's impact.
        This is the emotional payoff text.
        """
        if not self.composed_preset:
            return ""

        reactions = self.get_npc_reactions()
        progress = self.registry.progress

        lines = [
            f"\n{'─'*60}",
            f"  PRESET COMPOSED: \"{self.composed_preset.name}\"",
            f"{'─'*60}",
            "",
        ]

        if self.composed_preset.player_note:
            lines.append(f"  Player's note: \"{self.composed_preset.player_note}\"")
            lines.append("")

        lines.append("  The preset takes effect. The district shifts.\n")

        for reaction in reactions:
            char = CHARACTERS.get(reaction.character_id)
            name = char.name if char else reaction.character_id
            lines.append(f"  [{name}] ({reaction.emotion})")
            lines.append(f"  {reaction.dialogue}")
            if reaction.behavioural_change:
                lines.append(f"  → {reaction.behavioural_change}")
            lines.append("")

        if progress >= 0.5:
            lines.append(
                "  The wind changes. Not back to how it was — forward, "
                "to something the Society hasn't tried yet."
            )
        else:
            lines.append(
                "  A small shift. But the residents notice. "
                "That's how systems change — one default at a time."
            )

        lines.append(f"\n{'─'*60}")
        return "\n".join(lines)
