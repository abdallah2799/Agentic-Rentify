using Agentic_Rentify.Core.Common;
// Actually if in same namespace, no using needed for sibling types.
// But BaseEntity might need specific using if not in same namespace.
// Assuming BaseEntity is in Agentic_Rentify.Core.Entities or Common.
// Let's assume standard structure.

namespace Agentic_Rentify.Core.Entities;

public class Attraction : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public decimal Price { get; set; } 
    public string Currency { get; set; } = "$";

    // Coordinates
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    // Navigation Properties
    public List<AttractionImage> Images { get; set; } = [];
    public List<string> Categories { get; set; } = [];

    // Complex JSON Fields
    public AttractionReviewSummary ReviewSummary { get; set; } = new();
    public List<string> Amenities { get; set; } = [];
    public List<string> Highlights { get; set; } = [];
    public List<UserReview> UserReviews { get; set; } = [];
}