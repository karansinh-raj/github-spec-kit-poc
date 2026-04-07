using BookShelf.Application.Books.Commands.UpdateBook;
using BookShelf.Application.Books.DTOs;
using BookShelf.Application.Common.Interfaces;
using BookShelf.Domain.Entities;
using BookShelf.Domain.Enums;
using FluentAssertions;
using NSubstitute;

namespace BookShelf.Application.Tests.Books.Commands;

public class UpdateBookHandlerTests
{
    private readonly IBookRepository _repository;
    private readonly UpdateBookHandler _handler;

    public UpdateBookHandlerTests()
    {
        _repository = Substitute.For<IBookRepository>();
        _handler = new UpdateBookHandler(_repository);
    }

    [Fact]
    public async Task Handle_ExistingBook_UpdatesAndReturnsSuccess()
    {
        var existingBook = new Book
        {
            Id = 1, Title = "Old Title", Author = "Old Author",
            ISBN = "978-0553293357", PublishedDate = new DateOnly(1951, 1, 1),
            Genre = Genre.Science, CreatedAt = DateTime.UtcNow
        };

        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(existingBook);
        _repository.ExistsByIsbnAsync("978-0553293357", 1, Arg.Any<CancellationToken>()).Returns(false);

        var request = new UpdateBookRequest("New Title", "New Author", "978-0553293357",
            new DateOnly(1951, 1, 1), "Fiction", "Updated description");
        var command = new UpdateBookCommand(1, request);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Title.Should().Be("New Title");
        result.Value.Author.Should().Be("New Author");
    }

    [Fact]
    public async Task Handle_NonExistentBook_ReturnsFailure()
    {
        _repository.GetByIdAsync(999, Arg.Any<CancellationToken>()).Returns((Book?)null);

        var request = new UpdateBookRequest("Title", "Author", "978-0000000000",
            new DateOnly(2020, 1, 1), "Fiction", null);
        var command = new UpdateBookCommand(999, request);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Contain("not found");
    }

    [Fact]
    public async Task Handle_DuplicateIsbn_ReturnsFailure()
    {
        var existingBook = new Book
        {
            Id = 1, Title = "Title", Author = "Author",
            ISBN = "978-0553293357", PublishedDate = new DateOnly(1951, 1, 1),
            Genre = Genre.Science, CreatedAt = DateTime.UtcNow
        };

        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(existingBook);
        _repository.ExistsByIsbnAsync("978-9999999999", 1, Arg.Any<CancellationToken>()).Returns(true);

        var request = new UpdateBookRequest("Title", "Author", "978-9999999999",
            new DateOnly(2020, 1, 1), "Fiction", null);
        var command = new UpdateBookCommand(1, request);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Contain("already exists");
    }
}
