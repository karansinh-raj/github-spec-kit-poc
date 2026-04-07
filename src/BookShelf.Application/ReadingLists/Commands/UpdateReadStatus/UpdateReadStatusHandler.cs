using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using BookShelf.Application.ReadingLists.DTOs;
using MediatR;

namespace BookShelf.Application.ReadingLists.Commands.UpdateReadStatus;

public class UpdateReadStatusHandler : IRequestHandler<UpdateReadStatusCommand, Result<ReadingListBookDto>>
{
    private readonly IReadingListRepository _readingListRepository;
    private readonly IBookRepository _bookRepository;

    public UpdateReadStatusHandler(IReadingListRepository readingListRepository, IBookRepository bookRepository)
    {
        _readingListRepository = readingListRepository;
        _bookRepository = bookRepository;
    }

    public async Task<Result<ReadingListBookDto>> Handle(UpdateReadStatusCommand command, CancellationToken cancellationToken)
    {
        var entry = await _readingListRepository.GetReadingListBookAsync(command.ListId, command.BookId, cancellationToken);
        if (entry is null)
            return Result<ReadingListBookDto>.Failure("Book not found in this reading list");

        entry.IsRead = command.Request.IsRead;

        if (command.Request.IsRead)
        {
            entry.Notes = command.Request.Notes;
            entry.CompletedDate = command.Request.CompletedDate;
        }
        else
        {
            entry.Notes = null;
            entry.CompletedDate = null;
        }

        await _readingListRepository.UpdateReadingListBookAsync(entry, cancellationToken);

        var book = await _bookRepository.GetByIdAsync(command.BookId, cancellationToken);

        var dto = new ReadingListBookDto(
            command.BookId, book!.Title, book.Author,
            entry.IsRead, entry.Notes, entry.CompletedDate, entry.AddedAt);

        return Result<ReadingListBookDto>.Success(dto);
    }
}
