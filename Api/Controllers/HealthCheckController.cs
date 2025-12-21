using Microsoft.AspNetCore.Mvc;

namespace Agentic_Rentify.Api.Controllers;

/// <summary>
/// Provides system health status diagnostics.
/// </summary>
/// <remarks>
/// Basic health check endpoint for monitoring and service availability verification.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "Admin")]
public class HealthCheckController : ControllerBase
{
    /// <summary>
    /// Checks the API availability and version.
    /// </summary>
    /// <returns>System status, version, and framework info.</returns>
    /// <response code="200">System is online and responding.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get() => Ok(new { Status = "Online", Version = "1.0.0", Framework = ".NET 10" });
}