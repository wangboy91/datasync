using DataSync.Domain.Entities;
using Npgsql;

namespace DataSync.Infrastructure.SchemaProviders;

public class PostgresSchemaProvider : IDbSchemaProvider
{
    public bool CanHandle(string type) => type.ToLower() == "postgres" || type.ToLower() == "postgresql";

    public async Task<List<SchemaTable>> GetSchemaAsync(string connectionString)
    {
        var tables = new List<SchemaTable>();
        
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        // 1. Get Tables
        var validTables = new HashSet<string>();
        await using (var cmd = new NpgsqlCommand("SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' AND table_type = 'BASE TABLE'", conn))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var tableName = reader.GetString(0);
                validTables.Add(tableName);
                tables.Add(new SchemaTable { Name = tableName });
            }
        }

        // 2. Get Columns
        await using (var cmd = new NpgsqlCommand("SELECT table_name, column_name, data_type, is_nullable FROM information_schema.columns WHERE table_schema = 'public' ORDER BY table_name, ordinal_position", conn))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var tableName = reader.GetString(0);
                if (!validTables.Contains(tableName)) continue;

                var table = tables.First(t => t.Name == tableName);
                table.Columns.Add(new SchemaColumn
                {
                    Name = reader.GetString(1),
                    Type = reader.GetString(2),
                    Nullable = reader.GetString(3) == "YES"
                });
            }
        }

        return tables;
    }

    public async Task<SchemaTable?> GetTableSchemaAsync(string connectionString, string tableName)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        var table = new SchemaTable { Name = tableName };
        
        // Get Columns
        await using var cmd = new NpgsqlCommand("SELECT column_name, data_type, is_nullable FROM information_schema.columns WHERE table_schema = 'public' AND table_name = @tableName ORDER BY ordinal_position", conn);
        cmd.Parameters.AddWithValue("tableName", tableName);
        
        await using var reader = await cmd.ExecuteReaderAsync();
        if (!reader.HasRows) return null; // Table not found or no columns

        while (await reader.ReadAsync())
        {
            table.Columns.Add(new SchemaColumn
            {
                Name = reader.GetString(0),
                Type = reader.GetString(1),
                Nullable = reader.GetString(2) == "YES"
            });
        }
        return table;
    }

    public async Task<bool> TestConnectionAsync(string connectionString)
    {
        try
        {
            await using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
