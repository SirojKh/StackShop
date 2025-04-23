namespace CartService.Interfaces;

public interface IAnalyticsLogger
{
    Task LogAsync(string eventType, string? userId, string? data);
}