using BookShelf.Application.Books.Queries.GetBooks;
using BookShelf.Application.Common.Interfaces;
using BookShelf.Domain.Entities;
using BookShelf.Domain.Enums;
using FluentAssertions;
using NSubstitute;

namespace BookShelf.Application.Tests.Books.Queries;

public class GetBooksHandlerTests
{
    private readonly IBookRepository _repository;
    private readonly GetBooksHandler _handler;

    public GetBooksHandlerTests()
    {
        _repository = Substitute.For<IBookRepository>();
        _handler = new GetBooksHandler(_repository);
    }

    [Fact]
    public async Task Handle_ReturnsPagedResult()
    {
        var books = new List<Book>
        {
            new() { Id = 1, Title = "Book A", Author = "Author", ISBN = "111", Genre = Genre.Fiction, PublishedDate = new DateOnly(2020, 1, 1), CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Title = "Book B", Author = "Author", ISBN = "222", Genre = Genre.Science, PublishedDate = new DateOnly(2021, 1, 1), CreatedAt = DateTime.UtcNow }
        };

        _repository.GetAllAsync(1, 10, null, null, null, "title", "asc", Arg.Any<CancellationToken>())
            .Returns((books, 2));

        var result = await _handler.Handle(new GetBooksQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(2);
        result.Value.TotalCount.Should().Be(2);
        result.Value.CurrentPage.Should().Be(1);
    }

    [Fact]
    public async Task Handle_CapsPageSizeAt50()
    {
        _repository.GetAllAsync(1, 50, null, null, null, "title", "asc", Arg.Any<CancellationToken>())
            .Returns((new List<Book>(), 0));

        var query = new GetBooksQuery(PageSize: 100);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.PageSize.Should().Be(50);
    }

    [Fact]
    public async Task Handle_CalculatesTotalPagesCorrectly()
    {
        _repository.GetAllAsync(1, 10, null, null, null, "title", "asc", Arg.Any<CancellationToken>())
            .Returns((new List<Book>(), 25));

        var result = await _handler.Handle(new GetBooksQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TotalPages.Should().Be(3);
    }
}
