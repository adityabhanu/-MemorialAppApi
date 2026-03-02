using MemorialAppApi.Core.DTOs;

namespace MemorialAppApi.Core.Commands;

public record RegisterCommand : IRequest<AuthResponseDto>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string PublicName { get; init; } = string.Empty;
    public bool ReceiveEmail { get; init; } = false;
    public bool PhotoVolunteer { get; init; } = false;
    public bool TermsAndCondition { get; init; } = false;
}
