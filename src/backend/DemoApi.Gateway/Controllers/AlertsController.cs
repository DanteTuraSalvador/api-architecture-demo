using DemoApi.Application.Interfaces;
using DemoApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DemoApi.Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AlertsController : ControllerBase
{
    private readonly IAlertService _alertService;

    public AlertsController(IAlertService alertService)
    {
        _alertService = alertService;
    }

    /// <summary>
    /// Get all alerts
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Alert>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Alert>>> GetAll()
    {
        var alerts = await _alertService.GetAllAsync();
        return Ok(alerts);
    }

    /// <summary>
    /// Get an alert by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Alert), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Alert>> GetById(Guid id)
    {
        var alert = await _alertService.GetByIdAsync(id);
        if (alert == null)
            return NotFound();
        return Ok(alert);
    }

    /// <summary>
    /// Create a new alert
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Alert), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Alert>> Create([FromBody] Alert alert)
    {
        var created = await _alertService.CreateAsync(alert);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Delete an alert
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _alertService.DeleteAsync(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Get alerts by vehicle ID
    /// </summary>
    [HttpGet("vehicle/{vehicleId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<Alert>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Alert>>> GetByVehicle(Guid vehicleId)
    {
        var alerts = await _alertService.GetByVehicleIdAsync(vehicleId);
        return Ok(alerts);
    }

    /// <summary>
    /// Get unacknowledged alerts
    /// </summary>
    [HttpGet("unacknowledged")]
    [ProducesResponseType(typeof(IEnumerable<Alert>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Alert>>> GetUnacknowledged()
    {
        var alerts = await _alertService.GetUnacknowledgedAsync();
        return Ok(alerts);
    }

    /// <summary>
    /// Get recent alerts
    /// </summary>
    [HttpGet("recent")]
    [ProducesResponseType(typeof(IEnumerable<Alert>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Alert>>> GetRecent([FromQuery] int count = 10)
    {
        var alerts = await _alertService.GetRecentAlertsAsync(count);
        return Ok(alerts);
    }

    /// <summary>
    /// Acknowledge an alert
    /// </summary>
    [HttpPost("{id:guid}/acknowledge")]
    [ProducesResponseType(typeof(Alert), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Alert>> Acknowledge(Guid id)
    {
        var alert = await _alertService.AcknowledgeAsync(id);
        if (alert == null)
            return NotFound();
        return Ok(alert);
    }
}
