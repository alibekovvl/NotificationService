using Hangfire;
using NotificationService.Application.Filters;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Application.Services;

public class NotificationAppService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IAIAnalysisService _aiAnalysisService;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IRedisCacheService _redisCacheService;
    public NotificationAppService(INotificationRepository notificationRepository, IAIAnalysisService aiAnalysisService, IBackgroundJobClient backgroundJobClient, IRedisCacheService redisCacheService)
    {
        _notificationRepository = notificationRepository;
        _aiAnalysisService = aiAnalysisService;
        _backgroundJobClient = backgroundJobClient;
        _redisCacheService = redisCacheService;
    }
    public async Task SendNotificationAsync(NotificationDTOs notificationDto)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(), 
            Title = notificationDto.Title,
            Message = notificationDto.Message,
            RecipientEmail = notificationDto.RecipientEmail,
            CreatedAt = DateTime.UtcNow, 
        };
       
        var allNotifications = await _notificationRepository.GetAllAsync(new NotificationFilter(), new PageParams());
        var cachedNotification = await _redisCacheService.GetDataAsync<Notification>($"notification_{notification.Id}");
        if (cachedNotification != null)
        {
            await _redisCacheService.SetDataAsync($"notification_{notification.Id}", notification);
        }
        string jobId = _backgroundJobClient.Enqueue(() => ProcessNotificationAsync(notification.Id));
        Console.WriteLine($"[Hangfire] Создана задача с JobId: {jobId}");
    }

    public async Task ProcessNotificationAsync(Guid notificationId)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId);
        if (notification == null)
        {
            Console.WriteLine($"[ERROR] Уведомление с ID {notificationId} не найдено.");
            return;
        }
        var analysisResult = await _aiAnalysisService.AnalyzeTextResult(notification.Message);
        notification.Category = analysisResult.Category;
        notification.Confidence = analysisResult.Confidence;
        notification.ProcessingStatus = "completed";
        await _notificationRepository.UpdateAsync(notification);
        
        await _redisCacheService.SetDataAsync($"notification_{notification.Id}", notification);
    }
    public async Task<List<Notification>> GetNotificationsAsync(NotificationFilter filter,PageParams param)
    {
        string cacheKey = $"notifications_page{param.Page}_size{param.PageSize}";
        
        var notifications = await _redisCacheService.GetDataAsync<List<Notification>>(cacheKey);
        if (notifications != null)
            return notifications;
        
        notifications = await _notificationRepository.GetAllAsync(filter,param);
        await _redisCacheService.SetDataAsync(cacheKey, notifications);
        
        return notifications;
    }
    public async Task<Notification?> GetNotificationByIdAsync(Guid id)
    {
        var cachedNotifications = await _redisCacheService.GetDataAsync<Notification>($"notifications_{id}");
        if (cachedNotifications != null)
            return cachedNotifications;
        
        var notification = await _notificationRepository.GetByIdAsync(id);
        
        if (notification != null)
            await _redisCacheService.SetDataAsync($"notification_{id}", notification);
        
        return notification;
    }

    public async Task MarkAsReadAsync(Guid id)
    {
        await _notificationRepository.MarkAsReadAsync(id);
        var notification = await _notificationRepository.GetByIdAsync(id);
        if (notification != null)
            await _redisCacheService.SetDataAsync($"notification_{id}", notification);
    }
}