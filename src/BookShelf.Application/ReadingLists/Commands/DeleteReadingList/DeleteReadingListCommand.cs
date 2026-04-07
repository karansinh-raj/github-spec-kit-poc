using BookShelf.Application.Common.Models;
using MediatR;

namespace BookShelf.Application.ReadingLists.Commands.DeleteReadingList;

public record DeleteReadingListCommand(int Id) : IRequest<Result<bool>>;
