using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace NotificationService.Infrastructure.Services.Caching;
public class RedisCacheService : IRedisCacheService
{
    private readonly IDistributedCache _cache;
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromSeconds(2);
    private readonly IConnectionMultiplexer _redis;
    public RedisCacheService(IDistributedCache cache, IConnectionMultiplexer redis)
    {
        _cache = cache;
        _redis = redis;
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
            AbsoluteExpirationRelativeToNow = _defaultExpiration,
            SlidingExpiration = null
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

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
    }

    public async Task RemoveByPrefixAsync(string prefix)
    {
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        var keys = server.Keys(pattern: $"{prefix}").ToArray();

        foreach (var key in keys)
        {
            Console.WriteLine($"[REDIS] Removing key: {key}");
            await _cache.RemoveAsync(key);
        }
    }
}









