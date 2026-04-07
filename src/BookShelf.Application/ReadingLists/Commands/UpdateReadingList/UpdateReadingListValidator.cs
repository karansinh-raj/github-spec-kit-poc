using FluentValidation;

namespace BookShelf.Application.ReadingLists.Commands.UpdateReadingList;

public class UpdateReadingListValidator : AbstractValidator<UpdateReadingListCommand>
{
    public UpdateReadingListValidator()
    {
        RuleFor(x => x.Request.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Request.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");
    }
}
