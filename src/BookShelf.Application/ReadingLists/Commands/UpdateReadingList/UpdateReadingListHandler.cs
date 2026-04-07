using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using BookShelf.Application.ReadingLists.DTOs;
using MediatR;

namespace BookShelf.Application.ReadingLists.Commands.UpdateReadingList;

public class UpdateReadingListHandler : IRequestHandler<UpdateReadingListCommand, Result<ReadingListDto>>
{
    private readonly IReadingListRepository _repository;

    public UpdateReadingListHandler(IReadingListRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ReadingListDto>> Handle(UpdateReadingListCommand command, CancellationToken cancellationToken)
    {
        var readingList = await _repository.GetByIdWithBooksAsync(command.Id, cancellationToken);
        if (readingList is null)
            return Result<ReadingListDto>.Failure("Reading list not found");

        if (await _repository.ExistsByNameAsync(command.Request.Name, command.Id, cancellationToken))
            return Result<ReadingListDto>.Failure("A reading list with this name already exists");

        readingList.Name = command.Request.Name;
        readingList.Description = command.Request.Description;
        readingList.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(readingList, cancellationToken);

        var bookCount = readingList.ReadingListBooks.Count;
        var booksRead = readingList.ReadingListBooks.Count(b => b.IsRead);
        var percentage = bookCount > 0 ? Math.Round((double)booksRead / bookCount * 100, 1) : 0;

        var dto = new ReadingListDto(
            readingList.Id, readingList.Name, readingList.Description,
            bookCount, percentage, readingList.CreatedAt, readingList.UpdatedAt);

        return Result<ReadingListDto>.Success(dto);
    }
}
