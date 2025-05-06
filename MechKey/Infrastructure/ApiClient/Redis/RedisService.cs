using Application.Interfaces.IApiClient.Redis;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;

namespace Infrastructure.ApiClient.Redis
{
    public class RedisService : IRedisService
    {
        private readonly IDistributedCache _cache;
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisService(IDistributedCache cache, IConnectionMultiplexer connectionMultiplexer)
        {
            _cache = cache;
            _connectionMultiplexer = connectionMultiplexer;
        }

        public async Task ClearByPatternAsync(string key)
        {
            await _cache.RemoveAsync(key);
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
            await _cache?.SetStringAsync(key, data, options);
        }

        // References: https://stackoverflow.com/a/60385140
        public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                throw new ArgumentException("Prefix cannot be null or whitespace.", nameof(prefix));

            if (_connectionMultiplexer != null)
            {
                var keys = new List<string>();

                foreach (var endpoint in _connectionMultiplexer.GetEndPoints())
                {
                    var server = _connectionMultiplexer.GetServer(endpoint);

                    await foreach (var key in server.KeysAsync(pattern: $"{prefix}*").WithCancellation(cancellationToken))
                    {
                        keys.Add(key.ToString());
                    }
                }

                await Task.WhenAll(keys.Select(k => _cache.RemoveAsync(k, cancellationToken)));
            }
            else
            {
                throw new ArgumentException("Missing redis server or redis is not supported", nameof(_connectionMultiplexer));
            }
        }
    }
}
