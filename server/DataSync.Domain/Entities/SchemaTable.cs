using System.Collections.Generic;

namespace DataSync.Domain.Entities;

public class SchemaTable
{
    public Guid Id { get; set; }
    public Guid? SourceId { get; set; }
    public Guid? TargetId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<SchemaColumn> Columns { get; set; } = new();
}
