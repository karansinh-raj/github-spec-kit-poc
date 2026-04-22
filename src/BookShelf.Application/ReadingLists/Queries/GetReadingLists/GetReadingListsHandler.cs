using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using BookShelf.Application.ReadingLists.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;

namespace BookShelf.Application.ReadingLists.Queries.GetReadingLists;

public class GetReadingListsHandler : IRequestHandler<GetReadingListsQuery, Result<List<ReadingListDto>>>
{
    private readonly IReadingListRepository _repository;
    private readonly ILogger<GetReadingListsHandler> _logger;

    public GetReadingListsHandler(IReadingListRepository repository, ILogger<GetReadingListsHandler>? logger = null)
    {
        _repository = repository;
        _logger = logger ?? NullLogger<GetReadingListsHandler>.Instance;
    }

    public async Task<Result<List<ReadingListDto>>> Handle(GetReadingListsQuery query, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Handling GetReadingListsQuery");
        try
        {
            var lists = await _repository.GetAllWithBooksAsync(cancellationToken);

            var dtos = lists.Select(l =>
            {
                var bookCount = l.ReadingListBooks.Count;
                var booksRead = l.ReadingListBooks.Count(b => b.IsRead);
                var percentage = bookCount > 0 ? Math.Round((double)booksRead / bookCount * 100, 1) : 0;

                return new ReadingListDto(
                    l.Id, l.Name, l.Description,
                    bookCount, percentage, l.CreatedAt, l.UpdatedAt);
            }).ToList();

            _logger.LogInformation("GetReadingListsQuery succeeded returning {Count} reading lists", dtos.Count);
            return Result<List<ReadingListDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while handling GetReadingListsQuery");
            throw;
        }
        finally
        {
            _logger.LogInformation("Handled GetReadingListsQuery in {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
        }
    }
}
