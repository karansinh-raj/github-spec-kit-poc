using BookShelf.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookShelf.Infrastructure.Persistence;

public class BookShelfDbContext : DbContext
{
    public BookShelfDbContext(DbContextOptions<BookShelfDbContext> options) : base(options) { }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<ReadingList> ReadingLists => Set<ReadingList>();
    public DbSet<ReadingListBook> ReadingListBooks => Set<ReadingListBook>();

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

        modelBuilder.Entity<ReadingList>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasMany(e => e.ReadingListBooks)
                .WithOne(e => e.ReadingList)
                .HasForeignKey(e => e.ReadingListId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ReadingListBook>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.ReadingListId, e.BookId }).IsUnique();
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.HasOne(e => e.Book)
                .WithMany()
                .HasForeignKey(e => e.BookId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
