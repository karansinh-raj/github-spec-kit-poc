using BookShelf.Application.Common.Models;
using MediatR;

namespace BookShelf.Application.Books.Commands.DeleteBook;

public record DeleteBookCommand(int Id) : IRequest<Result<bool>>;
