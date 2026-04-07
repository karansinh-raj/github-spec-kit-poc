using BookShelf.Application.Books.Queries.GetBookById;
using BookShelf.Application.Common.Interfaces;
using BookShelf.Domain.Entities;
using BookShelf.Domain.Enums;
using FluentAssertions;
using NSubstitute;

namespace BookShelf.Application.Tests.Books.Queries;

public class GetBookByIdHandlerTests
{
    private readonly IBookRepository _repository;
    private readonly GetBookByIdHandler _handler;

    public GetBookByIdHandlerTests()
    {
        _repository = Substitute.For<IBookRepository>();
        _handler = new GetBookByIdHandler(_repository);
    }

    [Fact]
    public async Task Handle_ExistingBook_ReturnsBookDto()
    {
        var book = new Book
        {
            Id = 1, Title = "Foundation", Author = "Isaac Asimov",
            ISBN = "978-0553293357", PublishedDate = new DateOnly(1951, 1, 1),
            Genre = Genre.Science, Description = "Classic", CreatedAt = DateTime.UtcNow
        };

        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(book);

        var result = await _handler.Handle(new GetBookByIdQuery(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Title.Should().Be("Foundation");
        result.Value.Genre.Should().Be("Science");
    }

    [Fact]
    public async Task Handle_NonExistentBook_ReturnsFailure()
    {
        _repository.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((Book?)null);

        var result = await _handler.Handle(new GetBookByIdQuery(999), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Contain("not found");
    }
}
