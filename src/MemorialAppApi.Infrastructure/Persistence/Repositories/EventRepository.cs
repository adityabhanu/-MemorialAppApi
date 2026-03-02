using MemorialAppApi.Core.Entities;
using MemorialAppApi.Core.Interfaces;
using MemorialAppApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MemorialAppApi.Infrastructure.Persistence.Repositories;

public class EventRepository : IEventRepository
{
    private readonly ApplicationDbContext _context;

    public EventRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(Event eventEntity)
    {
        await _context.Events.AddAsync(eventEntity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Event>> GetByMemorialIdAsync(Guid memorialId)
    {
        return await _context.Events
            .Where(e => e.MemorialId == memorialId)
            .OrderBy(e => e.EventDate)
            .ToListAsync();
    }
}
