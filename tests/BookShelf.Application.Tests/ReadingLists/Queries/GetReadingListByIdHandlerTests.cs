using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.ReadingLists.Queries.GetReadingListById;
using BookShelf.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace BookShelf.Application.Tests.ReadingLists.Queries;

public class GetReadingListByIdHandlerTests
{
    private readonly IReadingListRepository _repository = Substitute.For<IReadingListRepository>();
    private readonly GetReadingListByIdHandler _handler;

    public GetReadingListByIdHandlerTests()
    {
        _handler = new GetReadingListByIdHandler(_repository);
    }

    [Fact]
    public async Task Handle_ExistingList_ReturnsDetailWithBooks()
    {
        var list = new ReadingList
        {
            Id = 1, Name = "Summer", CreatedAt = DateTime.UtcNow,
            ReadingListBooks = new List<ReadingListBook>
            {
                new()
                {
                    BookId = 10, IsRead = true, Notes = "Great",
                    CompletedDate = new DateOnly(2026, 4, 1), AddedAt = DateTime.UtcNow,
                    Book = new Book { Id = 10, Title = "Clean Code", Author = "Robert C. Martin" }
                }
            }
        };
        _repository.GetByIdWithBooksAsync(1, Arg.Any<CancellationToken>()).Returns(list);

        var result = await _handler.Handle(new GetReadingListByIdQuery(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Summer");
        result.Value.Books.Should().HaveCount(1);
        result.Value.Books[0].Title.Should().Be("Clean Code");
        result.Value.Books[0].IsRead.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_NotFound_ReturnsFailure()
    {
        _repository.GetByIdWithBooksAsync(99, Arg.Any<CancellationToken>()).Returns((ReadingList?)null);

        var result = await _handler.Handle(new GetReadingListByIdQuery(99), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("not found"));
    }
}
