using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using BookShelf.Application.ReadingLists.DTOs;
using MediatR;

namespace BookShelf.Application.ReadingLists.Queries.GetReadingLists;

public class GetReadingListsHandler : IRequestHandler<GetReadingListsQuery, Result<List<ReadingListDto>>>
{
    private readonly IReadingListRepository _repository;

    public GetReadingListsHandler(IReadingListRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<ReadingListDto>>> Handle(GetReadingListsQuery query, CancellationToken cancellationToken)
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

        return Result<List<ReadingListDto>>.Success(dtos);
    }
}
