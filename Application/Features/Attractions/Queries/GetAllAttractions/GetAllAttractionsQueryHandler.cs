using Agentic_Rentify.Application.Features.Attractions.DTOs;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Core.Interfaces;
using AutoMapper;
using MediatR;

namespace Agentic_Rentify.Application.Features.Attractions.Queries.GetAllAttractions;

public class GetAllAttractionsQueryHandler : IRequestHandler<GetAllAttractionsQuery, IReadOnlyList<AttractionResponseDTO>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllAttractionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<AttractionResponseDTO>> Handle(GetAllAttractionsQuery request, CancellationToken cancellationToken)
    {
        var attractions = await _unitOfWork.Repository<Attraction>().ListAllAsync();
        return _mapper.Map<IReadOnlyList<AttractionResponseDTO>>(attractions);
    }
}
