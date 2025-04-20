using AuditService.Models;

namespace AuditService.Interfaces;

public interface IAuditService
{
    Task LogAsync(AuditLog log);
    Task<IEnumerable<AuditLog>> GetAllAsync();
}