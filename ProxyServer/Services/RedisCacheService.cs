using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace ProxyServer.Services
{
    public class RedisCacheService
    {
        private readonly IDatabase? _db;
        private readonly ILogger<RedisCacheService> _logger;
        private readonly ConnectionMultiplexer? _mux;

        public RedisCacheService(ILogger<RedisCacheService> logger)
        {
            _logger = logger;

            var options = new ConfigurationOptions
            {
                EndPoints = { "redis:6379" },
                AbortOnConnectFail = false,
                ConnectRetry = 5,
                ConnectTimeout = 5000,
                SyncTimeout = 5000
            };

            int attempts = 5;
            int delayMs = 2000;
            ConnectionMultiplexer? mux = null;

            for (int i = 0; i < attempts; i++)
            {
                try
                {
                    mux = ConnectionMultiplexer.Connect(options);
                    if (mux != null && mux.IsConnected)
                    {
                        _logger.LogInformation("Connected to Redis successfully on attempt {Attempt}.", i + 1);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Attempt {Attempt} to connect to Redis failed.", i + 1);
                }

                Thread.Sleep(delayMs);
            }

            if (mux == null)
            {
                try
                {
                    mux = ConnectionMultiplexer.Connect(options);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Final attempt to connect to Redis failed. Continuing without cache.");
                }
            }

            _mux = mux;
            _db = _mux?.GetDatabase();
        }

        public async Task SetAsync(string key, object value, int ttlSeconds = 60)
        {
            if (_db == null)
            {
                _logger.LogWarning("Redis DB NOT available. SET skipped → {Key}", key);
                return;
            }

            var json = JsonSerializer.Serialize(value);
            await _db.StringSetAsync(key, json, TimeSpan.FromSeconds(ttlSeconds));

            _logger.LogInformation("CACHE SET → {Key} (TTL {TTL}s)", key, ttlSeconds);
        }


        public async Task<T?> GetAsync<T>(string key)
        {
            if (_db == null)
            {
                _logger.LogWarning("Redis DB NOT available. GET returns default → {Key}", key);
                return default;
            }

            var val = await _db.StringGetAsync(key);

            if (val.IsNullOrEmpty)
            {
                _logger.LogInformation("CACHE MISS → {Key}", key);
                return default;
            }

            _logger.LogInformation("CACHE HIT → {Key}", key);
            return JsonSerializer.Deserialize<T>(val!);
        }
    }
}
