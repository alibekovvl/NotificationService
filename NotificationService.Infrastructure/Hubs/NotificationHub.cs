using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;


namespace NotificationService.Infrastructure.Hubs;

public class NotificationHub : Hub<INotificationClient>
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        await Clients.All.ConnectionEstablished();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
public interface INotificationClient
{
    Task ReceiveNotification(NotificationDTOs notificationDto);
    Task ConnectionEstablished();
}