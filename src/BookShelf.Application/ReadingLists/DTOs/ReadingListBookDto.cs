namespace BookShelf.Application.ReadingLists.DTOs;

public record ReadingListBookDto(
    int BookId,
    string Title,
    string Author,
    bool IsRead,
    string? Notes,
    DateOnly? CompletedDate,
    DateTime AddedAt);
