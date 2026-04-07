using BookShelf.Application.Books.DTOs;
using BookShelf.Application.Common.Models;
using MediatR;

namespace BookShelf.Application.Books.Commands.UpdateBook;

public record UpdateBookCommand(int Id, UpdateBookRequest Request) : IRequest<Result<BookDto>>;
