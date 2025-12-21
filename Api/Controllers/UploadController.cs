using Microsoft.AspNetCore.Mvc;

namespace Agentic_Rentify.Api.Controllers;

/// <summary>
/// Handles media uploads to cloud storage (Cloudinary).
/// </summary>
/// <remarks>
/// Provides endpoints for uploading images (logos, profile pictures, etc.) to Cloudinary CDN.
/// Supports common image formats with size and type validation.
/// </remarks>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "Admin")]
public class UploadController : ControllerBase
{
    private readonly CloudinaryService _cloudinaryService;

    public UploadController(CloudinaryService cloudinaryService)
    {
        _cloudinaryService = cloudinaryService;
    }

    /// <summary>
    /// Uploads an image file (e.g., logo, profile picture) to Cloudinary.
    /// </summary>
    /// <param name="file">The image file to upload.</param>
    /// <returns>JSON object containing the secure URL of the uploaded image.</returns>
    /// <response code="200">Image uploaded successfully.</response>
    /// <response code="400">File upload failed (invalid format or cloud error).</response>
    [HttpPost("logo")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadLogo(IFormFile file)
    {
        var result = await _cloudinaryService.UploadPhotoAsync(file);
        if (string.IsNullOrEmpty(result)) return BadRequest("Could not upload photo.");
        
        return Ok(new { Url = result });
    }
}
