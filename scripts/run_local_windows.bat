@echo off
set PY=.venv\Scripts\python.exe
if not exist %PY% (
  echo Python virtual environment not found at %PY%
  exit /b 1
)
%PY% app.py
