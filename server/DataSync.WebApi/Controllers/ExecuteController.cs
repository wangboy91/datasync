using DataSync.Domain.Entities;
using DataSync.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DataSync.WebApi.Controllers;

[ApiController]
[Route("api/execute")]
[Tags("Execute")]
public class ExecuteController : ControllerBase
{
    private readonly IJobRepository _jobs;

    public ExecuteController(IJobRepository jobs)
    {
        _jobs = jobs;
    }

    [HttpPost]
    public async Task<ActionResult> Execute(Guid ruleId)
    {
        var job = new Job
        {
            RuleId = ruleId,
            StartTime = DateTimeOffset.UtcNow,
            Status = "running",
            RecordCount = 0
        };
        await _jobs.AddAsync(job);
        return Ok(new { jobId = job.Id });
    }

    [HttpGet("{jobId:guid}/progress")]
    public async Task<ActionResult> GetProgress(Guid jobId)
    {
        var job = await _jobs.GetAsync(jobId);
        if (job == null) return NotFound();
        var percent = job.Status == "running" ? 42 : job.Status == "completed" ? 100 : 0;
        return Ok(new { jobId, percent, phase = "sync", message = "in progress" });
    }

    [HttpPost("{jobId:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid jobId)
    {
        var job = await _jobs.GetAsync(jobId);
        if (job == null) return NotFound();
        job.Status = "canceled";
        job.EndTime = DateTimeOffset.UtcNow;
        await _jobs.UpdateAsync(job);
        return Ok();
    }
}
