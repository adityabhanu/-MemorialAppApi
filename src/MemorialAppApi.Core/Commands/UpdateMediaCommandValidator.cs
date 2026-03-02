using FluentValidation;

namespace MemorialAppApi.Core.Commands;

public class UpdateMediaCommandValidator : AbstractValidator<UpdateMediaCommand>
{
    public UpdateMediaCommandValidator()
    {
        RuleFor(x => x.MemorialId)
            .NotEmpty()
            .WithMessage("Memorial ID is required");
    }
}
