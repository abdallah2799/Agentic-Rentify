using Microsoft.AspNetCore.Mvc;
using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Application.Wrappers;
using MediatR;
using Agentic_Rentify.Application.Features.Photos.Commands.DeletePhoto;

namespace Agentic_Rentify.Api.Controllers;

/// <summary>
/// Handles media uploads and deletions to cloud storage (Cloudinary).
/// </summary>
/// <remarks>
/// <para>
/// Provides endpoints for uploading images to Cloudinary CDN with automatic transformations.
/// Supports common image formats (JPEG, PNG, GIF, WebP) with automatic optimization.
/// </para>
/// 
/// <para>
/// <strong>IMPORTANT - Frontend Integration:</strong>
/// 1. First, upload the image using POST api/Upload/photo endpoint
/// 2. The response contains <c>url</c> (display URL) and <c>publicId</c> (for deletion reference)
/// 3. When creating/updating entities (Trip, Hotel, Car, Attraction), send the <c>url</c> and <c>publicId</c> in the JSON payload
/// 4. Store both values in the database to enable future updates or deletions
/// 5. If deleting an image, use POST api/Upload/delete endpoint with the stored publicId
/// </para>
/// 
/// <para>
/// <strong>Image Management Best Practices:</strong>
/// - Always store the publicId returned from upload for later reference
/// - Use the publicId to delete images, not the URL
/// - Orphaned images (not referenced in database) are automatically cleaned up hourly
/// - Maximum file size: 100MB per upload
/// </para>
/// </remarks>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "Media/Upload Operations")]
public class UploadController(IPhotoService photoService, IMediator mediator) : ControllerBase
{
    private readonly IPhotoService _photoService = photoService;
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Uploads an image file to Cloudinary.
    /// </summary>
    /// <param name="file">The image file to upload (JPEG, PNG, GIF, WebP).</param>
    /// <returns>PhotoResponseDTO containing the secure URL and PublicId of the uploaded image.</returns>
    /// <response code="200">Image uploaded successfully.</response>
    /// <response code="400">File upload failed (invalid format, size exceeded, or cloud error).</response>
    /// <response code="500">Server error during upload.</response>
    /// <example>
    /// <code>
    /// POST /api/Upload/photo
    /// Content-Type: multipart/form-data
    /// 
    /// Response (200 OK):
    /// {
    ///   "url": "https://res.cloudinary.com/your-cloud/image/upload/v1234567890/folder/filename.jpg",
    ///   "publicId": "folder/filename"
    /// }
    /// </code>
    /// </example>
    [HttpPost("photo")]
    [ProducesResponseType(typeof(PhotoResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadPhoto(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { error = "No file provided or file is empty." });
            }

            var result = await _photoService.AddPhotoAsync(file);
            return Ok(result);
        }
        catch (ArgumentException)
        {
            return BadRequest(new { error = "Invalid file format or size." });
        }
        catch (InvalidOperationException)
        {
            return BadRequest(new { error = "Failed to upload image to Cloudinary." });
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { error = "An unexpected error occurred during upload." });
        }
    }

    /// <summary>
    /// Deletes a photo from Cloudinary using its PublicId.
    /// </summary>
    /// <param name="request">The request containing the PublicId of the image to delete.</param>
    /// <returns>Boolean indicating successful deletion.</returns>
    /// <response code="200">Image deleted successfully.</response>
    /// <response code="400">Invalid PublicId or deletion failed.</response>
    /// <response code="500">Server error during deletion.</response>
    /// <remarks>
    /// <strong>IMPORTANT:</strong> Before deleting an image, ensure it is no longer referenced in any entity.
    /// If the image is still in use, the entity data will have broken image URLs.
    /// Orphaned images (not in database) are automatically cleaned up hourly.
    /// </remarks>
    /// <example>
    /// <code>
    /// POST /api/Upload/delete
    /// Content-Type: application/json
    /// 
    /// Request Body:
    /// {
    ///   "publicId": "folder/filename"
    /// }
    /// 
    /// Response (200 OK):
    /// {
    ///   "success": true
    /// }
    /// </code>
    /// </example>
    [HttpPost("delete")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeletePhoto([FromBody] DeletePhotoRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.PublicId))
            {
                return BadRequest(new { error = "PublicId is required." });
            }

            var command = new DeletePhotoCommand { PublicId = request.PublicId };
            var result = await _mediator.Send(command);

            if (!result)
            {
                return BadRequest(new { error = "Failed to delete image from Cloudinary." });
            }

            return Ok(new { success = true, message = "Image deleted successfully." });
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "An unexpected error occurred during deletion." });
        }
    }

    /// <summary>
    /// Legacy endpoint for uploading logo images (deprecated, use /photo instead).
    /// </summary>
    /// <param name="file">The image file to upload.</param>
    /// <returns>Object containing the URL of the uploaded image.</returns>
    /// <response code="200">Image uploaded successfully.</response>
    /// <response code="400">File upload failed.</response>
    [HttpPost("logo")]
    [Obsolete("Use POST /api/Upload/photo instead. This endpoint will be removed in a future version.")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadLogo(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Could not upload photo.");
            }

            var result = await _photoService.AddPhotoAsync(file);
            return Ok(new { Url = result.Url });
        }
        catch
        {
            return BadRequest("Could not upload photo.");
        }
    }
}

/// <summary>
/// Request model for photo deletion.
/// </summary>
public class DeletePhotoRequest
{
    /// <summary>
    /// The Cloudinary public ID of the image to delete.
    /// </summary>
    public string PublicId { get; set; } = string.Empty;
}
