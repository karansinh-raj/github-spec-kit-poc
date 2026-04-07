using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using BookShelf.Application.ReadingLists.DTOs;
using MediatR;

namespace BookShelf.Application.ReadingLists.Queries.GetReadingListById;

public class GetReadingListByIdHandler : IRequestHandler<GetReadingListByIdQuery, Result<ReadingListDetailDto>>
{
    private readonly IReadingListRepository _repository;

    public GetReadingListByIdHandler(IReadingListRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ReadingListDetailDto>> Handle(GetReadingListByIdQuery query, CancellationToken cancellationToken)
    {
        var readingList = await _repository.GetByIdWithBooksAsync(query.Id, cancellationToken);
        if (readingList is null)
            return Result<ReadingListDetailDto>.Failure("Reading list not found");

        var books = readingList.ReadingListBooks.Select(rlb => new ReadingListBookDto(
            rlb.BookId, rlb.Book.Title, rlb.Book.Author,
            rlb.IsRead, rlb.Notes, rlb.CompletedDate, rlb.AddedAt)).ToList();

        var dto = new ReadingListDetailDto(
            readingList.Id, readingList.Name, readingList.Description,
            books, readingList.CreatedAt, readingList.UpdatedAt);

        return Result<ReadingListDetailDto>.Success(dto);
    }
}
