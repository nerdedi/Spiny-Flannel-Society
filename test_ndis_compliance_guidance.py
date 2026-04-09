"""Tests for NDIS governance guidance layer."""

from ndis_compliance_guidance import (
    get_document_guidance,
    list_governance_sources,
    format_guidance_summary,
)


def test_sources_available():
    sources = list_governance_sources()
    assert len(sources) == 3


def test_quote_guidance_contains_checklist_and_sources():
    guidance = get_document_guidance("quote")
    assert guidance["document_type"] == "quote"
    assert len(guidance["checklist"]) > 0
    assert len(guidance["sources"]) == 3


def test_report_guidance_has_snippets():
    guidance = get_document_guidance("report")
    total_snippets = sum(len(src["snippets"]) for src in guidance["sources"])
    assert total_snippets > 0


def test_summary_format():
    summary = format_guidance_summary("review")
    assert "Governance guidance" in summary
    assert "Sources:" in summary


def run_all_tests():
    test_sources_available()
    test_quote_guidance_contains_checklist_and_sources()
    test_report_guidance_has_snippets()
    test_summary_format()
    print("✓ NDIS compliance guidance tests passed")


if __name__ == "__main__":
    run_all_tests()
