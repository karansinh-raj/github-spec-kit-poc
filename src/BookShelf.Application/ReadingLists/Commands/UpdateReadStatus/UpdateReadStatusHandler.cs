using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using BookShelf.Application.ReadingLists.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;

namespace BookShelf.Application.ReadingLists.Commands.UpdateReadStatus;

public class UpdateReadStatusHandler : IRequestHandler<UpdateReadStatusCommand, Result<ReadingListBookDto>>
{
    private readonly IReadingListRepository _readingListRepository;
    private readonly IBookRepository _bookRepository;
    private readonly ILogger<UpdateReadStatusHandler> _logger;

    public UpdateReadStatusHandler(
        IReadingListRepository readingListRepository,
        IBookRepository bookRepository,
        ILogger<UpdateReadStatusHandler>? logger = null)
    {
        _readingListRepository = readingListRepository;
        _bookRepository = bookRepository;
        _logger = logger ?? NullLogger<UpdateReadStatusHandler>.Instance;
    }

    public async Task<Result<ReadingListBookDto>> Handle(UpdateReadStatusCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Handling UpdateReadStatusCommand for ReadingListId {ListId} and BookId {BookId}", command.ListId, command.BookId);
        try
        {
            var entry = await _readingListRepository.GetReadingListBookAsync(command.ListId, command.BookId, cancellationToken);
            if (entry is null)
            {
                _logger.LogWarning("UpdateReadStatusCommand failed because BookId {BookId} was not in ReadingListId {ListId}", command.BookId, command.ListId);
                return Result<ReadingListBookDto>.Failure("Book not found in this reading list");
            }

            entry.IsRead = command.Request.IsRead;

            if (command.Request.IsRead)
            {
                entry.Notes = command.Request.Notes;
                entry.CompletedDate = command.Request.CompletedDate;
            }
            else
            {
                entry.Notes = null;
                entry.CompletedDate = null;
            }

            await _readingListRepository.UpdateReadingListBookAsync(entry, cancellationToken);

            var book = await _bookRepository.GetByIdAsync(command.BookId, cancellationToken);

            var dto = new ReadingListBookDto(
                command.BookId, book!.Title, book.Author,
                entry.IsRead, entry.Notes, entry.CompletedDate, entry.AddedAt);

            _logger.LogInformation("UpdateReadStatusCommand succeeded for ReadingListId {ListId} and BookId {BookId}", command.ListId, command.BookId);
            return Result<ReadingListBookDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while handling UpdateReadStatusCommand for ReadingListId {ListId} and BookId {BookId}", command.ListId, command.BookId);
            throw;
        }
        finally
        {
            _logger.LogInformation("Handled UpdateReadStatusCommand for ReadingListId {ListId} and BookId {BookId} in {ElapsedMilliseconds}ms", command.ListId, command.BookId, stopwatch.ElapsedMilliseconds);
        }
    }
}
