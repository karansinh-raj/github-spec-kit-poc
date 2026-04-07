using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.ReadingLists.Commands.UpdateReadStatus;
using BookShelf.Application.ReadingLists.DTOs;
using BookShelf.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace BookShelf.Application.Tests.ReadingLists.Commands;

public class UpdateReadStatusHandlerTests
{
    private readonly IReadingListRepository _readingListRepository = Substitute.For<IReadingListRepository>();
    private readonly IBookRepository _bookRepository = Substitute.For<IBookRepository>();
    private readonly UpdateReadStatusHandler _handler;

    public UpdateReadStatusHandlerTests()
    {
        _handler = new UpdateReadStatusHandler(_readingListRepository, _bookRepository);
    }

    [Fact]
    public async Task Handle_MarkAsRead_SetsStatusNotesAndDate()
    {
        var entry = new ReadingListBook { ReadingListId = 1, BookId = 10, IsRead = false, AddedAt = DateTime.UtcNow };
        var book = new Book { Id = 10, Title = "Clean Code", Author = "Robert C. Martin" };
        _readingListRepository.GetReadingListBookAsync(1, 10, Arg.Any<CancellationToken>()).Returns(entry);
        _bookRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(book);

        var request = new UpdateReadStatusRequest(true, "Loved it!", new DateOnly(2026, 4, 1));
        var result = await _handler.Handle(new UpdateReadStatusCommand(1, 10, request), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.IsRead.Should().BeTrue();
        result.Value.Notes.Should().Be("Loved it!");
        result.Value.CompletedDate.Should().Be(new DateOnly(2026, 4, 1));
    }

    [Fact]
    public async Task Handle_MarkAsUnread_ClearsNotesAndDate()
    {
        var entry = new ReadingListBook
        {
            ReadingListId = 1, BookId = 10, IsRead = true,
            Notes = "Old notes", CompletedDate = new DateOnly(2026, 1, 1), AddedAt = DateTime.UtcNow
        };
        var book = new Book { Id = 10, Title = "Clean Code", Author = "Robert C. Martin" };
        _readingListRepository.GetReadingListBookAsync(1, 10, Arg.Any<CancellationToken>()).Returns(entry);
        _bookRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(book);

        var request = new UpdateReadStatusRequest(false, null, null);
        var result = await _handler.Handle(new UpdateReadStatusCommand(1, 10, request), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.IsRead.Should().BeFalse();
        result.Value.Notes.Should().BeNull();
        result.Value.CompletedDate.Should().BeNull();
    }

    [Fact]
    public async Task Handle_BookNotInList_ReturnsFailure()
    {
        _readingListRepository.GetReadingListBookAsync(1, 99, Arg.Any<CancellationToken>()).Returns((ReadingListBook?)null);

        var request = new UpdateReadStatusRequest(true, null, null);
        var result = await _handler.Handle(new UpdateReadStatusCommand(1, 99, request), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("not found"));
    }
}
