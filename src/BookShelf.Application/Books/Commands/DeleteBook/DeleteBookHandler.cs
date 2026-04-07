using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using MediatR;

namespace BookShelf.Application.Books.Commands.DeleteBook;

public class DeleteBookHandler : IRequestHandler<DeleteBookCommand, Result<bool>>
{
    private readonly IBookRepository _repository;

    public DeleteBookHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<bool>> Handle(DeleteBookCommand command, CancellationToken cancellationToken)
    {
        var book = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (book is null)
            return Result<bool>.Failure($"Book with Id '{command.Id}' was not found.");

        await _repository.DeleteAsync(book, cancellationToken);
        return Result<bool>.Success(true);
    }
}
