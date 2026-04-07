namespace BookShelf.Application.ReadingLists.DTOs;

public record ReadingListDetailDto(
    int Id,
    string Name,
    string? Description,
    List<ReadingListBookDto> Books,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
