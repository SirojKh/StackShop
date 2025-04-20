using System.Text;
using System.Text.Json;
using OrderingService.Interfaces;

namespace OrderingService.Services;

public class AuditHttpClient : IAuditLogger
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuditHttpClient> _logger;

    public AuditHttpClient(HttpClient httpClient, ILogger<AuditHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task LogAsync(string eventType, string? userId, string? message)
    {
        var payload = new
        {
            eventType,
            userId,
            message,
            timestamp = DateTime.UtcNow
        };

        try
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/audit/log", content);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("[Audit] Misslyckades logga event: {EventType}. Status: {StatusCode}", eventType, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Audit] Undantag vid loggning av event: {EventType}", eventType);
        }
    }
}