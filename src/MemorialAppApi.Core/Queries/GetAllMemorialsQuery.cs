using MemorialAppApi.Core.DTOs;

namespace MemorialAppApi.Core.Queries;

public record GetAllMemorialsQuery : IRequest<List<MemorialDto>>
{
    public Guid? UserId { get; init; }
    public int? Page { get; init; }
    public int? PageSize { get; init; }
}
