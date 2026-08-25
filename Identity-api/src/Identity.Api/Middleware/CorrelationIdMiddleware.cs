using System.Diagnostics;
using Serilog.Context;

namespace Identity.Api.Middleware;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    public const string HeaderName = "X-Correlation-ID";
    private const int MaxCorrelationIdLength = 128;

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrCreateCorrelationId(context);
        var traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;

        context.TraceIdentifier = correlationId;
        context.Response.Headers[HeaderName] = correlationId;

        using (LogContext.PushProperty("CorrelationId", correlationId))
        using (LogContext.PushProperty("TraceId", traceId))
        {
            await next(context);
        }
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        var candidate = context.Request.Headers[HeaderName].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(candidate)
            && candidate.Length <= MaxCorrelationIdLength
            && candidate.All(IsAllowedCharacter))
        {
            return candidate;
        }

        return Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString("N");
    }

    private static bool IsAllowedCharacter(char character) =>
        char.IsAsciiLetterOrDigit(character) || character is '-' or '_' or '.' or ':';
}
