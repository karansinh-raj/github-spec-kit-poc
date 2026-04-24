using BookShelf.Application.Books.DTOs;
using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using BookShelf.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;

namespace BookShelf.Application.Books.Commands.UpdateBook;

public class UpdateBookHandler : IRequestHandler<UpdateBookCommand, Result<BookDto>>
{
    private readonly IBookRepository _repository;
    private readonly ILogger<UpdateBookHandler> _logger;

    public UpdateBookHandler(IBookRepository repository, ILogger<UpdateBookHandler>? logger = null)
    {
        _repository = repository;
        _logger = logger ?? NullLogger<UpdateBookHandler>.Instance;
    }

    public async Task<Result<BookDto>> Handle(UpdateBookCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Handling UpdateBookCommand for BookId {BookId}", command.Id);

        try
        {
            var book = await _repository.GetByIdAsync(command.Id, cancellationToken);
            if (book is null)
            {
                _logger.LogWarning("UpdateBookCommand failed because book {BookId} was not found", command.Id);
                return Result<BookDto>.Failure($"Book with Id '{command.Id}' was not found.");
            }

            var request = command.Request;

            if (await _repository.ExistsByIsbnAsync(request.ISBN, excludeId: command.Id, cancellationToken: cancellationToken))
            {
                _logger.LogWarning("UpdateBookCommand rejected for BookId {BookId} due to duplicate ISBN {Isbn}", command.Id, request.ISBN);
                return Result<BookDto>.Failure($"A book with ISBN '{request.ISBN}' already exists.");
            }

            book.Title = request.Title;
            book.Author = request.Author;
            book.ISBN = request.ISBN;
            book.PublishedDate = request.PublishedDate;
            book.Genre = Enum.Parse<Genre>(request.Genre, ignoreCase: true);
            book.Description = request.Description;
            book.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(book, cancellationToken);
            _logger.LogInformation("UpdateBookCommand succeeded for BookId {BookId}", command.Id);

            return Result<BookDto>.Success(new BookDto(
                book.Id, book.Title, book.Author, book.ISBN,
                book.PublishedDate, book.Genre.ToString(),
                book.Description, book.CreatedAt, book.UpdatedAt));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while handling UpdateBookCommand for BookId {BookId}", command.Id);
            throw;
        }
        finally
        {
            _logger.LogInformation("Handled UpdateBookCommand for BookId {BookId} in {ElapsedMilliseconds}ms", command.Id, stopwatch.ElapsedMilliseconds);
        }
    }
}
