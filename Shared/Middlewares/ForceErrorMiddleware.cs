using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Text.Json;
using WebApplication1.Shared.Results;

namespace WebApplication1.Shared.Middlewares
{
    public class ForceErrorMiddleware
    {
        private readonly RequestDelegate _next;

        public ForceErrorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Only intercept API requests
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                var errorResponse = ApiResponse<object>.Fail("Forced error from middleware for testing purposes.");
                await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
                return; // Short-circuit the pipeline
            }

            await _next(context);
        }
    }

    public static class ForceErrorMiddlewareExtensions
    {
        public static IApplicationBuilder UseForceError(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ForceErrorMiddleware>();
        }
    }
}