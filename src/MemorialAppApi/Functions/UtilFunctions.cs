using MediatR;
using MemorialAppApi.Core.DTOs;
using MemorialAppApi.Core.Interfaces;
using MemorialAppApi.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace MemorialAppApi.Functions;

public class UtilFunctions
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly ILogger<UtilFunctions> _logger;

    public UtilFunctions(
        IBlobStorageService blobStorageService,
        ILogger<UtilFunctions> logger)
    {
        _blobStorageService = blobStorageService;
        _logger = logger;
    }

    [Function("GenerateUploadUrl")]
    public async Task<HttpResponseData> GenerateUploadUrl(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "utils/generate-upload-url")] HttpRequestData req,
        FunctionContext executionContext)
    {
        _logger.LogInformation("GenerateUploadUrl function triggered");

        try
        {
            // Get query parameters
            var queryParams = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            var type = queryParams["type"];
            var id = queryParams["id"];

            // Validate type
            if (string.IsNullOrWhiteSpace(type))
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteAsJsonAsync(new { error = "Type query parameter is required (memorial or cemetery)" });
                return badResponse;
            }

            // Validate id
            if (string.IsNullOrWhiteSpace(id))
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteAsJsonAsync(new { error = "Id query parameter is required" });
                return badResponse;
            }

            _logger.LogInformation("Generating upload URL for type: {Type}, id: {Id}", type, id);

            // Generate SAS URL
            var result = await _blobStorageService.GenerateUploadSasUrlAsync(type, id);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result);
            return response;
        }
        catch (ArgumentException argEx)
        {
            _logger.LogWarning("Invalid argument: {Message}", argEx.Message);
            var response = req.CreateResponse(HttpStatusCode.BadRequest);
            await response.WriteAsJsonAsync(new { error = argEx.Message });
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating upload URL");
            var response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteAsJsonAsync(new { error = "An error occurred while generating the upload URL" });
            return response;
        }
    }
}
