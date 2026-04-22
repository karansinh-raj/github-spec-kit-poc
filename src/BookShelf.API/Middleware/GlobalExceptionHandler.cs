using BookShelf.Application.Common.Models;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace BookShelf.API.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler>? logger = null)
    {
        _logger = logger ?? NullLogger<GlobalExceptionHandler>.Instance;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(
            exception,
            "Unhandled exception for {Method} {Path}. TraceId: {TraceId}",
            httpContext.Request.Method,
            httpContext.Request.Path,
            httpContext.TraceIdentifier);

        var (statusCode, response) = exception switch
        {
            ValidationException validationException => (
                StatusCodes.Status422UnprocessableEntity,
                ApiResponse<object>.Fail(
                    validationException.Errors
                        .Select(e => new ApiError(e.PropertyName, e.ErrorMessage))
                        .ToList())),
            _ => (
                StatusCodes.Status500InternalServerError,
                ApiResponse<object>.Fail("General", "An unexpected error occurred."))
        };

        if (exception is ValidationException failedValidation)
        {
            _logger.LogWarning(
                "Validation failed for {Method} {Path}. ErrorCount: {ErrorCount}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                failedValidation.Errors.Count());
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }
}
