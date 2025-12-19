using Agentic_Rentify.Application.Features.Hotels.DTOs;
using Agentic_Rentify.Core.Entities;
using AutoMapper;

namespace Agentic_Rentify.Application.Mappings;

public class HotelProfile : Profile
{
    public HotelProfile()
    {
        CreateMap<Hotel, HotelResponseDTO>()
            .ForMember(dest => dest.BasePrice, opt => opt.MapFrom(src => $"{src.BasePrice} $"));

        CreateMap<HotelRoom, HotelRoomDTO>().ReverseMap();

        CreateMap<CreateHotelDTO, Hotel>();
        CreateMap<UpdateHotelDTO, Hotel>();
    }
}
