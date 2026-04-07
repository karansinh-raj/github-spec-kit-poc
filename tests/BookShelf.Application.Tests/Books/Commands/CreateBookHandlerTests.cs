using BookShelf.Application.Books.Commands.CreateBook;
using BookShelf.Application.Books.DTOs;
using BookShelf.Application.Common.Interfaces;
using BookShelf.Domain.Entities;
using BookShelf.Domain.Enums;
using FluentAssertions;
using NSubstitute;

namespace BookShelf.Application.Tests.Books.Commands;

public class CreateBookHandlerTests
{
    private readonly IBookRepository _repository;
    private readonly CreateBookHandler _handler;

    public CreateBookHandlerTests()
    {
        _repository = Substitute.For<IBookRepository>();
        _handler = new CreateBookHandler(_repository);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccessWithBookDto()
    {
        var request = new CreateBookRequest("Foundation", "Isaac Asimov", "978-0553293357",
            new DateOnly(1951, 1, 1), "Science", "A classic sci-fi novel");
        var command = new CreateBookCommand(request);

        _repository.ExistsByIsbnAsync(request.ISBN, null, Arg.Any<CancellationToken>())
            .Returns(false);
        _repository.AddAsync(Arg.Any<Book>(), Arg.Any<CancellationToken>())
            .Returns(ci =>
            {
                var book = ci.Arg<Book>();
                book.Id = 1;
                return book;
            });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Title.Should().Be("Foundation");
        result.Value.Author.Should().Be("Isaac Asimov");
        result.Value.Id.Should().Be(1);
    }

    [Fact]
    public async Task Handle_DuplicateIsbn_ReturnsFailure()
    {
        var request = new CreateBookRequest("Foundation", "Isaac Asimov", "978-0553293357",
            new DateOnly(1951, 1, 1), "Science", null);
        var command = new CreateBookCommand(request);

        _repository.ExistsByIsbnAsync(request.ISBN, null, Arg.Any<CancellationToken>())
            .Returns(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Contain("already exists");
    }
}
