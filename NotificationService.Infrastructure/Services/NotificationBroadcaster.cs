using Microsoft.AspNetCore.SignalR;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;
using NotificationService.Infrastructure.Hubs;

namespace NotificationService.Infrastructure.Services;

public class NotificationBroadcaster : INotificationBroadcaster
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationBroadcaster(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task BroadcastNotificationAsync(Notification notification)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveNotification",notification);
    }
}