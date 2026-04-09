from __future__ import annotations

from io import BytesIO

from app import create_app


def _client():
    app = create_app({"TESTING": True, "SECRET_KEY": "test-secret"})
    return app.test_client()


def test_index_loads():
    client = _client()
    response = client.get("/")

    assert response.status_code == 200
    assert b"Windgap NDIS Workspace" in response.data
    assert b"Quote Builder" in response.data


def test_health_endpoint():
    client = _client()
    response = client.get("/health")

    assert response.status_code == 200
    assert response.json == {"status": "ok", "app": "Windgap NDIS Workspace"}


def test_generate_quote_downloads_html():
    client = _client()
    response = client.post(
        "/generate-quote",
        data={
            "participant_name": "Sample Participant",
            "ndis_number": "4300 000 000",
            "dob": "01/01/2000",
            "support_start": "2026-03-09",
            "support_end": "2026-04-27",
            "region": "national",
            "weeks_override": "8",
            "program[]": ["Core Supports"],
            "code[]": ["04_102_0136_6_1"],
            "name[]": ["Group Activities - Standard - Weekday Daytime"],
            "service_type[]": ["Face-to-Face"],
            "category[]": ["LLND_GROUP_ACTIVITIES"],
            "ratio[]": ["1:1"],
            "qty[]": ["1.0"],
            "days[]": ["1"],
        },
    )

    assert response.status_code == 200
    assert response.mimetype == "text/html"
    assert b"Participant Summary of Supports" in response.data


def test_analyze_notes_renders_recommendations():
    client = _client()
    response = client.post(
        "/analyze-notes",
        data={
            "progress_notes": [
                (
                    BytesIO(
                        b"Participant experienced sensory overload, fatigue, and anxiety during transitions."
                    ),
                    "progress_note.txt",
                )
            ]
        },
        content_type="multipart/form-data",
    )

    assert response.status_code == 200
    assert b"Enhanced sensory modulation options" in response.data
    assert b"Fatigue-aware pacing and scheduling" in response.data


def test_generate_document_downloads_markdown():
    client = _client()
    response = client.post(
        "/generate-document",
        data={
            "document_type": "report",
            "document_participant_name": "Sample Participant",
            "document_ndis_number": "4300 000 000",
            "author_name": "Provider Clinician",
            "organisation": "Windgap Support Services",
            "purpose": "Progress and recommendations for plan review",
            "period_start": "01/01/2026",
            "period_end": "31/03/2026",
            "key_points": "Improved engagement\nOngoing need for structured supports",
        },
    )

    assert response.status_code == 200
    assert response.mimetype == "text/markdown"
    assert b"# NDIS Participant Report Draft" in response.data
