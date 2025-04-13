using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Filters;
using NotificationService.Application.Services;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Extentions;

namespace NotificationService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly IRedisCacheService _redisCacheService;
    public NotificationController(INotificationService notificationService, IRedisCacheService redisCacheService)
    {
        _notificationService = notificationService;
        _redisCacheService = redisCacheService;
    }
    [HttpGet]
    public async Task<IActionResult> GetNotifications([FromQuery] NotificationFilter filter, [FromQuery] PageParams param)
    {
        string cacheKey = CacheKeyGenerator.Generator(filter, param);
        var notifications = await _redisCacheService.GetOrSetDataAsync(
            cacheKey,
            () => _notificationService.GetNotificationsAsync(filter, param));
        return Ok(notifications);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetNotificationById(Guid id)
    {
        var notification = await _redisCacheService.GetOrSetDataAsync(
            $"notification_{id}",
            () => _notificationService.GetNotificationByIdAsync(id));
        if (notification == null)
            return NotFound();
        return Ok(notification);
    }
    [HttpGet("status/{taskId}")]
    public IActionResult GetJobStatus(string taskId)
    {
        var jobDetails = Hangfire.JobStorage.Current.GetConnection().GetJobData(taskId);

        if (jobDetails == null)
            return NotFound(new { message = "Job not found" });
        
        return Ok(new
        {
            jobId = taskId,
            status = jobDetails.State
        });
    }
    [HttpPost]
    public async Task <IActionResult> CreateNotification([FromBody] NotificationDTOs notificationDto, [FromServices] IAIAnalysisService aiAnalysisService)
    {   
        if (notificationDto == null || string.IsNullOrWhiteSpace(notificationDto.Message))
            return BadRequest("Notification or message cannot be null.");
        var notification = new Notification
        {
            Id = Guid.NewGuid(), 
            Title = notificationDto.Title,
            Message = notificationDto.Message,
            RecipientEmail = notificationDto.RecipientEmail,
            CreatedAt = DateTime.UtcNow,  
            ProcessingStatus = "processing"
        };
        var analysisResult = await aiAnalysisService.AnalyzeTextResult(notification.Message);
        
        await _notificationService.SendNotificationAsync(notificationDto);
        return Ok(new{message = "Notification sent", category = notification.Category});
    }
    [HttpPost("MarkAsRead/{id}")]
    public async Task<IActionResult> MarkAsReadById(Guid id)
    {
        await _notificationService.MarkAsReadAsync(id);
        return Ok(new{message = "Notification is read"});
    }
}