using NotificationService.Application.Filters;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Application.Services;

public class NotificationAppService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IAIAnalysisService _aiAnalysisService;

    public NotificationAppService(INotificationRepository notificationRepository, IAIAnalysisService aiAnalysisService)
    {
        _notificationRepository = notificationRepository;
        _aiAnalysisService = aiAnalysisService;
    }
    public async Task SendNotificationAsync(Notification notification)
    {
        notification.ProcessingStatus = "processing";
        await _notificationRepository.AddAsync(notification);
        
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