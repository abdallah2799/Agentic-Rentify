using Agentic_Rentify.Application.Features.Attractions.DTOs;
using Agentic_Rentify.Application.Features.Trips.DTOs;
using Agentic_Rentify.Application.Features.Hotels.DTOs;
using Agentic_Rentify.Application.Features.Cars.DTOs;
using Agentic_Rentify.Core.Entities;
using AutoMapper;

namespace Agentic_Rentify.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Attraction, AttractionResponseDTO>()
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => $"{src.Price} {src.Currency}")) // Formatting price
            .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.ReviewSummary))
            .ForMember(dest => dest.UserReviews, opt => opt.MapFrom(src => src.UserReviews));

        CreateMap<AttractionReviewSummary, ReviewSummaryDTO>();
        CreateMap<RatingCriteria, RatingCriteriaDTO>();
        CreateMap<UserReview, UserReviewDTO>();

        // Trip Mappings
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

        // Hotel Mappings
        CreateMap<Hotel, HotelResponseDTO>()
            .ForMember(dest => dest.BasePrice, opt => opt.MapFrom(src => $"{src.BasePrice} $"));

        CreateMap<HotelRoom, HotelRoomDTO>().ReverseMap();

        CreateMap<CreateHotelDTO, Hotel>();
        CreateMap<UpdateHotelDTO, Hotel>();

        // Car Mappings
        CreateMap<Car, CarResponseDTO>()
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => $"{src.Price} $"));

        CreateMap<CreateCarDTO, Car>();
        CreateMap<UpdateCarDTO, Car>();
    }
}
