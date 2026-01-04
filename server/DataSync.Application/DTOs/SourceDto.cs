namespace DataSync.Application.DTOs;

public record SourceDto(Guid Id, string Name, string Type, string Connection, string Status);

