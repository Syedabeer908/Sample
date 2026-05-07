using WebApplication1.Infrastructure.Hangfire;
using WebApplication1.Jobs;
using WebApplication1.Services;

namespace WebApplication1.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            services.AddScoped<AuthService>();
            services.AddScoped<UserDomainService>();
            services.AddScoped<UserService>();
            services.AddScoped<AdminService>();
            services.AddScoped<RoleService>();
            services.AddScoped<RiskService>();
            services.AddScoped<ControlService>();
            services.AddScoped<RiskControlService>();
            services.AddScoped<RedisService>();
            services.AddScoped<ProfileService>();
            services.AddScoped<EmailService>();
            services.AddScoped<FileService>();
            services.AddScoped<ProviderService>();
            services.AddScoped<GoogleService>();
            services.AddScoped<AuthBackgroundJobs>();

            services.AddSingleton<HangfireAuthorizationFilter>();

            return services;
        }
    }
}
