namespace BookShelf.Application.ReadingLists.DTOs;

public record UpdateReadStatusRequest(bool IsRead, string? Notes, DateOnly? CompletedDate);
