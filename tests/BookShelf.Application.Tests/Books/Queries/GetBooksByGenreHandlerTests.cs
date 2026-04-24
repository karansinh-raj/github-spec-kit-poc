using BookShelf.Application.Books.Queries.GetBooksByGenre;
using BookShelf.Application.Common.Interfaces;
using BookShelf.Domain.Entities;
using BookShelf.Domain.Enums;
using FluentAssertions;
using NSubstitute;

namespace BookShelf.Application.Tests.Books.Queries;

public class GetBooksByGenreHandlerTests
{
    private readonly IBookRepository _repository;
    private readonly GetBooksByGenreHandler _handler;

    public GetBooksByGenreHandlerTests()
    {
        _repository = Substitute.For<IBookRepository>();
        _handler = new GetBooksByGenreHandler(_repository);
    }

    [Fact]
    public async Task Handle_ValidGenre_ReturnsMatchingBooks()
    {
        var books = new List<Book>
        {
            new() { Id = 1, Title = "Dune", Author = "Frank Herbert", ISBN = "978-0441013593",
                    Genre = Genre.Fiction, PublishedDate = new DateOnly(1965, 8, 1), CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Title = "Foundation", Author = "Isaac Asimov", ISBN = "978-0553293357",
                    Genre = Genre.Fiction, PublishedDate = new DateOnly(1951, 1, 1), CreatedAt = DateTime.UtcNow }
        };

        _repository.GetByGenreAsync(Genre.Fiction, Arg.Any<CancellationToken>()).Returns(books);

        var result = await _handler.Handle(new GetBooksByGenreQuery("Fiction"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value!.Should().AllSatisfy(b => b.Genre.Should().Be("Fiction"));
    }

    [Fact]
    public async Task Handle_ValidGenreCaseInsensitive_ReturnsBooks()
    {
        var books = new List<Book>
        {
            new() { Id = 3, Title = "A Brief History of Time", Author = "Stephen Hawking", ISBN = "978-0553380163",
                    Genre = Genre.Science, PublishedDate = new DateOnly(1988, 1, 1), CreatedAt = DateTime.UtcNow }
        };

        _repository.GetByGenreAsync(Genre.Science, Arg.Any<CancellationToken>()).Returns(books);

        var result = await _handler.Handle(new GetBooksByGenreQuery("science"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value![0].Title.Should().Be("A Brief History of Time");
    }

    [Fact]
    public async Task Handle_ValidGenreWithNoBooks_ReturnsEmptyList()
    {
        _repository.GetByGenreAsync(Genre.Romance, Arg.Any<CancellationToken>()).Returns([]);

        var result = await _handler.Handle(new GetBooksByGenreQuery("Romance"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_InvalidGenre_ReturnsFailure()
    {
        var result = await _handler.Handle(new GetBooksByGenreQuery("InvalidGenre"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.Should().Contain("InvalidGenre").And.Contain("not valid");

        await _repository.DidNotReceive().GetByGenreAsync(Arg.Any<Genre>(), Arg.Any<CancellationToken>());
    }
}
