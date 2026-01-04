using Microsoft.EntityFrameworkCore;
using DataSync.Domain.Entities;
using DataSync.Domain.Repositories;
using DataSync.Infrastructure.Persistence;

namespace DataSync.Infrastructure.Repositories;

public class EfTargetRepository : ITargetRepository
{
    private readonly AppDbContext _db;

    public EfTargetRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Target>> ListAsync(CancellationToken ct = default)
    {
        return await _db.Targets.ToListAsync(ct);
    }

    public async Task<Target?> GetAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Targets.FindAsync(new object[] { id }, ct);
    }

    public async Task AddAsync(Target target, CancellationToken ct = default)
    {
        await _db.Targets.AddAsync(target, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Target target, CancellationToken ct = default)
    {
        _db.Targets.Update(target);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await GetAsync(id, ct);
        if (entity != null)
        {
            _db.Targets.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}
