namespace DataSync.Domain.ValueObjects;

public readonly struct TableId
{
    public string SourceOrTarget { get; }
    public string Database { get; }
    public string Schema { get; }
    public string Table { get; }

    public TableId(string sourceOrTarget, string database, string schema, string table)
    {
        SourceOrTarget = sourceOrTarget;
        Database = database;
        Schema = schema;
        Table = table;
    }

    public override string ToString() => $"{SourceOrTarget}:{Database}.{Schema}.{Table}";
}

