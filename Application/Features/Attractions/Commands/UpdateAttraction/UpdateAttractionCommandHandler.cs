using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using AutoMapper;
using MediatR;

namespace Agentic_Rentify.Application.Features.Attractions.Commands.UpdateAttraction;

public class UpdateAttractionCommandHandler : IRequestHandler<UpdateAttractionCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateAttractionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<bool> Handle(UpdateAttractionCommand request, CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Attraction>();
        var attraction = await repository.GetByIdAsync(request.Id);

        if (attraction == null)
        {
            throw new Exception($"Attraction with ID {request.Id} not found.");
        }

        // Mapping properties
        attraction.Name = request.Name;
        attraction.City = request.City;
        attraction.Description = request.Description;
        attraction.Overview = request.Overview;
        attraction.Price = request.Price;
        attraction.Currency = request.Currency;
        attraction.Latitude = request.Latitude;
        attraction.Longitude = request.Longitude;
        
        // Updating Lists - assuming direct replacement
        attraction.Categories = request.Categories ?? new List<string>();
        attraction.Highlights = request.Highlights ?? new List<string>();
        attraction.Amenities = request.Amenities ?? new List<string>();
        
        // Images - careful if it's List<AttractionImage>
        // Attraction.Images became List<AttractionImage> in previous file view? 
        // No, I viewed Attraction.cs and it said: public List<AttractionImage> Images { get; set; } = [];
        // But seed data uses JSON array of {Url}.
        // So I must map List<string> (URLs) to List<AttractionImage>.
        
        if (request.Images != null)
        {
            attraction.Images = request.Images.Select(url => new AttractionImage { Url = url }).ToList();
        }

        await repository.UpdateAsync(attraction);
        await _unitOfWork.CompleteAsync();
        return true;
    }
}
