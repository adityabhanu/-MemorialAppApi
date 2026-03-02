using MemorialAppApi.Core.DTOs;

namespace MemorialAppApi.Core.Commands;

public record LoginCommand : IRequest<AuthResponseDto>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
