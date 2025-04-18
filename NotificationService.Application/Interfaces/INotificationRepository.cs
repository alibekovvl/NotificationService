using NotificationService.Domain.Entities;
using NotificationService.Domain.FiltersSortPaginations;

namespace NotificationService.Application.Interfaces;

public interface INotificationRepository
{
    Task AddAsync (Notification notification);
    Task <List<Notification>> GetUnreadNotificationsAsync(string userId);
    Task MarkAsReadAsync(Guid id);
    Task<List<Notification>> GetAllAsync(NotificationFilter filter, PageParams param);
    Task <Notification?> GetByIdAsync (Guid id);
    Task UpdateAsync (Notification notification);
}   