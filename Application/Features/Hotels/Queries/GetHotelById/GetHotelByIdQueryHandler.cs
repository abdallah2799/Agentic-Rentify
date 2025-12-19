using Agentic_Rentify.Application.Features.Hotels.DTOs;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using AutoMapper;
using MediatR;

namespace Agentic_Rentify.Application.Features.Hotels.Queries.GetHotelById;

public class GetHotelByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) 
    : IRequestHandler<GetHotelByIdQuery, HotelResponseDTO>
{
    public async Task<HotelResponseDTO> Handle(GetHotelByIdQuery request, CancellationToken cancellationToken)
    {
        var hotel = await unitOfWork.Repository<Hotel>().GetByIdAsync(request.Id);
        if (hotel == null)
        {
            throw new Exception($"Hotel with ID {request.Id} not found.");
        }

        return mapper.Map<HotelResponseDTO>(hotel);
    }
}
