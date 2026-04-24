using BookShelf.Application.Books.DTOs;
using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;

namespace BookShelf.Application.Books.Queries.GetBookById;

public class GetBookByIdHandler : IRequestHandler<GetBookByIdQuery, Result<BookDto>>
{
    private readonly IBookRepository _repository;
    private readonly ILogger<GetBookByIdHandler> _logger;

    public GetBookByIdHandler(IBookRepository repository, ILogger<GetBookByIdHandler>? logger = null)
    {
        _repository = repository;
        _logger = logger ?? NullLogger<GetBookByIdHandler>.Instance;
    }

    public async Task<Result<BookDto>> Handle(GetBookByIdQuery query, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Handling GetBookByIdQuery for BookId {BookId}", query.Id);
        try
        {
            var book = await _repository.GetByIdAsync(query.Id, cancellationToken);
            if (book is null)
            {
                _logger.LogWarning("GetBookByIdQuery did not find book {BookId}", query.Id);
                return Result<BookDto>.Failure($"Book with Id '{query.Id}' was not found.");
            }

            _logger.LogInformation("GetBookByIdQuery succeeded for BookId {BookId}", query.Id);
            return Result<BookDto>.Success(new BookDto(
                book.Id, book.Title, book.Author, book.ISBN,
                book.PublishedDate, book.Genre.ToString(),
                book.Description, book.CreatedAt, book.UpdatedAt));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while handling GetBookByIdQuery for BookId {BookId}", query.Id);
            throw;
        }
        finally
        {
            _logger.LogInformation("Handled GetBookByIdQuery for BookId {BookId} in {ElapsedMilliseconds}ms", query.Id, stopwatch.ElapsedMilliseconds);
        }
    }
}
