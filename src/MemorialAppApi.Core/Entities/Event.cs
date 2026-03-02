namespace MemorialAppApi.Core.Entities;

public class Event
{
    public Guid Id { get; set; }
    public Guid MemorialId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public string? EventDate { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public decimal? Latitude { get; set; } 
    public decimal? Longitude { get; set; } 
    public string Address { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;

    // Navigation property
    public Memorial? Memorial { get; set; }
}
