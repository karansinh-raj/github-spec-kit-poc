namespace BookShelf.Application.ReadingLists.DTOs;

public record ReadingListStatsDto(
    int TotalBooks,
    int BooksRead,
    int BooksUnread,
    double CompletionPercentage,
    DateOnly? MostRecentCompletion);
