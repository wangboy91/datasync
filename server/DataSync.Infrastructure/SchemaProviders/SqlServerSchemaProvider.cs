using DataSync.Domain.Entities;
using Microsoft.Data.SqlClient;

namespace DataSync.Infrastructure.SchemaProviders;

public class SqlServerSchemaProvider : IDbSchemaProvider
{
    public bool CanHandle(string type) => type.ToLower() == "sqlserver" || type.ToLower() == "mssql";

    public async Task<List<SchemaTable>> GetSchemaAsync(string connectionString)
    {
        var tables = new List<SchemaTable>();
        
        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();

        // 1. Get Tables
        var validTables = new HashSet<string>();
        await using (var cmd = new SqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'", conn))
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
        await using (var cmd = new SqlCommand("SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE, IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS ORDER BY TABLE_NAME, ORDINAL_POSITION", conn))
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
        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();

        var table = new SchemaTable { Name = tableName };

        // Get Columns
        await using var cmd = new SqlCommand("SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName ORDER BY ORDINAL_POSITION", conn);
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
            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
