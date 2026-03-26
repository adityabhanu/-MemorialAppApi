using MemorialAppApi.Core.DTOs;

namespace MemorialAppApi.Core.Commands;

public record AddTimelineCommand : IRequest<TimelineEntryDto>
{
    public Guid MemorialId { get; init; }
    public Guid? Id { get; init; }
    public string? Title { get; init; }
    public DateTime? Date { get; init; }
    public string? Description { get; init; }
    public TimelineMediaDto? Media { get; init; }
    public Guid? CreatedBy { get; init; }
    public Guid? UpdatedBy { get; init; }
}
