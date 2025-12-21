using System;
using Agentic_Rentify.Application.Features.Trips.DTOs;
using Agentic_Rentify.Application.Wrappers;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Application.Features.Trips.Specifications;
using AutoMapper;
using MediatR;

namespace Agentic_Rentify.Application.Features.Trips.Queries.GetAllTrips;

public class GetAllTripsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) 
    : IRequestHandler<GetAllTripsQuery, PagedResponse<TripResponseDTO>>
{
    public async Task<PagedResponse<TripResponseDTO>> Handle(GetAllTripsQuery request, CancellationToken cancellationToken)
    {
        var spec = new TripWithFiltersSpecification(request, applyPaging: true);
        var items = await unitOfWork.Repository<Trip>().ListAsync(spec);

        var countSpec = new TripWithFiltersSpecification(request, applyPaging: false);
        var totalCount = await unitOfWork.Repository<Trip>().CountAsync(countSpec);

        var dtos = mapper.Map<IReadOnlyList<TripResponseDTO>>(items);

        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Max(1, request.PageSize);

        return new PagedResponse<TripResponseDTO>(dtos, pageNumber, pageSize, totalCount);
    }
}
