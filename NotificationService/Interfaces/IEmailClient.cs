using System.Threading.Tasks;
using NotificationService.Models;

namespace NotificationService.Services;

public interface IEmailClient
{
    Task SendEmailAsync(EmailMessage message);
}