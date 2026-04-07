using BookShelf.Application.Common.Models;
using BookShelf.Application.ReadingLists.DTOs;
using MediatR;

namespace BookShelf.Application.ReadingLists.Commands.CreateReadingList;

public record CreateReadingListCommand(CreateReadingListRequest Request) : IRequest<Result<ReadingListDto>>;
