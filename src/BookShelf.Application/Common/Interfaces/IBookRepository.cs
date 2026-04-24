using BookShelf.Domain.Entities;
using BookShelf.Domain.Enums;

namespace BookShelf.Application.Common.Interfaces;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Book>> GetByGenreAsync(Genre genre, CancellationToken cancellationToken = default);
    Task<(List<Book> Items, int TotalCount)> GetAllAsync(
        int page, int pageSize,
        string? genre, string? author, string? search,
        string sortBy, string sortOrder,
        CancellationToken cancellationToken = default);
    Task<Book> AddAsync(Book book, CancellationToken cancellationToken = default);
    Task UpdateAsync(Book book, CancellationToken cancellationToken = default);
    Task DeleteAsync(Book book, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIsbnAsync(string isbn, int? excludeId = null, CancellationToken cancellationToken = default);
}
