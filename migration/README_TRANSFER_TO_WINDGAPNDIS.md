# Transfer package to Windgap NDIS repo/codespace

This folder contains migration artifacts from the wrong codespace.

At the time of packaging, direct push was not possible from this environment because the target repo/codespace was not accessible via GitHub CLI from this codespace.

## Artifacts

- `WindgapNDIS-migration-files.tar.gz` (complete file bundle)
- `WindgapNDIS-migration.patch` (diff, tracked-only)
- `file_list.txt` (files included in bundle)

## Recommended apply method (from target repo root)

1. Copy `WindgapNDIS-migration-files.tar.gz` into the target repo root.
2. Extract it:

```bash
tar -xzf WindgapNDIS-migration-files.tar.gz
```

3. Install dependencies and run checks:

```bash
python -m pip install -r requirements.txt
python -m pytest test_flask_app.py test_ndis_quote_generator.py test_ndis_document_generator.py test_ndis_progress_notes.py
python app.py
curl http://127.0.0.1:8000/health
```

If you only want quick verification without starting the web server:

```bash
python -m pytest test_flask_app.py test_ndis_quote_generator.py test_ndis_document_generator.py test_ndis_progress_notes.py
```

## Included application entrypoints

- `app.py` — Flask web app (current primary UI)
- `web_app.py` — Streamlit web app (legacy/alternate UI)

## Included UI assets

- `templates/` — Flask HTML templates
- `static/css/app.css` — professional Windgap-inspired styling
- `static/js/app.js` — tab behavior and dynamic quote rows

## Environment notes

- `.env` contains a placeholder `OPENAI_API_KEY`; replace it in the target repo as needed.
- `xhtml2pdf` may require Cairo system libraries in Linux containers (for example `libcairo2-dev`) before `pip install -r requirements.txt` succeeds fully.

## Previously suggested checks (still useful)

```bash
python test_ndis_progress_notes.py
python test_ndis_document_generator.py
python test_ndis_quote_generator.py
python -m pytest test_flask_app.py
```

## New capabilities included

- Governance-guided quote/report/review/letter outputs
- Progress-note upload analysis + adaptation recommendations
- Professional Flask web app (`app.py`) with Windgap-inspired styling
- Alternate Streamlit web app (`web_app.py`)
- SharePoint deployment docs and offline Windows build scripts
