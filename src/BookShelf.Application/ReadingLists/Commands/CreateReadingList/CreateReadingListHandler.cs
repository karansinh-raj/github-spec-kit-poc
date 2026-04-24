using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using BookShelf.Application.ReadingLists.DTOs;
using BookShelf.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;

namespace BookShelf.Application.ReadingLists.Commands.CreateReadingList;

public class CreateReadingListHandler : IRequestHandler<CreateReadingListCommand, Result<ReadingListDto>>
{
    private readonly IReadingListRepository _repository;
    private readonly ILogger<CreateReadingListHandler> _logger;

    public CreateReadingListHandler(IReadingListRepository repository, ILogger<CreateReadingListHandler>? logger = null)
    {
        _repository = repository;
        _logger = logger ?? NullLogger<CreateReadingListHandler>.Instance;
    }

    public async Task<Result<ReadingListDto>> Handle(CreateReadingListCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Handling CreateReadingListCommand for Name {ListName}", command.Request.Name);
        try
        {
            if (await _repository.ExistsByNameAsync(command.Request.Name, cancellationToken: cancellationToken))
            {
                _logger.LogWarning("CreateReadingListCommand rejected due to duplicate Name {ListName}", command.Request.Name);
                return Result<ReadingListDto>.Failure("A reading list with this name already exists");
            }

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

            _logger.LogInformation("CreateReadingListCommand succeeded with ReadingListId {ListId}", created.Id);
            return Result<ReadingListDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while handling CreateReadingListCommand for Name {ListName}", command.Request.Name);
            throw;
        }
        finally
        {
            _logger.LogInformation("Handled CreateReadingListCommand for Name {ListName} in {ElapsedMilliseconds}ms", command.Request.Name, stopwatch.ElapsedMilliseconds);
        }
    }
}
