using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Orders.Api.Exceptions;

namespace Orders.Api.ErrorHandling;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title, type) = exception switch
        {
            InputValidationException => (StatusCodes.Status400BadRequest, "Validation error", "https://httpstatuses.com/400"),
            OrderNotFoundException => (StatusCodes.Status404NotFound, "Resource not found", "https://httpstatuses.com/404"),
            _ => (StatusCodes.Status500InternalServerError, "Internal server error", "https://httpstatuses.com/500")
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = type,
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };

        if (exception is InputValidationException validationException)
        {
            problemDetails.Extensions["errors"] = validationException.Errors;
        }

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);

        return true;
    }
}
