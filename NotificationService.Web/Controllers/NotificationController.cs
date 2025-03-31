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
    
    [HttpPost]
    public async Task <IActionResult> CreateNotification([FromBody] Notification notification)
    {
        await _notificationService.SendNotificationAsync(notification);
        return Ok("Notification sent");
    }
}