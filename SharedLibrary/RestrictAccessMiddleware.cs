using Microsoft.AspNetCore.Http;

namespace SharedLibrary
{
    public class RestrictAccessMiddleware
    {
        private readonly RequestDelegate _next;

        public RestrictAccessMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var referrer = context.Request.Headers["Referer"].FirstOrDefault();

            if (string.IsNullOrEmpty(referrer))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Access denied");
                return;
            }
            
            await _next(context);
        }
    }
}
