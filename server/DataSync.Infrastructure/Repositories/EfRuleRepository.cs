using Microsoft.EntityFrameworkCore;
using DataSync.Domain.Entities;
using DataSync.Domain.Repositories;
using DataSync.Infrastructure.Persistence;

namespace DataSync.Infrastructure.Repositories;

public class EfRuleRepository : IRuleRepository
{
    private readonly AppDbContext _db;

    public EfRuleRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Rule>> ListAsync(CancellationToken ct = default)
    {
        return await _db.Rules
            .Include(r => r.Mappings)
            .Include(r => r.MergeStrategies)
            .ToListAsync(ct);
    }

    public async Task<Rule?> GetAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Rules
            .Include(r => r.Mappings)
            .Include(r => r.MergeStrategies)
            .FirstOrDefaultAsync(r => r.Id == id, ct);
    }

    public async Task AddAsync(Rule rule, CancellationToken ct = default)
    {
        await _db.Rules.AddAsync(rule, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Rule rule, CancellationToken ct = default)
    {
        var existing = await GetAsync(rule.Id, ct);
        if (existing != null)
        {
            _db.Entry(existing).CurrentValues.SetValues(rule);
            
            existing.Mappings.Clear();
            foreach (var m in rule.Mappings) existing.Mappings.Add(m);

            existing.MergeStrategies.Clear();
            foreach (var s in rule.MergeStrategies) existing.MergeStrategies.Add(s);
            
            existing.DedupeBy = rule.DedupeBy; 

            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task DisableAsync(Guid id, CancellationToken ct = default)
    {
        var rule = await GetAsync(id, ct);
        if (rule != null)
        {
            rule.Status = "disabled";
            await _db.SaveChangesAsync(ct);
        }
    }
}
