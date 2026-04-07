using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.ReadingLists.Commands.DeleteReadingList;
using BookShelf.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace BookShelf.Application.Tests.ReadingLists.Commands;

public class DeleteReadingListHandlerTests
{
    private readonly IReadingListRepository _repository = Substitute.For<IReadingListRepository>();
    private readonly DeleteReadingListHandler _handler;

    public DeleteReadingListHandlerTests()
    {
        _handler = new DeleteReadingListHandler(_repository);
    }

    [Fact]
    public async Task Handle_ExistingList_ReturnsSuccess()
    {
        var existing = new ReadingList { Id = 1, Name = "Test" };
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(existing);

        var result = await _handler.Handle(new DeleteReadingListCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _repository.Received(1).DeleteAsync(existing, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NotFound_ReturnsFailure()
    {
        _repository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((ReadingList?)null);

        var result = await _handler.Handle(new DeleteReadingListCommand(99), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("not found"));
    }
}
