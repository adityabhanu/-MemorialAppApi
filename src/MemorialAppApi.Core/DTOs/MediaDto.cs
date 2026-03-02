namespace MemorialAppApi.Core.DTOs;

public record MediaDto
{
    public string? Photos { get; init; }
    public string? Footprints { get; init; }
    public string? FamilyPhotos { get; init; }
    public string? WeddingPhotos { get; init; }
    public string? Videos { get; init; }
    public string? VoiceNotes { get; init; }
    public string? HandwrittenNotes { get; init; }
}
