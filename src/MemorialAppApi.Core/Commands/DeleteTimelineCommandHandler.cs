using MemorialAppApi.Core.Exceptions;
using MemorialAppApi.Core.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorialAppApi.Core.Commands
{
    public class DeleteTimelineCommandHandler : IRequestHandler<DeleteTimelineCommand, bool>
    {
        private readonly IMemorialRepository _repository;
        private readonly IBlobStorageService _blobService;
        private readonly ILogger<DeleteTimelineCommandHandler> _logger;

        public DeleteTimelineCommandHandler(
            IMemorialRepository repository,
            IBlobStorageService blobService,
            ILogger<DeleteTimelineCommandHandler> logger)
        {
            _repository = repository;
            _blobService = blobService;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteTimelineCommand request, CancellationToken cancellationToken)
        {
            var timeline = await _repository.GetTimelineByIdAsync(request.Id, cancellationToken);

            if (timeline == null)
                throw new NotFoundException("Timeline not found");

            // 🔥 Extract all media URLs
            var urls = new List<string>();
            urls.AddRange(MediaHelper.ExtractUrls($"{{\"photos\":{timeline.Photos}}}"));
            urls.AddRange(MediaHelper.ExtractUrls($"{{\"video\":{timeline.Video}}}"));
            urls.AddRange(MediaHelper.ExtractUrls($"{{\"audio\":{timeline.Audio}}}"));

            _logger.LogInformation("Deleting {Count} blobs for timeline {TimelineId}", urls.Count, timeline.Id);

            // 🔥 Delete blobs
            try
            {
                var cleanedUrls = urls
                .Where(u => !string.IsNullOrWhiteSpace(u))
                .Select(MediaHelper.CleanUrl)
                .ToList();

                _logger.LogInformation("Deleting cleaned URLs: {Urls}", string.Join(",", cleanedUrls));

                await _blobService.DeleteFilesAsync(cleanedUrls);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Blob deletion failed for timeline {TimelineId}", timeline.Id);
            }

            // 🔥 Soft delete
            timeline.IsDeleted = true;
            timeline.UpdatedBy = request.UpdatedBy ?? timeline.UpdatedBy;
            timeline.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateTimelineAsync(timeline, cancellationToken);

            return true;
        }
    }
}
