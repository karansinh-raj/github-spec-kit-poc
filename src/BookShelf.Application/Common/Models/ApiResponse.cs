namespace BookShelf.Application.Common.Models;

public record ApiResponse<T>(T? Data, List<ApiError>? Errors = null, object? Meta = null)
{
    public static ApiResponse<T> Ok(T data, object? meta = null) => new(data, null, meta);
    public static ApiResponse<T> Fail(List<ApiError> errors) => new(default, errors);
    public static ApiResponse<T> Fail(string field, string message) => new(default, [new ApiError(field, message)]);
}

public record ApiError(string Field, string Message);
