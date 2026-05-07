using Hangfire.Dashboard;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using WebApplication1.Settings;

namespace WebApplication1.Infrastructure.Hangfire
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly RoleSettings _roleSettings;

        public HangfireAuthorizationFilter(IOptions<RoleSettings> roleSettings)
        {
            _roleSettings = roleSettings.Value;
        }

        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            var isAuthenticated = httpContext.User.Identity?.IsAuthenticated == true;

            var roleClaim = httpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            return isAuthenticated &&
                   roleClaim != null &&
                   roleClaim == _roleSettings.Admin;
        }
    }
}
