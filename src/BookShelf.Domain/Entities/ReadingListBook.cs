namespace BookShelf.Domain.Entities;

public class ReadingListBook
{
    public int Id { get; set; }
    public int ReadingListId { get; set; }
    public int BookId { get; set; }
    public bool IsRead { get; set; }
    public string? Notes { get; set; }
    public DateOnly? CompletedDate { get; set; }
    public DateTime AddedAt { get; set; }
    public ReadingList ReadingList { get; set; } = null!;
    public Book Book { get; set; } = null!;
}
