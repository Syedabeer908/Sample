using System.Collections.Concurrent;
using WebApplication1.Common.Responses;
using WebApplication1.Common.Results;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApplication1.Middlewares
{
    public class IpRateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ErrorHelper _errorHelper;
        private static readonly ConcurrentDictionary<string, RequestCounter> _requests = new();

        private const int LIMIT = 100;
        private static readonly TimeSpan WINDOW = TimeSpan.FromMinutes(1);

        public IpRateLimitingMiddleware(RequestDelegate next)
        {
            _next = next;
            _errorHelper = new ErrorHelper();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var counter = _requests.GetOrAdd(ip, _ => new RequestCounter());

            lock (counter)
            {
                if (DateTime.UtcNow > counter.WindowStart.Add(WINDOW))
                {
                    counter.WindowStart = DateTime.UtcNow;
                    counter.Count = 0;
                }

                counter.Count++;

                if (counter.Count > LIMIT)
                {
                    ThrottledResponse(context, "Too many requests");
                    return;
                }
            }

            await _next(context);
        }

        private class RequestCounter
        {
            public int Count;
            public DateTime WindowStart = DateTime.UtcNow;
        }

        private Task ThrottledResponse(HttpContext context, string message)
        {
            var error = _errorHelper.CreateErrors("throttled", message);
            return ErrorResponseWriter.WriteErrorAsync(context, 429, error);
        }
    }
}
