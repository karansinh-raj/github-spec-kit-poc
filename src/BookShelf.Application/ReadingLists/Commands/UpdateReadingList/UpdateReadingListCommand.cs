using BookShelf.Application.Common.Models;
using BookShelf.Application.ReadingLists.DTOs;
using MediatR;

namespace BookShelf.Application.ReadingLists.Commands.UpdateReadingList;

public record UpdateReadingListCommand(int Id, UpdateReadingListRequest Request) : IRequest<Result<ReadingListDto>>;
