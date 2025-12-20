using Agentic_Rentify.Application.Features.Attractions.Queries.GetAllAttractions;
using Agentic_Rentify.Application.Features.Trips.DTOs;
using Agentic_Rentify.Application.Features.Trips.Queries.GetAllTrips;
using Agentic_Rentify.Application.Features.Trips.Queries.GetTripById;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace Agentic_Rentify.Infragentic.Plugins;

public class DiscoveryPlugin(IServiceScopeFactory serviceScopeFactory)
{
    [KernelFunction("get_all_attractions")]
    [Description("Retrieves a paginated list of tourist attractions. Use this to discover places to visit, museums, landmarks, and activities.")]
    public async Task<string> GetAllAttractionsAsync(
        [Description("Page number for pagination, defaults to 1")] int pageNumber = 1,
        [Description("Number of items per page, defaults to 10")] int pageSize = 10,
        [Description("Optional search term to filter attractions by name or description")] string? searchTerm = null)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        var query = new GetAllAttractionsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm
        };

        var result = await mediator.Send(query);
        return System.Text.Json.JsonSerializer.Serialize(result);
    }

    [KernelFunction("get_all_trips")]
    [Description("Retrieves a paginated list of available trips and tour packages. Use this to find multi-day tours, adventure packages, and organized experiences.")]
    public async Task<string> GetAllTripsAsync(
        [Description("Page number for pagination, defaults to 1")] int pageNumber = 1,
        [Description("Number of items per page, defaults to 10")] int pageSize = 10,
        [Description("Optional search term to filter trips by title or destination")] string? searchTerm = null)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        var query = new GetAllTripsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm
        };

        var result = await mediator.Send(query);
        return System.Text.Json.JsonSerializer.Serialize(result);
    }

    [KernelFunction("get_trip_details")]
    [Description("Gets detailed information about a specific trip including itinerary, pricing, hotel info, and available dates. Use this when the user asks about a particular trip.")]
    public async Task<string> GetTripDetailsAsync(
        [Description("The unique ID of the trip to retrieve")] int tripId)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        var query = new GetTripByIdQuery(tripId);
        var result = await mediator.Send(query);
        return System.Text.Json.JsonSerializer.Serialize(result);
    }
}
