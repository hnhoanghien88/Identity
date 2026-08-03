using FluentValidation;
using Identity.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Middleware;

public sealed class ExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = exception switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                ValidationException => StatusCodes.Status400BadRequest,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                ArgumentException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = context.Response.StatusCode,
                Title = exception switch
                {
                    NotFoundException => "Not found",
                    ValidationException => "Validation failed",
                    _ => "Request failed"
                },
                Detail = exception.Message
            });
        }
    }
}

