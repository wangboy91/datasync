namespace DataSync.Application.DTOs;

public record TargetDto(Guid Id, string Name, string Type, string Connection, string Status);

