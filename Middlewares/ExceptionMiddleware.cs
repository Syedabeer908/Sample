using WebApplication1.Common.Results;
using WebApplication1.Common.Exceptions;
using WebApplication1.Common.Responses;

namespace WebApplication1.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ErrorHelper _errorHelper;
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
            _errorHelper = new ErrorHelper();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                await throwException(context, 404, ex.Message);
            }
            catch (BadRequestException ex)
            {
                await throwException(context, 400, ex.Message);
            }
            catch (ForbiddenException ex)
            {
                await throwException(context, 403, ex.Message);
            }
            catch (Exception ex)
            {
                await throwException(context, 500, $"Internal server error: {ex.ToString()}");
            }
        }

        private async Task throwException(HttpContext context, int statusCode, string message)
        {
            var error = _errorHelper.CreateErrors("Exception", message);
            await ErrorResponseWriter.WriteErrorAsync(context, statusCode, error);        
        } 
    }
}
