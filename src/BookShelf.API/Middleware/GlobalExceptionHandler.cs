using BookShelf.Application.Common.Models;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace BookShelf.API.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
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

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }
}
