using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using BookShelf.Application.ReadingLists.DTOs;
using BookShelf.Domain.Entities;
using MediatR;

namespace BookShelf.Application.ReadingLists.Commands.AddBookToList;

public class AddBookToListHandler : IRequestHandler<AddBookToListCommand, Result<ReadingListBookDto>>
{
    private readonly IReadingListRepository _readingListRepository;
    private readonly IBookRepository _bookRepository;

    public AddBookToListHandler(IReadingListRepository readingListRepository, IBookRepository bookRepository)
    {
        _readingListRepository = readingListRepository;
        _bookRepository = bookRepository;
    }

    public async Task<Result<ReadingListBookDto>> Handle(AddBookToListCommand command, CancellationToken cancellationToken)
    {
        var readingList = await _readingListRepository.GetByIdAsync(command.ListId, cancellationToken);
        if (readingList is null)
            return Result<ReadingListBookDto>.Failure("Reading list not found");

        var book = await _bookRepository.GetByIdAsync(command.BookId, cancellationToken);
        if (book is null)
            return Result<ReadingListBookDto>.Failure("Book not found");

        var existing = await _readingListRepository.GetReadingListBookAsync(command.ListId, command.BookId, cancellationToken);
        if (existing is not null)
            return Result<ReadingListBookDto>.Failure("Book is already in this reading list");

        var entry = new ReadingListBook
        {
            ReadingListId = command.ListId,
            BookId = command.BookId,
            IsRead = false,
            AddedAt = DateTime.UtcNow
        };

        var created = await _readingListRepository.AddBookToListAsync(entry, cancellationToken);

        var dto = new ReadingListBookDto(
            book.Id, book.Title, book.Author,
            created.IsRead, created.Notes, created.CompletedDate, created.AddedAt);

        return Result<ReadingListBookDto>.Success(dto);
    }
}
