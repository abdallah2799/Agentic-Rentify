using Agentic_Rentify.Application.Features.Hotels.Commands.CreateHotel;
using Agentic_Rentify.Application.Features.Hotels.Commands.UpdateHotel;
using Agentic_Rentify.Application.Features.Hotels.Commands.DeleteHotel;
using Agentic_Rentify.Application.Features.Hotels.Queries.GetAllHotels;
using Agentic_Rentify.Application.Features.Hotels.Queries.GetHotelById;
using Agentic_Rentify.Application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Agentic_Rentify.Api.Controllers;

/// <summary>
/// Hotel catalog management with rooms, amenities, and pricing
/// </summary>
/// <remarks>
/// Manages hotel listings with room types, pricing, and availability.
/// All hotels are automatically indexed for semantic search.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "Catalog")]
public class HotelsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Get all hotels with pagination and optional search.
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="searchTerm">Optional search query across name, description, city</param>
    /// <returns>Paginated list of hotels</returns>
    /// <response code="200">Returns paginated hotel list</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
    {
        var query = new GetAllHotelsQuery { PageNumber = pageNumber, PageSize = pageSize, SearchTerm = searchTerm };
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get a hotel by ID with room details.
    /// </summary>
    /// <param name="id">Hotel identifier</param>
    /// <returns>Hotel details including available room types and pricing</returns>
    /// <response code="200">Returns the hotel with rooms</response>
    /// <response code="404">Hotel not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetHotelByIdQuery(id);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new hotel listing.
    /// </summary>
    /// <param name="command">Hotel details including name, city, price, amenities, and rooms</param>
    /// <returns>Created hotel ID</returns>
    /// <remarks>
    /// Automatically indexed for semantic search after creation.
    /// </remarks>
    /// <response code="201">Hotel created successfully</response>
    /// <response code="400">Validation failed</response>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateHotelCommand command)
    {
        var result = await mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result }, result);
    }

    /// <summary>
    /// Update hotel details or room configurations.
    /// </summary>
    /// <param name="id">Hotel ID to update</param>
    /// <param name="command">Updated hotel details</param>
    /// <returns>Updated hotel ID</returns>
    /// <response code="200">Hotel updated successfully</response>
    /// <response code="400">ID mismatch or validation error</response>
    /// <response code="404">Hotel not found</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateHotelCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID mismatch");
        }
        var result = await mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Delete a hotel (hard delete - removes all room data).
    /// </summary>
    /// <param name="id">Hotel ID to delete</param>
    /// <remarks>
    /// Removes hotel from vector search index and deletes all associated rooms.
    /// </remarks>
    /// <response code="200">Hotel deleted successfully</response>
    /// <response code="404">Hotel not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var command = new DeleteHotelCommand { Id = id };
        var result = await mediator.Send(command);
        return Ok(result);
    }
}
