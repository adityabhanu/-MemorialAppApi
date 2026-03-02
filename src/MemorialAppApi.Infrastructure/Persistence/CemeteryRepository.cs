using Microsoft.Extensions.Logging;

namespace MemorialAppApi.Infrastructure.Persistence;

public class CemeteryRepository : ICemeteryRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CemeteryRepository> _logger;

    public CemeteryRepository(
        ApplicationDbContext context,
        ILogger<CemeteryRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Cemetery> CreateAsync(Cemetery cemetery, CancellationToken cancellationToken)
    {
        try
        {
            await _context.Cemeteries.AddAsync(cemetery, cancellationToken);
            
            // Add contact if present
            if (cemetery.Contact != null)
            {
                await _context.Contacts.AddAsync(cemetery.Contact, cancellationToken);
            }
            
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Cemetery created in database with ID: {CemeteryId}", cemetery.Id);
            
            return cemetery;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating cemetery in database");
            throw;
        }
    }

    public async Task<Cemetery?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Cemeteries
            .Include(c => c.Contact)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Cemetery> UpdateAsync(Cemetery cemetery, CancellationToken cancellationToken)
    {
        try
        {
            cemetery.UpdatedAt = DateTime.UtcNow;
            _context.Cemeteries.Update(cemetery);
            
            // Update contact if present
            if (cemetery.Contact != null)
            {
                cemetery.Contact.UpdatedAt = DateTime.UtcNow;
                _context.Contacts.Update(cemetery.Contact);
            }
            
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Cemetery updated in database with ID: {CemeteryId}", cemetery.Id);
            
            return cemetery;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating cemetery in database");
            throw;
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var cemetery = await _context.Cemeteries.FindAsync(new object[] { id }, cancellationToken);
            if (cemetery == null)
            {
                _logger.LogWarning("Cemetery not found with ID: {CemeteryId}", id);
                return;
            }

            // Soft delete
            cemetery.IsDeleted = true;
            cemetery.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Cemetery soft deleted in database with ID: {CemeteryId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting cemetery in database");
            throw;
        }
    }

    public async Task<IEnumerable<Cemetery>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        return await _context.Cemeteries
            .Include(c => c.Contact)
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}
