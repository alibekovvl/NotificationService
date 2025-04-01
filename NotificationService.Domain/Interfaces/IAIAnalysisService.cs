using NotificationService.Domain.Entities;

namespace NotificationService.Domain.Interfaces;

public interface IAIAnalysisService
{
    Task<AIAlanysisResult> AnalyzeTextResult(string text);
}