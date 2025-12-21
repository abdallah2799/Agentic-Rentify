namespace Agentic_Rentify.Application.Wrappers;

/// <summary>
/// Response DTO for photo upload operations.
/// Contains the uploaded image URL and its Cloudinary public ID for future reference.
/// </summary>
public class PhotoResponseDTO
{
    /// <summary>
    /// The secure HTTPS URL of the uploaded image from Cloudinary CDN.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// The Cloudinary public ID for the uploaded image.
    /// Used to reference, transform, or delete the image later.
    /// </summary>
    public string PublicId { get; set; } = string.Empty;
}
