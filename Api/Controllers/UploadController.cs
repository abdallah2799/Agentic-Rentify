using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class UploadController : ControllerBase
{
    private readonly CloudinaryService _cloudinaryService;

    public UploadController(CloudinaryService cloudinaryService)
    {
        _cloudinaryService = cloudinaryService;
    }

    [HttpPost("logo")]
    public async Task<IActionResult> UploadLogo(IFormFile file)
    {
        var result = await _cloudinaryService.UploadPhotoAsync(file);
        if (string.IsNullOrEmpty(result)) return BadRequest("Could not upload photo.");
        
        return Ok(new { Url = result });
    }
}
