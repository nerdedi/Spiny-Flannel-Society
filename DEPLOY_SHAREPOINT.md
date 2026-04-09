# Deploy Windgap NDIS Web App for SharePoint Access

This guide provides two practical rollout paths for organisations using SharePoint.

## Option A (Recommended): Online hosted app + SharePoint link/embed

### 1) Host the app

Use your approved hosting platform (e.g., Azure App Service, Azure Container Apps, or internal VM).

For container-based deployment, build from this repo with `Dockerfile`.

The container now starts the Flask application from `app.py` on port `8000`.

### 2) Add app to SharePoint

- Add as **Site Link** (quickest)
- Or use **Embed web part** with hosted URL (if allowed by tenant policy)

### 3) Operational setup

- Use organisation SSO/reverse proxy where required
- Restrict access to approved groups
- Keep governance PDFs and JSON data in controlled storage

## Option B: Downloadable desktop app from SharePoint library

Best for staff who need offline/local access.

### 1) Build executable

Use PyInstaller (Windows build recommended):

- Install PyInstaller
- Build executable from `app.py`
- Use helper script: `scripts/build_windows_offline.ps1`

### 2) Publish to SharePoint

Upload installer/exe into your SharePoint Document Library and share with staff.

If needed, include `scripts/run_local_windows.bat` for local launch convenience.

### 3) Update strategy

Version releases (e.g. v1.0.1) and keep a changelog in the same library.

## Suggested architecture

- **Source of truth:** GitHub repo
- **Operational host:** Azure/internal environment
- **User access layer:** SharePoint links/embed
- **File distribution:** SharePoint Document Library

## Security and governance notes

- Do not upload sensitive participant data into version control.
- Keep uploaded progress notes in temporary processing storage.
- Apply your organisation's retention and audit requirements.
- Validate generated documents before formal submission.

## Local run command

```bash
python app.py
```
