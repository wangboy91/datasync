using DataSync.Application.DTOs;
using DataSync.Domain.Entities;
using DataSync.Domain.Repositories;
using DataSync.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace DataSync.WebApi.Controllers;

[ApiController]
[Route("api/rules")]
[Tags("Rules")]
public class RulesController : ControllerBase
{
    private readonly IRuleRepository _repo;

    public RulesController(IRuleRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RuleDto>>> GetAll()
    {
        var list = await _repo.ListAsync();
        var dto = list.Select(x => new RuleDto(
            x.Id,
            x.Name,
            x.SourceId,
            x.TargetId,
            x.SourceTable,
            x.TargetTable,
            x.Mappings.Select(m => new FieldMappingDto(m.Source, m.Target, m.Transform, m.Default, m.Dedupe)).ToList(),
            x.DedupeBy.ToList(),
            x.MergeStrategies.Select(s => new MergeStrategyDto(s.TargetField, s.Strategy.ToString())).ToList(),
            x.Status
        ));
        return Ok(dto);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RuleDto>> Get(Guid id)
    {
        var x = await _repo.GetAsync(id);
        if (x == null) return NotFound();
        var dto = new RuleDto(
            x.Id,
            x.Name,
            x.SourceId,
            x.TargetId,
            x.SourceTable,
            x.TargetTable,
            x.Mappings.Select(m => new FieldMappingDto(m.Source, m.Target, m.Transform, m.Default, m.Dedupe)).ToList(),
            x.DedupeBy.ToList(),
            x.MergeStrategies.Select(s => new MergeStrategyDto(s.TargetField, s.Strategy.ToString())).ToList(),
            x.Status
        );
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<RuleDto>> Create(RuleDto dto)
    {
        var entity = new Rule
        {
            Id = dto.Id,
            Name = dto.Name,
            SourceId = dto.SourceId,
            TargetId = dto.TargetId,
            SourceTable = dto.SourceTable,
            TargetTable = dto.TargetTable,
            Mappings = dto.Mappings.Select(m => new FieldMapping { Source = m.Source, Target = m.Target, Transform = m.Transform, Default = m.Default, Dedupe = m.Dedupe }).ToList(),
            DedupeBy = dto.DedupeBy.ToList(),
            MergeStrategies = dto.MergeStrategies.Select(s => new MergeStrategy { TargetField = s.TargetField, Strategy = Enum.Parse<Strategy>(s.Strategy) }).ToList(),
            Status = dto.Status
        };
        await _repo.AddAsync(entity);
        return CreatedAtAction(nameof(Get), new { id = entity.Id }, dto);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<RuleDto>> Update(Guid id, RuleDto dto)
    {
        var entity = new Rule
        {
            Id = id,
            Name = dto.Name,
            SourceId = dto.SourceId,
            TargetId = dto.TargetId,
            SourceTable = dto.SourceTable,
            TargetTable = dto.TargetTable,
            Mappings = dto.Mappings.Select(m => new FieldMapping { Source = m.Source, Target = m.Target, Transform = m.Transform, Default = m.Default, Dedupe = m.Dedupe }).ToList(),
            DedupeBy = dto.DedupeBy.ToList(),
            MergeStrategies = dto.MergeStrategies.Select(s => new MergeStrategy { TargetField = s.TargetField, Strategy = Enum.Parse<Strategy>(s.Strategy) }).ToList(),
            Status = dto.Status
        };
        await _repo.UpdateAsync(entity);
        return Ok(dto);
    }

    [HttpPost("{id:guid}/disable")]
    public async Task<IActionResult> Disable(Guid id)
    {
        await _repo.DisableAsync(id);
        return Ok();
    }
}
