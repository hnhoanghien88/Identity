using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Authorization;

public static class ProblemDetailsAuthorizationResults
{
    public static async Task WriteUnauthorizedAsync(HttpContext context)
    {
        if (context.Response.HasStarted) return;
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = "Authentication is required or the access token is invalid.",
                Extensions = { ["correlationId"] = context.TraceIdentifier },
            });
    }

    public static async Task WriteForbiddenAsync(HttpContext context)
    {
        if (context.Response.HasStarted) return;
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Title = "Forbidden",
                Detail = "You do not have permission to perform this action.",
                Extensions = { ["correlationId"] = context.TraceIdentifier },
            });
    }
}
