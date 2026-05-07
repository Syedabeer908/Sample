using WebApplication1.Entities;
using WebApplication1.Interfaces;
using WebApplication1.Repository;

namespace WebApplication1.Extensions
{
    public static class RepositoryExtensions
    {
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProviderRepository, ProviderRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRepository<Risk>, RiskRepository>();
            services.AddScoped<IRepository<Control>, ControlRepository>();
            services.AddScoped<IRepository<RiskControl>, RiskControlRepository>();
            services.AddScoped<ISoftRepository, SoftRepository>();
            services.AddScoped<IUserLoginHistoryRepository, UserLoginHistoryRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            
            return services;
        }
    }
}
