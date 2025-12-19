namespace Agentic_Rentify.Core.Entities;

public class HotelRoom
{
    public string Type { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Available { get; set; }
}