using System;
using Agentic_Rentify.Application.Features.Attractions.DTOs;
using Agentic_Rentify.Application.Wrappers;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Application.Features.Attractions.Specifications;
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
        var spec = new AttractionWithFiltersSpecification(request, applyPaging: true);
        var items = await _unitOfWork.Repository<Attraction>().ListAsync(spec);

        var countSpec = new AttractionWithFiltersSpecification(request, applyPaging: false);
        var totalCount = await _unitOfWork.Repository<Attraction>().CountAsync(countSpec);

        var dtos = _mapper.Map<IReadOnlyList<AttractionResponseDTO>>(items);

        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Max(1, request.PageSize);

        return new PagedResponse<AttractionResponseDTO>(dtos, pageNumber, pageSize, totalCount);
    }
}
