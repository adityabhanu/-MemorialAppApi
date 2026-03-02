using MemorialAppApi.Core.DTOs;

namespace MemorialAppApi.Core.Queries;

public record GetMemorialByIdQuery : IRequest<MemorialDto?>
{
    public Guid Id { get; init; }
}
