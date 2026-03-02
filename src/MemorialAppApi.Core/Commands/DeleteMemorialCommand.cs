namespace MemorialAppApi.Core.Commands;

public record DeleteMemorialCommand : IRequest<bool>
{
    public Guid Id { get; init; }
}
