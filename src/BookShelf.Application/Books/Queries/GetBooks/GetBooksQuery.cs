using BookShelf.Application.Books.DTOs;
using BookShelf.Application.Common.Models;
using MediatR;

namespace BookShelf.Application.Books.Queries.GetBooks;

public record GetBooksQuery(
    int Page = 1,
    int PageSize = 10,
    string? Genre = null,
    string? Author = null,
    string? Search = null,
    string SortBy = "title",
    string SortOrder = "asc") : IRequest<Result<PagedResult<BookDto>>>;
