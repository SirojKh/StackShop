using NotificationService.Models;

namespace NotificationService.Services;

public class NotificationService : INotificationService
{
    public Task SendNotificationAsync(NotificationMessage message)
    {
        Console.WriteLine($"[Notification] [{message.Type}] To: {message.Recipient} - {message.Content}");
        return Task.CompletedTask;
    }
}