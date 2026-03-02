using MemorialAppApi.Core.DTOs;

namespace MemorialAppApi.Core.Interfaces;

public interface IBlobStorageService
{
    /// <summary>
    /// Generates a SAS token URL for uploading files to Azure Blob Storage
    /// </summary>
    /// <param name="type">Type of upload (memorial or cemetery)</param>
    /// <param name="id">ID for the folder structure</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Blob upload response with SAS URL and base path</returns>
    Task<BlobUploadResponseDto> GenerateUploadSasUrlAsync(
        string type, 
        string id, 
        CancellationToken cancellationToken = default);
}
