using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using BookShelf.Application.ReadingLists.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;

namespace BookShelf.Application.ReadingLists.Queries.GetReadingListStats;

public class GetReadingListStatsHandler : IRequestHandler<GetReadingListStatsQuery, Result<ReadingListStatsDto>>
{
    private readonly IReadingListRepository _repository;
    private readonly ILogger<GetReadingListStatsHandler> _logger;

    public GetReadingListStatsHandler(IReadingListRepository repository, ILogger<GetReadingListStatsHandler>? logger = null)
    {
        _repository = repository;
        _logger = logger ?? NullLogger<GetReadingListStatsHandler>.Instance;
    }

    public async Task<Result<ReadingListStatsDto>> Handle(GetReadingListStatsQuery query, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Handling GetReadingListStatsQuery for ReadingListId {ListId}", query.Id);
        try
        {
            var readingList = await _repository.GetByIdWithBooksAsync(query.Id, cancellationToken);
            if (readingList is null)
            {
                _logger.LogWarning("GetReadingListStatsQuery did not find list {ListId}", query.Id);
                return Result<ReadingListStatsDto>.Failure("Reading list not found");
            }

            var totalBooks = readingList.ReadingListBooks.Count;
            var booksRead = readingList.ReadingListBooks.Count(b => b.IsRead);
            var booksUnread = totalBooks - booksRead;
            var percentage = totalBooks > 0 ? Math.Round((double)booksRead / totalBooks * 100, 1) : 0;
            var mostRecent = readingList.ReadingListBooks
                .Where(b => b.CompletedDate.HasValue)
                .OrderByDescending(b => b.CompletedDate)
                .Select(b => b.CompletedDate)
                .FirstOrDefault();

            var dto = new ReadingListStatsDto(totalBooks, booksRead, booksUnread, percentage, mostRecent);
            _logger.LogInformation("GetReadingListStatsQuery succeeded for ReadingListId {ListId}", query.Id);
            return Result<ReadingListStatsDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while handling GetReadingListStatsQuery for ReadingListId {ListId}", query.Id);
            throw;
        }
        finally
        {
            _logger.LogInformation("Handled GetReadingListStatsQuery for ReadingListId {ListId} in {ElapsedMilliseconds}ms", query.Id, stopwatch.ElapsedMilliseconds);
        }
    }
}
