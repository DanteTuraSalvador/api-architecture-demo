using DemoApi.Application.Interfaces;
using DemoApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DemoApi.Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class FleetsController : ControllerBase
{
    private readonly IFleetService _fleetService;

    public FleetsController(IFleetService fleetService)
    {
        _fleetService = fleetService;
    }

    /// <summary>
    /// Get all fleets
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Fleet>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Fleet>>> GetAll()
    {
        var fleets = await _fleetService.GetAllAsync();
        return Ok(fleets);
    }

    /// <summary>
    /// Get a fleet by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Fleet), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Fleet>> GetById(Guid id)
    {
        var fleet = await _fleetService.GetByIdAsync(id);
        if (fleet == null)
            return NotFound();
        return Ok(fleet);
    }

    /// <summary>
    /// Create a new fleet
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Fleet), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Fleet>> Create([FromBody] Fleet fleet)
    {
        var created = await _fleetService.CreateAsync(fleet);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Update a fleet
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Fleet), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Fleet>> Update(Guid id, [FromBody] Fleet fleet)
    {
        var updated = await _fleetService.UpdateAsync(id, fleet);
        if (updated == null)
            return NotFound();
        return Ok(updated);
    }

    /// <summary>
    /// Delete a fleet
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _fleetService.DeleteAsync(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Get fleet with its vehicles
    /// </summary>
    [HttpGet("{id:guid}/vehicles")]
    [ProducesResponseType(typeof(Fleet), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Fleet>> GetWithVehicles(Guid id)
    {
        var fleet = await _fleetService.GetWithVehiclesAsync(id);
        if (fleet == null)
            return NotFound();
        return Ok(fleet);
    }
}
