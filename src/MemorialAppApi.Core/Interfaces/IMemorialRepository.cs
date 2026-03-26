namespace MemorialAppApi.Core.Interfaces;

public interface IMemorialRepository
{
    Task<Memorial?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Memorial?> GetByIdWithTrackingAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Memorial>> GetAllAsync(int? page, int? pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<Memorial>> GetAllForUserAsync(Guid? userId, int? page, int? pageSize, CancellationToken cancellationToken = default);
    Task<Memorial> CreateAsync(Memorial memorial, CancellationToken cancellationToken = default);
    Task<Memorial> UpdateAsync(Memorial memorial, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Burial> CreateBurialAsync(Burial burial, CancellationToken cancellationToken = default);
    Task<Burial?> GetBurialByMemorialIdAsync(Guid burialId, CancellationToken cancellationToken = default);
    Task<MemorialTimeline> CreateTimelineAsync(MemorialTimeline timeline, CancellationToken cancellationToken = default);
    Task<MemorialTimeline?> GetTimelineByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<MemorialTimeline>> GetTimelinesByMemorialIdAsync(Guid memorialId, CancellationToken cancellationToken = default);
    Task<MemorialTimeline> UpdateTimelineAsync(MemorialTimeline timeline, CancellationToken cancellationToken = default);
    Task<IEnumerable<Memorial>> GetUpcomingMemorialsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> HasRecentContributionAsync(Guid userId, CancellationToken cancellationToken = default);
}
