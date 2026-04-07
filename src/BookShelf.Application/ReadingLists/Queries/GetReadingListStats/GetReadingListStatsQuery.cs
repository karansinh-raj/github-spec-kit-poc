using BookShelf.Application.Common.Models;
using BookShelf.Application.ReadingLists.DTOs;
using MediatR;

namespace BookShelf.Application.ReadingLists.Queries.GetReadingListStats;

public record GetReadingListStatsQuery(int Id) : IRequest<Result<ReadingListStatsDto>>;
