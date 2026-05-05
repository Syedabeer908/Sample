using UAParser;

namespace WebApplication1.Common.Parsers
{
    public class Parser
    {
        public UserInfo GetIpAndDeviceInfo(HttpContext context)
        {
            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
             ?? context.Connection.RemoteIpAddress?.ToString();

            var userAgent = context.Request.Headers["User-Agent"].ToString();

            var uaParser = UAParser.Parser.GetDefault();
            var clientInfo = uaParser.Parse(userAgent);

            var deviceInfo = $"{clientInfo.UA.Family} on {clientInfo.OS.Family} ({clientInfo.Device.Family})";

            return new UserInfo
            {
                IpAddress = ip,
                DeviceInfo = deviceInfo,
                LoginTime = DateTime.UtcNow,
            };
        }
    }
}
