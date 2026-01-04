using DataSync.Application.DTOs;
using DataSync.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace DataSync.WebApi.Controllers;

[ApiController]
[Route("api/schema")]
[Tags("Schema")]
public class SchemaController : ControllerBase
{
    private readonly SchemaService _schemaService;

    public SchemaController(SchemaService schemaService)
    {
        _schemaService = schemaService;
    }

    [HttpPost("sync")]
    public async Task<ActionResult> Sync(Guid? sourceId, Guid? targetId)
    {
        if (sourceId.HasValue)
        {
            await _schemaService.SyncSourceSchemaAsync(sourceId.Value);
        }
        else if (targetId.HasValue)
        {
            await _schemaService.SyncTargetSchemaAsync(targetId.Value);
        }
        else
        {
            return BadRequest("Either sourceId or targetId must be provided");
        }
        return Ok();
    }

    [HttpPost("sync-table")]
    public async Task<ActionResult> SyncTable(Guid? sourceId, Guid? targetId, string table)
    {
        if (string.IsNullOrEmpty(table)) return BadRequest("Table name is required");
        
        try 
        {
            await _schemaService.SyncTableSchemaAsync(sourceId, targetId, table);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("tables")]
    public async Task<ActionResult<IEnumerable<string>>> GetTables(Guid? sourceId, Guid? targetId)
    {
        var tables = await _schemaService.GetTablesAsync(sourceId, targetId);
        return Ok(tables.Select(t => t.Name));
    }

    [HttpGet("columns")]
    public async Task<ActionResult<SchemaTableDto>> GetColumns(Guid? sourceId, Guid? targetId, string table)
    {
        var columns = await _schemaService.GetColumnsAsync(sourceId, targetId, table);
        var dtos = columns.Select(c => new SchemaColumnDto(c.Name, c.Type, c.Nullable)).ToList();
        return Ok(new SchemaTableDto(table, dtos));
    }
}
