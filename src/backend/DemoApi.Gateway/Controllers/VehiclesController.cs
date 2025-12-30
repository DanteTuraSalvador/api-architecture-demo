using DemoApi.Application.Interfaces;
using DemoApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DemoApi.Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehiclesController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    /// <summary>
    /// Get all vehicles
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Vehicle>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Vehicle>>> GetAll()
    {
        var vehicles = await _vehicleService.GetAllAsync();
        return Ok(vehicles);
    }

    /// <summary>
    /// Get a vehicle by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Vehicle), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Vehicle>> GetById(Guid id)
    {
        var vehicle = await _vehicleService.GetByIdAsync(id);
        if (vehicle == null)
            return NotFound();
        return Ok(vehicle);
    }

    /// <summary>
    /// Create a new vehicle
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Vehicle), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Vehicle>> Create([FromBody] Vehicle vehicle)
    {
        var created = await _vehicleService.CreateAsync(vehicle);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Update a vehicle
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Vehicle), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Vehicle>> Update(Guid id, [FromBody] Vehicle vehicle)
    {
        var updated = await _vehicleService.UpdateAsync(id, vehicle);
        if (updated == null)
            return NotFound();
        return Ok(updated);
    }

    /// <summary>
    /// Delete a vehicle
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _vehicleService.DeleteAsync(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Get vehicles by fleet ID
    /// </summary>
    [HttpGet("fleet/{fleetId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<Vehicle>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Vehicle>>> GetByFleet(Guid fleetId)
    {
        var vehicles = await _vehicleService.GetByFleetIdAsync(fleetId);
        return Ok(vehicles);
    }

    /// <summary>
    /// Get vehicles by status
    /// </summary>
    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(IEnumerable<Vehicle>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Vehicle>>> GetByStatus(string status)
    {
        var vehicles = await _vehicleService.GetByStatusAsync(status);
        return Ok(vehicles);
    }
}
