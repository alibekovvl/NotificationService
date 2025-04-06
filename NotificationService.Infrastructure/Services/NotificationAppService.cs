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
    public NotificationAppService(INotificationRepository notificationRepository, IAIAnalysisService aiAnalysisService, IBackgroundJobClient backgroundJobClient)
    {
        _notificationRepository = notificationRepository;
        _aiAnalysisService = aiAnalysisService;
        _backgroundJobClient = backgroundJobClient;
    }
    public async Task SendNotificationAsync(Notification notification)
    {
        notification.ProcessingStatus = "processing";
        await _notificationRepository.AddAsync(notification);
        
        string jobId = _backgroundJobClient.Enqueue(() => ProcessNotificationAsync(notification.Id));
        
        Console.WriteLine($"[Hangfire] Создана задача с JobId: {jobId}");
    }

    public async Task ProcessNotificationAsync(Guid notificationId)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId);
        var analysisResult = await _aiAnalysisService.AnalyzeTextResult(notification.Message);
        notification.Category = analysisResult.Category;
        notification.Confidence = analysisResult.Confidence;
        notification.ProcessingStatus = "completed";
        await _notificationRepository.UpdateAsync(notification);
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