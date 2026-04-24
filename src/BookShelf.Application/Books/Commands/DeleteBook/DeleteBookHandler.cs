using BookShelf.Application.Common.Interfaces;
using BookShelf.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;

namespace BookShelf.Application.Books.Commands.DeleteBook;

public class DeleteBookHandler : IRequestHandler<DeleteBookCommand, Result<bool>>
{
    private readonly IBookRepository _repository;
    private readonly ILogger<DeleteBookHandler> _logger;

    public DeleteBookHandler(IBookRepository repository, ILogger<DeleteBookHandler>? logger = null)
    {
        _repository = repository;
        _logger = logger ?? NullLogger<DeleteBookHandler>.Instance;
    }

    public async Task<Result<bool>> Handle(DeleteBookCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Handling DeleteBookCommand for BookId {BookId}", command.Id);
        try
        {
            var book = await _repository.GetByIdAsync(command.Id, cancellationToken);
            if (book is null)
            {
                _logger.LogWarning("DeleteBookCommand failed because book {BookId} was not found", command.Id);
                return Result<bool>.Failure($"Book with Id '{command.Id}' was not found.");
            }

            await _repository.DeleteAsync(book, cancellationToken);
            _logger.LogInformation("DeleteBookCommand succeeded for BookId {BookId}", command.Id);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception while handling DeleteBookCommand for BookId {BookId}", command.Id);
            throw;
        }
        finally
        {
            _logger.LogInformation("Handled DeleteBookCommand for BookId {BookId} in {ElapsedMilliseconds}ms", command.Id, stopwatch.ElapsedMilliseconds);
        }
    }
}
