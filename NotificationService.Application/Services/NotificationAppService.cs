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

}