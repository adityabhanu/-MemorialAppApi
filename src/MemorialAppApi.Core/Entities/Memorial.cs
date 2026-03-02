namespace MemorialAppApi.Core.Entities;

public class Memorial
{
    public Guid Id { get; set; }
    
    // Profile Information
    public string ProfileType { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public string FullName { get; set; } = string.Empty;
    
    // Detailed Information (NVARCHAR(MAX) fields)
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
    
    // System Fields
    public Guid? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    
    // Navigation properties
    public ICollection<MemorialTimeline>? Timelines { get; set; }
    public ICollection<Event>? Events { get; set; }
}

