using Microsoft.EntityFrameworkCore;
using DataSync.Domain.Entities;
using DataSync.Domain.Repositories;
using DataSync.Infrastructure.Persistence;

namespace DataSync.Infrastructure.Repositories;

public class EfJobRepository : IJobRepository
{
    private readonly AppDbContext _db;

    public EfJobRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Job>> ListAsync(CancellationToken ct = default)
    {
        return await _db.Jobs.OrderByDescending(j => j.StartTime).ToListAsync(ct);
    }

    public async Task<Job?> GetAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Jobs.FindAsync(new object[] { id }, ct);
    }

    public async Task AddAsync(Job job, CancellationToken ct = default)
    {
        await _db.Jobs.AddAsync(job, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Job job, CancellationToken ct = default)
    {
        _db.Jobs.Update(job);
        await _db.SaveChangesAsync(ct);
    }
}
