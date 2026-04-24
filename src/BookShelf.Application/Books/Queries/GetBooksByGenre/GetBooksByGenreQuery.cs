using BookShelf.Application.Books.DTOs;
using BookShelf.Application.Common.Models;
using MediatR;

namespace BookShelf.Application.Books.Queries.GetBooksByGenre;

public record GetBooksByGenreQuery(string Genre) : IRequest<Result<List<BookDto>>>;
