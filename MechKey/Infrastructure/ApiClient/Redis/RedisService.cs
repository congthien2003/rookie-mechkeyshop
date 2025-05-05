using Application.Interfaces.IApiClient.Redis;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace Infrastructure.ApiClient.Redis
{
    public class RedisService : IRedisService
    {
        private readonly IDistributedCache _cache;

        public RedisService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public T Get<T>(string key)
        {
            var data = _cache?.GetString(key);
            if (data is null)
            {
                return default(T);
            }
            return JsonSerializer.Deserialize<T>(data);
        }

        public void Remove(string key)
        {
            _cache?.Remove(key);
        }

        public async Task Set<T>(string key, T value, int minutes)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minutes)
            };
            var data = JsonSerializer.Serialize<T>(value);
            _cache?.SetStringAsync(key, data, options);
        }
    }
}
