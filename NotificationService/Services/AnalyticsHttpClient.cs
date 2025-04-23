using System.Text;
using System.Text.Json;
using Shared.Contracts.Events;
using Shared.Contracts.Interfaces;

namespace NotificationService.Services;

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
        var payload = new AnalyticsEventDto
        {
            EventType = eventType,
            UserId = userId,
            Data = data
        };

        try
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _http.PostAsync("/api/analytics/events", content);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("[Analytics] Kunde inte logga event {EventType}. Status: {StatusCode}", eventType, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Analytics] Undantag vid loggning av {EventType}", eventType);
        }
    }
}