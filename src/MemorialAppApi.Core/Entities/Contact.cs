namespace MemorialAppApi.Core.Entities;

public class Contact
{
    public Guid Id { get; set; }
    public Guid CemeteryId { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? OfficeAddress { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    public Cemetery? Cemetery { get; set; }
}
