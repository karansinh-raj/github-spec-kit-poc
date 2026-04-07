using BookShelf.Application.Books.DTOs;
using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using BookShelf.Domain.Entities;
using BookShelf.Domain.Enums;
using MediatR;

namespace BookShelf.Application.Books.Commands.CreateBook;

public class CreateBookHandler : IRequestHandler<CreateBookCommand, Result<BookDto>>
{
    private readonly IBookRepository _repository;

    public CreateBookHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<BookDto>> Handle(CreateBookCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        if (await _repository.ExistsByIsbnAsync(request.ISBN, cancellationToken: cancellationToken))
            return Result<BookDto>.Failure($"A book with ISBN '{request.ISBN}' already exists.");

        var book = new Book
        {
            Title = request.Title,
            Author = request.Author,
            ISBN = request.ISBN,
            PublishedDate = request.PublishedDate,
            Genre = Enum.Parse<Genre>(request.Genre, ignoreCase: true),
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.AddAsync(book, cancellationToken);

        return Result<BookDto>.Success(MapToDto(created));
    }

    private static BookDto MapToDto(Book book) => new(
        book.Id, book.Title, book.Author, book.ISBN,
        book.PublishedDate, book.Genre.ToString(),
        book.Description, book.CreatedAt, book.UpdatedAt);
}
