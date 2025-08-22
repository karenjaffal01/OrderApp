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
            var path = context.Request.Path.Value?.ToLower();

            if (path != null && (path.StartsWith("/swagger") || path.StartsWith("/health")))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue("Referer", out var referer) ||
                referer != "Api-Gateway")
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden: request must go through API Gateway");
                return;
            }

            await _next(context);
        }
    }
}
