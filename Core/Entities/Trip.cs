using Agentic_Rentify.Core.Common;

namespace Agentic_Rentify.Core.Entities;

public class Trip : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public decimal Price { get; set; }
    public double Rating { get; set; }
    public int TotalReviews { get; set; }
    public string Duration { get; set; } = string.Empty; // "4 Days / 3 Nights"
    public string MainImage { get; set; } = string.Empty;
    public int MaxPeople { get; set; }

    // سيتخزن كـ JSON
    public List<string> Images { get; set; } = [];
    public List<string> AvailableDates { get; set; } = [];
    public List<string> Highlights { get; set; } = [];
    public List<ItineraryDay> Itinerary { get; set; } = []; // Nested Object

    // الـ Hotel المرتبط بالرحلة (ممكن يكون Null لو رحلة يوم واحد)
    public TripHotelInfo? HotelInfo { get; set; }
}