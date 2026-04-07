using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.ReadingLists.Commands.UpdateReadingList;
using BookShelf.Application.ReadingLists.DTOs;
using BookShelf.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace BookShelf.Application.Tests.ReadingLists.Commands;

public class UpdateReadingListHandlerTests
{
    private readonly IReadingListRepository _repository = Substitute.For<IReadingListRepository>();
    private readonly UpdateReadingListHandler _handler;

    public UpdateReadingListHandlerTests()
    {
        _handler = new UpdateReadingListHandler(_repository);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsUpdatedDto()
    {
        var existing = new ReadingList { Id = 1, Name = "Old Name", CreatedAt = DateTime.UtcNow, ReadingListBooks = [] };
        _repository.GetByIdWithBooksAsync(1, Arg.Any<CancellationToken>()).Returns(existing);
        _repository.ExistsByNameAsync("New Name", 1, Arg.Any<CancellationToken>()).Returns(false);

        var request = new UpdateReadingListRequest("New Name", "Updated desc");
        var result = await _handler.Handle(new UpdateReadingListCommand(1, request), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("New Name");
        result.Value.Description.Should().Be("Updated desc");
    }

    [Fact]
    public async Task Handle_NotFound_ReturnsFailure()
    {
        _repository.GetByIdWithBooksAsync(99, Arg.Any<CancellationToken>()).Returns((ReadingList?)null);

        var request = new UpdateReadingListRequest("Name", null);
        var result = await _handler.Handle(new UpdateReadingListCommand(99, request), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("not found"));
    }

    [Fact]
    public async Task Handle_DuplicateName_ReturnsFailure()
    {
        var existing = new ReadingList { Id = 1, Name = "Old", CreatedAt = DateTime.UtcNow, ReadingListBooks = [] };
        _repository.GetByIdWithBooksAsync(1, Arg.Any<CancellationToken>()).Returns(existing);
        _repository.ExistsByNameAsync("Taken", 1, Arg.Any<CancellationToken>()).Returns(true);

        var request = new UpdateReadingListRequest("Taken", null);
        var result = await _handler.Handle(new UpdateReadingListCommand(1, request), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("already exists"));
    }
}
