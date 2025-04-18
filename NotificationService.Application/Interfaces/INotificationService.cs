using NotificationService.Domain.Entities;
using NotificationService.Domain.FiltersSortPaginations;

namespace NotificationService.Application.Interfaces;

public interface INotificationService
{
    Task SendNotificationAsync (NotificationDTOs notificationDto);
    Task <List<Notification>> GetNotificationsAsync(NotificationFilter filter,PageParams param);
    Task<Notification?> GetNotificationByIdAsync (Guid id);
    Task MarkAsReadAsync(Guid id);
    Task ProcessNotificationAsync(Guid id);
}