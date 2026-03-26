using MediatR;
using MemorialAppApi.Core.DTOs;
using MemorialAppApi.Core.Interfaces;
using MemorialAppApi.Core.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace MemorialAppApi.Core.Queries;

public class GetUpcomingMemorialsQueryHandler
    : IRequestHandler<GetUpcomingMemorialsQuery, UpcomingMemorialResponseDto>
{
    private readonly IMemorialRepository _repository;
    private readonly ILogger<GetUpcomingMemorialsQueryHandler> _logger;

    public GetUpcomingMemorialsQueryHandler(
        IMemorialRepository repository,
        ILogger<GetUpcomingMemorialsQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<UpcomingMemorialResponseDto> Handle(
        GetUpcomingMemorialsQuery request,
        CancellationToken cancellationToken)
    {
        var memorials = await _repository
            .GetUpcomingMemorialsAsync(request.UserId, cancellationToken);

        var hasRecentContribution = await _repository
            .HasRecentContributionAsync(request.UserId, cancellationToken);

        var now = DateTime.UtcNow.Date;
        var nextWeek = now.AddDays(7);

        var result = new List<UpcomingMemorialDto>();

        foreach (var m in memorials)
        {
            DateTime? eventDate = null;
            string? message = null;

            var profileType = m.ProfileType?.ToUpper();

            try
            {
                if (profileType == "NEWBORN" || profileType == "LIVING")
                {
                    if (!string.IsNullOrEmpty(m.BirthDetails))
                    {
                        var birth = JsonSerializer.Deserialize<BirthDetailsModel>(m.BirthDetails);

                        if (DateTime.TryParse(birth?.birthDate, out var dob))
                        {
                            var thisYear = new DateTime(now.Year, dob.Month, dob.Day);

                            if (thisYear < now)
                                thisYear = thisYear.AddYears(1);

                            if (thisYear >= now && thisYear <= nextWeek)
                            {
                                eventDate = thisYear;
                                message = $"Birth anniversary of {m.FullName}";
                            }
                        }
                    }
                }
                else if (profileType == "PASSED")
                {
                    if (!string.IsNullOrEmpty(m.PassingDetails))
                    {
                        var passing = JsonSerializer.Deserialize<PassingDetailsModel>(m.PassingDetails);

                        if (DateTime.TryParse(passing?.passingDate, out var dod))
                        {
                            var thisYear = new DateTime(now.Year, dod.Month, dod.Day);

                            if (thisYear < now)
                                thisYear = thisYear.AddYears(1);

                            if (thisYear >= now && thisYear <= nextWeek)
                            {
                                eventDate = thisYear;
                                message = $"Death anniversary of {m.FullName}";
                            }
                        }
                    }
                }
            }
            catch
            {
                continue;
            }

            if (eventDate == null)
                continue;

            result.Add(new UpcomingMemorialDto
            {
                Id = m.Id,
                ProfileType = m.ProfileType,
                FullName = m.FullName,
                BirthDetails = m.BirthDetails,
                PassingDetails = m.PassingDetails,
                Media = m.Media,
                UpcomingEventDate = eventDate,
                Message = message
            });
        }

        return new UpcomingMemorialResponseDto
        {
            Memorials = result,
            HasRecentContribution = hasRecentContribution
        };
    }
}