using DataSync.Domain.Entities;

namespace DataSync.Domain.Repositories;

public interface IJobRepository
{
    Task<List<Job>> ListAsync(CancellationToken ct = default);
    Task<Job?> GetAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Job job, CancellationToken ct = default);
    Task UpdateAsync(Job job, CancellationToken ct = default);
}

