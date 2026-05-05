using WebApplication1.Common.Exceptions;

namespace WebApplication1.Common.Extensions
{
    public static class HttpContextExtensions
    {
        public static Guid GetUserId(this HttpContext context)
        {
            if (context.Items["UserId"] is not Guid userId)
                throw new NotFoundException("UserId missing");

            return userId;
        }
    }
}
