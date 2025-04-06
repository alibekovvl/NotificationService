using NotificationService.Application.Filters;
using NotificationService.Domain.Entities;

namespace NotificationService.Domain.Interfaces;

public interface INotificationService
{
    Task SendNotificationAsync (Notification notification);
    Task <List<Notification>> GetNotificationsAsync(NotificationFilter filter,PageParams param);
    Task<Notification> GetNotificationByIdAsync (Guid id);
    Task MarkAsReadAsync(Guid id);
    Task ProcessNotificationAsync(Guid id);
}