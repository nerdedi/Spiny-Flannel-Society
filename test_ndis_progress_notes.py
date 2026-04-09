"""Tests for progress-note ingestion and adaptation recommendations."""

from ndis_progress_notes import analyze_progress_notes


def test_analyze_progress_notes_generates_recommendations():
    analysis = analyze_progress_notes([
        "ndis_data/sample_progress_note.txt",
    ])

    assert analysis.total_lines_scanned > 0
    assert len(analysis.recommendations) > 0

    ids = {r.id for r in analysis.recommendations}
    assert "sensory_modulation_pack" in ids
    assert "fatigue_pacing_adjustments" in ids
    assert "routine_transition_support" in ids


def run_all_tests():
    test_analyze_progress_notes_generates_recommendations()
    print("✓ NDIS progress note analysis tests passed")


if __name__ == "__main__":
    run_all_tests()
