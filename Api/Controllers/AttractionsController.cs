using Agentic_Rentify.Application.Features.Attractions.Commands.CreateAttraction;
using Agentic_Rentify.Application.Features.Attractions.Queries.GetAllAttractions;
using Agentic_Rentify.Application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Agentic_Rentify.Api.Controllers;

/// <summary>
/// Tourist attractions catalog management
/// </summary>
/// <remarks>
/// Manages attraction listings with descriptions, pricing, and location details.
/// All attractions are automatically indexed for semantic search.
/// Supports soft delete (IsDeleted flag).
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "Catalog")]
public class AttractionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AttractionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all attractions with pagination and optional search/filters.
    /// </summary>
    /// <param name="query">Paging and filters: pageNumber, pageSize, searchTerm, city, category, minRating</param>
    /// <returns>Paginated list of attractions (excludes soft-deleted)</returns>
    /// <response code="200">Returns paginated attraction list</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllAttractionsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get an attraction by ID.
    /// </summary>
    /// <param name="id">Attraction identifier</param>
    /// <returns>Attraction details including pricing and location</returns>
    /// <response code="200">Returns the attraction</response>
    /// <response code="404">Attraction not found or soft-deleted</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new Agentic_Rentify.Application.Features.Attractions.Queries.GetAttractionById.GetAttractionByIdQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new attraction listing.
    /// </summary>
    /// <param name="command">Attraction details including name, city, price, and description</param>
    /// <returns>Created attraction ID</returns>
    /// <remarks>
    /// Automatically indexed for semantic search after creation.
    /// </remarks>
    /// <response code="201">Attraction created successfully</response>
    /// <response code="400">Validation failed</response>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateAttractionCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result }, result);
    }

    /// <summary>
    /// Update attraction details or pricing.
    /// </summary>
    /// <param name="id">Attraction ID to update</param>
    /// <param name="command">Updated attraction details</param>
    /// <returns>Updated attraction ID</returns>
    /// <response code="200">Attraction updated successfully</response>
    /// <response code="400">ID mismatch or validation error</response>
    /// <response code="404">Attraction not found</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] Agentic_Rentify.Application.Features.Attractions.Commands.UpdateAttraction.UpdateAttractionCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID mismatch");
        }
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Delete an attraction (soft delete - sets IsDeleted=true).
    /// </summary>
    /// <param name="id">Attraction ID to delete</param>
    /// <remarks>
    /// This is a soft delete operation. The attraction will be marked as deleted but not removed from the database.
    /// It will also be removed from the vector search index.
    /// </remarks>
    /// <response code="200">Attraction soft-deleted successfully</response>
    /// <response code="404">Attraction not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var command = new Agentic_Rentify.Application.Features.Attractions.Commands.DeleteAttraction.DeleteAttractionCommand(id);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
