using System;
using System.Linq.Expressions;
using Agentic_Rentify.Application.Features.Trips.Queries.GetAllTrips;
using Agentic_Rentify.Application.Specifications;
using Agentic_Rentify.Core.Entities;

namespace Agentic_Rentify.Application.Features.Trips.Specifications;

public sealed class TripWithFiltersSpecification(GetAllTripsQuery request, bool applyPaging = true)
    : BaseSpecification<Trip>(
        BuildCriteria(request),
        orderBy: trip => trip.Title,
        skip: GetSkip(request, applyPaging),
        take: GetTake(request, applyPaging),
        isPagingEnabled: applyPaging)
{
    private static Expression<Func<Trip, bool>> BuildCriteria(GetAllTripsQuery request)
    {
        return trip =>
            (string.IsNullOrWhiteSpace(request.SearchTerm)
                || trip.Title.Contains(request.SearchTerm)
                || trip.Description.Contains(request.SearchTerm))
            && (!request.MinPrice.HasValue || trip.Price >= request.MinPrice.Value)
            && (!request.MaxPrice.HasValue || trip.Price <= request.MaxPrice.Value)
            && (string.IsNullOrWhiteSpace(request.City) || trip.City == request.City)
            && (!request.StartDate.HasValue || (trip.StartDate.HasValue && trip.StartDate.Value.Date >= request.StartDate.Value.Date))
            && !trip.IsDeleted;
    }

    private static int? GetSkip(GetAllTripsQuery request, bool applyPaging)
    {
        if (!applyPaging)
        {
            return null;
        }

        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Max(1, request.PageSize);
        return (pageNumber - 1) * pageSize;
    }

    private static int? GetTake(GetAllTripsQuery request, bool applyPaging)
    {
        if (!applyPaging)
        {
            return null;
        }

        return Math.Max(1, request.PageSize);
    }
}
