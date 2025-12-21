using System;
using Agentic_Rentify.Application.Features.Attractions.Queries.GetAllAttractions;
using Agentic_Rentify.Application.Features.Cars.Queries.GetAllCars;
using Agentic_Rentify.Application.Features.Cars.Queries.GetCarById;
using Agentic_Rentify.Application.Features.Hotels.Queries.GetAllHotels;
using Agentic_Rentify.Application.Features.Hotels.Queries.GetHotelById;
using Agentic_Rentify.Application.Features.Trips.DTOs;
using Agentic_Rentify.Application.Features.Trips.Queries.GetAllTrips;
using Agentic_Rentify.Application.Features.Trips.Queries.GetTripById;
using Agentic_Rentify.Application.Interfaces;
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
        [Description("Optional search term to filter attractions by name or description")] string? searchTerm = null,
        [Description("Optional city to filter attractions by exact city match")] string? city = null,
        [Description("Optional category to filter attractions by category")] string? category = null,
        [Description("Optional minimum rating to filter attractions (inclusive)")] double? minRating = null)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        var query = new GetAllAttractionsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            City = city,
            Category = category,
            MinRating = minRating
        };

        var result = await mediator.Send(query);
        return System.Text.Json.JsonSerializer.Serialize(result);
    }

    [KernelFunction("get_all_trips")]
    [Description("Retrieves a paginated list of available trips and tour packages. Use this to find multi-day tours, adventure packages, and organized experiences.")]
    public async Task<string> GetAllTripsAsync(
        [Description("Page number for pagination, defaults to 1")] int pageNumber = 1,
        [Description("Number of items per page, defaults to 10")] int pageSize = 10,
        [Description("Optional search term to filter trips by title or destination")] string? searchTerm = null,
        [Description("Optional minimum price filter (inclusive)")] decimal? minPrice = null,
        [Description("Optional maximum price filter (inclusive)")] decimal? maxPrice = null,
        [Description("Optional city to filter trips by exact city match")] string? city = null,
        [Description("Optional start date filter; returns trips with startDate on or after this date")] DateTime? startDate = null)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        
        var query = new GetAllTripsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            City = city,
            StartDate = startDate
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

    [KernelFunction("semantic_search")]
    [Description("Performs semantic search over attractions and trips using vector embeddings.")]
    public async Task<string> SemanticSearchAsync(
        [Description("A natural-language description of what you are looking for, e.g., 'romantic quiet places'.")] string description,
        [Description("Maximum number of results to return, defaults to 5")] int topK = 5)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var vectorService = scope.ServiceProvider.GetRequiredService<IVectorDbService>();

        const string collection = "rentify_memory";
        await vectorService.CreateCollectionIfNotExists(collection);
        var results = await vectorService.SearchByTextAsync(collection, description, topK);
        return System.Text.Json.JsonSerializer.Serialize(results);
    }

    [KernelFunction("get_all_cars")]
    [Description("Retrieves a paginated list of available cars for rent. Use this to find cars by name or overview.")]
    public async Task<string> GetAllCarsAsync(
        [Description("Page number for pagination, defaults to 1")] int pageNumber = 1,
        [Description("Number of items per page, defaults to 10")] int pageSize = 10,
        [Description("Optional search term to filter cars by name")] string? searchTerm = null)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var query = new GetAllCarsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm
        };

        var result = await mediator.Send(query);
        return System.Text.Json.JsonSerializer.Serialize(result);
    }

    [KernelFunction("get_car_details")]
    [Description("Gets detailed information about a specific car including features, images, and pricing.")]
    public async Task<string> GetCarDetailsAsync(
        [Description("The unique ID of the car to retrieve")] int carId)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var query = new GetCarByIdQuery(carId);
        var result = await mediator.Send(query);
        return System.Text.Json.JsonSerializer.Serialize(result);
    }

    [KernelFunction("get_all_hotels")]
    [Description("Retrieves a paginated list of hotels. Use this to find stays filtered by name or city.")]
    public async Task<string> GetAllHotelsAsync(
        [Description("Page number for pagination, defaults to 1")] int pageNumber = 1,
        [Description("Number of items per page, defaults to 10")] int pageSize = 10,
        [Description("Optional search term to filter hotels by name or city")] string? searchTerm = null)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var query = new GetAllHotelsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm
        };

        var result = await mediator.Send(query);
        return System.Text.Json.JsonSerializer.Serialize(result);
    }

    [KernelFunction("get_hotel_details")]
    [Description("Gets detailed information about a specific hotel including pricing and location info.")]
    public async Task<string> GetHotelDetailsAsync(
        [Description("The unique ID of the hotel to retrieve")] int hotelId)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var query = new GetHotelByIdQuery(hotelId);
        var result = await mediator.Send(query);
        return System.Text.Json.JsonSerializer.Serialize(result);
    }
}
