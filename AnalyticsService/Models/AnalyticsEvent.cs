namespace AnalyticsService.Models;

public class AnalyticsEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string EventType { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? Data { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}