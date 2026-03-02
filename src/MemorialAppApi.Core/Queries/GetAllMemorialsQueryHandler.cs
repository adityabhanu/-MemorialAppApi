using Microsoft.Extensions.Logging;
using MemorialAppApi.Core.DTOs;
using System.Text.Json;

namespace MemorialAppApi.Core.Queries;

public class GetAllMemorialsQueryHandler : IRequestHandler<GetAllMemorialsQuery, List<MemorialDto>>
{
    private readonly IMemorialRepository _repository;
    private readonly ILogger<GetAllMemorialsQueryHandler> _logger;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public GetAllMemorialsQueryHandler(
        IMemorialRepository repository,
        ILogger<GetAllMemorialsQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<MemorialDto>> Handle(GetAllMemorialsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all memorials for user {UserId} - Page: {Page}, PageSize: {PageSize}", 
            request.UserId, request.Page, request.PageSize);

        var memorials = await _repository.GetAllForUserAsync(request.UserId, request.Page, request.PageSize, cancellationToken);

        return memorials.Select(m => new MemorialDto
        {
            Id = m.Id,
            ProfileType = m.ProfileType,
            IsPublic = m.IsPublic,
            FullName = m.FullName,
            BirthDetails = m.BirthDetails,
            PassingDetails = m.PassingDetails,
            AppearanceAtBirth = m.AppearanceAtBirth,
            Family = m.Family,
            Visitors = m.Visitors,
            ParentThoughts = m.ParentThoughts,
            Letters = m.Letters,
            Notes = m.Notes,
            Personalities = m.Personalities,
            Hobbies = m.Hobbies,
            LifeDetails = m.LifeDetails,
            Media = m.Media,
            CreatedBy = m.CreatedBy,
            CreatedAt = m.CreatedAt,
            UpdatedAt = m.UpdatedAt,
            Timelines = m.Timelines?.Select(t => new TimelineEntryDto
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
        }).ToList();
    }
}
