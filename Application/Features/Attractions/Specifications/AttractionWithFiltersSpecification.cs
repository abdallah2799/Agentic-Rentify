using System;
using System.Linq.Expressions;
using Agentic_Rentify.Application.Features.Attractions.Queries.GetAllAttractions;
using Agentic_Rentify.Application.Specifications;
using Agentic_Rentify.Core.Entities;

namespace Agentic_Rentify.Application.Features.Attractions.Specifications;

public sealed class AttractionWithFiltersSpecification(GetAllAttractionsQuery request, bool applyPaging = true)
    : BaseSpecification<Attraction>(
        BuildCriteria(request),
        orderBy: attraction => attraction.Name,
        skip: GetSkip(request, applyPaging),
        take: GetTake(request, applyPaging),
        isPagingEnabled: applyPaging)
{
    private static Expression<Func<Attraction, bool>> BuildCriteria(GetAllAttractionsQuery request)
    {
        return attraction =>
            (string.IsNullOrWhiteSpace(request.SearchTerm)
                || attraction.Name.Contains(request.SearchTerm)
                || attraction.Description.Contains(request.SearchTerm)
                || attraction.City.Contains(request.SearchTerm))
            && (string.IsNullOrWhiteSpace(request.City) || attraction.City == request.City)
            && (string.IsNullOrWhiteSpace(request.Category) || attraction.Categories.Contains(request.Category))
            && (!request.MinRating.HasValue || attraction.Rating >= request.MinRating.Value)
            && !attraction.IsDeleted;
    }

    private static int? GetSkip(GetAllAttractionsQuery request, bool applyPaging)
    {
        if (!applyPaging)
        {
            return null;
        }

        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Max(1, request.PageSize);
        return (pageNumber - 1) * pageSize;
    }

    private static int? GetTake(GetAllAttractionsQuery request, bool applyPaging)
    {
        if (!applyPaging)
        {
            return null;
        }

        return Math.Max(1, request.PageSize);
    }
}
