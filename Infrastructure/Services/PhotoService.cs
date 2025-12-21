using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Application.Wrappers;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Agentic_Rentify.Infrastructure.Services;

/// <summary>
/// Production-ready implementation of IPhotoService using Cloudinary.
/// Handles secure image uploads with transformations and deletions.
/// </summary>
public class PhotoService(IConfiguration configuration) : IPhotoService
{
    private readonly Cloudinary _cloudinary = InitializeCloudinary(configuration);

    /// <summary>
    /// Initializes the Cloudinary client with credentials from configuration.
    /// </summary>
    /// <param name="configuration">The application configuration containing Cloudinary credentials.</param>
    /// <returns>A configured Cloudinary instance.</returns>
    private static Cloudinary InitializeCloudinary(IConfiguration configuration)
    {
        var cloudName = configuration["CloudinarySettings:CloudName"]
            ?? throw new InvalidOperationException("CloudinarySettings:CloudName is not configured.");
        var apiKey = configuration["CloudinarySettings:ApiKey"]
            ?? throw new InvalidOperationException("CloudinarySettings:ApiKey is not configured.");
        var apiSecret = configuration["CloudinarySettings:ApiSecret"]
            ?? throw new InvalidOperationException("CloudinarySettings:ApiSecret is not configured.");

        var account = new Account(cloudName, apiKey, apiSecret);
        return new Cloudinary(account);
    }

    /// <summary>
    /// Uploads an image file to Cloudinary with optimized transformations.
    /// </summary>
    /// <param name="file">The image file to upload.</param>
    /// <returns>PhotoResponseDTO containing the secure URL and PublicId.</returns>
    /// <exception cref="ArgumentException">Thrown if the file is invalid or empty.</exception>
    public async Task<PhotoResponseDTO> AddPhotoAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File cannot be null or empty.", nameof(file));
        }

        using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Transformation = new Transformation()
                .Height(500)
                .Width(500)
                .Crop("fill")
                .Gravity("face")
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
        {
            throw new InvalidOperationException(
                $"Cloudinary upload failed: {uploadResult.Error.Message}");
        }

        return new PhotoResponseDTO
        {
            Url = uploadResult.SecureUrl.ToString(),
            PublicId = uploadResult.PublicId
        };
    }

    /// <summary>
    /// Deletes a photo from Cloudinary using its PublicId.
    /// </summary>
    /// <param name="publicId">The Cloudinary public ID of the image to delete.</param>
    /// <returns>True if deletion was successful; false otherwise.</returns>
    public async Task<bool> DeletePhotoAsync(string publicId)
    {
        if (string.IsNullOrWhiteSpace(publicId))
        {
            return false;
        }

        try
        {
            var deleteParams = new DeletionParams(publicId);
            var deleteResult = await _cloudinary.DestroyAsync(deleteParams);

            return deleteResult.Result == "ok";
        }
        catch
        {
            return false;
        }
    }
}
