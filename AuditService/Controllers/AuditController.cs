using AuditService.Interfaces;
using AuditService.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuditService.Controllers;

[ApiController]
[Route("api/audit")]
public class AuditController : ControllerBase
{
    private readonly IAuditService _auditService;

    public AuditController(IAuditService auditService)
    {
        _auditService = auditService;
    }

    [HttpPost("log")]
    public async Task<IActionResult> Log([FromBody] AuditLog log)
    {
        await _auditService.LogAsync(log);
        return Ok();
    }

    [HttpGet("logs")]
    public async Task<IActionResult> GetAll()
    {
        var logs = await _auditService.GetAllAsync();
        return Ok(logs);
    }
}