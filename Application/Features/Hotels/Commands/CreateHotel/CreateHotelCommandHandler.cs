using Agentic_Rentify.Application.Features.Hotels.DTOs;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Interfaces;
using AutoMapper;
using MediatR;

namespace Agentic_Rentify.Application.Features.Hotels.Commands.CreateHotel;

public class CreateHotelCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) 
    : IRequestHandler<CreateHotelCommand, int>
{
    public async Task<int> Handle(CreateHotelCommand request, CancellationToken cancellationToken)
    {
        var hotel = mapper.Map<Hotel>(request);
        await unitOfWork.Repository<Hotel>().AddAsync(hotel);
        await unitOfWork.CompleteAsync();

        return hotel.Id;
    }
}
