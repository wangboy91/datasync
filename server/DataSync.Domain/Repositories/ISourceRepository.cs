using DataSync.Domain.Entities;

namespace DataSync.Domain.Repositories;

public interface ISourceRepository
{
    Task<List<Source>> ListAsync(CancellationToken ct = default);
    Task<Source?> GetAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Source source, CancellationToken ct = default);
    Task UpdateAsync(Source source, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

