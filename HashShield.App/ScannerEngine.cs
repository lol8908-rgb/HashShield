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

public static class ScannerEngine
{
    public static async Task<HashScanResult> ScanAsync(string filePath, string? virusTotalApiKey, CancellationToken cancellationToken = default)
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

    private static async Task<string> CheckLocalRulesAsync(string filePath, CancellationToken cancellationToken)
    {
        var fileBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
        var fileText = Encoding.ASCII.GetString(fileBytes).ToLowerInvariant();
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        var matches = new List<string>();

        if (fileText.Contains("microsoft") && fileText.Contains("powershell"))
        {
            matches.Add("PowerShell-Typische Signatur gefunden");
        }

        if (fileBytes.Length > 0 && fileBytes[0] == 0x4D && fileBytes[1] == 0x5A)
        {
            matches.Add("PE-Header erkannt (MZ)");
        }

        if (extension is ".ps1" or ".psm1" or ".bat" && fileText.Contains("frombase64string"))
        {
            matches.Add("Base64-Decode-Muster erkannt");
        }

        if (extension is ".docm" or ".xlsm" or ".pptm" && fileText.Contains("autoopen"))
        {
            matches.Add("Makro-ähnliches AutoOpen-Muster gefunden");
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
            return "VirusTotal: Kein API-Key gesetzt";
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

            var root = document.RootElement;
            if (!root.TryGetProperty("data", out var dataElement))
            {
                return "VirusTotal: Keine Daten";
            }

            if (!dataElement.TryGetProperty("attributes", out var attributesElement))
            {
                return "VirusTotal: Keine Attribute";
            }

            if (!attributesElement.TryGetProperty("last_analysis_stats", out var statsElement))
            {
                return "VirusTotal: Keine Analyse-Statistik";
            }

            var malicious = statsElement.TryGetProperty("malicious", out var maliciousElement) && maliciousElement.ValueKind == JsonValueKind.Number
                ? maliciousElement.GetInt32()
                : 0;
            var suspicious = statsElement.TryGetProperty("suspicious", out var suspiciousElement) && suspiciousElement.ValueKind == JsonValueKind.Number
                ? suspiciousElement.GetInt32()
                : 0;

            return malicious > 0 || suspicious > 0
                ? $"VirusTotal: verdächtig ({malicious} malicious, {suspicious} suspicious)"
                : "VirusTotal: sauber oder noch nicht bekannt";
        }
        catch (Exception ex)
        {
            return $"VirusTotal: Fehler - {ex.Message}";
        }
    }
}
