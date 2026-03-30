using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MemorialAppApi.Core.DTOs;
using MemorialAppApi.Core.Interfaces;

namespace MemorialAppApi.Infrastructure.Services;

public class BlobStorageService : IBlobStorageService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<BlobStorageService> _logger;
    private readonly BlobServiceClient _blobServiceClient;
    private string _containerName;
    private readonly int _sasExpirationMinutes;

    public BlobStorageService(
        IConfiguration configuration,
        ILogger<BlobStorageService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        var connectionString = _configuration["AzureStorage:ConnectionString"];
        _containerName = _configuration["AzureStorage:ContainerName"] ?? "uploads";
        _sasExpirationMinutes = int.Parse(_configuration["AzureStorage:SasTokenExpirationMinutes"] ?? "20");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Azure Storage connection string is not configured");
        }

        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task<BlobUploadResponseDto> GenerateUploadSasUrlAsync(
        string type,
        string id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Determine container name based on type
            var containerName = _containerName; // Default container
            
            if (!string.IsNullOrEmpty(type))
            {
                var normalizedType = type.ToLowerInvariant();
                if (normalizedType == "memorial")
                    containerName = "memorial-uploads";
                else if (normalizedType == "cemetery")
                    containerName = "cemetery-uploads";
                else if (normalizedType == "profile")
                    containerName = "profile-uploads";
                else if (normalizedType == "timeline")
                    containerName = "timeline-uploads";
            }

            // Create the base path where files will be uploaded (inside the container)
            var basePath = $"{id}";

            // Get or create container
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            // Generate container-level SAS token
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                Resource = "c", // container
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-1), // 5 minutes grace period
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(_sasExpirationMinutes)
            };

            // Set permissions for upload (create, write, and list)
            sasBuilder.SetPermissions(BlobContainerSasPermissions.Create | 
                                     BlobContainerSasPermissions.Write | 
                                     BlobContainerSasPermissions.List);

            // Generate SAS URI for container
            var sasUri = containerClient.GenerateSasUri(sasBuilder);
            var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_sasExpirationMinutes).DateTime;

            // Build the base URL with path
            var baseUrl = $"{containerClient.Uri}/{basePath}";
            var fullUploadUrl = $"{baseUrl}{sasUri.Query}";

            _logger.LogInformation("Generated SAS URL for container: {ContainerName}, path: {BasePath}, expires at: {ExpiresAt}", 
                containerName, basePath, expiresAt);

            return new BlobUploadResponseDto
            {
                BlobUrl = baseUrl,
                BlobPath = basePath,
                SasToken = sasUri.Query,
                FullUploadUrl = fullUploadUrl,
                ExpiresAt = expiresAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating SAS URL for type: {Type}", type);
            throw;
        }
    }

    public async Task DeleteFileAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return;

        try
        {
            var uri = new Uri(url);

            // 🔥 Decode path
            var decodedPath = Uri.UnescapeDataString(uri.AbsolutePath);

            var segments = decodedPath.TrimStart('/').Split('/', 2);

            var containerName = segments[0];
            var blobPath = segments.Length > 1 ? segments[1] : string.Empty;

            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobPath);

            var result = await blobClient.DeleteIfExistsAsync();

            _logger.LogInformation("Delete result: {Result}, Blob: {Blob}", result.Value, blobPath);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete blob: {Url}", url);
        }
    }

    public async Task DeleteFilesAsync(IEnumerable<string> urls)
    {
        if (urls == null)
            return;

        var tasks = urls
            .Where(u => !string.IsNullOrWhiteSpace(u))
            .Select(DeleteFileAsync);

        await Task.WhenAll(tasks);
    }
}
