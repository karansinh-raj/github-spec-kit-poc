using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.ReadingLists.Commands.AddBookToList;
using BookShelf.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace BookShelf.Application.Tests.ReadingLists.Commands;

public class AddBookToListHandlerTests
{
    private readonly IReadingListRepository _readingListRepository = Substitute.For<IReadingListRepository>();
    private readonly IBookRepository _bookRepository = Substitute.For<IBookRepository>();
    private readonly AddBookToListHandler _handler;

    public AddBookToListHandlerTests()
    {
        _handler = new AddBookToListHandler(_readingListRepository, _bookRepository);
    }

    [Fact]
    public async Task Handle_ValidRequest_AddsBookAndReturnsDto()
    {
        var list = new ReadingList { Id = 1, Name = "Test" };
        var book = new Book { Id = 10, Title = "Clean Code", Author = "Robert C. Martin" };

        _readingListRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(list);
        _bookRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(book);
        _readingListRepository.GetReadingListBookAsync(1, 10, Arg.Any<CancellationToken>()).Returns((ReadingListBook?)null);
        _readingListRepository.AddBookToListAsync(Arg.Any<ReadingListBook>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<ReadingListBook>());

        var result = await _handler.Handle(new AddBookToListCommand(1, 10), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.BookId.Should().Be(10);
        result.Value.Title.Should().Be("Clean Code");
        result.Value.IsRead.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_BookAlreadyInList_ReturnsConflict()
    {
        var list = new ReadingList { Id = 1, Name = "Test" };
        var book = new Book { Id = 10, Title = "Clean Code", Author = "Robert C. Martin" };
        var existing = new ReadingListBook { ReadingListId = 1, BookId = 10 };

        _readingListRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(list);
        _bookRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(book);
        _readingListRepository.GetReadingListBookAsync(1, 10, Arg.Any<CancellationToken>()).Returns(existing);

        var result = await _handler.Handle(new AddBookToListCommand(1, 10), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("already"));
    }

    [Fact]
    public async Task Handle_ListNotFound_ReturnsFailure()
    {
        _readingListRepository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((ReadingList?)null);

        var result = await _handler.Handle(new AddBookToListCommand(99, 10), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Reading list not found"));
    }

    [Fact]
    public async Task Handle_BookNotFound_ReturnsFailure()
    {
        var list = new ReadingList { Id = 1, Name = "Test" };
        _readingListRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(list);
        _bookRepository.GetByIdAsync(99, Arg.Any<CancellationToken>()).Returns((Book?)null);

        var result = await _handler.Handle(new AddBookToListCommand(1, 99), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Book not found"));
    }
}
