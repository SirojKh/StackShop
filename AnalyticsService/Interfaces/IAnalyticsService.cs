using AnalyticsService.Models;

namespace AnalyticsService.Interfaces;

public interface IAnalyticsService
{
    Task LogEventAsync(AnalyticsEvent analyticsEvent);
    Task<Dictionary<string, int>> GetEventCountsAsync();
}