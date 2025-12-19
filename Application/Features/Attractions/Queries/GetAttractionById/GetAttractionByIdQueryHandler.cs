using Agentic_Rentify.Application.Features.Attractions.DTOs;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using AutoMapper;
using MediatR;

namespace Agentic_Rentify.Application.Features.Attractions.Queries.GetAttractionById;

public class GetAttractionByIdQueryHandler : IRequestHandler<GetAttractionByIdQuery, AttractionResponseDTO>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAttractionByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<AttractionResponseDTO> Handle(GetAttractionByIdQuery request, CancellationToken cancellationToken)
    {
        var attraction = await _unitOfWork.Repository<Attraction>().GetByIdAsync(request.Id);
        if (attraction == null)
        {
            // Ideally define a NotFoundException
            throw new Exception($"Attraction with ID {request.Id} not found.");
        }
        return _mapper.Map<AttractionResponseDTO>(attraction);
    }
}
