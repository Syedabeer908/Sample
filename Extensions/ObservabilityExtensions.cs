using Serilog;
using WebApplication1.Infrastructure.HealthChecks;

namespace WebApplication1.Extensions
{
    public static class ObservabilityExtensions
    {
        public static IServiceCollection AddObservabilityServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddHealthCheck(config);
            return services;
        }
        
        public static IServiceCollection AddHealthCheck(this IServiceCollection services, IConfiguration config)
        {
            services.AddHealthChecks()
                .AddSqlServer(
                    config.GetConnectionString("DefaultConnection"),
                    tags: new[] { "sql" }
                )
                .AddCheck<RedisLatencyHealthCheck>(
                    "redis",
                    tags: new[] { "redis" }
                );

            return services;
        }
        
        public static IHostBuilder AddSerilogLogging(this IHostBuilder host)
        {
            return host.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.File(
                        "Logs/log-.txt",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 7
                    );
            });
        }
    }
}
