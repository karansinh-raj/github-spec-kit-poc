using BookShelf.Domain.Entities;

namespace BookShelf.Application.Common.Interfaces;

public interface IReadingListRepository
{
    Task<ReadingList?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ReadingList?> GetByIdWithBooksAsync(int id, CancellationToken cancellationToken = default);
    Task<List<ReadingList>> GetAllWithBooksAsync(CancellationToken cancellationToken = default);
    Task<ReadingList> AddAsync(ReadingList readingList, CancellationToken cancellationToken = default);
    Task UpdateAsync(ReadingList readingList, CancellationToken cancellationToken = default);
    Task DeleteAsync(ReadingList readingList, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<ReadingListBook?> GetReadingListBookAsync(int listId, int bookId, CancellationToken cancellationToken = default);
    Task<ReadingListBook> AddBookToListAsync(ReadingListBook entry, CancellationToken cancellationToken = default);
    Task RemoveBookFromListAsync(ReadingListBook entry, CancellationToken cancellationToken = default);
    Task UpdateReadingListBookAsync(ReadingListBook entry, CancellationToken cancellationToken = default);
}
