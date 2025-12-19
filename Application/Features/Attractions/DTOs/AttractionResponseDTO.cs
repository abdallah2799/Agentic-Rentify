namespace Agentic_Rentify.Application.Features.Attractions.DTOs;

public class AttractionResponseDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public double Rating { get; set; }
    public List<string> Categories { get; set; } = new();
    public List<string> Images { get; set; } = new();
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public List<string> Highlights { get; set; } = new();
    public List<string> Amenities { get; set; } = new();
    public string Price { get; set; } = string.Empty; // Returning as string "120 $" to match JSON
    public ReviewSummaryDTO Reviews { get; set; } = new();
    public List<UserReviewDTO> UserReviews { get; set; } = new();
}

public class ReviewSummaryDTO
{
    public double OverallRating { get; set; }
    public int TotalReviews { get; set; }
    public RatingCriteriaDTO RatingCriteria { get; set; } = new();
}

public class RatingCriteriaDTO
{
    public double Experience { get; set; }
    public double Staff { get; set; }
    public double Accessibility { get; set; }
    public double Facilities { get; set; }
    public double ValueForMoney { get; set; }
}

public class UserReviewDTO
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserImage { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string Date { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
}
