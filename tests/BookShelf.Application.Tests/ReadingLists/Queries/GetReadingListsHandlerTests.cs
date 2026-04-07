using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.ReadingLists.Queries.GetReadingLists;
using BookShelf.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace BookShelf.Application.Tests.ReadingLists.Queries;

public class GetReadingListsHandlerTests
{
    private readonly IReadingListRepository _repository = Substitute.For<IReadingListRepository>();
    private readonly GetReadingListsHandler _handler;

    public GetReadingListsHandlerTests()
    {
        _handler = new GetReadingListsHandler(_repository);
    }

    [Fact]
    public async Task Handle_ReturnsAllListsWithCounts()
    {
        var lists = new List<ReadingList>
        {
            new()
            {
                Id = 1, Name = "List A", CreatedAt = DateTime.UtcNow,
                ReadingListBooks = new List<ReadingListBook>
                {
                    new() { IsRead = true },
                    new() { IsRead = false }
                }
            },
            new()
            {
                Id = 2, Name = "List B", CreatedAt = DateTime.UtcNow,
                ReadingListBooks = []
            }
        };
        _repository.GetAllWithBooksAsync(Arg.Any<CancellationToken>()).Returns(lists);

        var result = await _handler.Handle(new GetReadingListsQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().HaveCount(2);
        result.Value[0].BookCount.Should().Be(2);
        result.Value[0].CompletionPercentage.Should().Be(50);
        result.Value[1].BookCount.Should().Be(0);
        result.Value[1].CompletionPercentage.Should().Be(0);
    }
}
