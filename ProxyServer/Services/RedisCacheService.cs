using StackExchange.Redis;
using System.Text.Json;

namespace ProxyServer.Services
{
    public class RedisCacheService
    {
        private readonly IDatabase _db;

        public RedisCacheService()
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            _db = redis.GetDatabase();
        }

        public async Task SetAsync(string key, object value, int ttlSeconds = 60)
        {
            var json = JsonSerializer.Serialize(value);
            await _db.StringSetAsync(key, json, TimeSpan.FromSeconds(ttlSeconds));
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var val = await _db.StringGetAsync(key);

            if (val.IsNullOrEmpty)
                return default;

            return JsonSerializer.Deserialize<T>(val!);
        }
    }
}
