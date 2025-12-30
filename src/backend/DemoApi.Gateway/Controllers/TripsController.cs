using DemoApi.Application.Interfaces;
using DemoApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DemoApi.Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TripsController : ControllerBase
{
    private readonly ITripService _tripService;

    public TripsController(ITripService tripService)
    {
        _tripService = tripService;
    }

    /// <summary>
    /// Get all trips
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Trip>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Trip>>> GetAll()
    {
        var trips = await _tripService.GetAllAsync();
        return Ok(trips);
    }

    /// <summary>
    /// Get a trip by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Trip), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Trip>> GetById(Guid id)
    {
        var trip = await _tripService.GetByIdAsync(id);
        if (trip == null)
            return NotFound();
        return Ok(trip);
    }

    /// <summary>
    /// Create a new trip
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Trip), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Trip>> Create([FromBody] Trip trip)
    {
        var created = await _tripService.CreateAsync(trip);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Update a trip
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Trip), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Trip>> Update(Guid id, [FromBody] Trip trip)
    {
        var updated = await _tripService.UpdateAsync(id, trip);
        if (updated == null)
            return NotFound();
        return Ok(updated);
    }

    /// <summary>
    /// Delete a trip
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _tripService.DeleteAsync(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Get trips by vehicle ID
    /// </summary>
    [HttpGet("vehicle/{vehicleId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<Trip>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Trip>>> GetByVehicle(Guid vehicleId)
    {
        var trips = await _tripService.GetByVehicleIdAsync(vehicleId);
        return Ok(trips);
    }

    /// <summary>
    /// Get trips by driver ID
    /// </summary>
    [HttpGet("driver/{driverId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<Trip>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Trip>>> GetByDriver(Guid driverId)
    {
        var trips = await _tripService.GetByDriverIdAsync(driverId);
        return Ok(trips);
    }

    /// <summary>
    /// Get active trips
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<Trip>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Trip>>> GetActive()
    {
        var trips = await _tripService.GetActiveTripsAsync();
        return Ok(trips);
    }

    /// <summary>
    /// Start a trip
    /// </summary>
    [HttpPost("{id:guid}/start")]
    [ProducesResponseType(typeof(Trip), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Trip>> Start(Guid id)
    {
        var trip = await _tripService.StartTripAsync(id);
        if (trip == null)
            return NotFound();
        return Ok(trip);
    }

    /// <summary>
    /// Complete a trip
    /// </summary>
    [HttpPost("{id:guid}/complete")]
    [ProducesResponseType(typeof(Trip), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Trip>> Complete(Guid id)
    {
        var trip = await _tripService.CompleteTripAsync(id);
        if (trip == null)
            return NotFound();
        return Ok(trip);
    }
}
