using EmailService.Models;

namespace EmailService.Services;

public interface IEmailService
{
    Task SendEmailAsync(EmailMessage message);
}