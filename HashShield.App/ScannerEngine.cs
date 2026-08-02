using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace HashShield.App;

public sealed record HashScanResult(
    string FileName,
    string Hash,
    long Size,
    string LocalRuleSummary,
    string VirusTotalSummary);

public sealed record DirectoryScanSummary(
    int FilesScanned,
    int SuspiciousFiles,
    string ResultText);

public static class ScannerEngine
{
    private static readonly HashSet<string> SuspiciousExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".exe", ".dll", ".bat", ".cmd", ".scr", ".js", ".jse", ".vbs", ".vbe", ".ps1", ".msi", ".hta",
        ".jar", ".com", ".lnk", ".reg", ".wsf", ".wsh", ".pif"
    };

    public static async Task<HashScanResult> ScanFileAsync(string filePath, string? virusTotalApiKey, CancellationToken cancellationToken = default)
    {
        var fileInfo = new FileInfo(filePath);
        await using var inputStream = File.OpenRead(filePath);
        var hashBytes = SHA256.HashData(inputStream);
        var hash = Convert.ToHexString(hashBytes).ToLowerInvariant();

        var localRuleSummary = await CheckLocalRulesAsync(filePath, cancellationToken);
        var virusTotalSummary = await CheckVirusTotalAsync(hash, virusTotalApiKey, cancellationToken);

        return new HashScanResult(
            fileInfo.Name,
            hash,
            fileInfo.Length,
            localRuleSummary,
            virusTotalSummary);
    }

    public static async Task<DirectoryScanSummary> QuickScanAsync(string rootPath, string? virusTotalApiKey, CancellationToken cancellationToken = default)
    {
        var roots = GetQuickScanRoots(rootPath);
        return await ScanDirectoriesAsync(roots, false, virusTotalApiKey, cancellationToken);
    }

    public static async Task<DirectoryScanSummary> FullScanAsync(string rootPath, string? virusTotalApiKey, CancellationToken cancellationToken = default)
    {
        var roots = GetFullScanRoots(rootPath);
        return await ScanDirectoriesAsync(roots, true, virusTotalApiKey, cancellationToken);
    }

    private static async Task<DirectoryScanSummary> ScanDirectoriesAsync(IEnumerable<string> roots, bool fullScan, string? virusTotalApiKey, CancellationToken cancellationToken)
    {
        var suspiciousFiles = 0;
        var filesScanned = 0;
        var results = new List<string>();

        foreach (var root in roots.Where(Directory.Exists))
        {
            var options = new EnumerationOptions
            {
                RecurseSubdirectories = true,
                IgnoreInaccessible = true,
                AttributesToSkip = FileAttributes.Hidden | FileAttributes.System,
                MatchType = MatchType.Simple
            };

            foreach (var file in Directory.EnumerateFiles(root, "*", options))
            {
                if (ShouldSkipPath(file))
                {
                    continue;
                }

                filesScanned++;

                if (!fullScan && !ShouldInspectQuickly(file))
                {
                    continue;
                }

                var fileResult = await ScanFileAsync(file, virusTotalApiKey, cancellationToken);
                var localRule = fileResult.LocalRuleSummary;
                var isSuspicious = !string.Equals(localRule, "Keine lokalen Signaturen getroffen", StringComparison.OrdinalIgnoreCase);
                if (isSuspicious)
                {
                    suspiciousFiles++;
                    results.Add($"[verdächtig] {file} | {localRule} | {fileResult.VirusTotalSummary}");
                }
                else
                {
                    results.Add($"[neutral] {file} | {localRule} | {fileResult.VirusTotalSummary}");
                }
            }
        }

        var overview = $"Dateien geprüft: {filesScanned}{Environment.NewLine}" +
                       $"Verdächtige Treffer: {suspiciousFiles}{Environment.NewLine}" +
                       $"Ergebnisse:{Environment.NewLine}{string.Join(Environment.NewLine, results.Take(25))}";

        return new DirectoryScanSummary(filesScanned, suspiciousFiles, overview);
    }

    private static IEnumerable<string> GetQuickScanRoots(string rootPath)
    {
        var candidates = new List<string>();

        if (!string.IsNullOrWhiteSpace(rootPath))
        {
            candidates.Add(rootPath);
        }

        var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        var downloads = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        var temp = Path.GetTempPath();

        if (!string.IsNullOrWhiteSpace(desktop)) candidates.Add(desktop);
        if (Directory.Exists(downloads)) candidates.Add(downloads);
        if (!string.IsNullOrWhiteSpace(temp)) candidates.Add(temp);

        return candidates.Distinct(StringComparer.OrdinalIgnoreCase);
    }

    private static IEnumerable<string> GetFullScanRoots(string rootPath)
    {
        var drives = Directory.GetLogicalDrives();
        if (!string.IsNullOrWhiteSpace(rootPath) && Directory.Exists(rootPath))
        {
            return new[] { rootPath };
        }

        return drives.Where(Directory.Exists);
    }

    private static bool ShouldSkipPath(string filePath)
    {
        return filePath.Contains("$Recycle.Bin", StringComparison.OrdinalIgnoreCase)
            || filePath.Contains("System Volume Information", StringComparison.OrdinalIgnoreCase)
            || filePath.Contains("\\Windows\\WinSxS\\", StringComparison.OrdinalIgnoreCase)
            || filePath.Contains("\\Windows\\Installer\\", StringComparison.OrdinalIgnoreCase);
    }

    private static bool ShouldInspectQuickly(string filePath)
    {
        var extension = Path.GetExtension(filePath);
        return SuspiciousExtensions.Contains(extension)
            || filePath.Contains("Downloads", StringComparison.OrdinalIgnoreCase)
            || filePath.Contains("Desktop", StringComparison.OrdinalIgnoreCase)
            || filePath.Contains("Temp", StringComparison.OrdinalIgnoreCase)
            || filePath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)
            || filePath.EndsWith(".log", StringComparison.OrdinalIgnoreCase)
            || filePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
            || filePath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)
            || filePath.EndsWith(".cfg", StringComparison.OrdinalIgnoreCase)
            || filePath.EndsWith(".ini", StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<string> CheckLocalRulesAsync(string filePath, CancellationToken cancellationToken)
    {
        var fileBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        var textPreview = Encoding.ASCII.GetString(fileBytes.Take(2048).ToArray()).ToLowerInvariant();
        var matches = new List<string>();

        if (fileBytes.Length > 1 && fileBytes[0] == 0x4D && fileBytes[1] == 0x5A)
        {
            matches.Add("PE-Header erkannt (MZ)");
        }

        if (SuspiciousExtensions.Contains(extension))
        {
            matches.Add("Verdächtige Dateiendung erkannt");
        }

        if (textPreview.Contains("powershell") || textPreview.Contains("cmd.exe") || textPreview.Contains("rundll32") || textPreview.Contains("frombase64string"))
        {
            matches.Add("PowerShell-/CMD-Muster erkannt");
        }

        if (textPreview.Contains("autoopen") || textPreview.Contains("vba") || textPreview.Contains("macro"))
        {
            matches.Add("Makro-/Office-Muster erkannt");
        }

        if (textPreview.Contains("http://") || textPreview.Contains("https://") || textPreview.Contains("base64"))
        {
            matches.Add("Netzwerk-/Code-Decode-Muster erkannt");
        }

        if (matches.Count == 0)
        {
            return "Keine lokalen Signaturen getroffen";
        }

        return string.Join(" | ", matches);
    }

    private static async Task<string> CheckVirusTotalAsync(string hash, string? virusTotalApiKey, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(virusTotalApiKey))
        {
            return "VirusTotal: kein API-Key";
        }

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("x-apikey", virusTotalApiKey);

        try
        {
            using var response = await httpClient.GetAsync($"https://www.virustotal.com/api/v3/files/{hash}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return $"VirusTotal: HTTP {(int)response.StatusCode}";
            }

            await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var document = await JsonDocument.ParseAsync(responseStream, cancellationToken: cancellationToken);

            if (!document.RootElement.TryGetProperty("data", out var dataElement))
            {
                return "VirusTotal: keine Daten";
            }

            if (!dataElement.TryGetProperty("attributes", out var attributesElement))
            {
                return "VirusTotal: keine Attribute";
            }

            if (!attributesElement.TryGetProperty("last_analysis_stats", out var statsElement))
            {
                return "VirusTotal: keine Analyse-Statistik";
            }

            var malicious = statsElement.TryGetProperty("malicious", out var maliciousElement) && maliciousElement.ValueKind == JsonValueKind.Number
                ? maliciousElement.GetInt32()
                : 0;
            var suspicious = statsElement.TryGetProperty("suspicious", out var suspiciousElement) && suspiciousElement.ValueKind == JsonValueKind.Number
                ? suspiciousElement.GetInt32()
                : 0;

            return malicious > 0 || suspicious > 0
                ? $"VirusTotal: verdächtig ({malicious} malicious, {suspicious} suspicious)"
                : "VirusTotal: sauber/noch unbekannt";
        }
        catch (Exception ex)
        {
            return $"VirusTotal: Fehler - {ex.Message}";
        }
    }
}
