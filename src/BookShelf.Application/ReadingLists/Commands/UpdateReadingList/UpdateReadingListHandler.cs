using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using BookShelf.Application.ReadingLists.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;

namespace BookShelf.Application.ReadingLists.Commands.UpdateReadingList;

public class UpdateReadingListHandler : IRequestHandler<UpdateReadingListCommand, Result<ReadingListDto>>
{
    private readonly IReadingListRepository _repository;
    private readonly ILogger<UpdateReadingListHandler> _logger;

    public UpdateReadingListHandler(IReadingListRepository repository, ILogger<UpdateReadingListHandler>? logger = null)
    {
        _repository = repository;
        _logger = logger ?? NullLogger<UpdateReadingListHandler>.Instance;
    }

    public async Task<Result<ReadingListDto>> Handle(UpdateReadingListCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Handling UpdateReadingListCommand for ReadingListId {ListId}", command.Id);
        try
        {
            var readingList = await _repository.GetByIdWithBooksAsync(command.Id, cancellationToken);
            if (readingList is null)
            {
                _logger.LogWarning("UpdateReadingListCommand failed because list {ListId} was not found", command.Id);
                return Result<ReadingListDto>.Failure("Reading list not found");
            }

            if (await _repository.ExistsByNameAsync(command.Request.Name, command.Id, cancellationToken))
            {
                _logger.LogWarning("UpdateReadingListCommand rejected for list {ListId} due to duplicate Name {ListName}", command.Id, command.Request.Name);
                return Result<ReadingListDto>.Failure("A reading list with this name already exists");
            }

            readingList.Name = command.Request.Name;
            readingList.Description = command.Request.Description;
            readingList.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(readingList, cancellationToken);

            var bookCount = readingList.ReadingListBooks.Count;
            var booksRead = readingList.ReadingListBooks.Count(b => b.IsRead);
            var percentage = bookCount > 0 ? Math.Round((double)booksRead / bookCount * 100, 1) : 0;

            var dto = new ReadingListDto(
                readingList.Id, readingList.Name, readingList.Description,
                bookCount, percentage, readingList.CreatedAt, readingList.UpdatedAt);

            _logger.LogInformation("UpdateReadingListCommand succeeded for ReadingListId {ListId}", command.Id);
            return Result<ReadingListDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while handling UpdateReadingListCommand for ReadingListId {ListId}", command.Id);
            throw;
        }
        finally
        {
            _logger.LogInformation("Handled UpdateReadingListCommand for ReadingListId {ListId} in {ElapsedMilliseconds}ms", command.Id, stopwatch.ElapsedMilliseconds);
        }
    }
}
