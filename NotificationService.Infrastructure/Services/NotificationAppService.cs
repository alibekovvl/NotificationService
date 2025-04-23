using AutoMapper;
using Hangfire;
using Microsoft.Extensions.Caching.Memory;
using MockService.Services;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;
using NotificationService.Domain.FiltersSortPaginations;
using NotificationService.Infrastructure.Extentions;
using NotificationService.Infrastructure.Services.Caching;


namespace NotificationService.Infrastructure.Services;

public class NotificationAppService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IAiAnalysisService _aiAnalysisService;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IRedisCacheService _redisCacheService;
    private readonly IMapper _mapper;
    public NotificationAppService(
        INotificationRepository notificationRepository, 
        IAiAnalysisService aiAnalysisService, 
        IBackgroundJobClient backgroundJobClient,
        IRedisCacheService redisCacheService,
        IMapper mapper)
    {
        _notificationRepository = notificationRepository;
        _aiAnalysisService = aiAnalysisService;
        _backgroundJobClient = backgroundJobClient;
        _redisCacheService = redisCacheService;
        _mapper = mapper;
    }
    public async Task SendNotificationAsync(NotificationDTOs notificationDto)
    {
        var notification = _mapper.Map<Notification>(notificationDto);
        await _notificationRepository.AddAsync(notification);
        await _redisCacheService.RemoveByPrefixAsync("notification_page");
        
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
        string cacheKey = CacheGenerator.Generator(filter, param);
        var notifications = await _redisCacheService.GetDataAsync<List<Notification>>(cacheKey);
        
        if (notifications != null)
        {
            return notifications; 
        }
        notifications = await _notificationRepository.GetAllAsync(filter,param);   
        
        await _redisCacheService.SetDataAsync(cacheKey, notifications);
        return notifications;
    }
    public async Task<Notification?> GetNotificationByIdAsync(Guid id)
    {
        var notification = await _notificationRepository.GetByIdAsync(id);
        return notification;
    }
    public async Task MarkAsReadAsync(Guid id)
    {
        await _notificationRepository.MarkAsReadAsync(id);
    }
}