using DataSync.Domain.Entities;

namespace DataSync.Domain.Repositories;

public interface IRuleRepository
{
    Task<List<Rule>> ListAsync(CancellationToken ct = default);
    Task<Rule?> GetAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Rule rule, CancellationToken ct = default);
    Task UpdateAsync(Rule rule, CancellationToken ct = default);
    Task DisableAsync(Guid id, CancellationToken ct = default);
}

