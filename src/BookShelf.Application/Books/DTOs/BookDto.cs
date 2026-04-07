using BookShelf.Domain.Enums;

namespace BookShelf.Application.Books.DTOs;

public record BookDto(
    int Id,
    string Title,
    string Author,
    string ISBN,
    DateOnly PublishedDate,
    string Genre,
    string? Description,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
