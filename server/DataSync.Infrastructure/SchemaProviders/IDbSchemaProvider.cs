using DataSync.Domain.Entities;

namespace DataSync.Infrastructure.SchemaProviders;

public interface IDbSchemaProvider
{
    Task<List<SchemaTable>> GetSchemaAsync(string connectionString);
    Task<SchemaTable?> GetTableSchemaAsync(string connectionString, string tableName);
    Task<bool> TestConnectionAsync(string connectionString);
    bool CanHandle(string type);
}
