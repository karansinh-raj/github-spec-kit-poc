using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using BookShelf.Application.ReadingLists.DTOs;
using MediatR;

namespace BookShelf.Application.ReadingLists.Queries.GetReadingListStats;

public class GetReadingListStatsHandler : IRequestHandler<GetReadingListStatsQuery, Result<ReadingListStatsDto>>
{
    private readonly IReadingListRepository _repository;

    public GetReadingListStatsHandler(IReadingListRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ReadingListStatsDto>> Handle(GetReadingListStatsQuery query, CancellationToken cancellationToken)
    {
        var readingList = await _repository.GetByIdWithBooksAsync(query.Id, cancellationToken);
        if (readingList is null)
            return Result<ReadingListStatsDto>.Failure("Reading list not found");

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
        return Result<ReadingListStatsDto>.Success(dto);
    }
}
