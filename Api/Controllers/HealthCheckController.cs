using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Provides system health status diagnostics.
/// </summary>
[ApiController]
[Route("api/[controller]")]
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