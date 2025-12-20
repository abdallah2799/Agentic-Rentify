using Agentic_Rentify.Infragentic.Services;
using Microsoft.AspNetCore.Mvc;

namespace Agentic_Rentify.Api.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController(DataSyncService dataSyncService) : ControllerBase
{
    [HttpGet("vector-sync")]
    public async Task<IActionResult> VectorSync()
    {
        await dataSyncService.SyncAsync("rentify_memory");
        return Ok(new { status = "ok" });
    }
}
