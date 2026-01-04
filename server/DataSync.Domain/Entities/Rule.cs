using System.Collections.Generic;
using DataSync.Domain.ValueObjects;

namespace DataSync.Domain.Entities;

public class FieldMapping
{
    public string Source { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public string? Transform { get; set; }
    public string? Default { get; set; }
    public bool Dedupe { get; set; }
}

public class MergeStrategy
{
    public string TargetField { get; set; } = string.Empty;
    public Strategy Strategy { get; set; } = Strategy.Update;
}

public class Rule
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? SourceId { get; set; }
    public Guid? TargetId { get; set; }
    public string SourceTable { get; set; } = string.Empty;
    public string TargetTable { get; set; } = string.Empty;
    public List<FieldMapping> Mappings { get; set; } = new();
    public List<string> DedupeBy { get; set; } = new();
    public List<MergeStrategy> MergeStrategies { get; set; } = new();
    public string Status { get; set; } = "inactive";
}
