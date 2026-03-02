using MemorialAppApi.Core.DTOs;

namespace MemorialAppApi.Core.Commands;

public record CreateMemorialCommand : IRequest<MemorialDto>
{
    public Guid UserId { get; init; }
    public string ProfileType { get; init; } = string.Empty;
    public bool IsPublic { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string? BirthDetails { get; init; }
    public string? PassingDetails { get; init; }
    public string? AppearanceAtBirth { get; init; }
    public string? Family { get; init; }
    public string? Visitors { get; init; }
    public string? ParentThoughts { get; init; }
    public string? Letters { get; init; }
    public string? Notes { get; init; }
    public string? Personalities { get; init; }
    public string? Hobbies { get; init; }
    public string? LifeDetails { get; init; }
    public string? Media { get; init; }
    public Guid? CreatedBy { get; init; }
}
