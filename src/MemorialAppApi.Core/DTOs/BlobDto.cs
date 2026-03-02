namespace MemorialAppApi.Core.DTOs;

public record BlobUploadRequestDto
{
    public string Type { get; init; } = string.Empty; // "memorial" or "cemetery"
    public DateTime DateTime { get; init; }
    public string FileExtension { get; init; } = string.Empty; // e.g., ".jpg", ".mp4", ".mp3"
}

public record BlobUploadResponseDto
{
    public string BlobUrl { get; init; } = string.Empty;
    public string BlobPath { get; init; } = string.Empty;
    public string SasToken { get; init; } = string.Empty;
    public string FullUploadUrl { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}
