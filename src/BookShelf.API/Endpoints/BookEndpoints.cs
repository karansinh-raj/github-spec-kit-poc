using BookShelf.Application.Books.Commands.CreateBook;
using BookShelf.Application.Books.Commands.DeleteBook;
using BookShelf.Application.Books.Commands.UpdateBook;
using BookShelf.Application.Books.DTOs;
using BookShelf.Application.Books.Queries.GetBookById;
using BookShelf.Application.Books.Queries.GetBooks;
using BookShelf.Application.Books.Queries.GetBooksByGenre;
using BookShelf.Application.Common.Models;
using MediatR;

namespace BookShelf.API.Endpoints;

public static class BookEndpoints
{
    public static void MapBookEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/books").WithTags("Books");

        group.MapPost("/", async (CreateBookRequest request, IMediator mediator) =>
        {
            var result = await mediator.Send(new CreateBookCommand(request));
            if (!result.IsSuccess)
                return Results.Conflict(ApiResponse<BookDto>.Fail("ISBN", result.Errors.First()));
            return Results.Created($"/api/books/{result.Value!.Id}", ApiResponse<BookDto>.Ok(result.Value));
        })
        .WithName("CreateBook")
        .Produces<ApiResponse<BookDto>>(StatusCodes.Status201Created)
        .Produces<ApiResponse<BookDto>>(StatusCodes.Status409Conflict)
        .Produces<ApiResponse<BookDto>>(StatusCodes.Status422UnprocessableEntity);

        group.MapGet("/{id:int}", async (int id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetBookByIdQuery(id));
            if (!result.IsSuccess)
                return Results.NotFound(ApiResponse<BookDto>.Fail("Id", result.Errors.First()));
            return Results.Ok(ApiResponse<BookDto>.Ok(result.Value!));
        })
        .WithName("GetBookById")
        .Produces<ApiResponse<BookDto>>()
        .Produces<ApiResponse<BookDto>>(StatusCodes.Status404NotFound);

        group.MapGet("/", async (
            int? page, int? pageSize,
            string? genre, string? author, string? search,
            string? sortBy, string? sortOrder,
            IMediator mediator) =>
        {
            var query = new GetBooksQuery(
                Page: page ?? 1,
                PageSize: pageSize ?? 10,
                Genre: genre,
                Author: author,
                Search: search,
                SortBy: sortBy ?? "title",
                SortOrder: sortOrder ?? "asc");

            var result = await mediator.Send(query);
            var pagedResult = result.Value!;

            return Results.Ok(ApiResponse<List<BookDto>>.Ok(
                pagedResult.Items,
                new { pagedResult.TotalCount, pagedResult.TotalPages, pagedResult.CurrentPage, pagedResult.PageSize }));
        })
        .WithName("GetBooks")
        .Produces<ApiResponse<List<BookDto>>>();

        group.MapPut("/{id:int}", async (int id, UpdateBookRequest request, IMediator mediator) =>
        {
            var result = await mediator.Send(new UpdateBookCommand(id, request));
            if (!result.IsSuccess)
            {
                if (result.Errors.First().Contains("not found"))
                    return Results.NotFound(ApiResponse<BookDto>.Fail("Id", result.Errors.First()));
                return Results.Conflict(ApiResponse<BookDto>.Fail("ISBN", result.Errors.First()));
            }
            return Results.Ok(ApiResponse<BookDto>.Ok(result.Value!));
        })
        .WithName("UpdateBook")
        .Produces<ApiResponse<BookDto>>()
        .Produces<ApiResponse<BookDto>>(StatusCodes.Status404NotFound)
        .Produces<ApiResponse<BookDto>>(StatusCodes.Status409Conflict);

        group.MapDelete("/{id:int}", async (int id, IMediator mediator) =>
        {
            var result = await mediator.Send(new DeleteBookCommand(id));
            if (!result.IsSuccess)
                return Results.NotFound(ApiResponse<BookDto>.Fail("Id", result.Errors.First()));
            return Results.NoContent();
        })
        .WithName("DeleteBook")
        .Produces(StatusCodes.Status204NoContent)
        .Produces<ApiResponse<BookDto>>(StatusCodes.Status404NotFound);

        group.MapGet("/genre/{genre}", async (string genre, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetBooksByGenreQuery(genre));
            if (!result.IsSuccess)
                return Results.BadRequest(ApiResponse<List<BookDto>>.Fail("Genre", result.Errors.First()));
            return Results.Ok(ApiResponse<List<BookDto>>.Ok(result.Value!));
        })
        .WithName("GetBooksByGenre")
        .Produces<ApiResponse<List<BookDto>>>()
        .Produces<ApiResponse<List<BookDto>>>(StatusCodes.Status400BadRequest);
    }
}
