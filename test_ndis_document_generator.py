"""Tests for governance-guided NDIS document generator."""

from ndis_document_generator import (
    DocumentRequest,
    build_document_markdown,
    write_document_markdown,
)


def test_build_report_markdown_contains_sections():
    request = DocumentRequest(
        document_type="report",
        participant_name="Sample Participant",
        ndis_number="4300 000 000",
        key_points=["Participant has demonstrated improved community access."],
    )

    md = build_document_markdown(request)
    assert "# NDIS Participant Report Draft" in md
    assert "## Compliance checklist" in md
    assert "## Governance references used" in md


def test_build_review_markdown_contains_structure():
    request = DocumentRequest(
        document_type="review",
        participant_name="Sample Participant",
        period_start="01/01/2026",
        period_end="31/03/2026",
    )

    md = build_document_markdown(request)
    assert "Document type: review" in md
    assert "Review/Report period" in md


def test_markdown_includes_progress_note_recommendations():
    request = DocumentRequest(
        document_type="report",
        participant_name="Sample Participant",
        progress_note_paths=["ndis_data/sample_progress_note.txt"],
    )

    md = build_document_markdown(request)
    assert "## Progress note analysis" in md
    assert "## Recommended features/adaptations" in md
    assert "sensory_modulation_pack" in md


def test_write_document_markdown_creates_file():
    request = DocumentRequest(
        document_type="letter",
        participant_name="Sample Participant",
        purpose="Request for review consideration",
    )

    output = write_document_markdown(
        request=request,
        output_path="output/test_letter_sample_participant.md",
    )

    assert output.endswith(".md")


def run_all_tests():
    test_build_report_markdown_contains_sections()
    test_build_review_markdown_contains_structure()
    test_markdown_includes_progress_note_recommendations()
    test_write_document_markdown_creates_file()
    print("✓ NDIS document generator tests passed")


if __name__ == "__main__":
    run_all_tests()
