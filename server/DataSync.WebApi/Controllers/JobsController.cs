using DataSync.Application.DTOs;
using DataSync.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DataSync.WebApi.Controllers;

[ApiController]
[Route("api/jobs")]
[Tags("Jobs")]
public class JobsController : ControllerBase
{
    private readonly IJobRepository _jobs;

    public JobsController(IJobRepository jobs)
    {
        _jobs = jobs;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobDto>>> GetAll()
    {
        var list = await _jobs.ListAsync();
        var dto = list.Select(x => new JobDto(x.Id, x.RuleId, x.Trigger, x.StartTime?.ToString("O"), x.EndTime?.ToString("O"), x.Status, x.RecordCount, x.Message));
        return Ok(dto);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<JobDto>> Get(Guid id)
    {
        var x = await _jobs.GetAsync(id);
        if (x == null) return NotFound();
        return Ok(new JobDto(x.Id, x.RuleId, x.Trigger, x.StartTime?.ToString("O"), x.EndTime?.ToString("O"), x.Status, x.RecordCount, x.Message));
    }
}
