using NotificationService.Domain.Entities;

namespace NotificationService.Domain.Interfaces;

public interface INotificationService
{
    Task SendNotificationAsync (Notification notification);
}