using StackExchange.Redis;

namespace WebApplication1.Services
{
    public class RedisService
    {
        private readonly IConnectionMultiplexer? _redis;
        private readonly IDatabase? _db;
        private readonly TimeSpan _defaultTime;
        private readonly ILogger<RedisService> _logger;

        public RedisService(
            IConnectionMultiplexer? redis,
            ILogger<RedisService> logger)
        {
            _redis = redis;
            _db = redis?.GetDatabase(); 
            _defaultTime = TimeSpan.FromHours(1);
            _logger = logger;
        }

        private static string CreateKey(string prefix, string keyId)
            => $"{prefix}:{keyId}";

        public async Task SetAsync(string prefix, string keyId, string value, TimeSpan? expiry = null)
        {
            if (_db == null)
            {
                _logger.LogDebug("Redis unavailable. Skipping SET for {Key}", $"{prefix}:{keyId}");
                return;
            }

            try
            {
                var key = CreateKey(prefix, keyId);
                var expireTime = expiry ?? _defaultTime;

                await _db.StringSetAsync(key, value, expireTime);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis SET failed for {Key}", $"{prefix}:{keyId}");
            }
        }

        public async Task<string?> GetAsync(string prefix, string keyId)
        {
            if (_db == null)
            {
                _logger.LogDebug("Redis unavailable. Skipping GET for {Key}", $"{prefix}:{keyId}");
                return null;
            }

            try
            {
                var key = CreateKey(prefix, keyId);
                return await _db.StringGetAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis GET failed for {Key}", $"{prefix}:{keyId}");
                return null;
            }
        }

        public async Task RemoveAsync(string prefix, string keyId)
        {
            if (_db == null)
            {
                _logger.LogDebug("Redis unavailable. Skipping DELETE for {Key}", $"{prefix}:{keyId}");
                return;
            }

            try
            {
                var key = CreateKey(prefix, keyId);
                await _db.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis DELETE failed for {Key}", $"{prefix}:{keyId}");
            }
        }

        public async Task<bool> ExistsAsync(string prefix, string keyId)
        {
            if (_db == null)
            {
                _logger.LogDebug("Redis unavailable. Skipping EXISTS for {Key}", $"{prefix}:{keyId}");
                return false;
            }

            try
            {
                var key = CreateKey(prefix, keyId);
                return await _db.KeyExistsAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis EXISTS failed for {Key}", $"{prefix}:{keyId}");
                return false;
            }
        }
    }
}