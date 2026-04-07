using BookShelf.Application.Common.Models;
using BookShelf.Application.ReadingLists.DTOs;
using MediatR;

namespace BookShelf.Application.ReadingLists.Commands.UpdateReadStatus;

public record UpdateReadStatusCommand(int ListId, int BookId, UpdateReadStatusRequest Request) : IRequest<Result<ReadingListBookDto>>;
