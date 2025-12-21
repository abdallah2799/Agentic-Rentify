using Agentic_Rentify.Core.Entities;

namespace Agentic_Rentify.Application.Features.Trips.DTOs;

public class TripResponseDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public string Price { get; set; } = string.Empty; // Formatted with currency
    public double Rating { get; set; }
    public int TotalReviews { get; set; }
    public string Duration { get; set; } = string.Empty;
    public string MainImage { get; set; } = string.Empty;
    public int MaxPeople { get; set; }
    public List<string> Images { get; set; } = new();
    public List<string> AvailableDates { get; set; } = new();
    public List<string> Highlights { get; set; } = new();
    public List<ItineraryDayDTO> Itinerary { get; set; } = new();
    public TripHotelInfoDTO? HotelInfo { get; set; }
}

public class ItineraryDayDTO
{
    public int Day { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<ActivityDTO> Activities { get; set; } = new();
}

public class ActivityDTO
{
    public string Time { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class TripHotelInfoDTO
{
    public string Name { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string Image { get; set; } = string.Empty;
    public List<string> Features { get; set; } = new();
}

public class CreateTripDTO
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public decimal Price { get; set; }
    public string Duration { get; set; } = string.Empty;
    public int MaxPeople { get; set; }
    public List<string> Images { get; set; } = new();
    public List<string> AvailableDates { get; set; } = new();
    public List<string> Highlights { get; set; } = new();
    public List<ItineraryDayDTO> Itinerary { get; set; } = new();
    public TripHotelInfoDTO? HotelInfo { get; set; }
}

public class UpdateTripDTO : CreateTripDTO
{
    public int Id { get; set; }
}
