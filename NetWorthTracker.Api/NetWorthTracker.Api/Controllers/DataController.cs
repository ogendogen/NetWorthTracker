using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetWorthTracker.Api.Models;

namespace NetWorthTracker.Api.Controllers;

[ApiController]
[Authorize]
public sealed class DataController : ControllerBase
{
    [HttpGet("/data")]
    [ProducesResponseType<NetWorthSummary>(StatusCodes.Status200OK)]
    public ActionResult<NetWorthSummary> Get()
    {
        return Ok(new NetWorthSummary(
            NetWorth: 125_500m,
            Assets: 168_750m,
            Liabilities: 43_250m,
            UpdatedAt: DateTimeOffset.UtcNow));
    }
}