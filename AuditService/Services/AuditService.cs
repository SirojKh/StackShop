using AuditService.Interfaces;
using AuditService.Models;

namespace AuditService.Services;

public class AuditService : IAuditService
{
    private readonly List<AuditLog> _logs = new();

    public Task LogAsync(AuditLog log)
    {
        _logs.Add(log);
        Console.WriteLine($"[Audit] {log.Timestamp:O} | {log.EventType} | User: {log.UserId} | Msg: {log.Message}");
        return Task.CompletedTask;
    }

    public Task<IEnumerable<AuditLog>> GetAllAsync()
    {
        return Task.FromResult(_logs.AsEnumerable());
    }
}