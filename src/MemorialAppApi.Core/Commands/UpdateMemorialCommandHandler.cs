using Microsoft.Extensions.Logging;
using MemorialAppApi.Core.DTOs;
using MemorialAppApi.Core.Exceptions;

namespace MemorialAppApi.Core.Commands;

public class UpdateMemorialCommandHandler : IRequestHandler<UpdateMemorialCommand, MemorialDto>
{
    private readonly IMemorialRepository _memorialRepository;
    private readonly ILogger<UpdateMemorialCommandHandler> _logger;

    public UpdateMemorialCommandHandler(
        IMemorialRepository memorialRepository,
        ILogger<UpdateMemorialCommandHandler> logger)
    {
        _memorialRepository = memorialRepository;
        _logger = logger;
    }

    public async Task<MemorialDto> Handle(UpdateMemorialCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating memorial with ID: {MemorialId}", request.Id);

        var existing = await _memorialRepository.GetByIdWithTrackingAsync(request.Id, cancellationToken);
        if (existing == null)
        {
            _logger.LogWarning("Memorial not found with ID: {MemorialId}", request.Id);
            throw new NotFoundException($"Memorial with ID {request.Id} not found");
        }

        // Update all memorial fields
        existing.ProfileType = request.ProfileType;
        existing.IsPublic = request.IsPublic;
        existing.FullName = request.FullName;
        existing.BirthDetails = request.BirthDetails;
        existing.PassingDetails = request.PassingDetails;
        existing.AppearanceAtBirth = request.AppearanceAtBirth;
        existing.Family = request.Family;
        existing.Visitors = request.Visitors;
        existing.ParentThoughts = request.ParentThoughts;
        existing.Letters = request.Letters;
        existing.Notes = request.Notes;
        existing.Personalities = request.Personalities;
        existing.Hobbies = request.Hobbies;
        existing.LifeDetails = request.LifeDetails;
        existing.Media = request.Media;
        if (request.CreatedBy.HasValue)
        {
            existing.CreatedBy = request.CreatedBy;
        }
        existing.UpdatedAt = DateTime.UtcNow;

        var updated = await _memorialRepository.UpdateAsync(existing, cancellationToken);
        var result = await _memorialRepository.GetByIdAsync(updated.Id, cancellationToken);

        _logger.LogInformation("Memorial updated successfully with ID: {MemorialId}", updated.Id);

        return new MemorialDto
        {
            Id = result!.Id,
            ProfileType = result.ProfileType,
            IsPublic = result.IsPublic,
            FullName = result.FullName,
            BirthDetails = result.BirthDetails,
            PassingDetails = result.PassingDetails,
            AppearanceAtBirth = result.AppearanceAtBirth,
            Family = result.Family,
            Visitors = result.Visitors,
            ParentThoughts = result.ParentThoughts,
            Letters = result.Letters,
            Notes = result.Notes,
            Personalities = result.Personalities,
            Hobbies = result.Hobbies,
            LifeDetails = result.LifeDetails,
            Media = result.Media,
            CreatedBy = result.CreatedBy,
            CreatedAt = result.CreatedAt,
            UpdatedAt = result.UpdatedAt
        };
    }
}
