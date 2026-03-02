using MediatR;
using MemorialAppApi.Core.Commands;
using MemorialAppApi.Core.DTOs;
using MemorialAppApi.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace MemorialAppApi.Functions;

public class CemeteryFunctions
{
    private readonly IMediator _mediator;
    private readonly ILogger<CemeteryFunctions> _logger;

    public CemeteryFunctions(IMediator mediator, ILogger<CemeteryFunctions> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [Function("CreateCemetery")]
    public async Task<HttpResponseData> CreateCemetery(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "cemeteries/create")] HttpRequestData req,
        FunctionContext executionContext)
    {
        _logger.LogInformation("CreateCemetery function triggered");

        try
        {
            var createDto = await req.ReadFromJsonAsync<CemetryDto>();
            if (createDto == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteAsJsonAsync(new { error = "Invalid request body" });
                return badResponse;
            }

            // Extract UserId from JWT token
            var userId = JwtHelper.ExtractUserIdFromToken(req);

            var command = new CreateCemeteryCommand
            {
                Name = createDto.Name,
                Location = createDto.Location,
                StreetAddress = createDto.StreetAddress,
                Longitude = createDto.Longitude,
                Latitude = createDto.Latitude,
                Description = createDto.Description,
                AdditionalInfo = createDto.AdditionalInfo,
                Status = createDto.Status,
                ContactInfo = createDto.ContactInfo
            };

            var result = await _mediator.Send(command);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(result);
            response.Headers.Add("Location", $"{req.Url.Scheme}://{req.Url.Host}/api/cemeteries/{result.Id}");
            return response;
        }
        catch (FluentValidation.ValidationException vex)
        {
            _logger.LogWarning("Validation failed: {Errors}", string.Join(", ", vex.Errors.Select(e => e.ErrorMessage)));
            var response = req.CreateResponse(HttpStatusCode.BadRequest);
            await response.WriteAsJsonAsync(new
            {
                error = "Validation failed",
                errors = vex.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
            });
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating cemetery");
            var response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteAsJsonAsync(new { error = "An error occurred while creating the cemetery" });
            return response;
        }
    }
}
