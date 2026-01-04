using DataSync.Domain.Entities;

namespace DataSync.Domain.Repositories;

public interface ITargetRepository
{
    Task<List<Target>> ListAsync(CancellationToken ct = default);
    Task<Target?> GetAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Target target, CancellationToken ct = default);
    Task UpdateAsync(Target target, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

