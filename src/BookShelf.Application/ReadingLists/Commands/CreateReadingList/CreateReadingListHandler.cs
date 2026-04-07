using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using BookShelf.Application.ReadingLists.DTOs;
using BookShelf.Domain.Entities;
using MediatR;

namespace BookShelf.Application.ReadingLists.Commands.CreateReadingList;

public class CreateReadingListHandler : IRequestHandler<CreateReadingListCommand, Result<ReadingListDto>>
{
    private readonly IReadingListRepository _repository;

    public CreateReadingListHandler(IReadingListRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ReadingListDto>> Handle(CreateReadingListCommand command, CancellationToken cancellationToken)
    {
        if (await _repository.ExistsByNameAsync(command.Request.Name, cancellationToken: cancellationToken))
            return Result<ReadingListDto>.Failure("A reading list with this name already exists");

        var readingList = new ReadingList
        {
            Name = command.Request.Name,
            Description = command.Request.Description,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.AddAsync(readingList, cancellationToken);

        var dto = new ReadingListDto(
            created.Id, created.Name, created.Description,
            0, 0, created.CreatedAt, created.UpdatedAt);

        return Result<ReadingListDto>.Success(dto);
    }
}
