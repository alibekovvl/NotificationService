namespace NotificationService.Domain.Entities;

public class AIAlanysisResult
{
    public string Category { get; set; } = "info";
    
    public double Confidence { get; set; }
    
    public string ProcessingStatus { get; set; } = "pending";
}