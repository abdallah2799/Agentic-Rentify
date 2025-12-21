using Agentic_Rentify.Application.Wrappers;
using Microsoft.AspNetCore.Http;

namespace Agentic_Rentify.Application.Interfaces;

/// <summary>
/// Service for managing photo uploads and deletions in Cloudinary.
/// Implements the Photo service operations required by the application.
/// </summary>
public interface IPhotoService
{
    /// <summary>
    /// Asynchronously uploads a photo to Cloudinary.
    /// </summary>
    /// <param name="file">The image file to upload.</param>
    /// <returns>A PhotoResponseDTO containing the URL and PublicId of the uploaded image.</returns>
    /// <remarks>
    /// The returned PublicId must be stored in the database for later reference and deletion.
    /// Supported formats: JPEG, PNG, GIF, WebP. Maximum size: 100MB.
    /// </remarks>
    Task<PhotoResponseDTO> AddPhotoAsync(IFormFile file);

    /// <summary>
    /// Asynchronously deletes a photo from Cloudinary.
    /// </summary>
    /// <param name="publicId">The Cloudinary public ID of the image to delete.</param>
    /// <returns>True if deletion was successful; false otherwise.</returns>
    /// <remarks>
    /// Before deletion, ensure the image is no longer referenced in any entity (Trip, Hotel, Car, Attraction).
    /// This operation is irreversible.
    /// </remarks>
    Task<bool> DeletePhotoAsync(string publicId);
}
