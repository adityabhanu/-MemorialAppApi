using MemorialAppApi.Core.DTOs;

namespace MemorialAppApi.Core.Commands;

public record UpdateUserCommand : IRequest<UserDto>
{
    public Guid UserId { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? PublicName { get; init; }
    public string? ProfilePic { get; init; }
    public bool? ReceiveEmail { get; init; }
    public bool? PhotoVolunteer { get; init; }
    public bool? TermsAndCondition { get; init; }
}
