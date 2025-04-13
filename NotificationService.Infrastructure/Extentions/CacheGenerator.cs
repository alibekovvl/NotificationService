using NotificationService.Application.Filters;

namespace NotificationService.Infrastructure.Extentions;

public  class CacheKeyGenerator
{
    public static string Generator(NotificationFilter filter, PageParams param)
    {
        var parts = new List<string>
        {
            "notifications"
        };

        if (!string.IsNullOrWhiteSpace(filter.Title))
            parts.Add($"title:{filter.Title}");
        if (!string.IsNullOrWhiteSpace(filter.Message))
            parts.Add($"msg:{filter.Message}");
        if (!string.IsNullOrWhiteSpace(filter.RecipientEmail))
            parts.Add($"email:{filter.RecipientEmail}");

        parts.Add($"page:{param.Page ?? 1}");
        parts.Add($"size:{param.PageSize ?? 10}");

        return string.Join("_", parts);
    }
}
