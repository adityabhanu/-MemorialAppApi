namespace MemorialAppApi.Core.Entities;

public class Cemetery
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string StreetAddress { get; set; } = string.Empty;
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }
    public string Description { get; set; } = string.Empty;
    public string AdditionalInfo { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    
    // Navigation properties
    public Contact? Contact { get; set; }
}
