using FluentValidation;

namespace BookShelf.Application.ReadingLists.Commands.CreateReadingList;

public class CreateReadingListValidator : AbstractValidator<CreateReadingListCommand>
{
    public CreateReadingListValidator()
    {
        RuleFor(x => x.Request.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Request.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");
    }
}
