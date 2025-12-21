using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Core.Entities;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Agentic_Rentify.Infrastructure.Services;

/// <summary>
/// Service for identifying and deleting orphaned images from Cloudinary.
/// An orphaned image is one that exists in Cloudinary but is not referenced in the database.
/// </summary>
public class ImageCleanupService(
    IConfiguration configuration,
    IUnitOfWork unitOfWork,
    ILogger<ImageCleanupService> logger) : IImageCleanupService
{
    private readonly Cloudinary _cloudinary = InitializeCloudinary(configuration);
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<ImageCleanupService> _logger = logger;

    /// <summary>
    /// Initializes the Cloudinary client with credentials from configuration.
    /// </summary>
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
    /// Scans the database for all referenced images and deletes orphaned images from Cloudinary.
    /// </summary>
    /// <returns>The number of orphaned images successfully deleted.</returns>
    public async Task<int> CleanupOrphanedImagesAsync()
    {
        try
        {
            _logger.LogInformation("Starting image cleanup process...");

            // Step 1: Retrieve all PublicIds currently in use in the database
            var usedPublicIds = await GetAllUsedPublicIdsAsync();
            _logger.LogInformation("Found {Count} images in use in the database.", usedPublicIds.Count);

            // Step 2: List all images in Cloudinary
            var cloudinaryPublicIds = await GetAllCloudinaryPublicIdsAsync();
            _logger.LogInformation("Found {Count} images in Cloudinary.", cloudinaryPublicIds.Count);

            // Step 3: Identify orphaned images (in Cloudinary but not in database)
            var orphanedPublicIds = cloudinaryPublicIds
                .Except(usedPublicIds)
                .ToList();
            _logger.LogInformation("Identified {Count} orphaned images for deletion.", orphanedPublicIds.Count);

            // Step 4: Delete orphaned images
            int deletedCount = 0;
            foreach (var publicId in orphanedPublicIds)
            {
                if (await DeleteFromCloudinaryAsync(publicId))
                {
                    deletedCount++;
                    _logger.LogInformation("Deleted orphaned image: {PublicId}", publicId);
                }
            }

            _logger.LogInformation("Cleanup completed. Successfully deleted {Count} orphaned images.", deletedCount);
            return deletedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during image cleanup process.");
            return 0;
        }
    }

    /// <summary>
    /// Retrieves all PublicIds that are currently in use in the database across all entities.
    /// </summary>
    private async Task<HashSet<string>> GetAllUsedPublicIdsAsync()
    {
        var usedPublicIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        try
        {
            // Get all Trips with images
            var trips = await _unitOfWork.Repository<Trip>().ListAllAsync();
            foreach (var trip in trips)
            {
                if (!string.IsNullOrEmpty(trip.MainImage))
                {
                    var publicId = ExtractPublicIdFromUrl(trip.MainImage);
                    if (!string.IsNullOrEmpty(publicId))
                        usedPublicIds.Add(publicId);
                }

                foreach (var imageUrl in trip.Images)
                {
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        var publicId = ExtractPublicIdFromUrl(imageUrl);
                        if (!string.IsNullOrEmpty(publicId))
                            usedPublicIds.Add(publicId);
                    }
                }
            }

            // Get all Attractions with images
            var attractions = await _unitOfWork.Repository<Attraction>().ListAllAsync();
            foreach (var attraction in attractions)
            {
                foreach (var image in attraction.Images)
                {
                    if (!string.IsNullOrEmpty(image.Url))
                    {
                        var publicId = ExtractPublicIdFromUrl(image.Url);
                        if (!string.IsNullOrEmpty(publicId))
                            usedPublicIds.Add(publicId);
                    }
                }
            }

            // Get all Hotels with images
            var hotels = await _unitOfWork.Repository<Hotel>().ListAllAsync();
            foreach (var hotel in hotels)
            {
                foreach (var imageUrl in hotel.Images)
                {
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        var publicId = ExtractPublicIdFromUrl(imageUrl);
                        if (!string.IsNullOrEmpty(publicId))
                            usedPublicIds.Add(publicId);
                    }
                }
            }

            // Get all Cars with images
            var cars = await _unitOfWork.Repository<Car>().ListAllAsync();
            foreach (var car in cars)
            {
                foreach (var imageUrl in car.Images)
                {
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        var publicId = ExtractPublicIdFromUrl(imageUrl);
                        if (!string.IsNullOrEmpty(publicId))
                            usedPublicIds.Add(publicId);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving used PublicIds from database.");
        }

        return usedPublicIds;
    }

    /// <summary>
    /// Lists all PublicIds currently stored in Cloudinary.
    /// </summary>
    private async Task<HashSet<string>> GetAllCloudinaryPublicIdsAsync()
    {
        var publicIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        try
        {
            var listParams = new ListResourcesParams { Type = "upload", MaxResults = 500 };
            var listResult = await _cloudinary.ListResourcesAsync(listParams);

            if (listResult.Resources != null)
            {
                foreach (var resource in listResult.Resources)
                {
                    if (!string.IsNullOrEmpty(resource.PublicId))
                    {
                        publicIds.Add(resource.PublicId);
                    }
                }

                // Handle pagination if there are more results
                while (!string.IsNullOrEmpty(listResult.NextCursor))
                {
                    listParams.NextCursor = listResult.NextCursor;
                    listResult = await _cloudinary.ListResourcesAsync(listParams);

                    if (listResult.Resources != null)
                    {
                        foreach (var resource in listResult.Resources)
                        {
                            if (!string.IsNullOrEmpty(resource.PublicId))
                            {
                                publicIds.Add(resource.PublicId);
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing resources from Cloudinary.");
        }

        return publicIds;
    }

    /// <summary>
    /// Extracts the PublicId from a Cloudinary URL.
    /// Example: https://res.cloudinary.com/cloud/image/upload/v1234567890/folder/filename.jpg -> folder/filename
    /// </summary>
    private static string ExtractPublicIdFromUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
            return string.Empty;

        try
        {
            // Pattern: /upload/v[timestamp]/{publicId}.{extension}
            // Extract the part after upload/ and before the file extension
            var match = Regex.Match(url, @"/upload/(?:v\d+/)?(.+?)(?:\.\w+)?$");
            if (match.Success && match.Groups.Count > 1)
            {
                var publicId = match.Groups[1].Value;
                // Remove version string if present
                publicId = Regex.Replace(publicId, @"^v\d+/", "");
                // Remove file extension if present
                publicId = Regex.Replace(publicId, @"\.\w+$", "");
                return publicId;
            }

            // Fallback: assume the last part of the path is the public ID
            return Path.GetFileNameWithoutExtension(url);
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Deletes an image from Cloudinary.
    /// </summary>
    private async Task<bool> DeleteFromCloudinaryAsync(string publicId)
    {
        try
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result.Result == "ok";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image from Cloudinary: {PublicId}", publicId);
            return false;
        }
    }
}
