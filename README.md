# HashShield

HashShield ist ein lokales Windows-Desktop-Tool für schnelle Dateisicherheitsprüfungen.

## Ziel

- Dateien per Drag & Drop in die App ziehen
- SHA-256-Hash berechnen
- Lokale Signaturregeln gegen bekannte Malware-Muster prüfen
- Optional VirusTotal-API-Abfrage für externe Vergleichsdaten

## Tech-Stack

- C# / .NET 10
- Windows Forms für eine klassische Windows-Oberfläche

## Schnellstart unter Windows

1. .NET 10 SDK installieren
2. Das Repository öffnen
3. In den Ordner `HashShield.App` wechseln
4. App starten:

```powershell
dotnet run --project .\HashShield.App\HashShield.App.csproj
```

## Hinweis

Dieses Projekt ist als leicht erweiterbare Windows-Desktop-Basis gedacht. Für echte Enterprise-Schutzfunktionen kann später eine echte YARA-Engine, lokale Signaturdatenbanken und eine robuste VirusTotal-Integration ergänzt werden.