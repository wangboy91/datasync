namespace DataSync.Application.DTOs;

public record JobDto(Guid Id, Guid RuleId, string Trigger, string? StartTime, string? EndTime, string Status, int RecordCount, string? Message);

