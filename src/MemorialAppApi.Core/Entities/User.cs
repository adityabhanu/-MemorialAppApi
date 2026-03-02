namespace MemorialAppApi.Core.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PublicName { get; set; } = string.Empty;
    public string? ProfilePic { get; set; }
    public bool ReceiveEmail { get; set; } = false;
    public bool PhotoVolunteer { get; set; } = false;
    public bool TermsAndCondition { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}
