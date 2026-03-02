using MemorialAppApi.Core.DTOs;

namespace MemorialAppApi.Core.Commands;

public class UpdateMediaCommand : IRequest<MemorialDto>
{
    public Guid MemorialId { get; set; }
    public string? Photos { get; set; }
    public string? Footprints { get; set; }
    public string? FamilyPhotos { get; set; }
    public string? WeddingPhotos { get; set; }
    public string? Videos { get; set; }
    public string? VoiceNotes { get; set; }
    public string? HandwrittenNotes { get; set; }
}
