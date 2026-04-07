using BookShelf.Application.Books.DTOs;
using BookShelf.Application.Common.Models;
using MediatR;

namespace BookShelf.Application.Books.Commands.CreateBook;

public record CreateBookCommand(CreateBookRequest Request) : IRequest<Result<BookDto>>;
