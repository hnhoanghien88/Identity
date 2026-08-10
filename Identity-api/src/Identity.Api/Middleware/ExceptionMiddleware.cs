using FluentValidation;
using Identity.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Middleware;

public sealed class ExceptionMiddleware(
    RequestDelegate next,
    ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            if (exception is not NotFoundException
                and not ValidationException
                and not ConflictException
                and not UnauthorizedAccessException
                and not ArgumentException)
            {
                logger.LogError(exception, "Unhandled request failure for {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);
            }

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = exception switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                ValidationException => StatusCodes.Status400BadRequest,
                ConflictException => StatusCodes.Status409Conflict,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                ArgumentException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            if (exception is ValidationException validation)
            {
                var errors = validation.Errors
                    .GroupBy(x => string.IsNullOrWhiteSpace(x.PropertyName) ? "request" : char.ToLowerInvariant(x.PropertyName[0]) + x.PropertyName[1..])
                    .ToDictionary(x => x.Key, x => x.Select(e => e.ErrorMessage).ToArray());
                await context.Response.WriteAsJsonAsync(new ValidationProblemDetails(errors)
                {
                    Status = context.Response.StatusCode,
                    Title = "Validation failed"
                });
                return;
            }

            if (exception is ConflictException { Field: not null } conflict)
            {
                await context.Response.WriteAsJsonAsync(
                    new ValidationProblemDetails(
                        new Dictionary<string, string[]>
                        {
                            [conflict.Field] = [conflict.Message]
                        })
                    {
                        Status = context.Response.StatusCode,
                        Title = "Conflict"
                    });
                return;
            }

            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = context.Response.StatusCode,
                Title = exception switch
                {
                    NotFoundException => "Not found",
                    ConflictException => "Conflict",
                    _ => "Request failed"
                },
                Detail = exception is NotFoundException or ConflictException or UnauthorizedAccessException or ArgumentException
                    ? exception.Message
                    : "An unexpected error occurred."
            });
        }
    }
}
