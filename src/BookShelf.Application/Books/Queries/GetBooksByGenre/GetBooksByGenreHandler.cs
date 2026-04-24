using BookShelf.Application.Books.DTOs;
using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using BookShelf.Domain.Enums;
using MediatR;

namespace BookShelf.Application.Books.Queries.GetBooksByGenre;

public class GetBooksByGenreHandler : IRequestHandler<GetBooksByGenreQuery, Result<List<BookDto>>>
{
    private readonly IBookRepository _repository;

    public GetBooksByGenreHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<BookDto>>> Handle(GetBooksByGenreQuery query, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<Genre>(query.Genre, ignoreCase: true, out var genre))
            return Result<List<BookDto>>.Failure(
                $"Genre '{query.Genre}' is not valid. Must be one of: {string.Join(", ", Enum.GetNames<Genre>())}.");

        var books = await _repository.GetByGenreAsync(genre, cancellationToken);

        var dtos = books.Select(b => new BookDto(
            b.Id, b.Title, b.Author, b.ISBN,
            b.PublishedDate, b.Genre.ToString(),
            b.Description, b.CreatedAt, b.UpdatedAt)).ToList();

        return Result<List<BookDto>>.Success(dtos);
    }
}
