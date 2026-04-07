using BookShelf.Application.Common.Models;
using BookShelf.Application.ReadingLists.DTOs;
using MediatR;

namespace BookShelf.Application.ReadingLists.Commands.AddBookToList;

public record AddBookToListCommand(int ListId, int BookId) : IRequest<Result<ReadingListBookDto>>;
