using NotificationService.Domain.Entities;

namespace NotificationService.Domain.Interfaces;

public interface IAIAnalysisService
{
    Task<AiResultEntity> AnalyzeTextResult(string text);
}