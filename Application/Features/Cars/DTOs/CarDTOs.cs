namespace Agentic_Rentify.Application.Features.Cars.DTOs;

public class CarResponseDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Seats { get; set; }
    public string Transmission { get; set; } = "Auto";
    public string Price { get; set; } = string.Empty; // Formatted
    public string Overview { get; set; } = string.Empty;
    public List<string> Features { get; set; } = new();
    public List<string> Images { get; set; } = new();
    public List<string> Amenities { get; set; } = new();
}

public class CreateCarDTO
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Seats { get; set; }
    public string Transmission { get; set; } = "Auto";
    public decimal Price { get; set; }
    public string Overview { get; set; } = string.Empty;
    public List<string> Features { get; set; } = new();
    public List<string> Images { get; set; } = new();
    public List<string> Amenities { get; set; } = new();
}

public class UpdateCarDTO : CreateCarDTO
{
    public int Id { get; set; }
}
