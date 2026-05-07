using Hangfire;
using Hangfire.SqlServer;

namespace WebApplication1.Extensions
{
    public static class HangfireExtensions
    {
        public static IServiceCollection AddHangfireServices(
        this IServiceCollection services,
        IConfiguration config)
        {
            services.AddHangfire(x =>
            {
                x.UseSqlServerStorage(
                    config.GetConnectionString("DefaultConnection"),
                    new SqlServerStorageOptions
                    {
                        PrepareSchemaIfNecessary = false,
                        TryAutoDetectSchemaDependentOptions = false,
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    });
            });

            services.AddHangfireServer();

            return services;
        }
    }
}
