using MemorialAppApi.Core.Entities;

namespace MemorialAppApi.Core.Interfaces;

public interface IEventRepository
{
    Task CreateAsync(Event eventEntity);
    Task<IEnumerable<Event>> GetByMemorialIdAsync(Guid memorialId);
}
