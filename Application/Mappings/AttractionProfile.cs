using Agentic_Rentify.Application.Features.Attractions.DTOs;
using Agentic_Rentify.Core.Entities;
using AutoMapper;

namespace Agentic_Rentify.Application.Mappings;

public class AttractionProfile : Profile
{
    public AttractionProfile()
    {
        CreateMap<Attraction, AttractionResponseDTO>()
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => $"{src.Price} {src.Currency}"))
            .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.ReviewSummary))
            .ForMember(dest => dest.UserReviews, opt => opt.MapFrom(src => src.UserReviews));

        CreateMap<AttractionReviewSummary, ReviewSummaryDTO>();
        CreateMap<RatingCriteria, RatingCriteriaDTO>();
        CreateMap<UserReview, UserReviewDTO>();
    }
}
