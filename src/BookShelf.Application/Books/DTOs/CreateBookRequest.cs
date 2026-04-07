namespace BookShelf.Application.Books.DTOs;

public record CreateBookRequest(
    string Title,
    string Author,
    string ISBN,
    DateOnly PublishedDate,
    string Genre,
    string? Description);
