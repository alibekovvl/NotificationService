using NotificationService.Application.Filters;
using NotificationService.Domain.Entities;

namespace NotificationService.Domain.Interfaces;

public interface INotificationRepository
{
    Task AddAsync (Notification notification);
    Task <List<Notification>> GetUnreadNotificationsAsync(string userId);
    Task MarkAsReadAsync(Guid id);
    Task<List<Notification>> GetAllAsync(NotificationFilter filter, PageParams param);
    Task <Notification?> GetByIdAsync (Guid id);
    Task UpdateAsync (Notification notification);
}   