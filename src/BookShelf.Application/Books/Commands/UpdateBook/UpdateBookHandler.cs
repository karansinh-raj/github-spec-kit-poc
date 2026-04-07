using BookShelf.Application.Books.DTOs;
using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using BookShelf.Domain.Enums;
using MediatR;

namespace BookShelf.Application.Books.Commands.UpdateBook;

public class UpdateBookHandler : IRequestHandler<UpdateBookCommand, Result<BookDto>>
{
    private readonly IBookRepository _repository;

    public UpdateBookHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<BookDto>> Handle(UpdateBookCommand command, CancellationToken cancellationToken)
    {
        var book = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (book is null)
            return Result<BookDto>.Failure($"Book with Id '{command.Id}' was not found.");

        var request = command.Request;

        if (await _repository.ExistsByIsbnAsync(request.ISBN, excludeId: command.Id, cancellationToken: cancellationToken))
            return Result<BookDto>.Failure($"A book with ISBN '{request.ISBN}' already exists.");

        book.Title = request.Title;
        book.Author = request.Author;
        book.ISBN = request.ISBN;
        book.PublishedDate = request.PublishedDate;
        book.Genre = Enum.Parse<Genre>(request.Genre, ignoreCase: true);
        book.Description = request.Description;
        book.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(book, cancellationToken);

        return Result<BookDto>.Success(new BookDto(
            book.Id, book.Title, book.Author, book.ISBN,
            book.PublishedDate, book.Genre.ToString(),
            book.Description, book.CreatedAt, book.UpdatedAt));
    }
}
