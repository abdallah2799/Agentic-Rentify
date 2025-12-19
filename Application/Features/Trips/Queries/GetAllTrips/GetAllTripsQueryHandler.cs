using System.Linq.Expressions;
using Agentic_Rentify.Application.Features.Trips.DTOs;
using Agentic_Rentify.Application.Wrappers;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using AutoMapper;
using MediatR;

namespace Agentic_Rentify.Application.Features.Trips.Queries.GetAllTrips;

public class GetAllTripsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) 
    : IRequestHandler<GetAllTripsQuery, PagedResponse<TripResponseDTO>>
{
    public async Task<PagedResponse<TripResponseDTO>> Handle(GetAllTripsQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Trip, bool>>? filter = null;
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            filter = t => t.Title.Contains(request.SearchTerm) || t.Description.Contains(request.SearchTerm);
        }

        var (items, totalCount) = await unitOfWork.Repository<Trip>()
            .GetPagedAppAsync(request.PageNumber, request.PageSize, filter);

        var dtos = mapper.Map<IReadOnlyList<TripResponseDTO>>(items);

        return new PagedResponse<TripResponseDTO>(dtos, request.PageNumber, request.PageSize, totalCount);
    }
}
