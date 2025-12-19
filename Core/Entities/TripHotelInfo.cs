namespace Agentic_Rentify.Core.Entities;

public class TripHotelInfo
{
    public string Name { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string Image { get; set; } = string.Empty;
    public List<string> Features { get; set; } = [];
}