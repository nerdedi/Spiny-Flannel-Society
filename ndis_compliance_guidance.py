"""
NDIS Compliance Guidance Layer

Central policy guidance sourced from:
- OG - Reasonable and necessary supports.pdf
- C2025C00607.pdf (NDIS Act 2013)
- ndis-practice-standards-and-quality-indicators.pdf

Use this module anywhere the app generates professional outputs
(e.g., quotes, reports, reviews, and letters) to keep content aligned
with governance sources.
"""

from __future__ import annotations

from dataclasses import dataclass
from pathlib import Path
from typing import Dict, List
import re


NDIS_DATA_DIR = Path(__file__).resolve().parent / "ndis_data"


@dataclass(frozen=True)
class GovernanceSource:
    key: str
    title: str
    pdf_file: str
    extract_file: str


SOURCES: Dict[str, GovernanceSource] = {
    "reasonable_necessary": GovernanceSource(
        key="reasonable_necessary",
        title="Reasonable and necessary supports guidance",
        pdf_file="OG - Reasonable and necessary supports.pdf",
        extract_file="reasonable_and_necessary_extract.txt",
    ),
    "ndis_act_2013": GovernanceSource(
        key="ndis_act_2013",
        title="National Disability Insurance Scheme Act 2013",
        pdf_file="C2025C00607.pdf",
        extract_file="ndis_act_2013_extract.txt",
    ),
    "practice_standards": GovernanceSource(
        key="practice_standards",
        title="NDIS Practice Standards and Quality Indicators",
        pdf_file="ndis-practice-standards-and-quality-indicators.pdf",
        extract_file="practice_standards_extract.txt",
    ),
}


DOCUMENT_POLICIES: Dict[str, Dict[str, List[str]]] = {
    "quote": {
        "checklist": [
            "Ensure supports claimed align with participant goals and funded supports.",
            "Use current NDIS Pricing Arrangements and Price Limits for line-item rates.",
            "Use only claimable support items and valid claiming pathways.",
            "Document assumptions clearly (service type, ratio, frequency, period).",
            "Avoid overstating what can be claimed unless specifically NDIA-requested.",
        ],
        "keywords": [
            "price limits",
            "support item",
            "claim",
            "NDIA Requested Reports",
            "reasonable and necessary",
            "participant plan",
        ],
    },
    "report": {
        "checklist": [
            "Link recommendations to participant goals, outcomes, and functional impact.",
            "Use evidence-based language and document rationale for support recommendations.",
            "Clearly separate observations, assessment findings, and recommendations.",
            "Only represent NDIA-requested content where explicitly requested.",
            "Ensure report scope aligns with role, qualifications, and service agreement.",
        ],
        "keywords": [
            "NDIA Requested Reports",
            "reasonable and necessary",
            "goals",
            "functional",
            "assessment",
            "evidence",
        ],
    },
    "review": {
        "checklist": [
            "Capture progress against plan goals and current functional needs.",
            "Describe changes in circumstances and support effectiveness.",
            "Include participant voice, preferences, and consent considerations.",
            "Use accurate records and date ranges for review period summaries.",
            "Highlight any risk, safeguarding, or quality concerns with actions taken.",
        ],
        "keywords": [
            "review",
            "goals",
            "quality indicators",
            "outcomes",
            "participant",
            "safeguarding",
        ],
    },
    "letter": {
        "checklist": [
            "State purpose of correspondence and requested action clearly.",
            "Reference participant consent and authority where applicable.",
            "Use objective, factual, and evidence-backed statements.",
            "Avoid legal conclusions beyond documented evidence and provider role.",
            "Include relevant support item context when discussing funded supports.",
        ],
        "keywords": [
            "consent",
            "participant",
            "reasonable and necessary",
            "support",
            "quality",
            "rights",
        ],
    },
}


def _read_extract(path: Path) -> str:
    if not path.exists():
        return ""
    return path.read_text(encoding="utf-8", errors="ignore")


def list_governance_sources() -> List[GovernanceSource]:
    """Return all governance source metadata."""
    return list(SOURCES.values())


def _iter_pages(extract_text: str):
    chunks = re.split(r"\n===== PAGE (\d+) =====\n", extract_text)
    if len(chunks) <= 1:
        return

    # split format: [prefix, page_no, page_text, page_no, page_text, ...]
    for i in range(1, len(chunks), 2):
        page_no = chunks[i]
        page_text = chunks[i + 1] if i + 1 < len(chunks) else ""
        yield page_no, page_text


def _find_snippets(extract_text: str, keywords: List[str], limit: int = 6) -> List[Dict[str, str]]:
    snippets: List[Dict[str, str]] = []
    if not extract_text:
        return snippets

    patterns = [re.compile(re.escape(k), re.IGNORECASE) for k in keywords]

    for page_no, page_text in _iter_pages(extract_text):
        lines = page_text.splitlines()
        for idx, line in enumerate(lines):
            if any(p.search(line) for p in patterns):
                before = lines[idx - 1].strip() if idx > 0 else ""
                after = lines[idx + 1].strip() if idx + 1 < len(lines) else ""
                text = " ".join(part for part in [before, line.strip(), after] if part)
                snippets.append({"page": str(page_no), "text": text[:420]})
                if len(snippets) >= limit:
                    return snippets

    return snippets


def get_document_guidance(document_type: str, max_snippets_per_source: int = 4) -> Dict:
    """
    Get compliance checklist + evidence snippets for a document type.

    Supported types: quote, report, review, letter
    """
    doc_type = document_type.strip().lower()
    policy = DOCUMENT_POLICIES.get(doc_type)
    if policy is None:
        supported = ", ".join(sorted(DOCUMENT_POLICIES.keys()))
        raise ValueError(f"Unsupported document type '{document_type}'. Supported: {supported}")

    out = {
        "document_type": doc_type,
        "checklist": policy["checklist"],
        "sources": [],
    }

    for source in SOURCES.values():
        extract_path = NDIS_DATA_DIR / source.extract_file
        extract_text = _read_extract(extract_path)
        snippets = _find_snippets(
            extract_text=extract_text,
            keywords=policy["keywords"],
            limit=max_snippets_per_source,
        )
        out["sources"].append(
            {
                "source_key": source.key,
                "title": source.title,
                "pdf_file": source.pdf_file,
                "extract_file": source.extract_file,
                "snippets": snippets,
            }
        )

    return out


def format_guidance_summary(document_type: str) -> str:
    """Render a concise plain-text summary for insertion into generated outputs."""
    guidance = get_document_guidance(document_type, max_snippets_per_source=2)

    lines: List[str] = []
    lines.append(f"Governance guidance ({guidance['document_type']}):")
    for idx, item in enumerate(guidance["checklist"], start=1):
        lines.append(f"{idx}. {item}")

    lines.append("Sources:")
    for src in guidance["sources"]:
        lines.append(f"- {src['title']} ({src['pdf_file']})")

    return "\n".join(lines)
