using System.Text.Json.Serialization;

namespace MemorialAppApi.Core.DTOs;

public record LoginDto
{
    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;
    [JsonPropertyName("password")]
    public string Password { get; init; } = string.Empty;
}

public record RegisterDto
{
    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;
    [JsonPropertyName("password")]
    public string Password { get; init; } = string.Empty;
    [JsonPropertyName("firstName")]
    public string FirstName { get; init; } = string.Empty;
    [JsonPropertyName("lastName")]
    public string LastName { get; init; } = string.Empty;
    [JsonPropertyName("publicName")]
    public string PublicName { get; init; } = string.Empty;
    [JsonPropertyName("receiveEmail")]
    public bool ReceiveEmail { get; init; } = false;
    [JsonPropertyName("photoVolunteer")]
    public bool PhotoVolunteer { get; init; } = false;
    [JsonPropertyName("termsAndCondition")]
    public bool TermsAndCondition { get; init; } = false;
}

public record UserDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }
    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;
    [JsonPropertyName("firstName")]
    public string FirstName { get; init; } = string.Empty;
    [JsonPropertyName("lastName")]
    public string LastName { get; init; } = string.Empty;
    [JsonPropertyName("publicName")]
    public string PublicName { get; init; } = string.Empty;
    [JsonPropertyName("profilePic")]
    public string? ProfilePic { get; init; }
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; init; }
    [JsonPropertyName("receiveEmail")]
    public bool ReceiveEmail { get; init; } = false;
    [JsonPropertyName("photoVolunteer")]
    public bool PhotoVolunteer { get; init; } = false;
    [JsonPropertyName("termsAndCondition")]
    public bool TermsAndCondition { get; init; } = false;
}


public record AuthResponseDto
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }
    [JsonPropertyName("message")]
    public string Message { get; init; } = string.Empty;
    [JsonPropertyName("user")]
    public UserDto? User { get; init; }
    [JsonPropertyName("token")]
    public string? Token { get; init; }
}
