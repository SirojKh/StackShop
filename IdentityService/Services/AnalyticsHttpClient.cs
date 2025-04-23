using System.Text;
using System.Text.Json;
using IdentityService.Interfaces;
using Shared.Contracts.Events;

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
            var response = await _http.PostAsync(
                "/api/analytics/events",
                new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
                _logger.LogWarning("[Analytics] Kunde inte logga {EventType}", eventType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Analytics] Undantag vid loggning");
        }
    }
}