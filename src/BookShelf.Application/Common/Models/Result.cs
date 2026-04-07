namespace BookShelf.Application.Common.Models;

public class Result<T>
{
    public T? Value { get; }
    public bool IsSuccess { get; }
    public List<string> Errors { get; }

    private Result(T? value, bool isSuccess, List<string>? errors = null)
    {
        Value = value;
        IsSuccess = isSuccess;
        Errors = errors ?? [];
    }

    public static Result<T> Success(T value) => new(value, true);
    public static Result<T> Failure(string error) => new(default, false, [error]);
    public static Result<T> Failure(List<string> errors) => new(default, false, errors);
}
