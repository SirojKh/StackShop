namespace Shared.Contracts.Events;

public class AnalyticsEventDto
{
    public string EventType { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? Data { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}