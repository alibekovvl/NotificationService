using NotificationService.Domain.Entities;

namespace NotificationService.Application.Interfaces;

public interface IAiAnalysisService
{
    Task<AiResultEntity> AnalyzeTextResult(string text);
}