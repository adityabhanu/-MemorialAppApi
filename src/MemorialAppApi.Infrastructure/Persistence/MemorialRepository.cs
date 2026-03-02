using MemorialAppApi.Core.Entities;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace MemorialAppApi.Infrastructure.Persistence;

public class MemorialRepository : IMemorialRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MemorialRepository> _logger;

    public MemorialRepository(
        ApplicationDbContext context,
        ILogger<MemorialRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Memorial?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Memorials
            .Include(m => m.Timelines!.Where(t => !t.IsDeleted))
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<Memorial?> GetByIdWithTrackingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Memorials
            .Include(m => m.Timelines!.Where(t => !t.IsDeleted))
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Memorial>> GetAllAsync(
         int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        if (page != null && pageSize != null)
        {
            return await _context.Memorials
            .Include(m => m.Timelines!.Where(t => !t.IsDeleted))
            .AsNoTracking()
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page.Value - 1) * pageSize.Value)
            .Take(pageSize.Value)
            .ToListAsync(cancellationToken);
        }
        else
        {
            return await _context.Memorials
                .Include(m => m.Timelines!.Where(t => !t.IsDeleted))
                .AsNoTracking()
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<Memorial>> GetAllForUserAsync(
        Guid userId,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        if (page != null && pageSize != null)
        {
            return await _context.Memorials
                .Include(m => m.Timelines!.Where(t => !t.IsDeleted))
                .AsNoTracking()
                .Where(m => m.CreatedBy == userId || m.IsPublic)
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page.Value - 1) * pageSize.Value)
                .Take(pageSize.Value)
                .ToListAsync(cancellationToken);
        }
        else
        {
            return await _context.Memorials
                .Include(m => m.Timelines!.Where(t => !t.IsDeleted))
                .AsNoTracking()
                .Where(m => m.CreatedBy == userId || m.IsPublic)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }

    public async Task<Memorial> CreateAsync(Memorial memorial, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Memorials.AddAsync(memorial, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Memorial created in database with ID: {MemorialId}", memorial.Id);
            return memorial;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating memorial in database");
            throw;
        }
    }

    public async Task<Memorial> UpdateAsync(Memorial memorial, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Memorials.Update(memorial);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Memorial updated in database with ID: {MemorialId}", memorial.Id);
            return memorial;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating memorial in database");
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var memorial = await _context.Memorials.FindAsync(new object[] { id }, cancellationToken);
            if (memorial == null)
                return false;

            // Soft delete
            memorial.IsDeleted = true;
            memorial.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Memorial soft deleted in database with ID: {MemorialId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting memorial in database");
            throw;
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Memorials.AnyAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<Burial> CreateBurialAsync(Burial burial, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Burials.AddAsync(burial, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Burial created in database with ID: {BurialId}",
                burial.Id);
            return burial;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating burial in database");
            throw;
        }
    }

    public async Task<Burial?> GetBurialByMemorialIdAsync(Guid burialId, CancellationToken cancellationToken = default)
    {
        return await _context.Burials
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == burialId, cancellationToken);
    }

    public async Task<MemorialTimeline> CreateTimelineAsync(MemorialTimeline timeline, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.MemorialTimelines.AddAsync(timeline, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("MemorialTimeline created in database with ID: {TimelineId} for Memorial: {MemorialId}",
                timeline.Id, timeline.MemorialId);
            return timeline;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating memorial timeline in database");
            throw;
        }
    }

    public async Task<MemorialTimeline?> GetTimelineByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.MemorialTimelines
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, cancellationToken);
    }

    public async Task<List<MemorialTimeline>> GetTimelinesByMemorialIdAsync(Guid memorialId, CancellationToken cancellationToken = default)
    {
        return await _context.MemorialTimelines
            .Where(t => t.MemorialId == memorialId && !t.IsDeleted)
            .OrderBy(t => t.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<MemorialTimeline> UpdateTimelineAsync(MemorialTimeline timeline, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.MemorialTimelines.Update(timeline);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("MemorialTimeline updated in database with ID: {TimelineId} for Memorial: {MemorialId}",
                timeline.Id, timeline.MemorialId);
            return timeline;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating memorial timeline in database");
            throw;
        }
    }
}
