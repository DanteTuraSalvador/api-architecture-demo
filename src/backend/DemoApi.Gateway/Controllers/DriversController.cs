using DemoApi.Application.Interfaces;
using DemoApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DemoApi.Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DriversController : ControllerBase
{
    private readonly IDriverService _driverService;

    public DriversController(IDriverService driverService)
    {
        _driverService = driverService;
    }

    /// <summary>
    /// Get all drivers
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Driver>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Driver>>> GetAll()
    {
        var drivers = await _driverService.GetAllAsync();
        return Ok(drivers);
    }

    /// <summary>
    /// Get a driver by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Driver), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Driver>> GetById(Guid id)
    {
        var driver = await _driverService.GetByIdAsync(id);
        if (driver == null)
            return NotFound();
        return Ok(driver);
    }

    /// <summary>
    /// Create a new driver
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Driver), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Driver>> Create([FromBody] Driver driver)
    {
        var created = await _driverService.CreateAsync(driver);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Update a driver
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Driver), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Driver>> Update(Guid id, [FromBody] Driver driver)
    {
        var updated = await _driverService.UpdateAsync(id, driver);
        if (updated == null)
            return NotFound();
        return Ok(updated);
    }

    /// <summary>
    /// Delete a driver
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _driverService.DeleteAsync(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Get drivers by status
    /// </summary>
    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(IEnumerable<Driver>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Driver>>> GetByStatus(string status)
    {
        var drivers = await _driverService.GetByStatusAsync(status);
        return Ok(drivers);
    }

    /// <summary>
    /// Get available drivers
    /// </summary>
    [HttpGet("available")]
    [ProducesResponseType(typeof(IEnumerable<Driver>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Driver>>> GetAvailable()
    {
        var drivers = await _driverService.GetAvailableDriversAsync();
        return Ok(drivers);
    }
}
