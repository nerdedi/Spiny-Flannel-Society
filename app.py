from __future__ import annotations

from datetime import date
from io import BytesIO
from pathlib import Path
from tempfile import TemporaryDirectory
from typing import Any
import os

from dotenv import load_dotenv
from flask import Flask, flash, render_template, request, send_file, url_for

from ndis_document_generator import DocumentRequest, build_document_markdown
from ndis_progress_notes import analyze_progress_notes
from ndis_quote_generator import (
    Participant,
    ParticipantQuote,
    QuoteLine,
    generate_quote_html,
    load_default_catalog,
)


APP_TITLE = "Windgap NDIS Workspace"
DEFAULT_QUOTE_ROWS = [
    {
        "program": "Core Supports",
        "code": "04_102_0136_6_1",
        "name": "Group Activities - Standard - Weekday Daytime",
        "service_type": "Face-to-Face",
        "category": "LLND_GROUP_ACTIVITIES",
        "ratio": "1:1",
        "qty": "1.00",
        "days": "1",
    },
    {
        "program": "Core Supports",
        "code": "04_102_0136_6_1",
        "name": "Group Activities - Standard - Weekday Daytime",
        "service_type": "Non Face-to-Face",
        "category": "LLND_GROUP_ACTIVITIES",
        "ratio": "N/A",
        "qty": "0.03846",
        "days": "1",
    },
    {
        "program": "Core Supports",
        "code": "04_599_0136_6_1",
        "name": "Centre Capital Cost",
        "service_type": "Centre Capital Costs",
        "category": "LLND_CENTRE_CAPITAL",
        "ratio": "N/A",
        "qty": "1.00",
        "days": "1",
    },
]


def _catalog_options() -> dict[str, Any]:
    catalog = load_default_catalog()
    items = [
        {
            "code": item.code,
            "name": item.name,
            "rate": item.rate,
            "remote_rate": item.remote_rate,
            "very_remote_rate": item.very_remote_rate,
            "claiming_categories": item.claiming_categories,
        }
        for item in sorted(catalog.items.values(), key=lambda item: (item.code, item.name))
    ]
    categories = [
        {
            "code": category.code,
            "name": category.name,
        }
        for category in sorted(catalog.categories.values(), key=lambda category: (category.code, category.name))
    ]
    return {"items": items, "categories": categories}


def _parse_quote_lines(form) -> list[QuoteLine]:
    programs = form.getlist("program[]")
    codes = form.getlist("code[]")
    names = form.getlist("name[]")
    service_types = form.getlist("service_type[]")
    categories = form.getlist("category[]")
    ratios = form.getlist("ratio[]")
    quantities = form.getlist("qty[]")
    days = form.getlist("days[]")

    lines: list[QuoteLine] = []
    row_count = max(
        len(programs),
        len(codes),
        len(names),
        len(service_types),
        len(categories),
        len(ratios),
        len(quantities),
        len(days),
    )

    for index in range(row_count):
        code = (codes[index] if index < len(codes) else "").strip()
        if not code:
            continue

        lines.append(
            QuoteLine(
                program=(programs[index] if index < len(programs) else "Core Supports").strip() or "Core Supports",
                support_item_code=code,
                support_item_name=(names[index] if index < len(names) else "").strip(),
                service_type=(service_types[index] if index < len(service_types) else "").strip(),
                claiming_category=(categories[index] if index < len(categories) else "").strip(),
                ratio=(ratios[index] if index < len(ratios) else "N/A").strip() or "N/A",
                quantity_per_day=float((quantities[index] if index < len(quantities) else "0") or 0),
                days_per_week=int(float((days[index] if index < len(days) else "0") or 0)),
            )
        )

    return lines


def _save_uploaded_notes(files) -> tuple[list[str], TemporaryDirectory[str]]:
    temp_dir = TemporaryDirectory()
    paths: list[str] = []
    for storage in files:
        if not storage or not storage.filename:
            continue
        target = Path(temp_dir.name) / Path(storage.filename).name
        target.write_bytes(storage.read())
        paths.append(str(target))
    return paths, temp_dir


def _base_context(**extra: Any) -> dict[str, Any]:
    today = date.today()
    return {
        "app_title": APP_TITLE,
        "today": today,
        "default_quote_rows": DEFAULT_QUOTE_ROWS,
        "catalog_options": _catalog_options(),
        "regions": ["national", "remote", "very_remote"],
        "default_support_start": "2026-03-09",
        "default_support_end": "2026-04-27",
        "document_types": ["report", "review", "letter"],
        **extra,
    }


def create_app(test_config: dict[str, Any] | None = None) -> Flask:
    load_dotenv()

    app = Flask(__name__)
    app.config.update(
        SECRET_KEY=os.getenv("FLASK_SECRET_KEY", "windgap-dev-secret"),
        MAX_CONTENT_LENGTH=10 * 1024 * 1024,
    )

    if test_config:
        app.config.update(test_config)

    @app.get("/")
    def index():
        return render_template("index.html", **_base_context())

    @app.post("/generate-quote")
    def generate_quote():
        catalog = load_default_catalog()
        try:
            participant = Participant(
                name=request.form.get("participant_name", "Sample Participant").strip(),
                ndis_number=request.form.get("ndis_number", "").strip(),
                dob=request.form.get("dob", "").strip(),
            )
            quote = ParticipantQuote(
                participant=participant,
                support_start=date.fromisoformat(request.form.get("support_start", "2026-03-09")),
                support_end=date.fromisoformat(request.form.get("support_end", "2026-04-27")),
                lines=_parse_quote_lines(request.form),
                region=request.form.get("region", "national"),
            )
            weeks_override = float(request.form.get("weeks_override", "8") or 8)

            with TemporaryDirectory() as tmp:
                output_path = Path(tmp) / "participant_summary_of_supports.html"
                generate_quote_html(
                    quote=quote,
                    output_path=str(output_path),
                    catalog=catalog,
                    weeks_override=weeks_override,
                    region_override=quote.region,
                )
                html_bytes = output_path.read_bytes()

            return send_file(
                BytesIO(html_bytes),
                mimetype="text/html",
                as_attachment=True,
                download_name="participant_summary_of_supports.html",
            )
        except Exception as exc:
            flash(f"Quote generation failed: {exc}", "error")
            return render_template("index.html", **_base_context(active_tab="quote")), 400

    @app.post("/analyze-notes")
    def analyze_notes():
        files = request.files.getlist("progress_notes")
        if not any(file.filename for file in files):
            flash("Upload at least one progress note before analysis.", "error")
            return render_template("index.html", **_base_context(active_tab="notes")), 400

        temp_dir = None
        try:
            note_paths, temp_dir = _save_uploaded_notes(files)
            analysis = analyze_progress_notes(note_paths)
            context = _base_context(active_tab="notes", note_analysis=analysis)
            return render_template("index.html", **context)
        except Exception as exc:
            flash(f"Progress note analysis failed: {exc}", "error")
            return render_template("index.html", **_base_context(active_tab="notes")), 400
        finally:
            if temp_dir is not None:
                temp_dir.cleanup()

    @app.post("/generate-document")
    def generate_document():
        files = request.files.getlist("document_progress_notes")
        temp_dir = None
        try:
            note_paths, temp_dir = _save_uploaded_notes(files)
            request_model = DocumentRequest(
                document_type=request.form.get("document_type", "report"),
                participant_name=request.form.get("document_participant_name", "Sample Participant").strip(),
                ndis_number=request.form.get("document_ndis_number", "").strip(),
                author_name=request.form.get("author_name", "").strip(),
                organisation=request.form.get("organisation", "").strip(),
                purpose=request.form.get("purpose", "").strip(),
                period_start=request.form.get("period_start", "").strip(),
                period_end=request.form.get("period_end", "").strip(),
                key_points=[
                    line.strip()
                    for line in request.form.get("key_points", "").splitlines()
                    if line.strip()
                ],
                progress_note_paths=note_paths,
            )
            content = build_document_markdown(request_model, max_snippets_per_source=2)
            file_name = f"{request_model.normalized_type()}_{request_model.participant_name.replace(' ', '_')}.md"
            return send_file(
                BytesIO(content.encode("utf-8")),
                mimetype="text/markdown",
                as_attachment=True,
                download_name=file_name,
            )
        except Exception as exc:
            flash(f"Document generation failed: {exc}", "error")
            return render_template("index.html", **_base_context(active_tab="documents")), 400
        finally:
            if temp_dir is not None:
                temp_dir.cleanup()

    @app.get("/health")
    def health():
        return {"status": "ok", "app": APP_TITLE}

    return app


app = create_app()


if __name__ == "__main__":
    app.run(host="0.0.0.0", port=int(os.getenv("PORT", "8000")), debug=True)
