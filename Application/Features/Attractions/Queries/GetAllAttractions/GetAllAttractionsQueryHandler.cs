using System.Linq.Expressions;
using Agentic_Rentify.Application.Features.Attractions.DTOs;
using Agentic_Rentify.Application.Wrappers;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using AutoMapper;
using MediatR;

namespace Agentic_Rentify.Application.Features.Attractions.Queries.GetAllAttractions;

public class GetAllAttractionsQueryHandler : IRequestHandler<GetAllAttractionsQuery, PagedResponse<AttractionResponseDTO>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllAttractionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResponse<AttractionResponseDTO>> Handle(GetAllAttractionsQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Attraction, bool>>? filter = null;
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            filter = a => a.Name.Contains(request.SearchTerm) || a.City.Contains(request.SearchTerm);
        }

        var (items, totalCount) = await _unitOfWork.Repository<Attraction>()
            .GetPagedAppAsync(request.PageNumber, request.PageSize, filter);

        var dtos = _mapper.Map<IReadOnlyList<AttractionResponseDTO>>(items);

        return new PagedResponse<AttractionResponseDTO>(dtos, request.PageNumber, request.PageSize, totalCount);
    }
}
