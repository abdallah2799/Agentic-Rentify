using MediatR;

namespace Agentic_Rentify.Application.Features.Attractions.Commands.CreateAttraction;

public class CreateAttractionCommand : IRequest<int>
{
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = "$";
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public List<string> Images { get; set; } = new();
    public List<string> Categories { get; set; } = new();
    public List<string> Highlights { get; set; } = new();
    public List<string> Amenities { get; set; } = new();
}
