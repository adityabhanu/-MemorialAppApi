using FluentValidation;

namespace MemorialAppApi.Core.Commands;

public class CreateCemeteryCommandValidator : AbstractValidator<CreateCemeteryCommand>
{
    public CreateCemeteryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Cemetery name is required")
            .MaximumLength(255).WithMessage("Cemetery name must not exceed 255 characters");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required")
            .MaximumLength(500).WithMessage("Location must not exceed 500 characters");

        RuleFor(x => x.StreetAddress)
            .MaximumLength(500).WithMessage("Street address must not exceed 500 characters");

        RuleFor(x => x.Description)
            .MaximumLength(5000).WithMessage("Description must not exceed 5000 characters");

        RuleFor(x => x.AdditionalInfo)
            .MaximumLength(5000).WithMessage("Additional info must not exceed 5000 characters");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required")
            .MaximumLength(50).WithMessage("Status must not exceed 50 characters");

        RuleFor(x => x.Longitude)
            .Must(BeValidDecimal).WithMessage("Longitude must be a valid decimal number")
            .When(x => !string.IsNullOrEmpty(x.Longitude));

        RuleFor(x => x.Latitude)
            .Must(BeValidDecimal).WithMessage("Latitude must be a valid decimal number")
            .When(x => !string.IsNullOrEmpty(x.Latitude));

        RuleFor(x => x.ContactInfo!.Email)
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters")
            .When(x => x.ContactInfo != null && !string.IsNullOrEmpty(x.ContactInfo.Email));

        RuleFor(x => x.ContactInfo!.Phone)
            .MaximumLength(50).WithMessage("Phone must not exceed 50 characters")
            .When(x => x.ContactInfo != null && !string.IsNullOrEmpty(x.ContactInfo.Phone));

        RuleFor(x => x.ContactInfo!.Website)
            .MaximumLength(500).WithMessage("Website must not exceed 500 characters")
            .When(x => x.ContactInfo != null && !string.IsNullOrEmpty(x.ContactInfo.Website));
    }

    private bool BeValidDecimal(string value)
    {
        return decimal.TryParse(value, out _);
    }
}
