using DataSync.Domain.Entities;
using DataSync.Domain.Repositories;

namespace DataSync.Infrastructure.Repositories;

public class InMemoryTargetRepository : ITargetRepository
{
    private readonly List<Target> _items = new();

    public Task<List<Target>> ListAsync(CancellationToken ct = default) => Task.FromResult(_items.ToList());
    public Task<Target?> GetAsync(Guid id, CancellationToken ct = default) => Task.FromResult(_items.FirstOrDefault(x => x.Id == id));
    public Task AddAsync(Target target, CancellationToken ct = default)
    {
        if (target.Id == Guid.Empty) target.Id = Guid.NewGuid();
        _items.Add(target);
        return Task.CompletedTask;
    }
    public Task UpdateAsync(Target target, CancellationToken ct = default)
    {
        var idx = _items.FindIndex(x => x.Id == target.Id);
        if (idx >= 0) _items[idx] = target;
        return Task.CompletedTask;
    }
    public Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        _items.RemoveAll(x => x.Id == id);
        return Task.CompletedTask;
    }
}

