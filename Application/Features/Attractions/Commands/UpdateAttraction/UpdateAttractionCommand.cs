using MediatR;

namespace Agentic_Rentify.Application.Features.Attractions.Commands.UpdateAttraction;

public class UpdateAttractionCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = "$";
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    
    // Note: handling separate image/category updates might be complex but for now we replace.
    public List<string> Images { get; set; } = new();
    public List<string> Categories { get; set; } = new();
    public List<string> Highlights { get; set; } = new();
    public List<string> Amenities { get; set; } = new();
}
