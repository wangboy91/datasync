namespace DataSync.Domain.Entities;

public class SchemaColumn
{
    public Guid Id { get; set; }
    public Guid SchemaTableId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool Nullable { get; set; }
    // Navigation property if needed, but keeping it simple for now
    // public SchemaTable SchemaTable { get; set; }
}
