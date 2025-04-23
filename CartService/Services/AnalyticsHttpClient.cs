using System.Text;
using System.Text.Json;
using CartService.Interfaces;

namespace CartService.Services;

public class AnalyticsHttpClient : IAnalyticsLogger
{
    private readonly HttpClient _http;
    private readonly ILogger<AnalyticsHttpClient> _logger;

    public AnalyticsHttpClient(HttpClient http, ILogger<AnalyticsHttpClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task LogAsync(string eventType, string? userId, string? data)
    {
        var payload = new
        {
            EventType = eventType,
            UserId = userId,
            Data = data
        };

        try
        {
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _http.PostAsync("/api/analytics/events", content);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("[Analytics] Kunde inte logga {EventType}. Status: {StatusCode}", eventType, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Analytics] Undantag vid loggning av {EventType}", eventType);
        }
    }
}