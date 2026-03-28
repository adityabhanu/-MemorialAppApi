using Microsoft.Extensions.Logging;
using MemorialAppApi.Core.DTOs;
using MemorialAppApi.Core.Exceptions;
using MemorialAppApi.Core.Helpers;
using System.Text.Json;

namespace MemorialAppApi.Core.Commands;

public class UpdateMediaCommandHandler : IRequestHandler<UpdateMediaCommand, MemorialDto>
{
    private readonly IMemorialRepository _memorialRepository;
    private readonly ILogger<UpdateMediaCommandHandler> _logger;
    private readonly IBlobStorageService _blobStorageService;

    public UpdateMediaCommandHandler(
        IMemorialRepository memorialRepository,
        ILogger<UpdateMediaCommandHandler> logger,
        IBlobStorageService blobStorageService)
    {
        _memorialRepository = memorialRepository;
        _logger = logger;
        _blobStorageService = blobStorageService;
    }

    public async Task<MemorialDto> Handle(UpdateMediaCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating media for memorial {MemorialId}", request.MemorialId);

        var memorial = await _memorialRepository.GetByIdWithTrackingAsync(request.MemorialId, cancellationToken);

        if (memorial == null)
        {
            throw new NotFoundException($"Memorial with ID {request.MemorialId} not found");
        }

        // STEP 1: Extract OLD URLs
        var oldUrls = MediaHelper.ExtractUrls(memorial.Media);

        // STEP 2: Build new media object (same structure as before)
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

        var newMediaJson = JsonSerializer.Serialize(mediaObject);

        // STEP 3: Extract NEW URLs
        var newUrls = MediaHelper.ExtractUrls(newMediaJson);

        // STEP 4: Find removed files
        var toDelete = oldUrls.Except(newUrls).ToList();

        // STEP 5: Delete from blob
        if (toDelete.Any())
        {
            _logger.LogInformation("Deleting {Count} old media files", toDelete.Count);
            await _blobStorageService.DeleteFilesAsync(toDelete);
        }

        // STEP 6: Update DB
        memorial.Media = newMediaJson;
        memorial.UpdatedAt = DateTime.UtcNow;

        await _memorialRepository.UpdateAsync(memorial, cancellationToken);

        _logger.LogInformation("Media updated successfully for memorial {MemorialId}", request.MemorialId);

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
