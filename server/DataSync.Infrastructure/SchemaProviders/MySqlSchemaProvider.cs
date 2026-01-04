using DataSync.Domain.Entities;
using MySqlConnector;

namespace DataSync.Infrastructure.SchemaProviders;

public class MySqlSchemaProvider : IDbSchemaProvider
{
    public bool CanHandle(string type) => type.ToLower() == "mysql";

    public async Task<List<SchemaTable>> GetSchemaAsync(string connectionString)
    {
        var tables = new List<SchemaTable>();
        
        await using var conn = new MySqlConnection(connectionString);
        await conn.OpenAsync();
        
        var dbName = conn.Database;

        // 1. Get Tables
        var validTables = new HashSet<string>();
        await using (var cmd = new MySqlCommand("SELECT TABLE_NAME FROM information_schema.TABLES WHERE TABLE_SCHEMA = @dbName AND TABLE_TYPE = 'BASE TABLE'", conn))
        {
            cmd.Parameters.AddWithValue("@dbName", dbName);
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var tableName = reader.GetString(0);
                    validTables.Add(tableName);
                    tables.Add(new SchemaTable { Name = tableName });
                }
            }
        }

        // 2. Get Columns
        await using (var cmd = new MySqlCommand("SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE, IS_NULLABLE FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = @dbName ORDER BY TABLE_NAME, ORDINAL_POSITION", conn))
        {
            cmd.Parameters.AddWithValue("@dbName", dbName);
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
        }

        return tables;
    }

    public async Task<SchemaTable?> GetTableSchemaAsync(string connectionString, string tableName)
    {
        await using var conn = new MySqlConnection(connectionString);
        await conn.OpenAsync();
        var dbName = conn.Database;

        var table = new SchemaTable { Name = tableName };

        // Get Columns
        await using var cmd = new MySqlCommand("SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = @dbName AND TABLE_NAME = @tableName ORDER BY ORDINAL_POSITION", conn);
        cmd.Parameters.AddWithValue("@dbName", dbName);
        cmd.Parameters.AddWithValue("@tableName", tableName);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!reader.HasRows) return null;

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
            await using var conn = new MySqlConnection(connectionString);
            await conn.OpenAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
