using Agentic_Rentify.Infragentic.Services;
using Microsoft.AspNetCore.Mvc;

namespace Agentic_Rentify.Api.Controllers;

/// <summary>
/// Administrative operations and system management
/// </summary>
/// <remarks>
/// Provides endpoints for system maintenance and data synchronization.
/// Should be protected with admin-only authorization in production.
/// </remarks>
[ApiController]
[Route("api/admin")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "Admin")]
public class AdminController(DataSyncService dataSyncService) : ControllerBase
{
    /// <summary>
    /// Manually trigger vector database synchronization for all entities.
    /// </summary>
    /// <returns>Sync completion status</returns>
    /// <remarks>
    /// This endpoint forces a complete re-sync of all entities (Trips, Attractions, Hotels, Cars) to the Qdrant vector database.
    /// 
    /// **Use Cases:**
    /// - After database restore or migration
    /// - When vector search results seem outdated
    /// - Initial setup after deployment
    /// 
    /// **Note:** This operation is also performed automatically at application startup.
    /// The sync will embed all entity descriptions and update the search index.
    /// </remarks>
    /// <response code="200">Synchronization completed successfully</response>
    [HttpGet("vector-sync")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> VectorSync()
    {
        await dataSyncService.SyncAsync("rentify_memory");
        return Ok(new { status = "ok", message = "Vector database synchronization completed" });
    }
}
