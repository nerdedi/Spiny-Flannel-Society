"""
NDIS Governance-Guided Document Generator

Generates draft templates for:
- report
- review
- letter

Each generated draft includes:
- structured sections appropriate to the document type
- compliance checklist guidance
- governance source references and snippets

Sources are provided by ndis_compliance_guidance.py.
"""

from __future__ import annotations

from dataclasses import dataclass, field
from datetime import datetime
from pathlib import Path
from typing import Dict, List
import argparse
import re

from ndis_compliance_guidance import get_document_guidance
from ndis_progress_notes import analyze_progress_notes


SUPPORTED_TYPES = {"report", "review", "letter"}


@dataclass
class DocumentRequest:
    document_type: str
    participant_name: str
    ndis_number: str = ""
    author_name: str = ""
    organisation: str = ""
    purpose: str = ""
    period_start: str = ""
    period_end: str = ""
    key_points: List[str] = field(default_factory=list)
    progress_note_paths: List[str] = field(default_factory=list)

    def normalized_type(self) -> str:
        value = self.document_type.strip().lower()
        if value not in SUPPORTED_TYPES:
            supported = ", ".join(sorted(SUPPORTED_TYPES))
            raise ValueError(f"Unsupported document type '{self.document_type}'. Supported: {supported}")
        return value


STRUCTURE_HINTS: Dict[str, List[str]] = {
    "report": [
        "Referral context and purpose",
        "Participant background and current supports",
        "Assessment observations and functional impacts",
        "Progress against goals and outcomes",
        "Clinical/professional rationale for recommendations",
        "Recommendations and implementation considerations",
    ],
    "review": [
        "Review scope and period",
        "Summary of supports delivered",
        "Progress and outcomes against plan goals",
        "Changes in needs, risks, or circumstances",
        "Participant feedback and preferences",
        "Proposed adjustments for next review period",
    ],
    "letter": [
        "Purpose of this correspondence",
        "Participant context and consent statement",
        "Summary of relevant facts and evidence",
        "Requested action",
        "Supporting attachments/evidence list",
        "Closing statement and contact details",
    ],
}


def _safe_filename_part(value: str) -> str:
    cleaned = re.sub(r"[^A-Za-z0-9_-]+", "_", value.strip())
    cleaned = re.sub(r"_+", "_", cleaned).strip("_")
    return cleaned or "participant"


def _title_for(doc_type: str) -> str:
    return {
        "report": "NDIS Participant Report Draft",
        "review": "NDIS Participant Review Draft",
        "letter": "NDIS Participant Letter Draft",
    }[doc_type]


def build_document_markdown(request: DocumentRequest, max_snippets_per_source: int = 2) -> str:
    doc_type = request.normalized_type()
    guidance = get_document_guidance(doc_type, max_snippets_per_source=max_snippets_per_source)

    now = datetime.now().strftime("%d/%m/%Y")
    checklist = guidance["checklist"]
    source_blocks = guidance["sources"]
    structure = STRUCTURE_HINTS[doc_type]

    key_points = request.key_points or ["[Add key participant-specific point]"]
    note_analysis = analyze_progress_notes(request.progress_note_paths)

    lines: List[str] = []
    lines.append(f"# {_title_for(doc_type)}")
    lines.append("")
    lines.append("## Metadata")
    lines.append("")
    lines.append(f"- Date: {now}")
    lines.append(f"- Document type: {doc_type}")
    lines.append(f"- Participant: {request.participant_name}")
    lines.append(f"- NDIS number: {request.ndis_number or '[Add NDIS number]'}")
    lines.append(f"- Author: {request.author_name or '[Add author name]'}")
    lines.append(f"- Organisation: {request.organisation or '[Add organisation]'}")
    lines.append(f"- Purpose: {request.purpose or '[Add purpose]'}")
    if request.period_start or request.period_end:
        lines.append(
            f"- Review/Report period: {request.period_start or '[start]'} to {request.period_end or '[end]'}"
        )
    lines.append("")

    lines.append("## Compliance checklist")
    lines.append("")
    for idx, item in enumerate(checklist, start=1):
        lines.append(f"{idx}. {item}")
    lines.append("")

    lines.append("## Suggested structure")
    lines.append("")
    for idx, item in enumerate(structure, start=1):
        lines.append(f"{idx}. {item}")
    lines.append("")

    lines.append("## Participant-specific key points")
    lines.append("")
    for point in key_points:
        lines.append(f"- {point}")
    lines.append("")

    lines.append("## Progress note analysis")
    lines.append("")
    if note_analysis.note_files:
        lines.append(f"- Notes analyzed: {len(note_analysis.note_files)}")
        lines.append(f"- Total note lines scanned: {note_analysis.total_lines_scanned}")
        for note_file in note_analysis.note_files:
            lines.append(f"- Note file: `{note_file}`")
    else:
        lines.append("- No progress notes provided.")
    lines.append("")

    lines.append("## Recommended features/adaptations")
    lines.append("")
    if note_analysis.recommendations:
        for rec in note_analysis.recommendations:
            lines.append(
                f"- **{rec.title}** (`{rec.id}`) — Priority: {rec.priority.upper()} | Confidence: {rec.confidence:.2f}"
            )
            lines.append(f"  - Recommendation: {rec.description}")
            if rec.evidence:
                lines.append("  - Evidence from notes:")
                for ev in rec.evidence:
                    lines.append(f"    - {ev}")
    else:
        lines.append("- No adaptation recommendations were triggered from the uploaded notes.")
    lines.append("")

    lines.append("## Draft body scaffold")
    lines.append("")
    for section in structure:
        lines.append(f"### {section}")
        lines.append("")
        lines.append("[Draft content here]")
        lines.append("")

    lines.append("## Governance references used")
    lines.append("")
    for src in source_blocks:
        lines.append(f"### {src['title']}")
        lines.append("")
        lines.append(f"- Source file: `{src['pdf_file']}`")
        lines.append(f"- Extract file: `{src['extract_file']}`")
        if src["snippets"]:
            lines.append("- Relevant snippets:")
            for snip in src["snippets"]:
                lines.append(f"  - p.{snip['page']}: {snip['text']}")
        else:
            lines.append("- Relevant snippets: [No matching snippets found for current keywords]")
        lines.append("")

    return "\n".join(lines).strip() + "\n"


def write_document_markdown(
    request: DocumentRequest,
    output_path: str | None = None,
    max_snippets_per_source: int = 2,
) -> str:
    """Generate and write a governance-guided draft document as markdown."""
    doc_type = request.normalized_type()

    if output_path is None:
        participant_slug = _safe_filename_part(request.participant_name)
        output_path = f"output/{doc_type}_{participant_slug}.md"

    content = build_document_markdown(request, max_snippets_per_source=max_snippets_per_source)

    target = Path(output_path)
    target.parent.mkdir(parents=True, exist_ok=True)
    target.write_text(content, encoding="utf-8")
    return str(target)


def _parse_args():
    parser = argparse.ArgumentParser(description="Generate governance-guided NDIS document drafts")
    parser.add_argument("--type", required=True, choices=sorted(SUPPORTED_TYPES), help="Document type")
    parser.add_argument("--participant", required=True, help="Participant name")
    parser.add_argument("--ndis-number", default="", help="Participant NDIS number")
    parser.add_argument("--author", default="", help="Author name")
    parser.add_argument("--organisation", default="", help="Organisation name")
    parser.add_argument("--purpose", default="", help="Document purpose")
    parser.add_argument("--period-start", default="", help="Period start (optional)")
    parser.add_argument("--period-end", default="", help="Period end (optional)")
    parser.add_argument("--point", action="append", default=[], help="Add participant-specific key point (repeatable)")
    parser.add_argument(
        "--progress-note",
        action="append",
        default=[],
        help="Path to uploaded progress note file (txt/md/csv/pdf). Repeatable.",
    )
    parser.add_argument("--output", default="", help="Output markdown path")
    parser.add_argument("--snippets", type=int, default=2, help="Max snippets per governance source")
    return parser.parse_args()


def main() -> None:
    args = _parse_args()

    request = DocumentRequest(
        document_type=args.type,
        participant_name=args.participant,
        ndis_number=args.ndis_number,
        author_name=args.author,
        organisation=args.organisation,
        purpose=args.purpose,
        period_start=args.period_start,
        period_end=args.period_end,
        key_points=args.point,
        progress_note_paths=args.progress_note,
    )

    path = write_document_markdown(
        request=request,
        output_path=(args.output or None),
        max_snippets_per_source=args.snippets,
    )

    print("NDIS governance-guided document generated.")
    print(f"Type: {request.normalized_type()}")
    print(f"File: {path}")


if __name__ == "__main__":
    main()
