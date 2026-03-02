using MemorialAppApi.Core.DTOs;

namespace MemorialAppApi.Core.Commands;

public record CreateCemeteryCommand : IRequest<CemetryDto>
{
    public string Name { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public string StreetAddress { get; init; } = string.Empty;
    public string Longitude { get; init; } = string.Empty;
    public string Latitude { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string AdditionalInfo { get; init; } = string.Empty;
    public string Status { get; init; } = "Active";
    public ContactDto? ContactInfo { get; init; }
}
