using Microsoft.Extensions.Logging;
using MemorialAppApi.Core.DTOs;
using MemorialAppApi.Core.Entities;
using MemorialAppApi.Core.Exceptions;
using System.Text.Json;

namespace MemorialAppApi.Core.Commands;

public class AddTimelineCommandHandler : IRequestHandler<AddTimelineCommand, TimelineEntryDto>
{
    private readonly IMemorialRepository _memorialRepository;
    private readonly ILogger<AddTimelineCommandHandler> _logger;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public AddTimelineCommandHandler(
        IMemorialRepository memorialRepository,
        ILogger<AddTimelineCommandHandler> logger)
    {
        _memorialRepository = memorialRepository;
        _logger = logger;
    }

    public async Task<TimelineEntryDto> Handle(AddTimelineCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding/updating timeline entry for memorial {MemorialId}", request.MemorialId);

        // Verify memorial exists
        var memorialExists = await _memorialRepository.ExistsAsync(request.MemorialId, cancellationToken);
        if (!memorialExists)
        {
            _logger.LogWarning("Memorial not found with ID: {MemorialId}", request.MemorialId);
            throw new NotFoundException($"Memorial with ID {request.MemorialId} not found");
        }

        // Serialize media arrays to JSON strings
        var photosJson = request.Media?.Photos != null ? JsonSerializer.Serialize(request.Media.Photos, _jsonOptions) : null;
        var videoJson = request.Media?.Video != null ? JsonSerializer.Serialize(request.Media.Video, _jsonOptions) : null;
        var audioJson = request.Media?.Audio != null ? JsonSerializer.Serialize(request.Media.Audio, _jsonOptions) : null;

        MemorialTimeline result;

        // Check if updating existing timeline entry (Id provided)
        if (request.Id.HasValue && request.Id.Value != Guid.Empty)
        {
            var existingTimeline = await _memorialRepository.GetTimelineByIdAsync(request.Id.Value, cancellationToken);
            if (existingTimeline != null && existingTimeline.MemorialId == request.MemorialId)
            {
                existingTimeline.Title = request.Title;
                existingTimeline.Date = request.Date;
                existingTimeline.Description = request.Description;
                existingTimeline.Photos = photosJson;
                existingTimeline.Video = videoJson;
                existingTimeline.Audio = audioJson;
                existingTimeline.UpdatedAt = DateTime.UtcNow;
                result = await _memorialRepository.UpdateTimelineAsync(existingTimeline, cancellationToken);
                _logger.LogInformation("Updated timeline entry {TimelineId} for memorial {MemorialId}", result.Id, request.MemorialId);
            }
            else
            {
                throw new NotFoundException($"Timeline entry with ID {request.Id} not found for memorial {request.MemorialId}");
            }
        }
        else
        {
            // Create new timeline entry
            var timeline = new MemorialTimeline
            {
                Id = Guid.NewGuid(),
                MemorialId = request.MemorialId,
                Title = request.Title,
                Date = request.Date,
                Description = request.Description,
                Photos = photosJson,
                Video = videoJson,
                Audio = audioJson,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };
            result = await _memorialRepository.CreateTimelineAsync(timeline, cancellationToken);
            _logger.LogInformation("Created timeline entry {TimelineId} for memorial {MemorialId}", result.Id, request.MemorialId);
        }

        return new TimelineEntryDto
        {
            Id = result.Id,
            MemorialId = result.MemorialId,
            Title = result.Title,
            Date = result.Date,
            Description = result.Description,
            Media = new TimelineMediaDto
            {
                Photos = !string.IsNullOrEmpty(result.Photos) ? JsonSerializer.Deserialize<List<string>>(result.Photos, _jsonOptions) : null,
                Video = !string.IsNullOrEmpty(result.Video) ? JsonSerializer.Deserialize<List<string>>(result.Video, _jsonOptions) : null,
                Audio = !string.IsNullOrEmpty(result.Audio) ? JsonSerializer.Deserialize<List<string>>(result.Audio, _jsonOptions) : null
            }
        };
    }
}
