@echo off
setlocal
cd /d "%~dp0"

set "DOTNET_EXE="
if exist "%LOCALAPPDATA%\Microsoft\dotnet\dotnet.exe" set "DOTNET_EXE=%LOCALAPPDATA%\Microsoft\dotnet\dotnet.exe"
if not defined DOTNET_EXE if exist "%ProgramFiles%\dotnet\dotnet.exe" set "DOTNET_EXE=%ProgramFiles%\dotnet\dotnet.exe"
if not defined DOTNET_EXE for /f "delims=" %%I in ('where dotnet 2^>nul') do set "DOTNET_EXE=%%I"

if not defined DOTNET_EXE (
    echo .NET SDK wurde nicht gefunden.
    echo Installiere zuerst .NET 10 SDK und starte dann die Datei erneut.
    pause
    exit /b 1
)

"%DOTNET_EXE%" run --project ".\HashShield.App\HashShield.App.csproj"
if errorlevel 1 pause
exit /b %errorlevel%
