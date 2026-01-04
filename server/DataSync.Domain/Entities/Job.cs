namespace DataSync.Domain.Entities;

public class Job
{
    public Guid Id { get; set; }
    public Guid RuleId { get; set; }
    public string Trigger { get; set; } = "manual";
    public DateTimeOffset? StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }
    public string Status { get; set; } = "pending";
    public int RecordCount { get; set; }
    public string? Message { get; set; }
}

