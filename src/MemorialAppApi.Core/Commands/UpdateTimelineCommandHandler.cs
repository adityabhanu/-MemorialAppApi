using MemorialAppApi.Core.DTOs;
using MemorialAppApi.Core.Exceptions;
using Microsoft.Extensions.Logging;
using MemorialAppApi.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MemorialAppApi.Core.Commands
{
    public class UpdateTimelineCommandHandler : IRequestHandler<UpdateTimelineCommand, TimelineEntryDto>
    {
        private readonly IMemorialRepository _repository;
        private readonly IBlobStorageService _blobService;
        private readonly ILogger<UpdateTimelineCommandHandler> _logger;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public UpdateTimelineCommandHandler(
            IMemorialRepository repository,
            IBlobStorageService blobService,
            ILogger<UpdateTimelineCommandHandler> logger)
        {
            _repository = repository;
            _blobService = blobService;
            _logger = logger;
        }

        public async Task<TimelineEntryDto> Handle(UpdateTimelineCommand request, CancellationToken cancellationToken)
        {
            var existing = await _repository.GetTimelineByIdAsync(request.Id, cancellationToken);

            if (existing == null)
                throw new NotFoundException("Timeline not found");

            // 🔥 STEP 1: Extract OLD URLs
            var oldUrls = new List<string>();
            oldUrls.AddRange(JsonHelper.Parse(existing.Photos));
            oldUrls.AddRange(JsonHelper.Parse(existing.Video));
            oldUrls.AddRange(JsonHelper.Parse(existing.Audio));

            // 🔥 STEP 2: Extract NEW URLs
            var newUrls = new List<string>();
            newUrls.AddRange(request.Media?.Photos ?? []);
            newUrls.AddRange(request.Media?.Video ?? []);
            newUrls.AddRange(request.Media?.Audio ?? []);

            // 🔥 NORMALIZE (VERY IMPORTANT)
            oldUrls = JsonHelper.Normalize(oldUrls);
            newUrls = JsonHelper.Normalize(newUrls);

            // 🔥 STEP 3: DIFF
            var toDelete = oldUrls.Except(newUrls).ToList();

            _logger.LogInformation("Timeline {Id} media diff → OLD: {OldCount}, NEW: {NewCount}, DELETE: {DeleteCount}",
                request.Id, oldUrls.Count, newUrls.Count, toDelete.Count);

            // 🔥 STEP 4: DELETE BLOBS
            if (toDelete.Any())
            {
                try
                {
                    await _blobService.DeleteFilesAsync(toDelete);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed deleting blobs for timeline {Id}", request.Id);
                }
            }

            // 🔥 STEP 5: SAVE NEW MEDIA
            existing.Photos = request.Media?.Photos != null
                ? JsonSerializer.Serialize(request.Media.Photos, _jsonOptions)
                : null;

            existing.Video = request.Media?.Video != null
                ? JsonSerializer.Serialize(request.Media.Video, _jsonOptions)
                : null;

            existing.Audio = request.Media?.Audio != null
                ? JsonSerializer.Serialize(request.Media.Audio, _jsonOptions)
                : null;

            existing.Title = request.Title;
            existing.Date = request.Date;
            existing.Description = request.Description;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = request.UpdatedBy;

            var updated = await _repository.UpdateTimelineAsync(existing, cancellationToken);

            return new TimelineEntryDto
            {
                Id = updated.Id,
                MemorialId = updated.MemorialId,
                Title = updated.Title,
                Date = updated.Date,
                Description = updated.Description,
                Media = new TimelineMediaDto
                {
                    Photos = JsonHelper.Parse(updated.Photos),
                    Video = JsonHelper.Parse(updated.Video),
                    Audio = JsonHelper.Parse(updated.Audio)
                }
            };
        }
    }
}
