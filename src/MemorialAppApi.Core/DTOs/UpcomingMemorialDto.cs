namespace MemorialAppApi.Core.DTOs;

public record UpcomingMemorialDto
{
    public Guid Id { get; set; }
    public string ProfileType { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? BirthDetails { get; set; }
    public string? PassingDetails { get; set; }
    public string? Media { get; set; }
    public DateTime? UpcomingEventDate { get; set; }
    public string? Message { get; set; }
}

public record UpcomingMemorialResponseDto
{
    public List<UpcomingMemorialDto> Memorials { get; set; } = new();
    public bool HasRecentContribution { get; set; }
}
