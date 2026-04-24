using BookShelf.Application.Books.DTOs;
using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using BookShelf.Domain.Entities;
using BookShelf.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;

namespace BookShelf.Application.Books.Commands.CreateBook;

public class CreateBookHandler : IRequestHandler<CreateBookCommand, Result<BookDto>>
{
    private readonly IBookRepository _repository;
    private readonly ILogger<CreateBookHandler> _logger;

    public CreateBookHandler(IBookRepository repository, ILogger<CreateBookHandler>? logger = null)
    {
        _repository = repository;
        _logger = logger ?? NullLogger<CreateBookHandler>.Instance;
    }

    public async Task<Result<BookDto>> Handle(CreateBookCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var request = command.Request;
        _logger.LogInformation("Handling CreateBookCommand for ISBN {Isbn}", request.ISBN);

        try
        {
            if (await _repository.ExistsByIsbnAsync(request.ISBN, cancellationToken: cancellationToken))
            {
                _logger.LogWarning("CreateBookCommand rejected for duplicate ISBN {Isbn}", request.ISBN);
                return Result<BookDto>.Failure($"A book with ISBN '{request.ISBN}' already exists.");
            }

            var book = new Book
            {
                Title = request.Title,
                Author = request.Author,
                ISBN = request.ISBN,
                PublishedDate = request.PublishedDate,
                Genre = Enum.Parse<Genre>(request.Genre, ignoreCase: true),
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repository.AddAsync(book, cancellationToken);
            _logger.LogInformation("CreateBookCommand succeeded for ISBN {Isbn} with BookId {BookId}", request.ISBN, created.Id);

            return Result<BookDto>.Success(MapToDto(created));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while handling CreateBookCommand for ISBN {Isbn}", request.ISBN);
            throw;
        }
        finally
        {
            _logger.LogInformation("Handled CreateBookCommand for ISBN {Isbn} in {ElapsedMilliseconds}ms", request.ISBN, stopwatch.ElapsedMilliseconds);
        }
    }

    private static BookDto MapToDto(Book book) => new(
        book.Id, book.Title, book.Author, book.ISBN,
        book.PublishedDate, book.Genre.ToString(),
        book.Description, book.CreatedAt, book.UpdatedAt);
}
