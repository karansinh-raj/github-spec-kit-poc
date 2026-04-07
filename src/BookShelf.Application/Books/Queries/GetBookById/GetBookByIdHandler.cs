using BookShelf.Application.Books.DTOs;
using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using MediatR;

namespace BookShelf.Application.Books.Queries.GetBookById;

public class GetBookByIdHandler : IRequestHandler<GetBookByIdQuery, Result<BookDto>>
{
    private readonly IBookRepository _repository;

    public GetBookByIdHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<BookDto>> Handle(GetBookByIdQuery query, CancellationToken cancellationToken)
    {
        var book = await _repository.GetByIdAsync(query.Id, cancellationToken);
        if (book is null)
            return Result<BookDto>.Failure($"Book with Id '{query.Id}' was not found.");

        return Result<BookDto>.Success(new BookDto(
            book.Id, book.Title, book.Author, book.ISBN,
            book.PublishedDate, book.Genre.ToString(),
            book.Description, book.CreatedAt, book.UpdatedAt));
    }
}
