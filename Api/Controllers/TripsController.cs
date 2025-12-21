using Agentic_Rentify.Application.Features.Trips.Commands.CreateTrip;
using Agentic_Rentify.Application.Features.Trips.Commands.DeleteTrip;
using Agentic_Rentify.Application.Features.Trips.Commands.UpdateTrip;
using Agentic_Rentify.Application.Features.Trips.Queries.GetAllTrips;
using Agentic_Rentify.Application.Features.Trips.Queries.GetTripById;
using Agentic_Rentify.Application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Agentic_Rentify.Api.Controllers;

/// <summary>
/// Trip catalog management with itinerary details, activities, and hotels
/// </summary>
/// <remarks>
/// Manages complete trip packages including daily itineraries, activities, and accommodation.
/// All trips are automatically indexed in vector database for semantic search.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "Catalog")]
public class TripsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Get all trips with pagination and optional search/filters.
    /// </summary>
    /// <param name="query">Paging and filters: pageNumber, pageSize, searchTerm, minPrice, maxPrice, city, startDate</param>
    /// <returns>Paginated list of trips with metadata</returns>
    /// <remarks>
    /// Returns trips with basic information. Use GET /api/Trips/{id} for full itinerary details.
    /// Search is performed across name and description.
    /// Additional filters: minPrice, maxPrice, city (exact match), and startDate (inclusive, date-only).
    /// </remarks>
    /// <response code="200">Returns paginated trip list</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllTripsQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get a trip by ID with complete itinerary, activities, and hotel information.
    /// </summary>
    /// <param name="id">Trip identifier</param>
    /// <returns>Complete trip details with daily itinerary breakdown</returns>
    /// <remarks>
    /// **Response includes:**
    /// - Trip metadata (name, price, destination, duration)
    /// - Daily itinerary with activities for each day
    /// - Hotel recommendations with pricing
    /// - Total cost breakdown
    /// 
    /// **Example Response Structure:**
    /// ```json
    /// {
    ///   "id": 1,
    ///   "name": "Ancient Egypt Explorer",
    ///   "destination": "Cairo, Egypt",
    ///   "price": 1200.00,
    ///   "duration": 7,
    ///   "itineraryDays": [
    ///     {
    ///       "dayNumber": 1,
    ///       "title": "Pyramids of Giza",
    ///       "activities": [
    ///         { "time": "09:00", "description": "Guided tour of Great Pyramid" }
    ///       ]
    ///     }
    ///   ],
    ///   "hotels": [
    ///     { "name": "Nile Ritz-Carlton", "pricePerNight": 180.00 }
    ///   ]
    /// }
    /// ```
    /// </remarks>
    /// <response code="200">Returns the trip with full itinerary</response>
    /// <response code="404">Trip not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetTripByIdQuery(id);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new trip with itinerary and hotel options.
    /// </summary>
    /// <param name="command">Trip creation details including name, destination, price, duration, itinerary, and hotels</param>
    /// <returns>Created trip ID</returns>
    /// <remarks>
    /// **Required Fields:**
    /// - Name, Destination, Price, Duration
    /// - At least one ItineraryDay with activities
    /// 
    /// **Example Request:**
    /// ```json
    /// {
    ///   "name": "Romantic Paris Getaway",
    ///   "destination": "Paris, France",
    ///   "price": 2500.00,
    ///   "duration": 5,
    ///   "overview": "Experience the city of love...",
    ///   "itineraryDays": [
    ///     {
    ///       "dayNumber": 1,
    ///       "title": "Eiffel Tower and Seine River",
    ///       "activities": [
    ///         { "time": "10:00", "description": "Visit Eiffel Tower summit" },
    ///         { "time": "14:00", "description": "Seine River cruise" }
    ///       ]
    ///     }
    ///   ],
    ///   "hotels": [
    ///     { "name": "Hotel Plaza Athenee", "pricePerNight": 450.00 }
    ///   ]
    /// }
    /// ```
    /// 
    /// After creation, the trip is automatically indexed for semantic search.
    /// </remarks>
    /// <response code="201">Trip created successfully</response>
    /// <response code="400">Validation failed</response>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTripCommand command)
    {
        var result = await mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result }, result);
    }

    /// <summary>
    /// Update an existing trip's details, itinerary, or hotel options.
    /// </summary>
    /// <param name="id">Trip ID to update</param>
    /// <param name="command">Updated trip details</param>
    /// <returns>Updated trip ID</returns>
    /// <remarks>
    /// Updates the vector database index automatically after modification.
    /// Send complete trip data - partial updates are not supported.
    /// </remarks>
    /// <response code="200">Trip updated successfully</response>
    /// <response code="400">ID mismatch or validation error</response>
    /// <response code="404">Trip not found</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTripCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID mismatch");
        }
        var result = await mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Delete a trip (hard delete - removes all related itinerary and hotel data).
    /// </summary>
    /// <param name="id">Trip ID to delete</param>
    /// <remarks>
    /// **Warning:** This is a hard delete operation.
    /// All related itinerary days, activities, and hotel references will be permanently removed.
    /// The trip will also be removed from the vector search index.
    /// </remarks>
    /// <response code="200">Trip deleted successfully</response>
    /// <response code="404">Trip not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var command = new DeleteTripCommand(id);
        var result = await mediator.Send(command);
        return Ok(result);
    }
}
