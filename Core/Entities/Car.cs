using Agentic_Rentify.Core.Common;

namespace Agentic_Rentify.Core.Entities;

public class Car : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Seats { get; set; }
    public string Transmission { get; set; } = "Auto";
    public decimal Price { get; set; }
    public string Overview { get; set; } = string.Empty;

    // سيتخزن كـ JSON
    public List<string> Features { get; set; } = [];
    public List<string> Images { get; set; } = [];
    public List<string> Amenities { get; set; } = [];
}