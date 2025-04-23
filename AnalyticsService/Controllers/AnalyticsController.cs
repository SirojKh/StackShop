using AnalyticsService.Interfaces;
using AnalyticsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace AnalyticsService.Controllers;

[ApiController]
[Route("api/analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;
    private readonly HttpClient _http;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpPost("events")]
    public async Task<IActionResult> LogEvent([FromBody] AnalyticsEvent analyticsEvent)
    {
        await _analyticsService.LogEventAsync(analyticsEvent);
        return Ok();
    }

    [HttpGet("reports")]
    public async Task<IActionResult> GetReport()
    {
        var report = await _analyticsService.GetEventCountsAsync();
        return Ok(report);
    }
}