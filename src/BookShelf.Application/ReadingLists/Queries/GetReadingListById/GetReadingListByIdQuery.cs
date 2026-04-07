using BookShelf.Application.Common.Models;
using BookShelf.Application.ReadingLists.DTOs;
using MediatR;

namespace BookShelf.Application.ReadingLists.Queries.GetReadingListById;

public record GetReadingListByIdQuery(int Id) : IRequest<Result<ReadingListDetailDto>>;
