using System.Text;
using System.Text.Json;
using AnalyticsService.Interfaces;
using AnalyticsService.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
    [SwaggerOperation(Summary = "Hämta eventrapport", Description = "Filtrera valfritt på start och/eller slutdatum (format: yyyy-MM-dd)")]
    public async Task<IActionResult> GetReport(
        [FromQuery, SwaggerParameter("Startdatum (format: yyyy-MM-dd)", Required = false)] DateTime? from = null,
        [FromQuery, SwaggerParameter("Slutdatum (format: yyyy-MM-dd)", Required = false)] DateTime? to = null)
    {
        var report = await _analyticsService.GetEventCountsAsync(from, to);
        return Ok(report);
    }

    [HttpGet("export")]
    [SwaggerOperation(Summary = "Exportera eventdata", Description = "Export som JSON eller CSV, valbart datumfilter (format: yyyy-MM-dd)")]
    public IActionResult ExportEvents(
        [FromQuery, SwaggerParameter("Startdatum (format: yyyy-MM-dd)", Required = false)] DateTime? from = null,
        [FromQuery, SwaggerParameter("Slutdatum (format: yyyy-MM-dd)", Required = false)] DateTime? to = null,
        [FromQuery, SwaggerParameter("Exportformat (json eller csv)")] ExportFormat format = ExportFormat.json)
    {
        var events = _analyticsService.GetEvents(from, to);

        if (format == ExportFormat.csv)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Id,EventType,UserId,Data,Timestamp");

            foreach (var e in events)
            {
                csv.AppendLine($"\"{e.Id}\",\"{e.EventType}\",\"{e.UserId}\",\"{e.Data}\",\"{e.Timestamp:O}\"");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"analytics_export_{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
        }

        return Ok(events);
    }






}