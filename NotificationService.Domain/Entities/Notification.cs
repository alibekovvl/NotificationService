using System.ComponentModel.DataAnnotations;

namespace NotificationService.Domain.Entities;

public class Notification
{
    [Key]
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public string RecipientEmail { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; }
    
    
    public string Category { get; set; } = "info";
    
    public double Confidence { get; set; }
    
    public string ProcessingStatus { get; set; } = "pending";
}