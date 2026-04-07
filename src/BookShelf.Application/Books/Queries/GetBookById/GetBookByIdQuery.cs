using BookShelf.Application.Books.DTOs;
using BookShelf.Application.Common.Models;
using MediatR;

namespace BookShelf.Application.Books.Queries.GetBookById;

public record GetBookByIdQuery(int Id) : IRequest<Result<BookDto>>;
