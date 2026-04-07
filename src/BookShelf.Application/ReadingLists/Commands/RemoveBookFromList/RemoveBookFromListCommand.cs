using BookShelf.Application.Common.Models;
using MediatR;

namespace BookShelf.Application.ReadingLists.Commands.RemoveBookFromList;

public record RemoveBookFromListCommand(int ListId, int BookId) : IRequest<Result<bool>>;
