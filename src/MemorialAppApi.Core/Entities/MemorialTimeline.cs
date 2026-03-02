namespace MemorialAppApi.Core.Entities;

public class MemorialTimeline
{
    public Guid Id { get; set; }
    public Guid MemorialId { get; set; }
    public string? Title { get; set; }
    public DateTime? Date { get; set; }
    public string? Description { get; set; }
    public string? Photos { get; set; }
    public string? Video { get; set; }
    public string? Audio { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }

    // Navigation property
    public Memorial? Memorial { get; set; }
}
