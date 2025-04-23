using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MockService.Services;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;
using NotificationService.Domain.FiltersSortPaginations;
using NotificationService.Infrastructure.Extentions;
using NotificationService.Infrastructure.Services.Caching;

namespace NotificationService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly IRedisCacheService _redisCacheService;
    private readonly IMapper _mapper;
    public NotificationController(INotificationService notificationService, IRedisCacheService redisCacheService, IMapper mapper)
    {
        _notificationService = notificationService;
        _redisCacheService = redisCacheService;
        _mapper = mapper;
    }
    [HttpGet]
    public async Task<IActionResult> GetNotifications([FromQuery] NotificationFilter filter, [FromQuery] PageParams param)
    {
        var notifications = await _notificationService.GetNotificationsAsync(filter, param);
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
    public async Task <IActionResult> CreateNotification([FromBody] NotificationDTOs notificationDto, [FromServices] IAiAnalysisService aiAnalysisService)
    {   
        var notification = _mapper.Map<Notification>(notificationDto);
        await _notificationService.SendNotificationAsync(notificationDto);
        return Created();
    }
    [HttpPatch("MarkAsRead/{id}")]
    public async Task<IActionResult> MarkAsReadById(Guid id)
    {
        await _notificationService.MarkAsReadAsync(id);
        return Ok(new{message = "Notification is read"});
    }
}