using System.Collections.Generic;

namespace DataSync.Application.DTOs;

public record SchemaColumnDto(string Name, string Type, bool Nullable);
public record SchemaTableDto(string Table, List<SchemaColumnDto> Columns);

