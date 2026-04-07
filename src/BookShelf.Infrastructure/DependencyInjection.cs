using BookShelf.Application.Common.Interfaces;
using BookShelf.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookShelf.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<BookShelfDbContext>(options =>
            options.UseInMemoryDatabase("BookShelfDb"));

        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IReadingListRepository, ReadingListRepository>();

        return services;
    }
}
