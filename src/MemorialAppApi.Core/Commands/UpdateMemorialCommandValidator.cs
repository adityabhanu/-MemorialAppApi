using FluentValidation;

namespace MemorialAppApi.Core.Commands;

public class UpdateMemorialCommandValidator : AbstractValidator<UpdateMemorialCommand>
{
    public UpdateMemorialCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Memorial ID is required");

        RuleFor(x => x.ProfileType)
            .NotEmpty().WithMessage("Profile type is required")
            .MaximumLength(100).WithMessage("Profile type must not exceed 100 characters");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(500).WithMessage("Full name must not exceed 500 characters");
    }
}
