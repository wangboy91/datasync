using DataSync.Application.DTOs;
using DataSync.Domain.Entities;
using DataSync.Domain.Repositories;
using DataSync.Infrastructure.SchemaProviders;
using Microsoft.AspNetCore.Mvc;

namespace DataSync.WebApi.Controllers;

[ApiController]
[Route("api/sources")]
[Tags("Sources")]
public class SourcesController : ControllerBase
{
    private readonly ISourceRepository _repo;
    private readonly IEnumerable<IDbSchemaProvider> _providers;

    public SourcesController(ISourceRepository repo, IEnumerable<IDbSchemaProvider> providers)
    {
        _repo = repo;
        _providers = providers;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SourceDto>>> GetAll()
    {
        var list = await _repo.ListAsync();
        return Ok(list.Select(x => new SourceDto(x.Id, x.Name, x.Type, x.Connection, x.Status)));
    }

    [HttpPost]
    public async Task<ActionResult<SourceDto>> Create(SourceDto dto)
    {
        var entity = new Source { Id = dto.Id, Name = dto.Name, Type = dto.Type, Connection = dto.Connection, Status = dto.Status };
        await _repo.AddAsync(entity);
        return CreatedAtAction(nameof(GetAll), new { id = entity.Id }, new SourceDto(entity.Id, entity.Name, entity.Type, entity.Connection, entity.Status));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SourceDto>> Update(Guid id, SourceDto dto)
    {
        var entity = new Source { Id = id, Name = dto.Name, Type = dto.Type, Connection = dto.Connection, Status = dto.Status };
        await _repo.UpdateAsync(entity);
        return Ok(new SourceDto(entity.Id, entity.Name, entity.Type, entity.Connection, entity.Status));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _repo.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("test")]
    public async Task<ActionResult> Test(SourceDto dto)
    {
        var provider = _providers.FirstOrDefault(p => p.CanHandle(dto.Type));
        if (provider == null)
        {
            return BadRequest(new { ok = false, error = "Unknown source type" });
        }

        var ok = await provider.TestConnectionAsync(dto.Connection);
        return Ok(new { ok });
    }
}
