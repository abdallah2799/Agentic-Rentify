using Agentic_Rentify.Core.Common;

namespace Agentic_Rentify.Core.Entities;

public class Hotel : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public double Rating { get; set; }
    public int ReviewsCount { get; set; }
    public decimal BasePrice { get; set; }
    public string Description { get; set; } = string.Empty;

    // سيتخزن كـ JSON
    public List<string> Images { get; set; } = [];
    public List<string> Facilities { get; set; } = [];
    public List<HotelRoom> Rooms { get; set; } = [];

    public double Latitude { get; set; }
    public double Longitude { get; set; }
}