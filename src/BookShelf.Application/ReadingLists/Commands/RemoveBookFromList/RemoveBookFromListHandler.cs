using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;

namespace BookShelf.Application.ReadingLists.Commands.RemoveBookFromList;

public class RemoveBookFromListHandler : IRequestHandler<RemoveBookFromListCommand, Result<bool>>
{
    private readonly IReadingListRepository _repository;
    private readonly ILogger<RemoveBookFromListHandler> _logger;

    public RemoveBookFromListHandler(IReadingListRepository repository, ILogger<RemoveBookFromListHandler>? logger = null)
    {
        _repository = repository;
        _logger = logger ?? NullLogger<RemoveBookFromListHandler>.Instance;
    }

    public async Task<Result<bool>> Handle(RemoveBookFromListCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Handling RemoveBookFromListCommand for ReadingListId {ListId} and BookId {BookId}", command.ListId, command.BookId);
        try
        {
            var entry = await _repository.GetReadingListBookAsync(command.ListId, command.BookId, cancellationToken);
            if (entry is null)
            {
                _logger.LogWarning("RemoveBookFromListCommand failed because BookId {BookId} was not in ReadingListId {ListId}", command.BookId, command.ListId);
                return Result<bool>.Failure("Book not found in this reading list");
            }

            await _repository.RemoveBookFromListAsync(entry, cancellationToken);
            _logger.LogInformation("RemoveBookFromListCommand succeeded for ReadingListId {ListId} and BookId {BookId}", command.ListId, command.BookId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while handling RemoveBookFromListCommand for ReadingListId {ListId} and BookId {BookId}", command.ListId, command.BookId);
            throw;
        }
        finally
        {
            _logger.LogInformation("Handled RemoveBookFromListCommand for ReadingListId {ListId} and BookId {BookId} in {ElapsedMilliseconds}ms", command.ListId, command.BookId, stopwatch.ElapsedMilliseconds);
        }
    }
}
