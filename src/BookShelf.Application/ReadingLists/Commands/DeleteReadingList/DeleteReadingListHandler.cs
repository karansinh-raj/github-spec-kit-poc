using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;

namespace BookShelf.Application.ReadingLists.Commands.DeleteReadingList;

public class DeleteReadingListHandler : IRequestHandler<DeleteReadingListCommand, Result<bool>>
{
    private readonly IReadingListRepository _repository;
    private readonly ILogger<DeleteReadingListHandler> _logger;

    public DeleteReadingListHandler(IReadingListRepository repository, ILogger<DeleteReadingListHandler>? logger = null)
    {
        _repository = repository;
        _logger = logger ?? NullLogger<DeleteReadingListHandler>.Instance;
    }

    public async Task<Result<bool>> Handle(DeleteReadingListCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Handling DeleteReadingListCommand for ReadingListId {ListId}", command.Id);
        try
        {
            var readingList = await _repository.GetByIdAsync(command.Id, cancellationToken);
            if (readingList is null)
            {
                _logger.LogWarning("DeleteReadingListCommand failed because list {ListId} was not found", command.Id);
                return Result<bool>.Failure("Reading list not found");
            }

            await _repository.DeleteAsync(readingList, cancellationToken);
            _logger.LogInformation("DeleteReadingListCommand succeeded for ReadingListId {ListId}", command.Id);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while handling DeleteReadingListCommand for ReadingListId {ListId}", command.Id);
            throw;
        }
        finally
        {
            _logger.LogInformation("Handled DeleteReadingListCommand for ReadingListId {ListId} in {ElapsedMilliseconds}ms", command.Id, stopwatch.ElapsedMilliseconds);
        }
    }
}
