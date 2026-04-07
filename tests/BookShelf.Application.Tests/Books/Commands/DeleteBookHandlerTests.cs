using BookShelf.Application.Books.Commands.DeleteBook;
using BookShelf.Application.Common.Interfaces;
using BookShelf.Domain.Entities;
using BookShelf.Domain.Enums;
using FluentAssertions;
using NSubstitute;

namespace BookShelf.Application.Tests.Books.Commands;

public class DeleteBookHandlerTests
{
    private readonly IBookRepository _repository;
    private readonly DeleteBookHandler _handler;

    public DeleteBookHandlerTests()
    {
        _repository = Substitute.For<IBookRepository>();
        _handler = new DeleteBookHandler(_repository);
    }

    [Fact]
    public async Task Handle_ExistingBook_DeletesAndReturnsSuccess()
    {
        var book = new Book
        {
            Id = 1, Title = "Foundation", Author = "Asimov",
            ISBN = "978-0553293357", Genre = Genre.Science,
            PublishedDate = new DateOnly(1951, 1, 1), CreatedAt = DateTime.UtcNow
        };

        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(book);

        var result = await _handler.Handle(new DeleteBookCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _repository.Received(1).DeleteAsync(book, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NonExistentBook_ReturnsFailure()
    {
        _repository.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((Book?)null);

        var result = await _handler.Handle(new DeleteBookCommand(999), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Contain("not found");
    }
}
