using NotificationService.Models;

namespace NotificationService.Services;

public interface INotificationService
{
    Task SendNotificationAsync(NotificationMessage message);
}