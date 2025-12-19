using Agentic_Rentify.Core.Entities;

namespace Agentic_Rentify.Application.Features.Hotels.DTOs;

public class HotelResponseDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public double Rating { get; set; }
    public int ReviewsCount { get; set; }
    public string BasePrice { get; set; } = string.Empty; // Formatted
    public string Description { get; set; } = string.Empty;
    public List<string> Images { get; set; } = new();
    public List<string> Facilities { get; set; } = new();
    public List<HotelRoomDTO> Rooms { get; set; } = new();
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class HotelRoomDTO
{
    public string Type { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Available { get; set; }
}

public class CreateHotelDTO
{
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public double Rating { get; set; }
    public decimal BasePrice { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<string> Images { get; set; } = new();
    public List<string> Facilities { get; set; } = new();
    public List<HotelRoomDTO> Rooms { get; set; } = new();
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class UpdateHotelDTO : CreateHotelDTO
{
    public int Id { get; set; }
}
