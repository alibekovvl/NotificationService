using NotificationService.Domain.Entities;

namespace NotificationService.Application.Interfaces;

public interface INotificationBroadcaster
{
    Task BroadcastNotificationAsync(Notification notification);
}