using Microsoft.Extensions.Logging;
using MemorialAppApi.Core.Exceptions;

namespace MemorialAppApi.Core.Commands;

public class DeleteMemorialCommandHandler : IRequestHandler<DeleteMemorialCommand, bool>
{
    private readonly IMemorialRepository _repository;
    private readonly ILogger<DeleteMemorialCommandHandler> _logger;

    public DeleteMemorialCommandHandler(
        IMemorialRepository repository,
        ILogger<DeleteMemorialCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteMemorialCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting memorial with ID: {MemorialId}", request.Id);

        var exists = await _repository.ExistsAsync(request.Id, cancellationToken);
        if (!exists)
        {
            _logger.LogWarning("Memorial not found with ID: {MemorialId}", request.Id);
            throw new NotFoundException($"Memorial with ID {request.Id} not found");
        }

        var result = await _repository.DeleteAsync(request.Id, cancellationToken);

        _logger.LogInformation("Memorial deleted successfully with ID: {MemorialId}", request.Id);

        return result;
    }
}
