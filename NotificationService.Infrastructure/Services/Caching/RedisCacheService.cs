using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace NotificationService.Application.Services;
public class RedisCacheService : IRedisCacheService
{
    private readonly IDistributedCache _cache;
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(1);

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }
    public async Task <T?> GetDataAsync <T>(string key)
    {
        var data = await _cache.GetStringAsync(key);
        if (string.IsNullOrEmpty(data))
        {
            Console.WriteLine($"[CACHE MISS] Key '{key}' not found in Redis.");
            return default;
        }
        Console.WriteLine($"[CACHE HIT] Key '{key}' found in Redis.");
        return JsonSerializer.Deserialize<T>(data);
    }
    
    public async Task SetDataAsync<T>(string key, T value)
    {
        var options = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = _defaultExpiration
        };
        var serializedData = JsonSerializer.Serialize(value);
        Console.WriteLine($"[REDIS]SetDataAsync: {key}, Data = {serializedData}");
        
        await _cache.SetStringAsync(key, serializedData, options);
    }

    public async Task<T> GetOrSetDataAsync<T>(string key, Func<Task<T>> fetchData)
    {
        var cachedData = await GetDataAsync<T>(key);
        if (cachedData != null) 
            return cachedData;
        var data = await fetchData();
        await SetDataAsync<T>(key, data);
        return data;
    }
}









