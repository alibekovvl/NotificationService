using NotificationService.Domain.Entities;

namespace NotificationService.Domain.Interfaces;

public interface INotificationRepository
{
    Task AddAsync (Notification notification);
    Task <List<Notification>> GetUnreadNotificationsAsync(string userId);
    Task MarkAsReadAsync(Guid notificationId);
}