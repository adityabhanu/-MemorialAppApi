using Microsoft.Extensions.Logging;
using MemorialAppApi.Core.DTOs;

namespace MemorialAppApi.Core.Commands;

public class CreateCemeteryCommandHandler : IRequestHandler<CreateCemeteryCommand, CemetryDto>
{
    private readonly ICemeteryRepository _cemeteryRepository;
    private readonly ILogger<CreateCemeteryCommandHandler> _logger;

    public CreateCemeteryCommandHandler(
        ICemeteryRepository cemeteryRepository,
        ILogger<CreateCemeteryCommandHandler> logger)
    {
        _cemeteryRepository = cemeteryRepository;
        _logger = logger;
    }

    public async Task<CemetryDto> Handle(CreateCemeteryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating cemetery: {Name}", request.Name);

        var cemetery = new Cemetery
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Location = request.Location,
            StreetAddress = request.StreetAddress,
            Longitude = decimal.TryParse(request.Longitude, out var lon) ? lon : null,
            Latitude = decimal.TryParse(request.Latitude, out var lat) ? lat : null,
            Description = request.Description,
            AdditionalInfo = request.AdditionalInfo,
            Status = request.Status,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        // Create contact if provided
        if (request.ContactInfo != null)
        {
            cemetery.Contact = new Contact
            {
                Id = Guid.NewGuid(),
                CemeteryId = cemetery.Id,
                Email = request.ContactInfo.Email,
                Phone = request.ContactInfo.Phone,
                Website = request.ContactInfo.Website,
                OfficeAddress = request.ContactInfo.OfficeAddress,
                CreatedAt = DateTime.UtcNow
            };
        }

        var created = await _cemeteryRepository.CreateAsync(cemetery, cancellationToken);

        _logger.LogInformation("Cemetery created with ID: {CemeteryId}", created.Id);

        return new CemetryDto
        {
            Id = created.Id,
            Name = created.Name,
            Location = created.Location,
            StreetAddress = created.StreetAddress,
            Longitude = created.Longitude?.ToString() ?? string.Empty,
            Latitude = created.Latitude?.ToString() ?? string.Empty,
            Description = created.Description,
            AdditionalInfo = created.AdditionalInfo,
            Status = created.Status,
            ContactInfo = created.Contact != null ? new ContactDto
            {
                Email = created.Contact.Email,
                Phone = created.Contact.Phone,
                Website = created.Contact.Website,
                OfficeAddress = created.Contact.OfficeAddress
            } : null,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt
        };
    }
}
