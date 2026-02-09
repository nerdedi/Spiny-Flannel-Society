"""
Spiny Flannel Society — First Playable Minute
Design script for the opening 60 seconds using Read/Rewrite as the hook.

This is a linear design document expressed as code, describing
exactly what happens in the first minute of gameplay.  An engine
implementer should be able to build this sequence directly.

GOAL: In 60 seconds the player should:
    1. Move through a space
    2. Hit a blocked default
    3. Read it
    4. Rewrite it
    5. Feel the world change
    6. Understand the game's promise
"""

from dataclasses import dataclass, field
from typing import List, Dict


@dataclass
class Beat:
    """One moment in the first playable minute."""
    timestamp: str       # Approximate time mark (e.g., "0:00")
    duration: str        # How long this beat lasts
    location: str        # Where the player is
    action: str          # What the player does
    world_state: str     # What the world looks like
    audio: str           # What the player hears
    teaching: str        # What the player learns (implicitly)


# ─── The First Playable Minute ───────────────────────────────────────

FIRST_MINUTE: List[Beat] = [

    Beat(
        timestamp="0:00",
        duration="8 seconds",
        location="Windgap Academy — Arrival Platform",
        action=(
            "Player fades in standing on a warm sandstone platform. "
            "Wind moves gently.  A single path leads forward — wide, "
            "clear, sunlit.  The player walks forward naturally."
        ),
        world_state=(
            "Warm amber light. Eucalyptus canopy overhead. "
            "Soft brutalist architecture — curves, not edges. "
            "The path is generous. No UI yet."
        ),
        audio=(
            "Wind. A single sustained note — warm, like a cello. "
            "Bird call (Australian magpie, distant). "
            "Footsteps on sandstone — soft, rhythmic."
        ),
        teaching="I can move. This feels safe. I want to go forward.",
    ),

    Beat(
        timestamp="0:08",
        duration="7 seconds",
        location="Windgap Academy — First Corridor",
        action=(
            "The path narrows slightly.  Three gear-platforms rotate "
            "ahead, gaps flashing open and shut rapidly. "
            "Player approaches. The gap timing is visibly tight — "
            "200 ms windows."
        ),
        world_state=(
            "The corridor is still beautiful, but the gears feel "
            "out-of-place. They clank with a metallic urgency that "
            "clashes with the warm aesthetic. The colour around "
            "them is slightly grey — a visual Drift indicator."
        ),
        audio=(
            "The warm note bends slightly flat. "
            "Gear clanking — rhythmic but tense. "
            "Wind is still present but muffled."
        ),
        teaching="Something is wrong here. The timing is hostile.",
    ),

    Beat(
        timestamp="0:15",
        duration="5 seconds",
        location="Windgap Academy — Gear Corridor",
        action=(
            "DAZIE's voice (calm, not a tutorial bark): "
            "'The gears were tuned for one speed of thought. "
            "That's the assumption. Can you see it?'\n\n"
            "A subtle prompt appears: [Read Default]"
        ),
        world_state=(
            "The gear mechanism glows with a faint amber outline — "
            "highlighting that it contains a readable assumption. "
            "The outline pulses gently, like a heartbeat."
        ),
        audio=(
            "DAZIE's voice — warm, unhurried. "
            "A listening tone (soft chime) follows the prompt."
        ),
        teaching="I can READ things in this world. There are hidden rules.",
    ),

    Beat(
        timestamp="0:20",
        duration="10 seconds",
        location="Windgap Academy — Gear Corridor",
        action=(
            "Player activates Read Default on the gear mechanism.\n\n"
            "The world pauses softly (not a freeze — a held breath). "
            "The assumption is revealed:\n\n"
            "  TIMING WINDOW WIDTH\n"
            "  'Assumes all players react within 200 ms.'\n"
            "  'Penalises slower processing speeds.'\n\n"
            "The gears are now outlined in distinct amber, showing "
            "the gap timing as a visible rhythm. The player can "
            "SEE the assumption."
        ),
        world_state=(
            "Read Default view: a translucent overlay shows the "
            "timing window as amber pulses. The gap is visibly "
            "narrow. Other elements in the corridor dim slightly "
            "to focus attention."
        ),
        audio=(
            "A resonant tone — the sound of understanding. "
            "The gear clanking slows to match the reading pace. "
            "DAZIE: 'There it is.'"
        ),
        teaching="I can see the rules that govern this space. They're assumptions, not laws.",
    ),

    Beat(
        timestamp="0:30",
        duration="5 seconds",
        location="Windgap Academy — Gear Corridor",
        action=(
            "A second prompt appears: [Rewrite Default]\n\n"
            "DAZIE: 'Now you can change it. Use Cushion — "
            "widen the window.'"
        ),
        world_state=(
            "The read overlay is still active. The Windprint Rig "
            "icon appears — Cushion mode highlighted. The rewrite "
            "prompt pulses gently near the gear mechanism."
        ),
        audio=(
            "A second tone — anticipatory, rising. "
            "DAZIE's voice has a note of encouragement."
        ),
        teaching="I can CHANGE the rules. I have a tool for this.",
    ),

    Beat(
        timestamp="0:35",
        duration="10 seconds",
        location="Windgap Academy — Gear Corridor",
        action=(
            "Player activates Rewrite Default via Cushion.\n\n"
            "THE MOMENT:\n"
            "The gears slow. The 200 ms gap widens to 500 ms. "
            "The amber highlight shifts to teal — 'rewritten.' "
            "The metallic clanking softens to a wooden click. "
            "The grey Drift colour around the gears warms back "
            "to amber.\n\n"
            "The player walks through the gears easily."
        ),
        world_state=(
            "The corridor transforms. Not just the gears — "
            "the light shifts, the architecture relaxes slightly, "
            "a bench appears nearby (it only renders when timing "
            "is generous). The Drift is visibly reduced in this area."
        ),
        audio=(
            "A harmonic resolution — the flat note returns to tune. "
            "The cello warmth returns. Bird call (closer now). "
            "Wind picks up gently, carrying warmth."
        ),
        teaching="I changed the world by changing its assumptions. The world responded.",
    ),

    Beat(
        timestamp="0:45",
        duration="8 seconds",
        location="Windgap Academy — Beyond the Gears",
        action=(
            "Past the gears, the path opens into a small courtyard. "
            "An NPC is sitting on the bench that appeared. "
            "They look up:\n\n"
            "'Oh — the gears slowed. I've been waiting here for… "
            "I don't know how long. I couldn't get through.'\n\n"
            "They stand and walk through the corridor."
        ),
        world_state=(
            "The courtyard is warm and open. The NPC's body "
            "language was tense; now they relax. Other small details "
            "have shifted — signage is clearer, a wind chime moves "
            "gently, a plant has bloomed."
        ),
        audio=(
            "The NPC's voice (quiet, genuine). "
            "Wind chime. The sustained note is now a chord — "
            "richer, warmer. The space sounds alive."
        ),
        teaching=(
            "My rewrite helped someone else. This isn't about me — "
            "it's about the system. Changing defaults changes lives."
        ),
    ),

    Beat(
        timestamp="0:53",
        duration="7 seconds",
        location="Windgap Academy — Courtyard",
        action=(
            "DAZIE (from ahead, not looking back):\n\n"
            "'You didn't fix the gears. You didn't fix the person. "
            "You rewrote the assumption that was between them.'\n\n"
            "The path continues. A new area opens. "
            "The first chapter title fades in gently:\n\n"
            "  Chapter 1: Bract Theory\n"
            "  'Supports without proof.'"
        ),
        world_state=(
            "The courtyard leads to a wider vista — the Society "
            "is visible in the distance, terraces and canopies "
            "stretching along the wind current. Beautiful but "
            "streaked with grey Drift patches. There's work to do."
        ),
        audio=(
            "DAZIE's line lands in silence — no music competing. "
            "Then the main theme enters: a simple, warm melody. "
            "Wind carries it. The Society has a sound now."
        ),
        teaching=(
            "Core promise delivered: 'A world that becomes coherent "
            "not by fixing individuals, but by redesigning society.' "
            "I understand what this game is."
        ),
    ),
]


# ─── Utility ─────────────────────────────────────────────────────────

def print_first_minute():
    """Print the full first-minute script for review."""
    print("\n" + "=" * 70)
    print("  THE FIRST PLAYABLE MINUTE")
    print("  Spiny Flannel Society — Opening Sequence")
    print("=" * 70)

    for beat in FIRST_MINUTE:
        print(f"\n┌─ {beat.timestamp} ({beat.duration}) ─────────────────────")
        print(f"│  📍 {beat.location}")
        print(f"│")
        print(f"│  ACTION:")
        for line in beat.action.split("\n"):
            print(f"│    {line}")
        print(f"│")
        print(f"│  WORLD:")
        for line in beat.world_state.split("\n"):
            print(f"│    {line}")
        print(f"│")
        print(f"│  AUDIO:")
        for line in beat.audio.split("\n"):
            print(f"│    {line}")
        print(f"│")
        print(f"│  🧠 TEACHES: {beat.teaching}")
        print(f"└───────────────────────────────────────────────────────")

    print(f"\n{'='*70}")
    print(f"  60 seconds. One rewrite. One life changed. Game understood.")
    print(f"{'='*70}\n")


if __name__ == "__main__":
    print_first_minute()
