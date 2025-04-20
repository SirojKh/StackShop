using System;
using System.Threading.Tasks;
using EmailService.Models;
using EmailService.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmailService.Controllers;

[ApiController]
[Route("api/email")]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public EmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendEmail([FromBody] EmailMessage message)
    {
        try
        {
            await _emailService.SendEmailAsync(message);
            return Ok("Email sent successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EmailController] Fel vid e-postutskick: {ex.Message}");
            return StatusCode(500, "Ett fel inträffade när e-post skulle skickas.");
        }
    }
}