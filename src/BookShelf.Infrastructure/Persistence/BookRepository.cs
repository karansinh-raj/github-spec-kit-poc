using BookShelf.Application.Common.Interfaces;
using BookShelf.Domain.Entities;
using BookShelf.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookShelf.Infrastructure.Persistence;

public class BookRepository : IBookRepository
{
    private readonly BookShelfDbContext _context;

    public BookRepository(BookShelfDbContext context)
    {
        _context = context;
    }

    public async Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Books.FindAsync([id], cancellationToken);
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
    }

    public async Task<Book> AddAsync(Book book, CancellationToken cancellationToken = default)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync(cancellationToken);
        return book;
    }

    public async Task UpdateAsync(Book book, CancellationToken cancellationToken = default)
    {
        _context.Books.Update(book);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Book book, CancellationToken cancellationToken = default)
    {
        _context.Books.Remove(book);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByIsbnAsync(string isbn, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Books.Where(b => b.ISBN == isbn);
        if (excludeId.HasValue)
            query = query.Where(b => b.Id != excludeId.Value);
        return await query.AnyAsync(cancellationToken);
    }
}
