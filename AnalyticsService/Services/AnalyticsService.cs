using AnalyticsService.Interfaces;
using AnalyticsService.Models;

namespace AnalyticsService.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly List<AnalyticsEvent> _events = new();

    public Task LogEventAsync(AnalyticsEvent analyticsEvent)
    {
        _events.Add(analyticsEvent);
        Console.WriteLine($"[Analytics] {analyticsEvent.Timestamp:O} | {analyticsEvent.EventType} | User: {analyticsEvent.UserId}");
        return Task.CompletedTask;
    }

    public Task<Dictionary<string, int>> GetEventCountsAsync()
    {
        var report = _events
            .GroupBy(e => e.EventType)
            .ToDictionary(g => g.Key, g => g.Count());

        return Task.FromResult(report);
    }
}