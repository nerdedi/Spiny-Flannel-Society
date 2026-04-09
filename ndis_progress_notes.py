"""
NDIS Progress Note Analyzer

Ingests uploaded progress notes and recommends practical feature/adaptation
options based on observed participant needs and patterns.
"""

from __future__ import annotations

from dataclasses import dataclass, field
from pathlib import Path
from typing import Dict, List
import re


@dataclass(frozen=True)
class AdaptationRule:
    """Maps note signals to suggested features/adaptations."""

    id: str
    title: str
    description: str
    keywords: List[str]
    priority: str = "medium"


@dataclass
class AdaptationRecommendation:
    """A recommendation generated from one or more matched note signals."""

    id: str
    title: str
    description: str
    priority: str
    confidence: float
    evidence: List[str] = field(default_factory=list)


@dataclass
class ProgressNoteAnalysis:
    """Structured result of progress note analysis."""

    note_files: List[str]
    total_lines_scanned: int
    recommendations: List[AdaptationRecommendation]


RULES: List[AdaptationRule] = [
    AdaptationRule(
        id="sensory_modulation_pack",
        title="Enhanced sensory modulation options",
        description=(
            "Add configurable sensory supports (reduced noise/visual clutter, "
            "quiet-mode options, and pacing-friendly environments)."
        ),
        keywords=["overload", "sensory", "noise", "bright", "crowd", "meltdown", "distress"],
        priority="high",
    ),
    AdaptationRule(
        id="communication_mode_expansion",
        title="Expanded communication supports",
        description=(
            "Provide multi-modal communication pathways (plain language, visual supports, "
            "structured prompts, and alternate response formats)."
        ),
        keywords=["communication", "understand", "verbal", "non-verbal", "prompt", "instructions"],
        priority="high",
    ),
    AdaptationRule(
        id="fatigue_pacing_adjustments",
        title="Fatigue-aware pacing and scheduling",
        description=(
            "Introduce shorter sessions, additional breaks, flexible session duration, "
            "and reduced transition load where fatigue is observed."
        ),
        keywords=["fatigue", "tired", "exhausted", "burnout", "energy", "rest"],
        priority="high",
    ),
    AdaptationRule(
        id="mobility_environment_adaptations",
        title="Mobility and access adaptations",
        description=(
            "Review physical access needs and add modifications or alternate pathways "
            "for safer mobility and participation."
        ),
        keywords=["mobility", "balance", "falls", "wheelchair", "walking", "access"],
        priority="medium",
    ),
    AdaptationRule(
        id="routine_transition_support",
        title="Routine and transition supports",
        description=(
            "Add advance notice, visual schedules, and gradual transitions to reduce "
            "anxiety and improve consistency during change."
        ),
        keywords=["transition", "change", "routine", "anxiety", "unexpected", "schedule"],
        priority="medium",
    ),
    AdaptationRule(
        id="behaviour_support_review",
        title="Behaviour support strategy review",
        description=(
            "Review behaviour support approaches with proactive, least-restrictive strategies, "
            "and monitor response data for iterative adjustment."
        ),
        keywords=["incident", "aggression", "self-harm", "restrictive", "escalation", "behaviour"],
        priority="high",
    ),
]


def load_progress_note_text(file_path: str) -> str:
    """Load progress note text from txt/md/pdf files."""
    path = Path(file_path)
    if not path.exists():
        raise FileNotFoundError(f"Progress note file not found: {file_path}")

    suffix = path.suffix.lower()
    if suffix in {".txt", ".md", ".csv"}:
        return path.read_text(encoding="utf-8", errors="ignore")

    if suffix == ".pdf":
        try:
            from pypdf import PdfReader
        except Exception as e:  # pragma: no cover - environment dependent
            raise RuntimeError(
                "PDF support requires pypdf. Install it in your Python environment."
            ) from e

        reader = PdfReader(str(path))
        pages = [(p.extract_text() or "") for p in reader.pages]
        return "\n".join(pages)

    raise ValueError(f"Unsupported note format: {suffix}. Use txt, md, csv, or pdf.")


def _score_rule_matches(lines: List[str], rule: AdaptationRule) -> Dict:
    matched: List[str] = []
    patterns = [re.compile(re.escape(keyword), re.IGNORECASE) for keyword in rule.keywords]

    for line in lines:
        text = line.strip()
        if not text:
            continue
        if any(p.search(text) for p in patterns):
            matched.append(text[:260])

    return {
        "count": len(matched),
        "evidence": matched[:5],
    }


def analyze_progress_notes(note_paths: List[str]) -> ProgressNoteAnalysis:
    """Analyze uploaded progress notes and recommend features/adaptations."""
    if not note_paths:
        return ProgressNoteAnalysis(note_files=[], total_lines_scanned=0, recommendations=[])

    lines: List[str] = []
    for path in note_paths:
        text = load_progress_note_text(path)
        lines.extend(text.splitlines())

    recommendations: List[AdaptationRecommendation] = []

    for rule in RULES:
        result = _score_rule_matches(lines, rule)
        count = result["count"]
        if count == 0:
            continue

        # Lightweight confidence based on match density with a practical ceiling.
        confidence = min(0.95, 0.35 + (count * 0.08))
        recommendations.append(
            AdaptationRecommendation(
                id=rule.id,
                title=rule.title,
                description=rule.description,
                priority=rule.priority,
                confidence=round(confidence, 2),
                evidence=result["evidence"],
            )
        )

    recommendations.sort(key=lambda r: (r.priority != "high", -r.confidence, r.title))

    return ProgressNoteAnalysis(
        note_files=note_paths,
        total_lines_scanned=len(lines),
        recommendations=recommendations,
    )
