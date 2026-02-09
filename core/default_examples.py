"""
Spiny Flannel Society — Default Examples
One concrete example per default category showing:
    1. The obvious default (what the player encounters)
    2. The dramatic rewrite (what changes)
    3. The visible downstream effect (what the player sees/feels)

These examples serve two purposes:
    - Player onboarding: each category is legible on first encounter
    - Developer reference: engine implementers know exactly what to build
"""

from typing import Dict, List
from dataclasses import dataclass, field


@dataclass
class DownstreamEffect:
    """A visible change in another system caused by rewriting a default."""
    system: str              # Which system is affected
    description: str         # What the player sees
    magnitude: str           # "subtle", "noticeable", "dramatic"


@dataclass
class DefaultExample:
    """
    A worked example of one default being Read and Rewritten,
    showing exactly what happens across the game.
    """
    category: str
    default_key: str
    label: str

    # What the player encounters (before Read)
    encounter_scene: str

    # What Read Default reveals
    assumption_text: str

    # What the rewrite does
    rewrite_description: str

    # What changes across systems (the payoff)
    downstream_effects: List[DownstreamEffect] = field(default_factory=list)


# ─── One Example Per Category ───────────────────────────────────────

EXAMPLES: Dict[str, DefaultExample] = {

    # ── TIMING ───────────────────────────────────────────────────
    "timing": DefaultExample(
        category="TIMING",
        default_key="timing_window",
        label="Timing Window Width",

        encounter_scene=(
            "Windgap Academy, Chapter 1. Three spinning gear-platforms "
            "guard the path to the first Design Terminal.  The gaps between "
            "gears flash open for 200 ms — barely a blink.  Players who "
            "hesitate fall to a soft-catch net and must retry.\n\n"
            "DAZIE says: 'Notice the gears? They were tuned for one speed of "
            "thought. That's the assumption.'"
        ),

        assumption_text=(
            "TIMING WINDOW WIDTH\n"
            "Assumes all players react within 200 ms.\n"
            "Penalises slower processing speeds.\n"
            "Rigid value: 0.2 seconds"
        ),

        rewrite_description=(
            "Player activates Read Default on the gear mechanism.\n"
            "A shimmer highlights the gap timing in amber.\n"
            "Player then uses Rewrite Default (via Cushion).\n"
            "The gears visibly slow — gaps widen to 500 ms.\n"
            "The amber highlight turns teal: rewritten."
        ),

        downstream_effects=[
            DownstreamEffect(
                system="Movement",
                description="All timing-gated obstacles in Windgap Academy "
                            "widen to 500 ms. Jump windows, wall-run timing, "
                            "and grapple catch windows all become more generous.",
                magnitude="dramatic",
            ),
            DownstreamEffect(
                system="Combat",
                description="Distortion cycle patterns in this district now "
                            "hold their vulnerable phase 2.5× longer, giving "
                            "the player more time to use Pulse.",
                magnitude="noticeable",
            ),
            DownstreamEffect(
                system="Narrative",
                description="Winton acknowledges the change: 'Gear timing "
                            "now matches the range of residents, not just the "
                            "fastest.' DAZIE's next dialogue adapts.",
                magnitude="subtle",
            ),
            DownstreamEffect(
                system="World",
                description="Other timing-dependent elements in adjacent "
                            "districts shift slightly — the Society is "
                            "beginning to loosen.",
                magnitude="subtle",
            ),
        ],
    ),

    # ── SENSORY ──────────────────────────────────────────────────
    "sensory": DefaultExample(
        category="SENSORY",
        default_key="visual_clutter",
        label="Visual Density",

        encounter_scene=(
            "Windgap Academy, Chapter 2.  The Archive Walk is a corridor "
            "lined with glowing data shelves, floating particle motes, "
            "animated inscription ribbons, and flickering light sources — "
            "all at once.  The path is clear but the sensory load is "
            "overwhelming.\n\n"
            "June says: 'I designed these archives with filtration layers. "
            "Someone removed them. Now it's all signal, no rest.'"
        ),

        assumption_text=(
            "VISUAL DENSITY\n"
            "All particle effects, decorations, and ambient motion "
            "rendered simultaneously.\n"
            "Assumes high sensory filtering capacity.\n"
            "Rigid value: 1.0 (maximum)"
        ),

        rewrite_description=(
            "Player uses Read Default on the archive corridor.\n"
            "Overlapping visual layers become outlined in distinct colours, "
            "showing which are informational vs decorative.\n"
            "Player uses Rewrite Default (via Cushion).\n"
            "60% of decorative particles fade. Inscription ribbons hold still "
            "until approached. The corridor breathes."
        ),

        downstream_effects=[
            DownstreamEffect(
                system="Rendering / Sensory",
                description="Particle density across Windgap Academy drops to "
                            "40%. Ambient animations pause until the player "
                            "approaches. Light sources hold steady colours.",
                magnitude="dramatic",
            ),
            DownstreamEffect(
                system="Audio",
                description="Audio layering ducks concurrent sounds. The "
                            "archive's background hum quiets to a single tone.",
                magnitude="noticeable",
            ),
            DownstreamEffect(
                system="Narrative",
                description="June smiles and says: 'Look — the filtration "
                            "layers are back. The archive remembers how to "
                            "be gentle.'",
                magnitude="subtle",
            ),
            DownstreamEffect(
                system="NPC Behaviour",
                description="Archive NPCs stop flinching. One sits down on "
                            "a bench that wasn't there before — the bench "
                            "only renders when clutter is reduced.",
                magnitude="dramatic",
            ),
        ],
    ),

    # ── ROUTING ──────────────────────────────────────────────────
    "routing": DefaultExample(
        category="ROUTING",
        default_key="safe_route_visibility",
        label="Safe Route Visibility",

        encounter_scene=(
            "Sandstone Quarter, Chapter 7.  A wide plaza connects three "
            "elevated pathways.  Only one is lit — a narrow, high-speed "
            "sequence of wall-runs and precision jumps.  Below the lit path, "
            "a wider, gentler ramp is present but dimmed to near-invisibility.\n\n"
            "DAZIE says: 'The safe route was always there. The lighting "
            "was designed to hide it — to punish caution.'"
        ),

        assumption_text=(
            "SAFE ROUTE VISIBILITY\n"
            "Accessible routes are hidden behind harder paths.\n"
            "Assumes safe routes are 'easy mode' and should be invisible.\n"
            "Rigid value: 0.0 (hidden)"
        ),

        rewrite_description=(
            "Player uses Read Default on the plaza junction.\n"
            "Both paths glow — the hidden ramp outlines in warm amber.\n"
            "Player uses Rewrite Default (via Guard: pinning the safe "
            "route as a permanent, visible main path).\n"
            "The ramp brightens to full illumination.  The precision path "
            "remains — but is now clearly labelled as optional."
        ),

        downstream_effects=[
            DownstreamEffect(
                system="Routing",
                description="All safe routes across the Sandstone Quarter "
                            "become fully visible.  They are now the main "
                            "path; precision routes are explicitly optional.",
                magnitude="dramatic",
            ),
            DownstreamEffect(
                system="World / District",
                description="Sandstone Quarter's drift level drops visibly.  "
                            "Signage re-renders in multiple modalities — "
                            "text, icons, colour-coded wayfinding.",
                magnitude="noticeable",
            ),
            DownstreamEffect(
                system="NPCs",
                description="A family of NPCs who were stuck at the junction "
                            "(unable to take the precision path) now walk "
                            "confidently along the ramp. One waves.",
                magnitude="dramatic",
            ),
            DownstreamEffect(
                system="Accessibility Preset",
                description="The 'Gentle Current' preset now includes this "
                            "route change as a baseline.  Future preset "
                            "selections are enriched by your rewrite.",
                magnitude="subtle",
            ),
        ],
    ),

    # ── SOCIAL ───────────────────────────────────────────────────
    "social": DefaultExample(
        category="SOCIAL",
        default_key="communication_rigidity",
        label="Communication Mode",

        encounter_scene=(
            "Windgap Academy, Chapter 3.  A Design Terminal asks the player "
            "to respond to a prompt.  But the input field only accepts "
            "typed text — no icon option, no minimal-speech option, "
            "no gesture input.\n\n"
            "Winton says (bluntly): 'This terminal was built by someone "
            "who assumed talking is thinking. It hasn't been updated "
            "since the Drift began.'"
        ),

        assumption_text=(
            "COMMUNICATION MODE\n"
            "Only one expression style (full text) is accepted.\n"
            "Penalises non-verbal, icon-based, or minimal communication.\n"
            "Rigid value: 1.0 (single mode demanded)"
        ),

        rewrite_description=(
            "Player uses Read Default on the Design Terminal.\n"
            "The single text-input field highlights; ghost outlines of "
            "three alternative input modes appear alongside it.\n"
            "Player uses Rewrite Default (via Cushion: widening acceptable "
            "input).\n"
            "All four communication modes — Direct, Scripted, Icons, "
            "Minimal — become selectable. The terminal reshapes."
        ),

        downstream_effects=[
            DownstreamEffect(
                system="Dialogue",
                description="All dialogue options across the Academy now "
                            "appear in the player's chosen communication "
                            "style.  NPCs respond to icon input with equal "
                            "warmth and information.",
                magnitude="dramatic",
            ),
            DownstreamEffect(
                system="Narrative",
                description="DAZIE's next lesson explicitly references the "
                            "change: 'Expression mode no longer determines "
                            "worth.  That's the third civic rule restored.'",
                magnitude="noticeable",
            ),
            DownstreamEffect(
                system="NPC Behaviour",
                description="An NPC who was previously silent (unable to "
                            "use the text-only interface) now communicates "
                            "via icons.  They offer a side observation about "
                            "the Academy's history.",
                magnitude="dramatic",
            ),
            DownstreamEffect(
                system="Electives",
                description="A new Elective (literacy room) unlocks, where "
                            "signal decoding is done across all four "
                            "communication modes simultaneously.",
                magnitude="subtle",
            ),
        ],
    ),

    # ── FAILURE ──────────────────────────────────────────────────
    "failure": DefaultExample(
        category="FAILURE",
        default_key="failure_penalty",
        label="Failure Penalty",

        encounter_scene=(
            "Sandstone Quarter, Chapter 8.  The player falls from a "
            "skybridge.  Instead of a soft respawn, the game resets them "
            "to the district entrance — a three-minute walk.  A counter "
            "flashes: 'Attempt 2 of 5.  Resources deducted.'\n\n"
            "DAZIE's voice echoes: 'The old system treats failure as "
            "deviation. But falling is information, not a mistake.'"
        ),

        assumption_text=(
            "FAILURE PENALTY\n"
            "Falling or missing a jump resets significant progress.\n"
            "Assumes failure is deviation, not information.\n"
            "Rigid value: 1.0 (full reset, resource cost)"
        ),

        rewrite_description=(
            "Player uses Read Default on the respawn point.\n"
            "The penalty structure becomes visible: a chain of loss "
            "reactions (reset → resource deduction → attempt counter).\n"
            "Player uses Rewrite Default (via Guard: pinning safe "
            "respawn as the structural norm).\n"
            "The attempt counter dissolves.  Falls now land the player "
            "on the nearest safe ledge, 5 seconds back.  No resource loss."
        ),

        downstream_effects=[
            DownstreamEffect(
                system="Movement / Respawn",
                description="All future falls across the game reposition "
                            "the player gently.  No progress loss, no attempt "
                            "counters.  Soft-catch nets appear under "
                            "challenging sections.",
                magnitude="dramatic",
            ),
            DownstreamEffect(
                system="Combat",
                description="Pattern encounters that previously punished "
                            "failure with restart now allow the player "
                            "to remain in the encounter and retry the last "
                            "verb attempt.",
                magnitude="noticeable",
            ),
            DownstreamEffect(
                system="Narrative",
                description="Winton logs the change: 'Failure mode updated. "
                            "The Society now classifies a fall as data, "
                            "not deficiency.  That's structural.'",
                magnitude="subtle",
            ),
            DownstreamEffect(
                system="Psychology",
                description="Players who were avoiding difficult-looking "
                            "areas start exploring them.  The environment "
                            "feels safer to experiment in.",
                magnitude="dramatic",
            ),
        ],
    ),

    # ── CONSENT ──────────────────────────────────────────────────
    "consent": DefaultExample(
        category="CONSENT",
        default_key="consent_gates",
        label="Consent Gates",

        encounter_scene=(
            "Umbel Gardens, Chapter 5.  A suspended bridge leads to a "
            "Distortion zone.  There's no warning — the player walks "
            "across and suddenly the environment shifts: platforms shake, "
            "audio distorts, Echo Forms circle.  There was no choice to "
            "enter; the transition was invisible.\n\n"
            "DAZIE says: 'A boundary without a gate is an ambush.  "
            "No one should be forced into difficulty without being asked.'"
        ),

        assumption_text=(
            "CONSENT GATES\n"
            "No confirmation before entering danger zones.\n"
            "Assumes willingness to proceed.\n"
            "Rigid value: 0.0 (no gates exist)"
        ),

        rewrite_description=(
            "Player uses Read Default at the bridge threshold.\n"
            "An invisible boundary becomes visible as a shimmering "
            "threshold line.\n"
            "Player uses Rewrite Default (via Guard: creating a "
            "consent gate).\n"
            "A gentle prompt appears at the threshold: 'Distortion zone "
            "ahead.  Proceed? / Prepare first? / Find another way?'\n"
            "The gate becomes permanent architecture."
        ),

        downstream_effects=[
            DownstreamEffect(
                system="World / Districts",
                description="All danger zone transitions across the Umbel "
                            "Gardens now have consent gates.  The gates are "
                            "physical objects — part of the world, not a UI "
                            "overlay.",
                magnitude="dramatic",
            ),
            DownstreamEffect(
                system="Combat",
                description="Encounter start-up now includes a 3-second "
                            "preparation window.  Players can survey patterns "
                            "before engaging.",
                magnitude="noticeable",
            ),
            DownstreamEffect(
                system="Narrative",
                description="DAZIE teaches: 'This is what consent as "
                            "structure means.  The gate isn't politeness — "
                            "it's engineering.'  Chapter 5 civic rule "
                            "(CONSENT_AS_STRUCTURE) restores.",
                magnitude="noticeable",
            ),
            DownstreamEffect(
                system="NPC Behaviour",
                description="NPCs near danger zones relax visibly.  One NPC "
                            "says: 'I can see the boundary now.  I was "
                            "scared to walk here before.'  They begin to "
                            "use the bridge freely.",
                magnitude="dramatic",
            ),
        ],
    ),
}


# ─── Utility ─────────────────────────────────────────────────────────

def get_example(category: str) -> DefaultExample:
    """Get the worked example for a category."""
    return EXAMPLES.get(category.lower())


def print_example(category: str):
    """Print a formatted walkthrough of one default example."""
    ex = get_example(category)
    if not ex:
        print(f"No example for category: {category}")
        return

    print(f"\n{'='*70}")
    print(f"  DEFAULT EXAMPLE: {ex.category}")
    print(f"  Key: {ex.default_key} — \"{ex.label}\"")
    print(f"{'='*70}")

    print(f"\n📍 ENCOUNTER:")
    print(f"  {ex.encounter_scene}")

    print(f"\n👁️ READ DEFAULT reveals:")
    print(f"  {ex.assumption_text}")

    print(f"\n✏️ REWRITE:")
    print(f"  {ex.rewrite_description}")

    print(f"\n🌊 DOWNSTREAM EFFECTS:")
    for i, effect in enumerate(ex.downstream_effects, 1):
        print(f"\n  {i}. [{effect.system}] ({effect.magnitude})")
        print(f"     {effect.description}")

    print(f"\n{'='*70}\n")
