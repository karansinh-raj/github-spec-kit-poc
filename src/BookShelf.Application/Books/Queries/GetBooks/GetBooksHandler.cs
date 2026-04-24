using BookShelf.Application.Books.DTOs;
using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;

namespace BookShelf.Application.Books.Queries.GetBooks;

public class GetBooksHandler : IRequestHandler<GetBooksQuery, Result<PagedResult<BookDto>>>
{
    private readonly IBookRepository _repository;
    private readonly ILogger<GetBooksHandler> _logger;

    public GetBooksHandler(IBookRepository repository, ILogger<GetBooksHandler>? logger = null)
    {
        _repository = repository;
        _logger = logger ?? NullLogger<GetBooksHandler>.Instance;
    }

    public async Task<Result<PagedResult<BookDto>>> Handle(GetBooksQuery query, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation(
            "Handling GetBooksQuery for Page {Page}, PageSize {PageSize}, Genre {Genre}, Author {Author}",
            query.Page, query.PageSize, query.Genre, query.Author);
        try
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

            _logger.LogInformation("GetBooksQuery succeeded returning {Count} books", dtos.Count);
            return Result<PagedResult<BookDto>>.Success(
                new PagedResult<BookDto>(dtos, totalCount, totalPages, page, pageSize));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while handling GetBooksQuery for Page {Page}", query.Page);
            throw;
        }
        finally
        {
            _logger.LogInformation("Handled GetBooksQuery in {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
        }
    }
}
