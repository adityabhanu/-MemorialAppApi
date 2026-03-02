namespace MemorialAppApi.Core.DTOs;


public record MemorialDto
{
    public Guid? Id { get; set; }
    public string ProfileType { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? BirthDetails { get; set; } 
    public string? PassingDetails { get; set; } 
    public string? AppearanceAtBirth { get; set; } 
    public string? Family { get; set; } 
    public string? Visitors { get; set; } 
    public string? ParentThoughts { get; set; } 
    public string? Letters { get; set; } 
    public string? Notes { get; set; } 
    public string? Personalities { get; set; } 
    public string? Hobbies { get; set; } 
    public string? LifeDetails { get; set; } 
    public string? Media { get; set; } 
    public Guid? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<TimelineEntryDto>? Timelines { get; set; }
}

public record UpdateMemorialDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public DateTime DateOfDeath { get; set; }
    public string Biography { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public string? Location { get; set; }
}
