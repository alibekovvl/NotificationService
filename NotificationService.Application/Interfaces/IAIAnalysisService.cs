using MockService.Entities;

namespace MockService.Services;

public interface IAiAnalysisService
{
    Task<AiResultEntity> AnalyzeTextResult(string text);
}