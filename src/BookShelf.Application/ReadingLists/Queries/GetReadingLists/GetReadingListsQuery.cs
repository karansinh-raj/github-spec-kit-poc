using BookShelf.Application.Common.Models;
using BookShelf.Application.ReadingLists.DTOs;
using MediatR;

namespace BookShelf.Application.ReadingLists.Queries.GetReadingLists;

public record GetReadingListsQuery() : IRequest<Result<List<ReadingListDto>>>;
