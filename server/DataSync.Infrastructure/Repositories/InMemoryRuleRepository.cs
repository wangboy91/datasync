using DataSync.Domain.Entities;
using DataSync.Domain.Repositories;

namespace DataSync.Infrastructure.Repositories;

public class InMemoryRuleRepository : IRuleRepository
{
    private readonly List<Rule> _items = new();

    public Task<List<Rule>> ListAsync(CancellationToken ct = default) => Task.FromResult(_items.ToList());
    public Task<Rule?> GetAsync(Guid id, CancellationToken ct = default) => Task.FromResult(_items.FirstOrDefault(x => x.Id == id));
    public Task AddAsync(Rule rule, CancellationToken ct = default)
    {
        if (rule.Id == Guid.Empty) rule.Id = Guid.NewGuid();
        _items.Add(rule);
        return Task.CompletedTask;
    }
    public Task UpdateAsync(Rule rule, CancellationToken ct = default)
    {
        var idx = _items.FindIndex(x => x.Id == rule.Id);
        if (idx >= 0) _items[idx] = rule;
        return Task.CompletedTask;
    }
    public Task DisableAsync(Guid id, CancellationToken ct = default)
    {
        var item = _items.FirstOrDefault(x => x.Id == id);
        if (item != null) item.Status = "disabled";
        return Task.CompletedTask;
    }
}

