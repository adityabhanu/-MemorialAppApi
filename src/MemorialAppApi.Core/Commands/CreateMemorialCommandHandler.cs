using Microsoft.Extensions.Logging;
using MemorialAppApi.Core.DTOs;
using System.Text.Json;

namespace MemorialAppApi.Core.Commands;

public class CreateMemorialCommandHandler : IRequestHandler<CreateMemorialCommand, MemorialDto>
{
    private readonly IMemorialRepository _memorialRepository;
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<CreateMemorialCommandHandler> _logger;

    public CreateMemorialCommandHandler(
        IMemorialRepository memorialRepository,
        IEventRepository eventRepository,
        ILogger<CreateMemorialCommandHandler> logger)
    {
        _memorialRepository = memorialRepository;
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task<MemorialDto> Handle(CreateMemorialCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating memorial for {FullName}", request.FullName);

        // Create memorial
        var memorial = new Memorial
        {
            Id = Guid.NewGuid(),
            ProfileType = request.ProfileType,
            IsPublic = request.IsPublic,
            FullName = request.FullName,
            BirthDetails = request.BirthDetails,
            PassingDetails = request.PassingDetails,
            AppearanceAtBirth = request.AppearanceAtBirth,
            Family = request.Family,
            Visitors = request.Visitors,
            ParentThoughts = request.ParentThoughts,
            Letters = request.Letters,
            Notes = request.Notes,
            Personalities = request.Personalities,
            Hobbies = request.Hobbies,
            LifeDetails = request.LifeDetails,
            Media = request.Media,
            CreatedBy = request.CreatedBy,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        var created = await _memorialRepository.CreateAsync(memorial, cancellationToken);

        _logger.LogInformation("Memorial created with ID: {MemorialId}", created.Id);

        // Create events from BirthDetail and PassingDetail if they contain location data
        await CreateEventsFromDetailsAsync(created.Id, request.BirthDetails, request.PassingDetails, request.CreatedBy);

        // Build response DTO
        return new MemorialDto
        {
            Id = created.Id,
            ProfileType = created.ProfileType,
            IsPublic = created.IsPublic,
            FullName = created.FullName,
            BirthDetails = created.BirthDetails,
            PassingDetails = created.PassingDetails,
            AppearanceAtBirth = created.AppearanceAtBirth,
            Family = created.Family,
            Visitors = created.Visitors,
            ParentThoughts = created.ParentThoughts,
            Letters = created.Letters,
            Notes = created.Notes,
            Personalities = created.Personalities,
            Hobbies = created.Hobbies,
            LifeDetails = created.LifeDetails,
            Media = created.Media,
            CreatedBy = created.CreatedBy,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt
        };
    }

    private async Task CreateEventsFromDetailsAsync(Guid memorialId, string? birthDetail, string? passingDetail, Guid? createdBy)
    {
        try
        {
            // Process BirthDetail
            if (!string.IsNullOrWhiteSpace(birthDetail))
            {
                var birthEventData = JsonSerializer.Deserialize<BirthDetail>(birthDetail);
                if (birthEventData != null)
                {
                    var birthEvent = new Event
                    {
                        Id = Guid.NewGuid(),
                        MemorialId = memorialId,
                        EventName = "Birth",
                        EventDate = birthEventData.BirthDate,
                        LocationName = birthEventData.BirthPlace?.Name,
                        Address = birthEventData.BirthPlace?.Address,
                        Latitude = birthEventData.BirthPlace?.Latitude,
                        Longitude = birthEventData.BirthPlace?.Longitude,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = createdBy?.ToString()
                    };

                    await _eventRepository.CreateAsync(birthEvent);
                    _logger.LogInformation("Birth event created for memorial {MemorialId}", memorialId);
                }
            }

            // Process PassingDetail
            if (!string.IsNullOrWhiteSpace(passingDetail))
            {
                var passingEventData = JsonSerializer.Deserialize<PassingDetail>(passingDetail);
                if (passingEventData != null)
                {
                    var passingEvent = new Event
                    {
                        Id = Guid.NewGuid(),
                        MemorialId = memorialId,
                        EventName = "Passing",
                        EventDate = passingEventData.PassingDate,
                        LocationName = passingEventData.PassingPlace?.Name,
                        Address = passingEventData.PassingPlace?.Address,
                        Latitude = passingEventData.PassingPlace?.Latitude,
                        Longitude = passingEventData.PassingPlace?.Longitude,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = createdBy?.ToString()
                    };

                    await _eventRepository.CreateAsync(passingEvent);
                    _logger.LogInformation("Passing event created for memorial {MemorialId}", memorialId);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to create events from details for memorial {MemorialId}", memorialId);
            // Don't throw - event creation failure shouldn't fail memorial creation
        }
    }

}
