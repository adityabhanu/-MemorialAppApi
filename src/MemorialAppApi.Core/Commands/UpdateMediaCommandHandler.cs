using Microsoft.Extensions.Logging;
using MemorialAppApi.Core.DTOs;
using MemorialAppApi.Core.Exceptions;
using System.Text.Json;

namespace MemorialAppApi.Core.Commands;

public class UpdateMediaCommandHandler : IRequestHandler<UpdateMediaCommand, MemorialDto>
{
    private readonly IMemorialRepository _memorialRepository;
    private readonly ILogger<UpdateMediaCommandHandler> _logger;

    public UpdateMediaCommandHandler(
        IMemorialRepository memorialRepository,
        ILogger<UpdateMediaCommandHandler> logger)
    {
        _memorialRepository = memorialRepository;
        _logger = logger;
    }

    public async Task<MemorialDto> Handle(UpdateMediaCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating media for memorial {MemorialId}", request.MemorialId);

        var memorial = await _memorialRepository.GetByIdWithTrackingAsync(request.MemorialId, cancellationToken);
        if (memorial == null)
        {
            throw new NotFoundException($"Memorial with ID {request.MemorialId} not found");
        }

        // Build media JSON object from the request
        var mediaObject = new
        {
            photos = request.Photos,
            footprints = request.Footprints,
            familyPhotos = request.FamilyPhotos,
            weddingPhotos = request.WeddingPhotos,
            videos = request.Videos,
            voiceNotes = request.VoiceNotes,
            handwrittenNotes = request.HandwrittenNotes
        };

        memorial.Media = JsonSerializer.Serialize(mediaObject, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });
        memorial.UpdatedAt = DateTime.UtcNow;

        await _memorialRepository.UpdateAsync(memorial, cancellationToken);

        _logger.LogInformation("Media updated for memorial {MemorialId}", request.MemorialId);

        return new MemorialDto
        {
            Id = memorial.Id,
            ProfileType = memorial.ProfileType,
            IsPublic = memorial.IsPublic,
            FullName = memorial.FullName,
            BirthDetails = memorial.BirthDetails,
            PassingDetails = memorial.PassingDetails,
            AppearanceAtBirth = memorial.AppearanceAtBirth,
            Family = memorial.Family,
            Visitors = memorial.Visitors,
            ParentThoughts = memorial.ParentThoughts,
            Letters = memorial.Letters,
            Notes = memorial.Notes,
            Personalities = memorial.Personalities,
            Hobbies = memorial.Hobbies,
            LifeDetails = memorial.LifeDetails,
            Media = memorial.Media,
            CreatedBy = memorial.CreatedBy,
            CreatedAt = memorial.CreatedAt,
            UpdatedAt = memorial.UpdatedAt
        };
    }
}
