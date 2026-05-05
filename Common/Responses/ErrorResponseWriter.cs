using System.Text.Json;
using WebApplication1.Common.Results;

namespace WebApplication1.Common.Responses
{
    public class ErrorResponseWriter
    {
        public static async Task WriteErrorAsync(
        HttpContext context,
        int statusCode,
        List<Error> errors)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var result = Result.Failure(statusCode, errors);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(result, options));
        }
    }
}
