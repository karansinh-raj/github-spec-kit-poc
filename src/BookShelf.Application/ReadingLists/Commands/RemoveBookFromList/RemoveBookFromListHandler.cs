using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using MediatR;

namespace BookShelf.Application.ReadingLists.Commands.RemoveBookFromList;

public class RemoveBookFromListHandler : IRequestHandler<RemoveBookFromListCommand, Result<bool>>
{
    private readonly IReadingListRepository _repository;

    public RemoveBookFromListHandler(IReadingListRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<bool>> Handle(RemoveBookFromListCommand command, CancellationToken cancellationToken)
    {
        var entry = await _repository.GetReadingListBookAsync(command.ListId, command.BookId, cancellationToken);
        if (entry is null)
            return Result<bool>.Failure("Book not found in this reading list");

        await _repository.RemoveBookFromListAsync(entry, cancellationToken);
        return Result<bool>.Success(true);
    }
}
