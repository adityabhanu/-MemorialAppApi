using Microsoft.Extensions.Logging;
using MemorialAppApi.Core.DTOs;
using System.Text.Json;

namespace MemorialAppApi.Core.Queries;

public class GetMemorialByIdQueryHandler : IRequestHandler<GetMemorialByIdQuery, MemorialDto?>
{
    private readonly IMemorialRepository _repository;
    private readonly ILogger<GetMemorialByIdQueryHandler> _logger;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public GetMemorialByIdQueryHandler(
        IMemorialRepository repository,
        ILogger<GetMemorialByIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<MemorialDto?> Handle(GetMemorialByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting memorial with ID: {MemorialId}", request.Id);

        var memorial = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (memorial == null)
        {
            _logger.LogWarning("Memorial not found with ID: {MemorialId}", request.Id);
            return null;
        }

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
            UpdatedAt = memorial.UpdatedAt,
            Timelines = memorial.Timelines?.Select(t => new TimelineEntryDto
            {
                Id = t.Id,
                MemorialId = t.MemorialId,
                Title = t.Title,
                Date = t.Date,
                Description = t.Description,
                Media = new TimelineMediaDto
                {
                    Photos = !string.IsNullOrEmpty(t.Photos) ? JsonSerializer.Deserialize<List<string>>(t.Photos, _jsonOptions) : null,
                    Video = !string.IsNullOrEmpty(t.Video) ? JsonSerializer.Deserialize<List<string>>(t.Video, _jsonOptions) : null,
                    Audio = !string.IsNullOrEmpty(t.Audio) ? JsonSerializer.Deserialize<List<string>>(t.Audio, _jsonOptions) : null
                }
            }).ToList()
        };
    }
}
