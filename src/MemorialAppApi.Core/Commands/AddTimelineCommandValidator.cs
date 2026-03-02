using FluentValidation;

namespace MemorialAppApi.Core.Commands;

public class AddTimelineCommandValidator : AbstractValidator<AddTimelineCommand>
{
    public AddTimelineCommandValidator()
    {
        RuleFor(x => x.MemorialId)
            .NotEmpty().WithMessage("Memorial ID is required");
    }
}
