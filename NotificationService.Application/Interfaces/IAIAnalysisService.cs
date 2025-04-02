using NotificationService.Domain.Entities;

namespace NotificationService.Domain.Interfaces;

public interface IAIAnalysisService
{
    Task<Notification> AnalyzeTextResult(string text);
}