using BookShelf.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookShelf.Infrastructure.Persistence;

public class BookShelfDbContext : DbContext
{
    public BookShelfDbContext(DbContextOptions<BookShelfDbContext> options) : base(options) { }

    public DbSet<Book> Books => Set<Book>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Author).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ISBN).IsRequired().HasMaxLength(17);
            entity.HasIndex(e => e.ISBN).IsUnique();
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Genre).HasConversion<string>();
        });
    }
}
