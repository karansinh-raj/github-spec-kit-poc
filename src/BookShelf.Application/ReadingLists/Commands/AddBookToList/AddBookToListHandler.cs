using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using BookShelf.Application.ReadingLists.DTOs;
using BookShelf.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;

namespace BookShelf.Application.ReadingLists.Commands.AddBookToList;

public class AddBookToListHandler : IRequestHandler<AddBookToListCommand, Result<ReadingListBookDto>>
{
    private readonly IReadingListRepository _readingListRepository;
    private readonly IBookRepository _bookRepository;
    private readonly ILogger<AddBookToListHandler> _logger;

    public AddBookToListHandler(
        IReadingListRepository readingListRepository,
        IBookRepository bookRepository,
        ILogger<AddBookToListHandler>? logger = null)
    {
        _readingListRepository = readingListRepository;
        _bookRepository = bookRepository;
        _logger = logger ?? NullLogger<AddBookToListHandler>.Instance;
    }

    public async Task<Result<ReadingListBookDto>> Handle(AddBookToListCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Handling AddBookToListCommand for ReadingListId {ListId} and BookId {BookId}", command.ListId, command.BookId);
        try
        {
            var readingList = await _readingListRepository.GetByIdAsync(command.ListId, cancellationToken);
            if (readingList is null)
            {
                _logger.LogWarning("AddBookToListCommand failed because list {ListId} was not found", command.ListId);
                return Result<ReadingListBookDto>.Failure("Reading list not found");
            }

            var book = await _bookRepository.GetByIdAsync(command.BookId, cancellationToken);
            if (book is null)
            {
                _logger.LogWarning("AddBookToListCommand failed because book {BookId} was not found", command.BookId);
                return Result<ReadingListBookDto>.Failure("Book not found");
            }

            var existing = await _readingListRepository.GetReadingListBookAsync(command.ListId, command.BookId, cancellationToken);
            if (existing is not null)
            {
                _logger.LogWarning("AddBookToListCommand rejected because BookId {BookId} is already in ReadingListId {ListId}", command.BookId, command.ListId);
                return Result<ReadingListBookDto>.Failure("Book is already in this reading list");
            }

            var entry = new ReadingListBook
            {
                ReadingListId = command.ListId,
                BookId = command.BookId,
                IsRead = false,
                AddedAt = DateTime.UtcNow
            };

            var created = await _readingListRepository.AddBookToListAsync(entry, cancellationToken);

            var dto = new ReadingListBookDto(
                book.Id, book.Title, book.Author,
                created.IsRead, created.Notes, created.CompletedDate, created.AddedAt);

            _logger.LogInformation("AddBookToListCommand succeeded for ReadingListId {ListId} and BookId {BookId}", command.ListId, command.BookId);
            return Result<ReadingListBookDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while handling AddBookToListCommand for ReadingListId {ListId} and BookId {BookId}", command.ListId, command.BookId);
            throw;
        }
        finally
        {
            _logger.LogInformation("Handled AddBookToListCommand for ReadingListId {ListId} and BookId {BookId} in {ElapsedMilliseconds}ms", command.ListId, command.BookId, stopwatch.ElapsedMilliseconds);
        }
    }
}
