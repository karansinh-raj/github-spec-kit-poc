using BookShelf.Application.Common.Interfaces;
using BookShelf.Domain.Entities;
using BookShelf.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;

namespace BookShelf.Infrastructure.Persistence;

public class BookRepository : IBookRepository
{
    private readonly BookShelfDbContext _context;
    private readonly ILogger<BookRepository> _logger;

    public BookRepository(BookShelfDbContext context, ILogger<BookRepository>? logger = null)
    {
        _context = context;
        _logger = logger ?? NullLogger<BookRepository>.Instance;
    }

    public async Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithLoggingAsync(
            "GetByIdAsync",
            new { BookId = id },
            async () => await _context.Books.FindAsync([id], cancellationToken));
    }

    public async Task<List<Book>> GetByGenreAsync(Genre genre, CancellationToken cancellationToken = default)
    {
        return await _context.Books
            .Where(b => b.Genre == genre)
            .OrderBy(b => b.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<(List<Book> Items, int TotalCount)> GetAllAsync(
        int page, int pageSize,
        string? genre, string? author, string? search,
        string sortBy, string sortOrder,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteWithLoggingAsync(
            "GetAllAsync",
            new { page, pageSize, genre, author, search, sortBy, sortOrder },
            async () =>
            {
                var query = _context.Books.AsQueryable();

                if (!string.IsNullOrWhiteSpace(genre) && Enum.TryParse<Genre>(genre, ignoreCase: true, out var genreEnum))
                    query = query.Where(b => b.Genre == genreEnum);

                if (!string.IsNullOrWhiteSpace(author))
                    query = query.Where(b => b.Author == author);

                if (!string.IsNullOrWhiteSpace(search))
                    query = query.Where(b => b.Title.ToLower().Contains(search.ToLower()));

                var totalCount = await query.CountAsync(cancellationToken);

                query = sortBy.ToLowerInvariant() switch
                {
                    "publisheddate" => sortOrder.ToLowerInvariant() == "desc"
                        ? query.OrderByDescending(b => b.PublishedDate)
                        : query.OrderBy(b => b.PublishedDate),
                    _ => sortOrder.ToLowerInvariant() == "desc"
                        ? query.OrderByDescending(b => b.Title)
                        : query.OrderBy(b => b.Title)
                };

                var items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                return (items, totalCount);
            });
    }

    public async Task<Book> AddAsync(Book book, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithLoggingAsync(
            "AddAsync",
            new { book.ISBN, book.Title },
            async () =>
            {
                _context.Books.Add(book);
                await _context.SaveChangesAsync(cancellationToken);
                return book;
            });
    }

    public async Task UpdateAsync(Book book, CancellationToken cancellationToken = default)
    {
        await ExecuteWithLoggingAsync(
            "UpdateAsync",
            new { book.Id, book.ISBN },
            async () =>
            {
                _context.Books.Update(book);
                await _context.SaveChangesAsync(cancellationToken);
            });
    }

    public async Task DeleteAsync(Book book, CancellationToken cancellationToken = default)
    {
        await ExecuteWithLoggingAsync(
            "DeleteAsync",
            new { book.Id, book.ISBN },
            async () =>
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync(cancellationToken);
            });
    }

    public async Task<bool> ExistsByIsbnAsync(string isbn, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithLoggingAsync(
            "ExistsByIsbnAsync",
            new { isbn, excludeId },
            async () =>
            {
                var query = _context.Books.Where(b => b.ISBN == isbn);
                if (excludeId.HasValue)
                    query = query.Where(b => b.Id != excludeId.Value);
                return await query.AnyAsync(cancellationToken);
            });
    }

    private async Task<T> ExecuteWithLoggingAsync<T>(string operation, object context, Func<Task<T>> action)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var result = await action();
            _logger.LogInformation("BookRepository.{Operation} completed in {ElapsedMilliseconds}ms with context {@Context}", operation, stopwatch.ElapsedMilliseconds, context);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "BookRepository.{Operation} failed in {ElapsedMilliseconds}ms with context {@Context}", operation, stopwatch.ElapsedMilliseconds, context);
            throw;
        }
    }

    private async Task ExecuteWithLoggingAsync(string operation, object context, Func<Task> action)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await action();
            _logger.LogInformation("BookRepository.{Operation} completed in {ElapsedMilliseconds}ms with context {@Context}", operation, stopwatch.ElapsedMilliseconds, context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "BookRepository.{Operation} failed in {ElapsedMilliseconds}ms with context {@Context}", operation, stopwatch.ElapsedMilliseconds, context);
            throw;
        }
    }
}
