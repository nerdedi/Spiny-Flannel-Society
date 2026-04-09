#!/usr/bin/env python3
"""
Spiny Flannel Society — Python → Unity Sync Pipeline

Exports Python prototype data as JSON files into Assets/_SFS/Resources/SyncData/.
Unity's SFSSyncImporter (editor script) watches that folder and loads the JSON
into ScriptableObjects and runtime systems automatically.

Usage:
    python sync_to_unity.py                          # Export to repo Assets/
    python sync_to_unity.py --watch                  # Watch + auto-export
    python sync_to_unity.py --unity-project "C:/..." # Export to external Unity project
    python sync_to_unity.py --save-config            # Save --unity-project path for reuse

Windows example (your project):
    python sync_to_unity.py --unity-project "C:\\Users\\nerdedi\\OneDrive - Windgap Foundation\\Desktop\\new\\_SFS\\My project sfs" --save-config
    python sync_to_unity.py --watch   # uses saved path automatically
"""

import json
import os
import sys
import time
import hashlib
from pathlib import Path

# ---------------------------------------------------------------------------
# Paths
# ---------------------------------------------------------------------------
PROJECT_ROOT = Path(__file__).parent
CONFIG_FILE = PROJECT_ROOT / ".sync_config.json"

# Default: export into THIS repo's Assets folder
DEFAULT_SYNC_DIR = PROJECT_ROOT / "Assets" / "_SFS" / "Resources" / "SyncData"


def _load_config() -> dict:
    """Load saved sync config if it exists."""
    if CONFIG_FILE.exists():
        try:
            return json.loads(CONFIG_FILE.read_text(encoding="utf-8"))
        except (json.JSONDecodeError, OSError):
            pass
    return {}


def _save_config(cfg: dict):
    """Save sync config."""
    CONFIG_FILE.write_text(json.dumps(cfg, indent=2), encoding="utf-8")
    print(f"[SFS Sync] Config saved → {CONFIG_FILE.name}")


def _resolve_output_dirs(cli_unity_project: str = None) -> list:
    """
    Return a list of output directories to write the JSON into.
    Always includes the repo's Assets/ folder.
    Optionally includes the external Unity project path.
    """
    dirs = [DEFAULT_SYNC_DIR]
    config = _load_config()

    # CLI argument takes priority over saved config
    unity_path = cli_unity_project or config.get("unity_project")

    if unity_path:
        unity_root = Path(unity_path)
        # Accept either the project root or the Assets folder directly
        if (unity_root / "Assets").is_dir():
            ext_dir = unity_root / "Assets" / "_SFS" / "Resources" / "SyncData"
        elif unity_root.name == "Assets":
            ext_dir = unity_root / "_SFS" / "Resources" / "SyncData"
        else:
            # Assume it's the project root even if Assets/ doesn't exist yet
            ext_dir = unity_root / "Assets" / "_SFS" / "Resources" / "SyncData"
        dirs.append(ext_dir)

    return dirs


SYNC_OUTPUT_DIR = DEFAULT_SYNC_DIR  # Module-level default; overridden by CLI

# ---------------------------------------------------------------------------
# Import project modules
# ---------------------------------------------------------------------------
sys.path.insert(0, str(PROJECT_ROOT))

from game_config import (
    GAME_TITLE, VERSION, GAME_DESCRIPTION, WORLD_SETTING,
    TOTAL_CHAPTERS, TOTAL_DISTRICTS,
    CHAPTER_DATA, DISTRICT_DATA, CHARACTER_DATA, CIVIC_RULES,
    COMBAT_VERB_STATS, CUSHION_MODE_EFFECTS, GUARD_MODE_EFFECTS,
    WINDPRINT_ENERGY_MAX, WINDPRINT_ENERGY_REGEN, WINDPRINT_MODE_SWITCH_COST,
    PLAYER_SPEED, PLAYER_JUMP_HEIGHT, PLAYER_WALL_RUN_SPEED,
    PLAYER_AIR_DASH_SPEED, PLAYER_AIR_DASH_DISTANCE, PLAYER_GLIDE_SPEED,
    PLAYER_GRAPPLE_SPEED, TRIPLE_HOP_HEIGHTS, TRIPLE_HOP_WINDOW,
    COYOTE_TIME, JUMP_BUFFER_TIME,
    DRIFT_REDUCTION_PER_CHAPTER, DRIFT_INTENSITY_MIN, DRIFT_INTENSITY_MAX,
    REWRITE_ENERGY_COST, REWRITE_COOLDOWN, ASSUMPTION_SCAN_RADIUS,
    DESIGN_TERMINAL_INTERACTION_RANGE,
    Districts, Characters, CombatVerbs, TranslatorAbilities,
    WindprintModes, CommunicationModes, ElectiveSubjects,
    ANTAGONIST_DATA, COLOR_PALETTES, WIND_PATTERNS,
)
from chapters import get_chapter_intro_dialogue


# ---------------------------------------------------------------------------
# Export helpers
# ---------------------------------------------------------------------------

def _dialogue_line_to_dict(line) -> dict:
    return {
        "speaker": line.speaker,
        "text": line.text,
        "iconVersion": line.icon_version,
        "minimalVersion": line.minimal_version,
        "emotion": getattr(line, "emotion", "neutral"),
    }


def _class_constants(cls) -> dict:
    """Extract non-dunder string constants from a constants class."""
    return {
        k: getattr(cls, k)
        for k in sorted(dir(cls))
        if not k.startswith("_") and isinstance(getattr(cls, k), str)
    }


# ---------------------------------------------------------------------------
# Build the full export payload
# ---------------------------------------------------------------------------

def build_sync_data() -> dict:
    """Assemble every piece of Python prototype data into one JSON-friendly dict."""

    # -- Chapters ----------------------------------------------------------
    chapters = []
    for cid in sorted(CHAPTER_DATA.keys()):
        cd = CHAPTER_DATA[cid]
        district_info = DISTRICT_DATA.get(cd.location, {})
        intro = get_chapter_intro_dialogue(cid)
        chapters.append({
            "id": cd.id,
            "name": cd.name,
            "location": cd.location,
            "locationName": district_info.get("name", cd.location),
            "theme": cd.theme,
            "civicRule": cd.civic_rule,
            "civicRuleDescription": CIVIC_RULES.get(cd.civic_rule, ""),
            "primaryMechanic": cd.primary_mechanic,
            "npcs": cd.npcs,
            "introDialogue": [_dialogue_line_to_dict(l) for l in intro],
        })

    # -- Districts ---------------------------------------------------------
    districts = []
    for did, ddata in DISTRICT_DATA.items():
        districts.append({
            "id": did,
            "name": ddata["name"],
            "description": ddata["description"],
            "colorTheme": ddata["color_theme"],
            "windPattern": ddata["wind_pattern"],
        })

    # -- Characters --------------------------------------------------------
    characters = []
    for cid, cdata in CHARACTER_DATA.items():
        characters.append({
            "id": cid,
            "name": cdata["name"],
            "role": cdata["role"],
            "description": cdata["description"],
            "voice": cdata.get("voice", ""),
            "introduces": cdata.get("introduces", []),
        })

    # -- Combat verbs ------------------------------------------------------
    combat_verbs = []
    for verb_key, stats in COMBAT_VERB_STATS.items():
        combat_verbs.append({
            "id": verb_key,
            "energyCost": stats["energy_cost"],
            "cooldown": stats["cooldown"],
            "effectRadius": stats["effect_radius"],
            "duration": stats.get("duration", 0),
            "description": stats["description"],
        })

    # -- Antagonists -------------------------------------------------------
    antagonists = []
    for aid, adata in ANTAGONIST_DATA.items():
        antagonists.append({
            "id": aid,
            "name": adata.name,
            "description": adata.description,
            "resolutionVerb": adata.resolution_verb,
            "secondaryVerb": adata.secondary_verb,
            "baseIntensity": adata.base_intensity,
        })

    # -- Civic rules -------------------------------------------------------
    civic_rules = [{"id": k, "description": v} for k, v in CIVIC_RULES.items()]

    # -- Windprint ---------------------------------------------------------
    windprint = {
        "modes": _class_constants(WindprintModes),
        "energyMax": WINDPRINT_ENERGY_MAX,
        "energyRegen": WINDPRINT_ENERGY_REGEN,
        "modeSwitchCost": WINDPRINT_MODE_SWITCH_COST,
        "cushionEffects": CUSHION_MODE_EFFECTS,
        "guardEffects": {
            k: v for k, v in GUARD_MODE_EFFECTS.items()
            # bools → 1/0 for JSON safety
            if not isinstance(v, bool)
        },
        "guardConsentGateActive": GUARD_MODE_EFFECTS.get("consent_gate_active", True),
    }

    # -- Player stats ------------------------------------------------------
    player_stats = {
        "speed": PLAYER_SPEED,
        "jumpHeight": PLAYER_JUMP_HEIGHT,
        "wallRunSpeed": PLAYER_WALL_RUN_SPEED,
        "airDashSpeed": PLAYER_AIR_DASH_SPEED,
        "airDashDistance": PLAYER_AIR_DASH_DISTANCE,
        "glideSpeed": PLAYER_GLIDE_SPEED,
        "grappleSpeed": PLAYER_GRAPPLE_SPEED,
        "tripleHopHeights": list(TRIPLE_HOP_HEIGHTS),
        "tripleHopWindow": TRIPLE_HOP_WINDOW,
        "coyoteTime": COYOTE_TIME,
        "jumpBufferTime": JUMP_BUFFER_TIME,
    }

    # -- Drift config ------------------------------------------------------
    drift_config = {
        "reductionPerChapter": DRIFT_REDUCTION_PER_CHAPTER,
        "min": DRIFT_INTENSITY_MIN,
        "max": DRIFT_INTENSITY_MAX,
    }

    # -- Environment rewriting ---------------------------------------------
    rewriting = {
        "energyCost": REWRITE_ENERGY_COST,
        "cooldown": REWRITE_COOLDOWN,
        "scanRadius": ASSUMPTION_SCAN_RADIUS,
        "terminalInteractionRange": DESIGN_TERMINAL_INTERACTION_RANGE,
    }

    # -- Enums / constants for C# code-gen ---------------------------------
    enums = {
        "translatorAbilities": _class_constants(TranslatorAbilities),
        "combatVerbs": _class_constants(CombatVerbs),
        "communicationModes": _class_constants(CommunicationModes),
        "electiveSubjects": _class_constants(ElectiveSubjects),
        "districts": _class_constants(Districts),
        "characters": _class_constants(Characters),
    }

    # -- Wind patterns -----------------------------------------------------
    wind_patterns = {}
    for name, pat in WIND_PATTERNS.items():
        wind_patterns[name] = {k: v for k, v in pat.items()}

    # -- Full payload ------------------------------------------------------
    return {
        "_meta": {
            "generator": "sync_to_unity.py",
            "gameTitle": GAME_TITLE,
            "version": VERSION,
            "totalChapters": TOTAL_CHAPTERS,
            "totalDistricts": TOTAL_DISTRICTS,
            "exportedAt": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
        },
        "chapters": chapters,
        "districts": districts,
        "characters": characters,
        "combatVerbs": combat_verbs,
        "antagonists": antagonists,
        "civicRules": civic_rules,
        "windprint": windprint,
        "playerStats": player_stats,
        "driftConfig": drift_config,
        "rewriting": rewriting,
        "enums": enums,
        "windPatterns": wind_patterns,
    }


# ---------------------------------------------------------------------------
# Write JSON
# ---------------------------------------------------------------------------

def export_sync_data(output_dirs: list = None) -> list:
    """Export the sync JSON file to all output directories. Returns output paths."""
    if output_dirs is None:
        output_dirs = [SYNC_OUTPUT_DIR]

    data = build_sync_data()
    json_str = json.dumps(data, indent=2, ensure_ascii=False)
    paths = []

    for out_dir in output_dirs:
        out_dir = Path(out_dir)
        out_dir.mkdir(parents=True, exist_ok=True)
        out_path = out_dir / "sfs_prototype_data.json"
        out_path.write_text(json_str, encoding="utf-8")
        # Show relative path if inside project, otherwise absolute
        try:
            display = out_path.relative_to(PROJECT_ROOT)
        except ValueError:
            display = out_path
        print(f"[SFS Sync] Exported → {display}")
        paths.append(out_path)

    return paths


# ---------------------------------------------------------------------------
# File-watching loop
# ---------------------------------------------------------------------------

# Python source files that should trigger a re-export
WATCHED_FILES = [
    "game_config.py",
    "game_entities.py",
    "chapters.py",
    "combat_system.py",
    "platformer_mechanics.py",
    "windprint_rig.py",
    "narrative/chapters_data.py",
    "narrative/characters.py",
    "narrative/dialogue.py",
    "accessibility/defaults.py",
    "accessibility/presets.py",
    "core/defaults_registry.py",
    "core/events.py",
    "core/state.py",
    "systems/combat.py",
    "systems/movement.py",
    "systems/signals.py",
    "systems/windprint.py",
    "world/distortions.py",
    "world/districts.py",
    "world/routes.py",
    "design/first_playable_minute.py",
]


def _file_hash(path: Path) -> str:
    """Quick content hash for change detection."""
    if not path.exists():
        return ""
    return hashlib.md5(path.read_bytes()).hexdigest()


def watch_and_sync(poll_interval: float = 1.0, output_dirs: list = None):
    """Poll Python sources and re-export when any change."""
    if output_dirs is None:
        output_dirs = [SYNC_OUTPUT_DIR]

    print(f"[SFS Sync] Watching {len(WATCHED_FILES)} Python sources for changes…")
    for d in output_dirs:
        try:
            display = Path(d).relative_to(PROJECT_ROOT)
        except ValueError:
            display = d
        print(f"[SFS Sync] Output → {display}/")
    print("[SFS Sync] Press Ctrl+C to stop.\n")

    hashes: dict[str, str] = {}
    # Initial export
    export_sync_data(output_dirs)
    for rel in WATCHED_FILES:
        hashes[rel] = _file_hash(PROJECT_ROOT / rel)

    while True:
        time.sleep(poll_interval)
        changed = False
        for rel in WATCHED_FILES:
            h = _file_hash(PROJECT_ROOT / rel)
            if h != hashes.get(rel):
                print(f"[SFS Sync] Change detected: {rel}")
                hashes[rel] = h
                changed = True
        if changed:
            try:
                # Re-import modules to pick up changes
                for mod_name in list(sys.modules.keys()):
                    if mod_name.split(".")[0] in (
                        "game_config", "game_entities", "chapters",
                        "combat_system", "platformer_mechanics", "windprint_rig",
                        "narrative", "accessibility", "core", "systems", "world", "design",
                    ):
                        del sys.modules[mod_name]
                # Re-import after clearing
                exec("from game_config import *", globals())
                exec("from chapters import get_chapter_intro_dialogue", globals())
                export_sync_data(output_dirs)
            except Exception as e:
                print(f"[SFS Sync] Export failed: {e}")


# ---------------------------------------------------------------------------
# CLI
# ---------------------------------------------------------------------------

def _parse_args():
    """Parse command-line arguments."""
    import argparse
    parser = argparse.ArgumentParser(
        description="Sync SFS Python prototype data to Unity project(s)."
    )
    parser.add_argument(
        "--watch", action="store_true",
        help="Watch Python files and auto-export on change."
    )
    parser.add_argument(
        "--unity-project", type=str, default=None,
        help="Path to your local Unity project (e.g. 'C:/Users/.../My project sfs')."
    )
    parser.add_argument(
        "--save-config", action="store_true",
        help="Save --unity-project path to .sync_config.json for future runs."
    )
    return parser.parse_args()


if __name__ == "__main__":
    args = _parse_args()

    # Save config if requested
    if args.save_config and args.unity_project:
        _save_config({"unity_project": args.unity_project})

    # Resolve output directories
    output_dirs = _resolve_output_dirs(args.unity_project)

    print(f"[SFS Sync] Targets ({len(output_dirs)}):")
    for d in output_dirs:
        print(f"  • {d}")
    print()

    if args.watch:
        try:
            watch_and_sync(output_dirs=output_dirs)
        except KeyboardInterrupt:
            print("\n[SFS Sync] Stopped.")
    else:
        export_sync_data(output_dirs)
