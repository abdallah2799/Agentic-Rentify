namespace Agentic_Rentify.Application.Interfaces;

/// <summary>
/// Service for identifying and cleaning up orphaned images in Cloudinary.
/// An orphaned image is one that exists in Cloudinary but is not referenced in the database.
/// </summary>
public interface IImageCleanupService
{
    /// <summary>
    /// Asynchronously scans the database for all referenced images and deletes orphaned images from Cloudinary.
    /// </summary>
    /// <returns>The number of orphaned images successfully deleted.</returns>
    /// <remarks>
    /// This operation:
    /// 1. Retrieves all PublicIds currently in use from database entities (Trips, Attractions, Hotels, Cars)
    /// 2. Lists all images currently in Cloudinary
    /// 3. Identifies PublicIds in Cloudinary but not in the database
    /// 4. Deletes the orphaned images
    /// 
    /// Should be run periodically (e.g., hourly) via Hangfire to maintain a clean cloud storage.
    /// </remarks>
    Task<int> CleanupOrphanedImagesAsync();
}
