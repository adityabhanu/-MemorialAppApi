namespace MemorialAppApi.Core.DTOs;

public record TimelineMediaDto
{
    public List<string>? Photos { get; init; }
    public List<string>? Video { get; init; }
    public List<string>? Audio { get; init; }
}

public record TimelineEntryDto
{
    public Guid? Id { get; init; }
    public Guid MemorialId { get; init; }
    public string? Title { get; init; }
    public DateTime? Date { get; init; }
    public string? Description { get; init; }
    public TimelineMediaDto? Media { get; init; }
}
