using NotificationService.Application.Filters;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Application.Services;

public class NotificationAppService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationAppService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }
    public async Task SendNotificationAsync(Notification notification)
    {
        await _notificationRepository.AddAsync(notification);
    }

    public async Task<List<Notification>> GetNotificationsAsync(NotificationFilter filter,PageParams param)
    {
        return await _notificationRepository.GetAllAsync(filter,param);
    }

    public async Task<Notification?> GetNotificationByIdAsync(Guid id)
    {
        return await _notificationRepository.GetByIdAsync(id);
    }

    public async Task MarkAsReadAsync(Guid id)
    {
        await _notificationRepository.MarkAsReadAsync(id);
    }
}