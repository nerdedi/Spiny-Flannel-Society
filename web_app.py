"""
Windgap NDIS Workspace (Streamlit)

Professional web interface for:
- Quote generation
- Governance-guided report/review/letter generation
- Progress note upload and adaptation recommendations

Theme colors are discretely inspired by the Windgap quote aesthetic.
"""

from __future__ import annotations

from datetime import date
from pathlib import Path
from tempfile import TemporaryDirectory
import streamlit as st

from ndis_quote_generator import (
    Participant,
    ParticipantQuote,
    QuoteLine,
    load_default_catalog,
    generate_quote_html,
)
from ndis_document_generator import DocumentRequest, write_document_markdown
from ndis_progress_notes import analyze_progress_notes


APP_TITLE = "Windgap NDIS Workspace"


def inject_theme() -> None:
    st.set_page_config(page_title=APP_TITLE, layout="wide", page_icon="🧾")
    st.markdown(
        """
        <style>
        :root {
            --windgap-burgundy: #b72d2f;
            --windgap-teal: #66c1c4;
            --windgap-teal-dark: #2f7f84;
            --windgap-ink: #1f2937;
            --windgap-surface: #f7f9fb;
        }

        .stApp {
            background: linear-gradient(180deg, #ffffff 0%, var(--windgap-surface) 100%);
        }

        .hero {
            background: linear-gradient(120deg, rgba(183,45,47,0.92), rgba(102,193,196,0.88));
            border-radius: 14px;
            padding: 1rem 1.25rem;
            color: white;
            margin-bottom: 0.8rem;
            box-shadow: 0 6px 16px rgba(47,127,132,0.18);
        }

        .soft-card {
            background: white;
            border: 1px solid rgba(31,41,55,0.08);
            border-radius: 12px;
            padding: 0.8rem 1rem;
            box-shadow: 0 3px 10px rgba(31,41,55,0.06);
            margin-bottom: 0.75rem;
        }

        .pill {
            display: inline-block;
            background: rgba(102,193,196,0.15);
            color: var(--windgap-teal-dark);
            border: 1px solid rgba(102,193,196,0.5);
            border-radius: 999px;
            padding: 2px 10px;
            font-size: 0.8rem;
            margin-right: 6px;
        }
        </style>
        """,
        unsafe_allow_html=True,
    )


def render_header() -> None:
    st.markdown(
        """
        <div class="hero">
            <h2 style="margin:0;">Windgap NDIS Workspace</h2>
            <p style="margin:6px 0 0 0;">Professional quote, progress-note, and governance-guided document workflows.</p>
        </div>
        """,
        unsafe_allow_html=True,
    )


def quote_builder_tab() -> None:
    st.subheader("Quote Builder")
    st.markdown("<div class='soft-card'>Create Participant Summary of Supports quotes with regional pricing.</div>", unsafe_allow_html=True)

    catalog = load_default_catalog()

    col1, col2, col3 = st.columns(3)
    participant_name = col1.text_input("Participant name", value="Sample Participant")
    ndis_number = col2.text_input("NDIS number", value="4300 000 000")
    dob = col3.text_input("Date of birth", value="01/01/2000")

    col4, col5, col6 = st.columns(3)
    start = col4.date_input("Support start", value=date(2026, 3, 9))
    end = col5.date_input("Support end", value=date(2026, 4, 27))
    region = col6.selectbox("Rate region", ["national", "remote", "very_remote"], index=0)

    st.markdown("#### Weekly support lines")

    default_rows = [
        {
            "program": "Core Supports",
            "code": "04_102_0136_6_1",
            "name": "Group Activities - Standard - Weekday Daytime",
            "service_type": "Face-to-Face",
            "category": "LLND_GROUP_ACTIVITIES",
            "ratio": "1:1",
            "qty": 1.00,
            "days": 1,
        },
        {
            "program": "Core Supports",
            "code": "04_102_0136_6_1",
            "name": "Group Activities - Standard - Weekday Daytime",
            "service_type": "Non Face-to-Face",
            "category": "LLND_GROUP_ACTIVITIES",
            "ratio": "N/A",
            "qty": 0.03846,
            "days": 1,
        },
        {
            "program": "Core Supports",
            "code": "04_599_0136_6_1",
            "name": "Centre Capital Cost",
            "service_type": "Centre Capital Costs",
            "category": "LLND_CENTRE_CAPITAL",
            "ratio": "N/A",
            "qty": 1.00,
            "days": 1,
        },
    ]

    edited = st.data_editor(default_rows, num_rows="dynamic", use_container_width=True)

    weeks_override = st.slider("Weeks override for quote total", min_value=1, max_value=52, value=8)

    if st.button("Generate quote", type="primary"):
        participant = Participant(name=participant_name, ndis_number=ndis_number, dob=dob)
        lines = []
        for row in edited:
            if not row.get("code"):
                continue
            lines.append(
                QuoteLine(
                    program=str(row.get("program", "Core Supports")),
                    support_item_code=str(row.get("code")),
                    support_item_name=str(row.get("name", "")),
                    service_type=str(row.get("service_type", "")),
                    claiming_category=str(row.get("category", "")),
                    ratio=str(row.get("ratio", "N/A")),
                    quantity_per_day=float(row.get("qty", 0.0)),
                    days_per_week=int(row.get("days", 1)),
                )
            )

        quote = ParticipantQuote(
            participant=participant,
            support_start=start,
            support_end=end,
            lines=lines,
            region=region,
        )

        with TemporaryDirectory() as tmp:
            output = Path(tmp) / "participant_summary_of_supports.html"
            generate_quote_html(
                quote=quote,
                output_path=str(output),
                catalog=catalog,
                weeks_override=float(weeks_override),
                region_override=region,
            )
            html_bytes = output.read_bytes()

        st.success("Quote generated successfully.")
        st.download_button(
            label="Download quote HTML",
            data=html_bytes,
            file_name="participant_summary_of_supports.html",
            mime="text/html",
        )


def notes_and_adaptations_tab() -> None:
    st.subheader("Progress Notes & Adaptation Recommendations")
    st.markdown("<div class='soft-card'>Upload progress notes to auto-suggest features/adaptations with evidence lines.</div>", unsafe_allow_html=True)

    uploaded = st.file_uploader(
        "Upload progress notes (txt, md, csv, pdf)",
        type=["txt", "md", "csv", "pdf"],
        accept_multiple_files=True,
    )

    if st.button("Analyze notes"):
        if not uploaded:
            st.warning("Upload at least one progress note file first.")
            return

        with TemporaryDirectory() as tmp:
            paths = []
            for file in uploaded:
                target = Path(tmp) / file.name
                target.write_bytes(file.getvalue())
                paths.append(str(target))

            analysis = analyze_progress_notes(paths)

        st.info(f"Scanned {analysis.total_lines_scanned} lines across {len(analysis.note_files)} file(s).")

        if not analysis.recommendations:
            st.warning("No recommendations were triggered from these notes.")
            return

        for rec in analysis.recommendations:
            with st.expander(f"{rec.title}  |  Priority: {rec.priority.upper()}  |  Confidence: {rec.confidence:.2f}"):
                st.write(rec.description)
                if rec.evidence:
                    st.caption("Evidence from notes")
                    for item in rec.evidence:
                        st.markdown(f"- {item}")


def document_generator_tab() -> None:
    st.subheader("Governance-Guided Documents")
    st.markdown("<div class='soft-card'>Generate professional report/review/letter drafts with integrated governance and adaptation planning.</div>", unsafe_allow_html=True)

    col1, col2, col3 = st.columns(3)
    doc_type = col1.selectbox("Document type", ["report", "review", "letter"])
    participant = col2.text_input("Participant", value="Sample Participant")
    ndis_number = col3.text_input("NDIS number", value="4300 000 000")

    col4, col5, col6 = st.columns(3)
    author = col4.text_input("Author", value="Provider Clinician")
    organisation = col5.text_input("Organisation", value="Windgap Support Services")
    purpose = col6.text_input("Purpose", value="Progress and recommendations for plan review")

    col7, col8 = st.columns(2)
    period_start = col7.text_input("Period start (optional)", value="")
    period_end = col8.text_input("Period end (optional)", value="")

    key_points_text = st.text_area(
        "Key points (one per line)",
        value="Improved engagement in community activities\nOngoing need for structured supports",
        height=120,
    )

    progress_files = st.file_uploader(
        "Attach progress notes for recommendations (optional)",
        type=["txt", "md", "csv", "pdf"],
        accept_multiple_files=True,
        key="doc_progress_notes",
    )

    if st.button("Generate document", type="primary"):
        key_points = [line.strip() for line in key_points_text.splitlines() if line.strip()]

        with TemporaryDirectory() as tmp:
            note_paths = []
            for file in progress_files or []:
                target = Path(tmp) / file.name
                target.write_bytes(file.getvalue())
                note_paths.append(str(target))

            request = DocumentRequest(
                document_type=doc_type,
                participant_name=participant,
                ndis_number=ndis_number,
                author_name=author,
                organisation=organisation,
                purpose=purpose,
                period_start=period_start,
                period_end=period_end,
                key_points=key_points,
                progress_note_paths=note_paths,
            )

            output_path = Path(tmp) / f"{doc_type}_{participant.replace(' ', '_')}.md"
            write_document_markdown(
                request=request,
                output_path=str(output_path),
                max_snippets_per_source=2,
            )
            md_bytes = output_path.read_bytes()

        st.success("Document generated successfully.")
        st.download_button(
            label=f"Download {doc_type} draft (.md)",
            data=md_bytes,
            file_name=f"{doc_type}_{participant.replace(' ', '_')}.md",
            mime="text/markdown",
        )


def deployment_tab() -> None:
    st.subheader("SharePoint-friendly Deployment Options")
    st.markdown("<div class='soft-card'>Use this tab to align online + downloadable access with organisation workflows.</div>", unsafe_allow_html=True)

    st.markdown("<span class='pill'>Recommended</span><strong>Host app online and link/embed in SharePoint</strong>", unsafe_allow_html=True)
    st.markdown(
        """
- Deploy this app to an internal cloud host (e.g., Azure App Service / Container Apps).
- Add URL to SharePoint as a Site Link or Embed web part.
- Users access the latest version anytime online with central updates.
"""
    )

    st.markdown("<span class='pill'>Offline-capable</span><strong>Distribute desktop bundle</strong>", unsafe_allow_html=True)
    st.markdown(
        """
- Build a Windows executable with PyInstaller for offline local use.
- Publish the installer/exe via SharePoint Document Library.
- Team can download and run without local Python setup.
"""
    )

    st.info("Detailed rollout instructions are in DEPLOY_SHAREPOINT.md in this repository.")


def main() -> None:
    inject_theme()
    render_header()

    tabs = st.tabs([
        "Quote Builder",
        "Progress Notes",
        "Document Generator",
        "SharePoint Deployment",
    ])

    with tabs[0]:
        quote_builder_tab()
    with tabs[1]:
        notes_and_adaptations_tab()
    with tabs[2]:
        document_generator_tab()
    with tabs[3]:
        deployment_tab()


if __name__ == "__main__":
    main()
