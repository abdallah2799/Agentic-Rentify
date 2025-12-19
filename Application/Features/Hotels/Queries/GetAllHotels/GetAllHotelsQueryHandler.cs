using System.Linq.Expressions;
using Agentic_Rentify.Application.Features.Hotels.DTOs;
using Agentic_Rentify.Application.Wrappers;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using AutoMapper;
using MediatR;

namespace Agentic_Rentify.Application.Features.Hotels.Queries.GetAllHotels;

public class GetAllHotelsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) 
    : IRequestHandler<GetAllHotelsQuery, PagedResponse<HotelResponseDTO>>
{
    public async Task<PagedResponse<HotelResponseDTO>> Handle(GetAllHotelsQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Hotel, bool>>? filter = null;
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            filter = h => h.Name.Contains(request.SearchTerm) || h.City.Contains(request.SearchTerm);
        }

        var (items, totalCount) = await unitOfWork.Repository<Hotel>()
            .GetPagedAppAsync(request.PageNumber, request.PageSize, filter);

        var dtos = mapper.Map<IReadOnlyList<HotelResponseDTO>>(items);

        return new PagedResponse<HotelResponseDTO>(dtos, request.PageNumber, request.PageSize, totalCount);
    }
}
