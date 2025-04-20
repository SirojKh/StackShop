namespace OrderingService.Interfaces;

public interface IAuditLogger
{
    Task LogAsync(string eventType, string? userId, string? message);
}