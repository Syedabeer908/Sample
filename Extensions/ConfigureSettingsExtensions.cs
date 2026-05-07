using WebApplication1.Common.Constants;
using WebApplication1.Settings;

namespace WebApplication1.Extensions
{
    public static class ConfigureSettingsExtensions
    {
        public static IServiceCollection AddConfigurationService(this IServiceCollection services, IConfiguration config)
        {
            // Adding AuthConstants
            services.Configure<AuthSettings>(config.GetSection("SecretKeys:Jwt"));

            // Adding RoleConstants
            services.Configure<RoleSettings>(config.GetSection("StartUpRole"));

            // Adding FileSettings
            services.Configure<FileSettings>(config.GetSection("FileStorage"));

            // Adding EmailSettings
            services.Configure<EmailSettings>(config.GetSection("EmailSettings"));

            // Adding GoogleSettings
            services.Configure<GoogleSettings>(config.GetSection("GoogleOAuth"));

            return services;
        }
    }
}
