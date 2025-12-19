using Agentic_Rentify.Application.Features.Trips.DTOs;
using Agentic_Rentify.Core.Entities;
using AutoMapper;

namespace Agentic_Rentify.Application.Mappings;

public class TripProfile : Profile
{
    public TripProfile()
    {
        CreateMap<Trip, TripResponseDTO>()
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => $"{src.Price} $"));

        CreateMap<ItineraryDay, ItineraryDayDTO>().ReverseMap();
        CreateMap<Activity, ActivityDTO>()
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Desc))
            .ReverseMap()
            .ForMember(dest => dest.Desc, opt => opt.MapFrom(src => src.Description));
        CreateMap<TripHotelInfo, TripHotelInfoDTO>().ReverseMap();

        CreateMap<CreateTripDTO, Trip>();
        CreateMap<UpdateTripDTO, Trip>();
    }
}
