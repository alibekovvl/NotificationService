using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Filters;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }
    [HttpGet]
    public async Task<IActionResult> GetNotifications([FromQuery] NotificationFilter filter, [FromQuery] PageParams param)
    {
        var notifications = await _notificationService.GetNotificationsAsync(filter,param);
        return Ok(notifications);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetNotificationById(Guid id)
    {
        var notifications = await _notificationService.GetNotificationByIdAsync(id);
        if (notifications == null)
            return NotFound();
        
        return Ok(notifications);
    }

    [HttpGet("status/{jobId}")]
    public async Task<IActionResult> GetJobStatus(string jobId)
    {
        var jobDetails = Hangfire.JobStorage.Current.GetConnection().GetJobData(jobId);

        if (jobDetails == null)
        {
            return NotFound(new { message = "Job not found" });
        }

        return Ok(new
        {
            jobId,
            status = jobDetails.State
        });

    }
    [HttpPost]
    public async Task <IActionResult> CreateNotification([FromBody] Notification notification, [FromServices] IAIAnalysisService aiAnalysisService)
    {   
        if (notification == null || string.IsNullOrWhiteSpace(notification.Message))
        {
            return BadRequest("Notification or message cannot be null.");
        }
        var analysisResult = await aiAnalysisService.AnalyzeTextResult(notification.Message);
        await _notificationService.SendNotificationAsync(notification);
        return Ok(new{message = "Notification sent", category = notification.Category});
    }
    [HttpPost("MarkAsRead/{id}")]
    public async Task<IActionResult> MarkAsReadById(Guid id)
    {
        await _notificationService.MarkAsReadAsync(id);
        return Ok(new{message = "Notification is read"});
    }
}