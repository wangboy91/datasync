using DataSync.Domain.Entities;
using DataSync.Domain.Repositories;

namespace DataSync.Infrastructure.Repositories;

public class InMemoryJobRepository : IJobRepository
{
    private readonly List<Job> _items = new();

    public Task<List<Job>> ListAsync(CancellationToken ct = default) => Task.FromResult(_items.ToList());
    public Task<Job?> GetAsync(Guid id, CancellationToken ct = default) => Task.FromResult(_items.FirstOrDefault(x => x.Id == id));
    public Task AddAsync(Job job, CancellationToken ct = default)
    {
        if (job.Id == Guid.Empty) job.Id = Guid.NewGuid();
        _items.Add(job);
        return Task.CompletedTask;
    }
    public Task UpdateAsync(Job job, CancellationToken ct = default)
    {
        var idx = _items.FindIndex(x => x.Id == job.Id);
        if (idx >= 0) _items[idx] = job;
        return Task.CompletedTask;
    }
}

