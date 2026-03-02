namespace MemorialAppApi.Core.Interfaces;

public interface ICemeteryRepository
{
    Task<Cemetery> CreateAsync(Cemetery cemetery, CancellationToken cancellationToken);
    Task<Cemetery?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Cemetery> UpdateAsync(Cemetery cemetery, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Cemetery>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken);
}
