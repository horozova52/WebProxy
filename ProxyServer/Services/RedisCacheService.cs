using StackExchange.Redis;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace ProxyServer.Services
{
    public class RedisCacheService
    {
        private readonly IDatabase _db;
        private readonly ILogger<RedisCacheService> _logger;
        private readonly ConnectionMultiplexer _mux;

        public RedisCacheService(ILogger<RedisCacheService> logger)
        {
            _logger = logger;

            var options = new ConfigurationOptions
            {
                EndPoints = { "redis:6379" },
                AbortOnConnectFail = false,    // permite reconectarea
                ConnectRetry = 5,              // încearcă de 5 ori
                ConnectTimeout = 5000,         // până la 5 secunde pe încercare
                SyncTimeout = 5000
            };

            try
            {
                _mux = ConnectionMultiplexer.Connect(options);
                _db = _mux.GetDatabase();

                _logger.LogInformation("Connected to Redis successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis is not ready yet! Will retry automatically.");
                _mux = ConnectionMultiplexer.Connect(options); // încearcă din nou automat
                _db = _mux.GetDatabase();
            }
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
