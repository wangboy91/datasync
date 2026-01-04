using Microsoft.EntityFrameworkCore;
using DataSync.Domain.Entities;
using DataSync.Domain.Repositories;
using DataSync.Infrastructure.Persistence;

namespace DataSync.Infrastructure.Repositories;

public class EfSourceRepository : ISourceRepository
{
    private readonly AppDbContext _db;

    public EfSourceRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Source>> ListAsync(CancellationToken ct = default)
    {
        return await _db.Sources.ToListAsync(ct);
    }

    public async Task<Source?> GetAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Sources.FindAsync(new object[] { id }, ct);
    }

    public async Task AddAsync(Source source, CancellationToken ct = default)
    {
        await _db.Sources.AddAsync(source, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Source source, CancellationToken ct = default)
    {
        _db.Sources.Update(source);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await GetAsync(id, ct);
        if (entity != null)
        {
            _db.Sources.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}
