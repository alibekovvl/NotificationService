namespace NotificationService.Domain.Entities;

public class AiResultEntity
{
    public string Category { get; set; } = "info";
    public double Confidence { get; set; }
    public string ProcessingStatus { get; set; } = "pending";
}