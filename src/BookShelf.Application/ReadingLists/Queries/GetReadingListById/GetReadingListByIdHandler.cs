using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using BookShelf.Application.ReadingLists.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;

namespace BookShelf.Application.ReadingLists.Queries.GetReadingListById;

public class GetReadingListByIdHandler : IRequestHandler<GetReadingListByIdQuery, Result<ReadingListDetailDto>>
{
    private readonly IReadingListRepository _repository;
    private readonly ILogger<GetReadingListByIdHandler> _logger;

    public GetReadingListByIdHandler(IReadingListRepository repository, ILogger<GetReadingListByIdHandler>? logger = null)
    {
        _repository = repository;
        _logger = logger ?? NullLogger<GetReadingListByIdHandler>.Instance;
    }

    public async Task<Result<ReadingListDetailDto>> Handle(GetReadingListByIdQuery query, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Handling GetReadingListByIdQuery for ReadingListId {ListId}", query.Id);
        try
        {
            var readingList = await _repository.GetByIdWithBooksAsync(query.Id, cancellationToken);
            if (readingList is null)
            {
                _logger.LogWarning("GetReadingListByIdQuery did not find list {ListId}", query.Id);
                return Result<ReadingListDetailDto>.Failure("Reading list not found");
            }

            var books = readingList.ReadingListBooks.Select(rlb => new ReadingListBookDto(
                rlb.BookId, rlb.Book.Title, rlb.Book.Author,
                rlb.IsRead, rlb.Notes, rlb.CompletedDate, rlb.AddedAt)).ToList();

            var dto = new ReadingListDetailDto(
                readingList.Id, readingList.Name, readingList.Description,
                books, readingList.CreatedAt, readingList.UpdatedAt);

            _logger.LogInformation("GetReadingListByIdQuery succeeded for ReadingListId {ListId}", query.Id);
            return Result<ReadingListDetailDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while handling GetReadingListByIdQuery for ReadingListId {ListId}", query.Id);
            throw;
        }
        finally
        {
            _logger.LogInformation("Handled GetReadingListByIdQuery for ReadingListId {ListId} in {ElapsedMilliseconds}ms", query.Id, stopwatch.ElapsedMilliseconds);
        }
    }
}
