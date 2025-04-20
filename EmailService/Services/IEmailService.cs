using System.Threading.Tasks;
using EmailService.Models;

namespace EmailService.Services;

public interface IEmailService
{
    Task SendEmailAsync(EmailMessage message);
}