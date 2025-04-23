using AnalyticsService.Models;

namespace AnalyticsService.Interfaces;

public interface IAnalyticsService
{
    Task LogEventAsync(AnalyticsEvent analyticsEvent);
    Task<Dictionary<string, int>> GetEventCountsAsync(DateTime? from = null, DateTime? to = null);
    IEnumerable<AnalyticsEvent> GetEvents(DateTime? from = null, DateTime? to = null); // <-- lägg till denna

}