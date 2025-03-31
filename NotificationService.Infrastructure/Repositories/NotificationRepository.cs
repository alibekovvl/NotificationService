﻿using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Filters;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Data;
using NotificationService.Infrastructure.Filtration;

namespace NotificationService.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;

    public NotificationRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();
    }

    public  Task <List<Notification>> GetUnreadNotificationsAsync(string userId)
    {
        return _context.Notifications
            .Where(s => !s.IsRead && s.RecipientEmail == userId)
            .ToListAsync();
    }

    public async Task MarkAsReadAsync(Guid id)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification != null)
        {
            notification.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }
    public async Task<List<Notification>> GetAllAsync(NotificationFilter filter, PageParams param )
    {
        return await _context.Notifications
            .Filter(filter)
            .Page(param)
            .ToListAsync();
    }
    public async Task<Notification?> GetByIdAsync(Guid id)
    {
       return await _context.Notifications.FindAsync(id);
    }
    
   
}