using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.ReadingLists.Commands.CreateReadingList;
using BookShelf.Application.ReadingLists.DTOs;
using BookShelf.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace BookShelf.Application.Tests.ReadingLists.Commands;

public class CreateReadingListHandlerTests
{
    private readonly IReadingListRepository _repository = Substitute.For<IReadingListRepository>();
    private readonly CreateReadingListHandler _handler;

    public CreateReadingListHandlerTests()
    {
        _handler = new CreateReadingListHandler(_repository);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccessWithDto()
    {
        var request = new CreateReadingListRequest("Summer 2026", "Beach reads");
        _repository.ExistsByNameAsync(request.Name, cancellationToken: Arg.Any<CancellationToken>()).Returns(false);
        _repository.AddAsync(Arg.Any<ReadingList>(), Arg.Any<CancellationToken>())
            .Returns(ci =>
            {
                var rl = ci.Arg<ReadingList>();
                rl.Id = 1;
                return rl;
            });

        var result = await _handler.Handle(new CreateReadingListCommand(request), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Summer 2026");
        result.Value.Description.Should().Be("Beach reads");
        result.Value.BookCount.Should().Be(0);
        result.Value.CompletionPercentage.Should().Be(0);
    }

    [Fact]
    public async Task Handle_DuplicateName_ReturnsFailure()
    {
        var request = new CreateReadingListRequest("Summer 2026", null);
        _repository.ExistsByNameAsync(request.Name, cancellationToken: Arg.Any<CancellationToken>()).Returns(true);

        var result = await _handler.Handle(new CreateReadingListCommand(request), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("already exists"));
    }
}
