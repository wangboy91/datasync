using DataSync.Domain.Entities;
using DataSync.Infrastructure.Persistence;
using DataSync.Infrastructure.SchemaProviders;
using Microsoft.EntityFrameworkCore;

namespace DataSync.Infrastructure.Services;

public class SchemaService
{
    private readonly AppDbContext _context;
    private readonly IEnumerable<IDbSchemaProvider> _providers;

    public SchemaService(AppDbContext context, IEnumerable<IDbSchemaProvider> providers)
    {
        _context = context;
        _providers = providers;
    }

    public async Task SyncSourceSchemaAsync(Guid sourceId)
    {
        var source = await _context.Sources.FindAsync(sourceId);
        if (source == null) throw new Exception("Source not found");

        var provider = _providers.FirstOrDefault(p => p.CanHandle(source.Type));
        if (provider == null) throw new Exception($"No provider for source type {source.Type}");

        var tables = await provider.GetSchemaAsync(source.Connection);

        // Remove existing schema for this source
        var existing = await _context.SchemaTables.Where(t => t.SourceId == sourceId).ToListAsync();
        _context.SchemaTables.RemoveRange(existing);

        // Add new schema
        foreach (var table in tables)
        {
            table.SourceId = sourceId;
            _context.SchemaTables.Add(table);
        }

        await _context.SaveChangesAsync();
    }

    public async Task SyncTargetSchemaAsync(Guid targetId)
    {
        var target = await _context.Targets.FindAsync(targetId);
        if (target == null) throw new Exception("Target not found");

        var provider = _providers.FirstOrDefault(p => p.CanHandle(target.Type));
        if (provider == null) throw new Exception($"No provider for target type {target.Type}");

        var tables = await provider.GetSchemaAsync(target.Connection);

        // Remove existing schema for this target
        var existing = await _context.SchemaTables.Where(t => t.TargetId == targetId).ToListAsync();
        _context.SchemaTables.RemoveRange(existing);

        // Add new schema
        foreach (var table in tables)
        {
            table.TargetId = targetId;
            _context.SchemaTables.Add(table);
        }

        await _context.SaveChangesAsync();
    }

    public async Task SyncTableSchemaAsync(Guid? sourceId, Guid? targetId, string tableName)
    {
        string connectionString;
        string type;
        
        if (sourceId.HasValue)
        {
            var source = await _context.Sources.FindAsync(sourceId.Value);
            if (source == null) throw new Exception("Source not found");
            connectionString = source.Connection;
            type = source.Type;
        }
        else if (targetId.HasValue)
        {
            var target = await _context.Targets.FindAsync(targetId.Value);
            if (target == null) throw new Exception("Target not found");
            connectionString = target.Connection;
            type = target.Type;
        }
        else
        {
            throw new ArgumentException("Either sourceId or targetId must be provided");
        }

        var provider = _providers.FirstOrDefault(p => p.CanHandle(type));
        if (provider == null) throw new Exception($"No provider for database type {type}");

        var newTableSchema = await provider.GetTableSchemaAsync(connectionString, tableName);
        if (newTableSchema == null) throw new Exception($"Table {tableName} not found in database");

        // Find existing table in DB
        var existingTable = await _context.SchemaTables
            .Include(t => t.Columns)
            .FirstOrDefaultAsync(t => 
                (sourceId.HasValue && t.SourceId == sourceId && t.Name == tableName) ||
                (targetId.HasValue && t.TargetId == targetId && t.Name == tableName));

        if (existingTable != null)
        {
            // Update existing
            _context.SchemaColumns.RemoveRange(existingTable.Columns);
            existingTable.Columns = newTableSchema.Columns;
        }
        else
        {
            // Insert new
            if (sourceId.HasValue) newTableSchema.SourceId = sourceId;
            if (targetId.HasValue) newTableSchema.TargetId = targetId;
            _context.SchemaTables.Add(newTableSchema);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<SchemaTable>> GetTablesAsync(Guid? sourceId, Guid? targetId)
    {
        if (sourceId.HasValue)
        {
            return await _context.SchemaTables.Where(t => t.SourceId == sourceId).ToListAsync();
        }
        if (targetId.HasValue)
        {
            return await _context.SchemaTables.Where(t => t.TargetId == targetId).ToListAsync();
        }
        return new List<SchemaTable>();
    }

    public async Task<List<SchemaColumn>> GetColumnsAsync(Guid? sourceId, Guid? targetId, string tableName)
    {
        IQueryable<SchemaTable> query = _context.SchemaTables.Include(t => t.Columns);
        
        if (sourceId.HasValue)
        {
            query = query.Where(t => t.SourceId == sourceId);
        }
        else if (targetId.HasValue)
        {
            query = query.Where(t => t.TargetId == targetId);
        }
        else
        {
            return new List<SchemaColumn>();
        }

        var table = await query.FirstOrDefaultAsync(t => t.Name == tableName);
        return table?.Columns ?? new List<SchemaColumn>();
    }
}
