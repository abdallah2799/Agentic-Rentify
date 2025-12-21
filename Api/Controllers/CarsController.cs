using Agentic_Rentify.Application.Features.Cars.Commands.CreateCar;
using Agentic_Rentify.Application.Features.Cars.Commands.UpdateCar;
using Agentic_Rentify.Application.Features.Cars.Commands.DeleteCar;
using Agentic_Rentify.Application.Features.Cars.Queries.GetAllCars;
using Agentic_Rentify.Application.Features.Cars.Queries.GetCarById;
using Agentic_Rentify.Application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Agentic_Rentify.Api.Controllers;

/// <summary>
/// Car rental catalog management
/// </summary>
/// <remarks>
/// Manages car rental listings with specifications, pricing, and availability.
/// All cars are automatically indexed for semantic search.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "Catalog")]
public class CarsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Get all cars with pagination and optional search.
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="searchTerm">Optional search query across brand, model, city</param>
    /// <returns>Paginated list of rental cars</returns>
    /// <response code="200">Returns paginated car list</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
    {
        var query = new GetAllCarsQuery { PageNumber = pageNumber, PageSize = pageSize, SearchTerm = searchTerm };
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get a car by ID with specifications.
    /// </summary>
    /// <param name="id">Car identifier</param>
    /// <returns>Car details including brand, model, capacity, and pricing</returns>
    /// <response code="200">Returns the car</response>
    /// <response code="404">Car not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetCarByIdQuery(id);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Add a new rental car to the catalog.
    /// </summary>
    /// <param name="command">Car details including brand, model, city, price, and specifications</param>
    /// <returns>Created car ID</returns>
    /// <remarks>
    /// Automatically indexed for semantic search after creation.
    /// </remarks>
    /// <response code="201">Car created successfully</response>
    /// <response code="400">Validation failed</response>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCarCommand command)
    {
        var result = await mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result }, result);
    }

    /// <summary>
    /// Update car details or pricing.
    /// </summary>
    /// <param name="id">Car ID to update</param>
    /// <param name="command">Updated car details</param>
    /// <returns>Updated car ID</returns>
    /// <response code="200">Car updated successfully</response>
    /// <response code="400">ID mismatch or validation error</response>
    /// <response code="404">Car not found</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCarCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID mismatch");
        }
        var result = await mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Delete a rental car (hard delete).
    /// </summary>
    /// <param name="id">Car ID to delete</param>
    /// <remarks>
    /// Removes car from vector search index.
    /// </remarks>
    /// <response code="200">Car deleted successfully</response>
    /// <response code="404">Car not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var command = new DeleteCarCommand(id);
        var result = await mediator.Send(command);
        return Ok(result);
    }
}
