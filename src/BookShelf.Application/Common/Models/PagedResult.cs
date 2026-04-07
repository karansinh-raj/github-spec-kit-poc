namespace BookShelf.Application.Common.Models;

public record PagedResult<T>(
    List<T> Items,
    int TotalCount,
    int TotalPages,
    int CurrentPage,
    int PageSize);
