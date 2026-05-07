using StackExchange.Redis;

namespace WebApplication1.Extensions
{
    public static class CacheExtensions
    {
        public static IServiceCollection AddCacheServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<IConnectionMultiplexer?>(sp =>
            {
                var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("Redis");

                var configuration = config["Redis:ConnectionString"];

                if (string.IsNullOrEmpty(configuration))
                {
                    logger.LogWarning("Redis connection string is missing. Running without Redis.");
                    return null;
                }

                var options = ConfigurationOptions.Parse(configuration);

                options.AbortOnConnectFail = false;
                options.ConnectRetry = 5;
                options.ConnectTimeout = 5000;
                options.SyncTimeout = 5000;
                options.ReconnectRetryPolicy = new ExponentialRetry(5000);

                var retries = 3;

                for (int i = 1; i <= retries; i++)
                {
                    try
                    {
                        logger.LogInformation("Connecting to Redis (Attempt {Attempt})", i);

                        var connection = ConnectionMultiplexer.Connect(options);

                        logger.LogInformation("Connected to Redis successfully");
                        return connection;
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Redis connection failed (Attempt {Attempt})", i);

                        if (i == retries)
                        {
                            logger.LogError("Redis unavailable. Continuing without cache.");
                            return null; // ✅ DO NOT crash
                        }

                        Thread.Sleep(3000);
                    }
                }

                return null;
            });

            return services;
        }
    }
}
