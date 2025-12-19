using Agentic_Rentify.Application.Features.Attractions.Commands.CreateAttraction;
using Agentic_Rentify.Application.Features.Attractions.Queries.GetAllAttractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Agentic_Rentify.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttractionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AttractionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
    {
        var query = new GetAllAttractionsQuery { PageNumber = pageNumber, PageSize = pageSize, SearchTerm = searchTerm };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new Agentic_Rentify.Application.Features.Attractions.Queries.GetAttractionById.GetAttractionByIdQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAttractionCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Agentic_Rentify.Application.Features.Attractions.Commands.UpdateAttraction.UpdateAttractionCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID mismatch");
        }
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var command = new Agentic_Rentify.Application.Features.Attractions.Commands.DeleteAttraction.DeleteAttractionCommand(id);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
