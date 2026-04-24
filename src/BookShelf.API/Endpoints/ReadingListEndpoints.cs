using BookShelf.Application.Common.Models;
using BookShelf.Application.ReadingLists.Commands.AddBookToList;
using BookShelf.Application.ReadingLists.Commands.CreateReadingList;
using BookShelf.Application.ReadingLists.Commands.DeleteReadingList;
using BookShelf.Application.ReadingLists.Commands.RemoveBookFromList;
using BookShelf.Application.ReadingLists.Commands.UpdateReadingList;
using BookShelf.Application.ReadingLists.Commands.UpdateReadStatus;
using BookShelf.Application.ReadingLists.DTOs;
using BookShelf.Application.ReadingLists.Queries.GetReadingListById;
using BookShelf.Application.ReadingLists.Queries.GetReadingLists;
using BookShelf.Application.ReadingLists.Queries.GetReadingListStats;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BookShelf.API.Endpoints;

public static class ReadingListEndpoints
{
    public static void MapReadingListEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reading-lists").WithTags("Reading Lists");

        group.AddEndpointFilter(async (context, next) =>
        {
            var stopwatch = Stopwatch.StartNew();
            var httpContext = context.HttpContext;
            var logger = httpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("ReadingListEndpoints");
            var method = SanitizeForLog(httpContext.Request.Method);
            var path = SanitizeForLog(httpContext.Request.Path.ToString());

            logger.LogInformation("Handling endpoint request {Method} {Path}", method, path);
            try
            {
                var result = await next(context);
                logger.LogInformation("Handled endpoint request {Method} {Path} in {ElapsedMilliseconds}ms", method, path, stopwatch.ElapsedMilliseconds);
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception in endpoint request {Method} {Path}", method, path);
                throw;
            }
        });

        group.MapPost("/", async (CreateReadingListRequest request, IMediator mediator) =>
        {
            var result = await mediator.Send(new CreateReadingListCommand(request));
            if (!result.IsSuccess)
                return Results.Conflict(ApiResponse<ReadingListDto>.Fail("Name", result.Errors.First()));
            return Results.Created($"/api/reading-lists/{result.Value!.Id}", ApiResponse<ReadingListDto>.Ok(result.Value));
        })
        .WithName("CreateReadingList")
        .Produces<ApiResponse<ReadingListDto>>(StatusCodes.Status201Created)
        .Produces<ApiResponse<ReadingListDto>>(StatusCodes.Status409Conflict);

        group.MapGet("/", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetReadingListsQuery());
            return Results.Ok(ApiResponse<List<ReadingListDto>>.Ok(result.Value!));
        })
        .WithName("GetReadingLists")
        .Produces<ApiResponse<List<ReadingListDto>>>();

        group.MapGet("/{id:int}", async (int id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetReadingListByIdQuery(id));
            if (!result.IsSuccess)
                return Results.NotFound(ApiResponse<ReadingListDetailDto>.Fail("Id", result.Errors.First()));
            return Results.Ok(ApiResponse<ReadingListDetailDto>.Ok(result.Value!));
        })
        .WithName("GetReadingListById")
        .Produces<ApiResponse<ReadingListDetailDto>>()
        .Produces<ApiResponse<ReadingListDetailDto>>(StatusCodes.Status404NotFound);

        group.MapPut("/{id:int}", async (int id, UpdateReadingListRequest request, IMediator mediator) =>
        {
            var result = await mediator.Send(new UpdateReadingListCommand(id, request));
            if (!result.IsSuccess)
            {
                if (result.Errors.First().Contains("not found"))
                    return Results.NotFound(ApiResponse<ReadingListDto>.Fail("Id", result.Errors.First()));
                return Results.Conflict(ApiResponse<ReadingListDto>.Fail("Name", result.Errors.First()));
            }
            return Results.Ok(ApiResponse<ReadingListDto>.Ok(result.Value!));
        })
        .WithName("UpdateReadingList")
        .Produces<ApiResponse<ReadingListDto>>()
        .Produces<ApiResponse<ReadingListDto>>(StatusCodes.Status404NotFound)
        .Produces<ApiResponse<ReadingListDto>>(StatusCodes.Status409Conflict);

        group.MapDelete("/{id:int}", async (int id, IMediator mediator) =>
        {
            var result = await mediator.Send(new DeleteReadingListCommand(id));
            if (!result.IsSuccess)
                return Results.NotFound(ApiResponse<bool>.Fail("Id", result.Errors.First()));
            return Results.NoContent();
        })
        .WithName("DeleteReadingList")
        .Produces(StatusCodes.Status204NoContent)
        .Produces<ApiResponse<bool>>(StatusCodes.Status404NotFound);

        group.MapPost("/{listId:int}/books/{bookId:int}", async (int listId, int bookId, IMediator mediator) =>
        {
            var result = await mediator.Send(new AddBookToListCommand(listId, bookId));
            if (!result.IsSuccess)
            {
                if (result.Errors.First().Contains("already"))
                    return Results.Conflict(ApiResponse<ReadingListBookDto>.Fail("BookId", result.Errors.First()));
                return Results.NotFound(ApiResponse<ReadingListBookDto>.Fail("Id", result.Errors.First()));
            }
            return Results.Created($"/api/reading-lists/{listId}/books/{bookId}",
                ApiResponse<ReadingListBookDto>.Ok(result.Value!));
        })
        .WithName("AddBookToList")
        .Produces<ApiResponse<ReadingListBookDto>>(StatusCodes.Status201Created)
        .Produces<ApiResponse<ReadingListBookDto>>(StatusCodes.Status404NotFound)
        .Produces<ApiResponse<ReadingListBookDto>>(StatusCodes.Status409Conflict);

        group.MapDelete("/{listId:int}/books/{bookId:int}", async (int listId, int bookId, IMediator mediator) =>
        {
            var result = await mediator.Send(new RemoveBookFromListCommand(listId, bookId));
            if (!result.IsSuccess)
                return Results.NotFound(ApiResponse<bool>.Fail("Id", result.Errors.First()));
            return Results.NoContent();
        })
        .WithName("RemoveBookFromList")
        .Produces(StatusCodes.Status204NoContent)
        .Produces<ApiResponse<bool>>(StatusCodes.Status404NotFound);

        group.MapPut("/{listId:int}/books/{bookId:int}/status", async (int listId, int bookId, UpdateReadStatusRequest request, IMediator mediator) =>
        {
            var result = await mediator.Send(new UpdateReadStatusCommand(listId, bookId, request));
            if (!result.IsSuccess)
                return Results.NotFound(ApiResponse<ReadingListBookDto>.Fail("Id", result.Errors.First()));
            return Results.Ok(ApiResponse<ReadingListBookDto>.Ok(result.Value!));
        })
        .WithName("UpdateReadStatus")
        .Produces<ApiResponse<ReadingListBookDto>>()
        .Produces<ApiResponse<ReadingListBookDto>>(StatusCodes.Status404NotFound);

        group.MapGet("/{id:int}/stats", async (int id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetReadingListStatsQuery(id));
            if (!result.IsSuccess)
                return Results.NotFound(ApiResponse<ReadingListStatsDto>.Fail("Id", result.Errors.First()));
            return Results.Ok(ApiResponse<ReadingListStatsDto>.Ok(result.Value!));
        })
        .WithName("GetReadingListStats")
        .Produces<ApiResponse<ReadingListStatsDto>>()
        .Produces<ApiResponse<ReadingListStatsDto>>(StatusCodes.Status404NotFound);
    }

    private static string SanitizeForLog(string value) =>
        value.Replace("\r", "\\r", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal);
}
