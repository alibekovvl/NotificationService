using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NotificationService.Application.Filters;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Filtration;

public static class NotificationExtentions
{
    public static IQueryable<Notification> Page(this IQueryable<Notification> query, PageParams param)
    {
        var page = param.Page ?? 1;
        var pageSize = param.PageSize ?? 10;
        
        var skip =(page - 1) * pageSize;
        return query.Skip(skip).Take(pageSize);
    }
    public static IQueryable<Notification> Filter(this IQueryable<Notification> query, NotificationFilter filter)
    {
        if (!string.IsNullOrEmpty(filter.Title))
            query = query.Where(n => n.Title.Contains(filter.Title));
        if (!string.IsNullOrEmpty(filter.Message))
            query = query.Where(n => n.Message.Contains(filter.Message));
        if (!string.IsNullOrEmpty(filter.RecipientEmail))
            query = query.Where(n => n.RecipientEmail.Contains(filter.RecipientEmail));
        
        return query;
    }
}