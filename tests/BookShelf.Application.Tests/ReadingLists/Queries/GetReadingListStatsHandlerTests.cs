using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.ReadingLists.Queries.GetReadingListStats;
using BookShelf.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace BookShelf.Application.Tests.ReadingLists.Queries;

public class GetReadingListStatsHandlerTests
{
    private readonly IReadingListRepository _repository = Substitute.For<IReadingListRepository>();
    private readonly GetReadingListStatsHandler _handler;

    public GetReadingListStatsHandlerTests()
    {
        _handler = new GetReadingListStatsHandler(_repository);
    }

    [Fact]
    public async Task Handle_ListWithBooks_ReturnsAccurateStats()
    {
        var list = new ReadingList
        {
            Id = 1, Name = "Test", CreatedAt = DateTime.UtcNow,
            ReadingListBooks = new List<ReadingListBook>
            {
                new() { IsRead = true, CompletedDate = new DateOnly(2026, 3, 15) },
                new() { IsRead = true, CompletedDate = new DateOnly(2026, 4, 1) },
                new() { IsRead = false },
                new() { IsRead = false }
            }
        };
        _repository.GetByIdWithBooksAsync(1, Arg.Any<CancellationToken>()).Returns(list);

        var result = await _handler.Handle(new GetReadingListStatsQuery(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalBooks.Should().Be(4);
        result.Value.BooksRead.Should().Be(2);
        result.Value.BooksUnread.Should().Be(2);
        result.Value.CompletionPercentage.Should().Be(50);
        result.Value.MostRecentCompletion.Should().Be(new DateOnly(2026, 4, 1));
    }

    [Fact]
    public async Task Handle_EmptyList_ReturnsZeroStats()
    {
        var list = new ReadingList
        {
            Id = 1, Name = "Empty", CreatedAt = DateTime.UtcNow,
            ReadingListBooks = []
        };
        _repository.GetByIdWithBooksAsync(1, Arg.Any<CancellationToken>()).Returns(list);

        var result = await _handler.Handle(new GetReadingListStatsQuery(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalBooks.Should().Be(0);
        result.Value.CompletionPercentage.Should().Be(0);
        result.Value.MostRecentCompletion.Should().BeNull();
    }

    [Fact]
    public async Task Handle_NotFound_ReturnsFailure()
    {
        _repository.GetByIdWithBooksAsync(99, Arg.Any<CancellationToken>()).Returns((ReadingList?)null);

        var result = await _handler.Handle(new GetReadingListStatsQuery(99), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("not found"));
    }
}
