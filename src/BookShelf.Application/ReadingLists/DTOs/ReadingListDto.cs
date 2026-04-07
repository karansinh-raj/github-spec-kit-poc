namespace BookShelf.Application.ReadingLists.DTOs;

public record ReadingListDto(
    int Id,
    string Name,
    string? Description,
    int BookCount,
    double CompletionPercentage,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
