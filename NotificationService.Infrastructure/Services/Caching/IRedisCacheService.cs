namespace NotificationService.Infrastructure.Services.Caching;

public interface IRedisCacheService
{
    Task<T?> GetDataAsync <T>(string key);
    Task SetDataAsync <T>(string key, T value);
    Task<T> GetOrSetDataAsync<T>(string key, Func<Task<T>> fetchData);
    Task RemoveAsync(string key);
    Task RemoveByPrefixAsync(string prefix);
}