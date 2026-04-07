using BookShelf.Domain.Enums;
using FluentValidation;

namespace BookShelf.Application.Books.Commands.CreateBook;

public class CreateBookValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookValidator()
    {
        RuleFor(x => x.Request.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Request.Author)
            .NotEmpty().WithMessage("Author is required.")
            .MaximumLength(100).WithMessage("Author must not exceed 100 characters.");

        RuleFor(x => x.Request.ISBN)
            .NotEmpty().WithMessage("ISBN is required.")
            .MaximumLength(17).WithMessage("ISBN must not exceed 17 characters.");

        RuleFor(x => x.Request.PublishedDate)
            .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Published date cannot be in the future.");

        RuleFor(x => x.Request.Genre)
            .Must(genre => Enum.TryParse<Genre>(genre, ignoreCase: true, out _))
            .WithMessage("Genre must be one of: Fiction, NonFiction, Science, Technology, History, Biography, Fantasy, Mystery, Romance, Other.");

        RuleFor(x => x.Request.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.")
            .When(x => x.Request.Description != null);
    }
}
