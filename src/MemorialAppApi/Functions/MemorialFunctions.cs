using MediatR;
using MemorialAppApi.Core.Commands;
using MemorialAppApi.Core.DTOs;
using MemorialAppApi.Core.Queries;
using MemorialAppApi.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace MemorialAppApi.Functions;

public class MemorialFunctions
{
    private readonly IMediator _mediator;
    private readonly ILogger<MemorialFunctions> _logger;

    public MemorialFunctions(IMediator mediator, ILogger<MemorialFunctions> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [Function("GetAllMemorials")]
    public async Task<HttpResponseData> GetAllMemorials(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "memorials/list")] HttpRequestData req,
        FunctionContext executionContext)
    {
        _logger.LogInformation("GetAllMemorials function triggered");

        try
        {
            var userId = JwtHelper.ExtractUserIdFromToken(req);
            Guid? effectiveUserId = userId == Guid.Empty ? null : userId;

            var queryParams = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            int? page = int.TryParse(queryParams["page"], out var p) ? p : null;
            int? pageSize = int.TryParse(queryParams["pageSize"], out var ps) ? ps : null;

            var query = new GetAllMemorialsQuery { UserId = effectiveUserId, Page = page, PageSize = pageSize };
            var result = await _mediator.Send(query);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving memorials");
            var response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteAsJsonAsync(new { error = "An error occurred while retrieving memorials" });
            return response;
        }
    }

    [Function("GetMemorialById")]
    public async Task<HttpResponseData> GetMemorialById(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "memorials/{id}")] HttpRequestData req,
        FunctionContext executionContext,
        string id)
    {
        _logger.LogInformation("GetMemorialById function triggered for ID: {Id}", id);

        try
        {
            //// Validate authorization header
            //var userId = JwtHelper.ExtractUserIdFromToken(req);
            //if (userId == Guid.Empty)
            //{
            //    var unauthorizedResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
            //    await unauthorizedResponse.WriteAsJsonAsync(new { error = "Authorization token is required or invalid" });
            //    return unauthorizedResponse;
            //}

            if (!Guid.TryParse(id, out var memorialId))
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteAsJsonAsync(new { error = "Invalid memorial ID format" });
                return badResponse;
            }

            var query = new GetMemorialByIdQuery { Id = memorialId };
            var result = await _mediator.Send(query);

            if (result == null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                await notFoundResponse.WriteAsJsonAsync(new { error = $"Memorial with ID {id} not found" });
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving memorial with ID: {Id}", id);
            var response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteAsJsonAsync(new { error = "An error occurred while retrieving the memorial" });
            return response;
        }
    }

    [Function("CreateMemorial")]
    public async Task<HttpResponseData> CreateMemorial(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "memorials/create")] HttpRequestData req,
        FunctionContext executionContext)
    {
        _logger.LogInformation("CreateMemorial function triggered");

        try
        {
            var createDto = await req.ReadFromJsonAsync<MemorialDto>();
            if (createDto == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteAsJsonAsync(new { error = "Invalid request body" });
                return badResponse;
            }

            // Extract UserId from JWT token in the Authorization header
            var userId = JwtHelper.ExtractUserIdFromToken(req);

            var command = new CreateMemorialCommand
            {
                UserId = userId,
                ProfileType = createDto.ProfileType,
                IsPublic = createDto.IsPublic,
                FullName = createDto.FullName,
                BirthDetails = createDto.BirthDetails,
                PassingDetails = createDto.PassingDetails,
                AppearanceAtBirth = createDto.AppearanceAtBirth,
                Family = createDto.Family,
                Visitors = createDto.Visitors,
                ParentThoughts = createDto.ParentThoughts,
                Letters = createDto.Letters,
                Notes = createDto.Notes,
                Personalities = createDto.Personalities,
                Hobbies = createDto.Hobbies,
                LifeDetails = createDto.LifeDetails,
                Media = createDto.Media,
                CreatedBy = userId != Guid.Empty ? userId : null
            };

            var result = await _mediator.Send(command);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(result);
            response.Headers.Add("Location", $"{req.Url.Scheme}://{req.Url.Host}/api/memorials/{result.Id}");
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
            _logger.LogError(ex, "Error creating memorial");
            var response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteAsJsonAsync(new { error = "An error occurred while creating the memorial" });
            return response;
        }
    }

    [Function("UpdateMemorial")]
    public async Task<HttpResponseData> UpdateMemorial(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "memorials/{id}")] HttpRequestData req,
        FunctionContext executionContext,
        string id)
    {
        _logger.LogInformation("UpdateMemorial function triggered for ID: {Id}", id);

        try
        {
            if (!Guid.TryParse(id, out var memorialId))
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteAsJsonAsync(new { error = "Invalid memorial ID format" });
                return badResponse;
            }

            var updateDto = await req.ReadFromJsonAsync<MemorialDto>();
            if (updateDto == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteAsJsonAsync(new { error = "Invalid request body" });
                return badResponse;
            }

            // Extract UserId from JWT token if CreatedBy is not provided
            Guid? createdBy = JwtHelper.ExtractUserIdFromTokenNullable(req) ?? updateDto.CreatedBy;

            var command = new UpdateMemorialCommand
            {
                Id = memorialId,
                ProfileType = updateDto.ProfileType,
                IsPublic = updateDto.IsPublic,
                FullName = updateDto.FullName,
                BirthDetails = updateDto.BirthDetails,
                PassingDetails = updateDto.PassingDetails,
                AppearanceAtBirth = updateDto.AppearanceAtBirth,
                Family = updateDto.Family,
                Visitors = updateDto.Visitors,
                ParentThoughts = updateDto.ParentThoughts,
                Letters = updateDto.Letters,
                Notes = updateDto.Notes,
                Personalities = updateDto.Personalities,
                Hobbies = updateDto.Hobbies,
                LifeDetails = updateDto.LifeDetails,
                Media = updateDto.Media,
                CreatedBy = createdBy
            };

            var result = await _mediator.Send(command);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result);
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
        catch (MemorialAppApi.Core.Exceptions.NotFoundException nfex)
        {
            _logger.LogWarning("Memorial not found: {Message}", nfex.Message);
            var response = req.CreateResponse(HttpStatusCode.NotFound);
            await response.WriteAsJsonAsync(new { error = nfex.Message });
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating memorial with ID: {Id}", id);
            var response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteAsJsonAsync(new { error = "An error occurred while updating the memorial" });
            return response;
        }
    }

    [Function("AddTimeline")]
    public async Task<HttpResponseData> AddTimeline(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "memorials/timeline")] HttpRequestData req,
        FunctionContext executionContext)
    {
        _logger.LogInformation("AddTimeline function triggered");

        try
        {
            var timelineEntry = await req.ReadFromJsonAsync<TimelineEntryDto>();
            if (timelineEntry == null || timelineEntry.MemorialId == Guid.Empty)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteAsJsonAsync(new { error = "Invalid request body or missing MemorialId" });
                return badResponse;
            }

            // Extract UserId from JWT token
            var userId = JwtHelper.ExtractUserIdFromToken(req);

            var command = new AddTimelineCommand
            {
                MemorialId = timelineEntry.MemorialId,
                Id = timelineEntry.Id,
                Title = timelineEntry.Title,
                Date = timelineEntry.Date,
                Description = timelineEntry.Description,
                Media = timelineEntry.Media
            };

            var result = await _mediator.Send(command);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(result);
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
        catch (MemorialAppApi.Core.Exceptions.NotFoundException nfex)
        {
            _logger.LogWarning("Memorial not found: {Message}", nfex.Message);
            var response = req.CreateResponse(HttpStatusCode.NotFound);
            await response.WriteAsJsonAsync(new { error = nfex.Message });
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding timeline entries");
            var response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteAsJsonAsync(new { error = "An error occurred while adding timeline entries" });
            return response;
        }
    }

    [Function("UpdateMedia")]
    public async Task<HttpResponseData> UpdateMedia(
        [HttpTrigger(AuthorizationLevel.Function, "patch", Route = "memorials/{id}/media")] HttpRequestData req,
        FunctionContext executionContext,
        string id)
    {
        _logger.LogInformation("UpdateMedia function triggered for ID: {Id}", id);

        try
        {
            // Validate authorization header
            var userId = JwtHelper.ExtractUserIdFromToken(req);
            if (userId == Guid.Empty)
            {
                var unauthorizedResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
                await unauthorizedResponse.WriteAsJsonAsync(new { error = "Authorization token is required or invalid" });
                return unauthorizedResponse;
            }

            if (!Guid.TryParse(id, out var memorialId))
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteAsJsonAsync(new { error = "Invalid memorial ID format" });
                return badResponse;
            }

            var mediaDto = await req.ReadFromJsonAsync<MediaDto>();
            if (mediaDto == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteAsJsonAsync(new { error = "Invalid request body" });
                return badResponse;
            }

            var command = new UpdateMediaCommand
            {
                MemorialId = memorialId,
                Photos = mediaDto.Photos,
                Footprints = mediaDto.Footprints,
                FamilyPhotos = mediaDto.FamilyPhotos,
                WeddingPhotos = mediaDto.WeddingPhotos,
                Videos = mediaDto.Videos,
                VoiceNotes = mediaDto.VoiceNotes,
                HandwrittenNotes = mediaDto.HandwrittenNotes
            };

            var result = await _mediator.Send(command);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result);
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
        catch (MemorialAppApi.Core.Exceptions.NotFoundException nfex)
        {
            _logger.LogWarning("Memorial not found: {Message}", nfex.Message);
            var response = req.CreateResponse(HttpStatusCode.NotFound);
            await response.WriteAsJsonAsync(new { error = nfex.Message });
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating media for memorial with ID: {Id}", id);
            var response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteAsJsonAsync(new { error = "An error occurred while updating media" });
            return response;
        }
    }

    [Function("DeleteMemorial")]
    public async Task<HttpResponseData> DeleteMemorial(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "memorials/{id}")] HttpRequestData req,
        FunctionContext executionContext,
        string id)
    {
        _logger.LogInformation("DeleteMemorial function triggered for ID: {Id}", id);

        try
        {
            if (!Guid.TryParse(id, out var memorialId))
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteAsJsonAsync(new { error = "Invalid memorial ID format" });
                return badResponse;
            }

            var command = new DeleteMemorialCommand { Id = memorialId };
            await _mediator.Send(command);

            var response = req.CreateResponse(HttpStatusCode.NoContent);
            return response;
        }
        catch (MemorialAppApi.Core.Exceptions.NotFoundException nfex)
        {
            _logger.LogWarning("Memorial not found: {Message}", nfex.Message);
            var response = req.CreateResponse(HttpStatusCode.NotFound);
            await response.WriteAsJsonAsync(new { error = nfex.Message });
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting memorial with ID: {Id}", id);
            var response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteAsJsonAsync(new { error = "An error occurred while deleting the memorial" });
            return response;
        }
    }
}
