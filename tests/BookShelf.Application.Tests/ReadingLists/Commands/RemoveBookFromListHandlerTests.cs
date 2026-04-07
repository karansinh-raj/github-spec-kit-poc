using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.ReadingLists.Commands.RemoveBookFromList;
using BookShelf.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace BookShelf.Application.Tests.ReadingLists.Commands;

public class RemoveBookFromListHandlerTests
{
    private readonly IReadingListRepository _repository = Substitute.For<IReadingListRepository>();
    private readonly RemoveBookFromListHandler _handler;

    public RemoveBookFromListHandlerTests()
    {
        _handler = new RemoveBookFromListHandler(_repository);
    }

    [Fact]
    public async Task Handle_BookInList_RemovesAndReturnsSuccess()
    {
        var entry = new ReadingListBook { ReadingListId = 1, BookId = 10 };
        _repository.GetReadingListBookAsync(1, 10, Arg.Any<CancellationToken>()).Returns(entry);

        var result = await _handler.Handle(new RemoveBookFromListCommand(1, 10), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _repository.Received(1).RemoveBookFromListAsync(entry, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_BookNotInList_ReturnsFailure()
    {
        _repository.GetReadingListBookAsync(1, 99, Arg.Any<CancellationToken>()).Returns((ReadingListBook?)null);

        var result = await _handler.Handle(new RemoveBookFromListCommand(1, 99), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("not found"));
    }
}
