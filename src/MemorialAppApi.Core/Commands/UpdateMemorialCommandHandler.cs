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

        if (existing == null || existing.IsDeleted)
        {
            throw new NotFoundException($"Memorial with ID {request.Id} not found");
        }

        // Authorization check
        if (request.CreatedBy.HasValue && existing.CreatedBy != request.CreatedBy)
        {
            throw new UnauthorizedAccessException("You are not allowed to update this memorial");
        }

        // Update fields
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

        existing.UpdatedAt = DateTime.UtcNow;

        var updated = await _memorialRepository.UpdateAsync(existing, cancellationToken);

        return new MemorialDto
        {
            Id = updated.Id,
            ProfileType = updated.ProfileType,
            IsPublic = updated.IsPublic,
            FullName = updated.FullName,
            BirthDetails = updated.BirthDetails,
            PassingDetails = updated.PassingDetails,
            AppearanceAtBirth = updated.AppearanceAtBirth,
            Family = updated.Family,
            Visitors = updated.Visitors,
            ParentThoughts = updated.ParentThoughts,
            Letters = updated.Letters,
            Notes = updated.Notes,
            Personalities = updated.Personalities,
            Hobbies = updated.Hobbies,
            LifeDetails = updated.LifeDetails,
            Media = updated.Media,
            CreatedBy = updated.CreatedBy,
            CreatedAt = updated.CreatedAt,
            UpdatedAt = updated.UpdatedAt
        };
    }
}
