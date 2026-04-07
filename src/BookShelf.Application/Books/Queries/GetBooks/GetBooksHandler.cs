using BookShelf.Application.Books.DTOs;
using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using MediatR;

namespace BookShelf.Application.Books.Queries.GetBooks;

public class GetBooksHandler : IRequestHandler<GetBooksQuery, Result<PagedResult<BookDto>>>
{
    private readonly IBookRepository _repository;

    public GetBooksHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PagedResult<BookDto>>> Handle(GetBooksQuery query, CancellationToken cancellationToken)
    {
        var pageSize = Math.Min(query.PageSize, 50);
        var page = Math.Max(query.Page, 1);

        var (items, totalCount) = await _repository.GetAllAsync(
            page, pageSize,
            query.Genre, query.Author, query.Search,
            query.SortBy, query.SortOrder,
            cancellationToken);

        var dtos = items.Select(b => new BookDto(
            b.Id, b.Title, b.Author, b.ISBN,
            b.PublishedDate, b.Genre.ToString(),
            b.Description, b.CreatedAt, b.UpdatedAt)).ToList();

        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        return Result<PagedResult<BookDto>>.Success(
            new PagedResult<BookDto>(dtos, totalCount, totalPages, page, pageSize));
    }
}
