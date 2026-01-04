using DataSync.Domain.Entities;
using DataSync.Domain.Repositories;

namespace DataSync.Infrastructure.Repositories;

public class InMemorySourceRepository : ISourceRepository
{
    private readonly List<Source> _items = new();

    public Task<List<Source>> ListAsync(CancellationToken ct = default) => Task.FromResult(_items.ToList());
    public Task<Source?> GetAsync(Guid id, CancellationToken ct = default) => Task.FromResult(_items.FirstOrDefault(x => x.Id == id));
    public Task AddAsync(Source source, CancellationToken ct = default)
    {
        if (source.Id == Guid.Empty) source.Id = Guid.NewGuid();
        _items.Add(source);
        return Task.CompletedTask;
    }
    public Task UpdateAsync(Source source, CancellationToken ct = default)
    {
        var idx = _items.FindIndex(x => x.Id == source.Id);
        if (idx >= 0) _items[idx] = source;
        return Task.CompletedTask;
    }
    public Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        _items.RemoveAll(x => x.Id == id);
        return Task.CompletedTask;
    }
}

