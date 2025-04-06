using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Application.Services;
public class MockAIAnalysisService : IAIAnalysisService
{
    private static readonly Random _random = new();
    public async Task<Notification> AnalyzeTextResult(string text)
    {
        await Task.Delay(_random.Next(1000, 3000));

        string category;
        double confidence;
        
        if (text.Contains("error", StringComparison.OrdinalIgnoreCase) ||
            text.Contains("exception", StringComparison.OrdinalIgnoreCase) ||
            text.Contains("failed", StringComparison.OrdinalIgnoreCase))
        {
            category = "critical";
            confidence = _random.NextDouble() * (0.95 - 0.7) + 0.7;
        }
        else if (text.Contains("warning", StringComparison.OrdinalIgnoreCase) ||
                 text.Contains("attention", StringComparison.OrdinalIgnoreCase) ||
                 text.Contains("careful", StringComparison.OrdinalIgnoreCase))
        {
            category = "warning";
            confidence = _random.NextDouble() * (0.9 - 0.6) + 0.6;
        }
        else
        {
            category = "info";
            confidence = _random.NextDouble() * (0.99 - 0.8) + 0.8;
        }
        return new Notification()
        {
            Category = category,
            Confidence = confidence,
            ProcessingStatus = "completed"
        };
    }
}