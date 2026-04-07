using BookShelf.Application.Common.Interfaces;
using BookShelf.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookShelf.Infrastructure.Persistence;

public class ReadingListRepository : IReadingListRepository
{
    private readonly BookShelfDbContext _context;

    public ReadingListRepository(BookShelfDbContext context)
    {
        _context = context;
    }

    public async Task<ReadingList?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.ReadingLists.FindAsync([id], cancellationToken);
    }

    public async Task<ReadingList?> GetByIdWithBooksAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.ReadingLists
            .Include(rl => rl.ReadingListBooks)
                .ThenInclude(rlb => rlb.Book)
            .FirstOrDefaultAsync(rl => rl.Id == id, cancellationToken);
    }

    public async Task<List<ReadingList>> GetAllWithBooksAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ReadingLists
            .Include(rl => rl.ReadingListBooks)
            .OrderBy(rl => rl.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<ReadingList> AddAsync(ReadingList readingList, CancellationToken cancellationToken = default)
    {
        _context.ReadingLists.Add(readingList);
        await _context.SaveChangesAsync(cancellationToken);
        return readingList;
    }

    public async Task UpdateAsync(ReadingList readingList, CancellationToken cancellationToken = default)
    {
        _context.ReadingLists.Update(readingList);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ReadingList readingList, CancellationToken cancellationToken = default)
    {
        _context.ReadingLists.Remove(readingList);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        return await _context.ReadingLists
            .AnyAsync(rl => rl.Name == name && (!excludeId.HasValue || rl.Id != excludeId.Value), cancellationToken);
    }

    public async Task<ReadingListBook?> GetReadingListBookAsync(int listId, int bookId, CancellationToken cancellationToken = default)
    {
        return await _context.ReadingListBooks
            .FirstOrDefaultAsync(rlb => rlb.ReadingListId == listId && rlb.BookId == bookId, cancellationToken);
    }

    public async Task<ReadingListBook> AddBookToListAsync(ReadingListBook entry, CancellationToken cancellationToken = default)
    {
        _context.ReadingListBooks.Add(entry);
        await _context.SaveChangesAsync(cancellationToken);
        return entry;
    }

    public async Task RemoveBookFromListAsync(ReadingListBook entry, CancellationToken cancellationToken = default)
    {
        _context.ReadingListBooks.Remove(entry);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateReadingListBookAsync(ReadingListBook entry, CancellationToken cancellationToken = default)
    {
        _context.ReadingListBooks.Update(entry);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
