using BookShelf.Application.Common.Interfaces;
using BookShelf.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;

namespace BookShelf.Infrastructure.Persistence;

public class ReadingListRepository : IReadingListRepository
{
    private readonly BookShelfDbContext _context;
    private readonly ILogger<ReadingListRepository> _logger;

    public ReadingListRepository(BookShelfDbContext context, ILogger<ReadingListRepository>? logger = null)
    {
        _context = context;
        _logger = logger ?? NullLogger<ReadingListRepository>.Instance;
    }

    public async Task<ReadingList?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithLoggingAsync(
            "GetByIdAsync",
            new { ReadingListId = id },
            async () => await _context.ReadingLists.FindAsync([id], cancellationToken));
    }

    public async Task<ReadingList?> GetByIdWithBooksAsync(int id, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithLoggingAsync(
            "GetByIdWithBooksAsync",
            new { ReadingListId = id },
            async () => await _context.ReadingLists
                .Include(rl => rl.ReadingListBooks)
                    .ThenInclude(rlb => rlb.Book)
                .FirstOrDefaultAsync(rl => rl.Id == id, cancellationToken));
    }

    public async Task<List<ReadingList>> GetAllWithBooksAsync(CancellationToken cancellationToken = default)
    {
        return await ExecuteWithLoggingAsync(
            "GetAllWithBooksAsync",
            new { },
            async () => await _context.ReadingLists
                .Include(rl => rl.ReadingListBooks)
                .OrderBy(rl => rl.Name)
                .ToListAsync(cancellationToken));
    }

    public async Task<ReadingList> AddAsync(ReadingList readingList, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithLoggingAsync(
            "AddAsync",
            new { readingList.Name },
            async () =>
            {
                _context.ReadingLists.Add(readingList);
                await _context.SaveChangesAsync(cancellationToken);
                return readingList;
            });
    }

    public async Task UpdateAsync(ReadingList readingList, CancellationToken cancellationToken = default)
    {
        await ExecuteWithLoggingAsync(
            "UpdateAsync",
            new { readingList.Id, readingList.Name },
            async () =>
            {
                _context.ReadingLists.Update(readingList);
                await _context.SaveChangesAsync(cancellationToken);
            });
    }

    public async Task DeleteAsync(ReadingList readingList, CancellationToken cancellationToken = default)
    {
        await ExecuteWithLoggingAsync(
            "DeleteAsync",
            new { readingList.Id, readingList.Name },
            async () =>
            {
                _context.ReadingLists.Remove(readingList);
                await _context.SaveChangesAsync(cancellationToken);
            });
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithLoggingAsync(
            "ExistsByNameAsync",
            new { name, excludeId },
            async () => await _context.ReadingLists
                .AnyAsync(rl => rl.Name == name && (!excludeId.HasValue || rl.Id != excludeId.Value), cancellationToken));
    }

    public async Task<ReadingListBook?> GetReadingListBookAsync(int listId, int bookId, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithLoggingAsync(
            "GetReadingListBookAsync",
            new { listId, bookId },
            async () => await _context.ReadingListBooks
                .FirstOrDefaultAsync(rlb => rlb.ReadingListId == listId && rlb.BookId == bookId, cancellationToken));
    }

    public async Task<ReadingListBook> AddBookToListAsync(ReadingListBook entry, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithLoggingAsync(
            "AddBookToListAsync",
            new { entry.ReadingListId, entry.BookId },
            async () =>
            {
                _context.ReadingListBooks.Add(entry);
                await _context.SaveChangesAsync(cancellationToken);
                return entry;
            });
    }

    public async Task RemoveBookFromListAsync(ReadingListBook entry, CancellationToken cancellationToken = default)
    {
        await ExecuteWithLoggingAsync(
            "RemoveBookFromListAsync",
            new { entry.ReadingListId, entry.BookId },
            async () =>
            {
                _context.ReadingListBooks.Remove(entry);
                await _context.SaveChangesAsync(cancellationToken);
            });
    }

    public async Task UpdateReadingListBookAsync(ReadingListBook entry, CancellationToken cancellationToken = default)
    {
        await ExecuteWithLoggingAsync(
            "UpdateReadingListBookAsync",
            new { entry.ReadingListId, entry.BookId, entry.IsRead },
            async () =>
            {
                _context.ReadingListBooks.Update(entry);
                await _context.SaveChangesAsync(cancellationToken);
            });
    }

    private async Task<T> ExecuteWithLoggingAsync<T>(string operation, object context, Func<Task<T>> action)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var result = await action();
            _logger.LogInformation("ReadingListRepository.{Operation} completed in {ElapsedMilliseconds}ms with context {@Context}", operation, stopwatch.ElapsedMilliseconds, context);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ReadingListRepository.{Operation} failed in {ElapsedMilliseconds}ms with context {@Context}", operation, stopwatch.ElapsedMilliseconds, context);
            throw;
        }
    }

    private async Task ExecuteWithLoggingAsync(string operation, object context, Func<Task> action)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await action();
            _logger.LogInformation("ReadingListRepository.{Operation} completed in {ElapsedMilliseconds}ms with context {@Context}", operation, stopwatch.ElapsedMilliseconds, context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ReadingListRepository.{Operation} failed in {ElapsedMilliseconds}ms with context {@Context}", operation, stopwatch.ElapsedMilliseconds, context);
            throw;
        }
    }
}
