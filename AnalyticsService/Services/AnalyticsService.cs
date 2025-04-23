using AnalyticsService.Interfaces;
using AnalyticsService.Models;
using System.Collections.Concurrent;

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

    public Task<Dictionary<string, int>> GetEventCountsAsync(DateTime? from = null, DateTime? to = null)
    {
        var filtered = _events.AsEnumerable();

        if (from.HasValue)
            filtered = filtered.Where(e => e.Timestamp >= from.Value);

        if (to.HasValue)
            filtered = filtered.Where(e => e.Timestamp <= to.Value);

        var report = filtered
            .GroupBy(e => e.EventType)
            .ToDictionary(g => g.Key, g => g.Count());

        return Task.FromResult(report);
    }


    public Task<IEnumerable<AnalyticsEvent>> GetEventsAsync(DateTime? from = null, DateTime? to = null)
    {
        IEnumerable<AnalyticsEvent> result = _events;

        if (from.HasValue)
            result = result.Where(e => e.Timestamp >= from.Value);

        if (to.HasValue)
            result = result.Where(e => e.Timestamp <= to.Value);

        return Task.FromResult(result);
    }

    public IEnumerable<AnalyticsEvent> GetEvents(DateTime? from = null, DateTime? to = null)
    {
        return _events.Where(e =>
            (!from.HasValue || e.Timestamp >= from.Value) &&
            (!to.HasValue || e.Timestamp <= to.Value));
    }



}