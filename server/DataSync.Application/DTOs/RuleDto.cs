using System.Collections.Generic;

namespace DataSync.Application.DTOs;

public record FieldMappingDto(string Source, string Target, string? Transform, string? Default, bool Dedupe);
public record MergeStrategyDto(string TargetField, string Strategy);
public record RuleDto(Guid Id, string Name, Guid? SourceId, Guid? TargetId, string SourceTable, string TargetTable, List<FieldMappingDto> Mappings, List<string> DedupeBy, List<MergeStrategyDto> MergeStrategies, string Status);
