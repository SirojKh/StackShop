using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NotificationService.Models;
using NotificationService.Services;

public class EmailClient : IEmailClient
{
    private readonly HttpClient _httpClient;

    public EmailClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task SendEmailAsync(EmailMessage message)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/email/send", message);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("[EmailClient] E-post skickades via EmailService.");
        }
        else
        {
            Console.WriteLine($"[EmailClient] Misslyckades skicka e-post: {response.StatusCode}");
        }
    }
}