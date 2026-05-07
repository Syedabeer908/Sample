using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace WebApplication1.Infrastructure.HealthChecks
{
    public class RedisLatencyHealthCheck : IHealthCheck
    {
        private readonly IConnectionMultiplexer? _redis;

        public RedisLatencyHealthCheck(IConnectionMultiplexer? redis)
        {
            _redis = redis;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            if (_redis == null)
            {
                return HealthCheckResult.Unhealthy("Redis not configured or unavailable");
            }

            try
            {
                var db = _redis.GetDatabase();

                var latency = await db.PingAsync();
                var latencyMs = latency.TotalMilliseconds;

                var key = $"healthcheck:{Environment.MachineName}:{Guid.NewGuid()}";
                await db.StringSetAsync(key, "ok", TimeSpan.FromSeconds(5));
                var value = await db.StringGetAsync(key);

                if (value != "ok")
                    return HealthCheckResult.Unhealthy("Redis read/write failed");

                if (latencyMs > 200)
                    return HealthCheckResult.Degraded($"Redis slow: {latencyMs}ms");

                return HealthCheckResult.Healthy($"Redis OK: {latencyMs}ms");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Redis failed", ex);
            }
        }
    }
}
