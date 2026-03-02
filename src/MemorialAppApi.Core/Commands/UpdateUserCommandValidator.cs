using FluentValidation;

namespace MemorialAppApi.Core.Commands;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.ProfilePic)
            .MaximumLength(500).WithMessage("Profile picture URL must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.ProfilePic));
    }
}
