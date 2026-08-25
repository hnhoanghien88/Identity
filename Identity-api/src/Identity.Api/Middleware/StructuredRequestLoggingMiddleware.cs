using System.Diagnostics;

namespace Identity.Api.Middleware;

public sealed class StructuredRequestLoggingMiddleware(
    RequestDelegate next,
    ILogger<StructuredRequestLoggingMiddleware> logger)
{
    private const double SlowRequestThresholdMs = 3000;

    public async Task InvokeAsync(HttpContext context)
    {
        var startedAt = Stopwatch.GetTimestamp();

        await next(context);

        var elapsedMs = Stopwatch.GetElapsedTime(startedAt).TotalMilliseconds;
        var userId = context.User.FindFirst("uid")?.Value;
        var statusCode = context.Response.StatusCode;

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(
                "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {ElapsedMs:F3} ms for user {UserId}",
                context.Request.Method,
                context.Request.Path,
                statusCode,
                elapsedMs,
                userId);
        }
        else if (elapsedMs > SlowRequestThresholdMs)
        {
            logger.LogWarning(
                "Slow HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {ElapsedMs:F3} ms for user {UserId}",
                context.Request.Method,
                context.Request.Path,
                statusCode,
                elapsedMs,
                userId);
        }
        else
        {
            logger.LogInformation(
                "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {ElapsedMs:F3} ms for user {UserId}",
                context.Request.Method,
                context.Request.Path,
                statusCode,
                elapsedMs,
                userId);
        }
    }
}
