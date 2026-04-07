using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using MediatR;

namespace BookShelf.Application.ReadingLists.Commands.DeleteReadingList;

public class DeleteReadingListHandler : IRequestHandler<DeleteReadingListCommand, Result<bool>>
{
    private readonly IReadingListRepository _repository;

    public DeleteReadingListHandler(IReadingListRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<bool>> Handle(DeleteReadingListCommand command, CancellationToken cancellationToken)
    {
        var readingList = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (readingList is null)
            return Result<bool>.Failure("Reading list not found");

        await _repository.DeleteAsync(readingList, cancellationToken);
        return Result<bool>.Success(true);
    }
}
