using DataSync.Domain.Entities;

namespace DataSync.Domain.Repositories;

public interface ISchemaRepository
{
    Task<List<string>> GetTablesAsync(Guid sourceId, CancellationToken ct = default);
    Task<SchemaTable?> GetColumnsAsync(Guid sourceId, string table, CancellationToken ct = default);
}

