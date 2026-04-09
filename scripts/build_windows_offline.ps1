Param(
    [string]$PythonExe = ".venv\Scripts\python.exe"
)

Write-Host "Building Windgap NDIS Workspace offline executable..."

if (-Not (Test-Path $PythonExe)) {
    Write-Error "Python executable not found: $PythonExe"
    exit 1
}

& $PythonExe -m pip install pyinstaller
& $PythonExe -m PyInstaller --onefile --name WindgapNDISWorkspace app.py

Write-Host "Build complete. Distribute dist\WindgapNDISWorkspace.exe via SharePoint."
