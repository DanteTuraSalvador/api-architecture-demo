using DemoApi.Application.Interfaces;
using DemoApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DemoApi.Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DeliveriesController : ControllerBase
{
    private readonly IDeliveryService _deliveryService;

    public DeliveriesController(IDeliveryService deliveryService)
    {
        _deliveryService = deliveryService;
    }

    /// <summary>
    /// Get all deliveries
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Delivery>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Delivery>>> GetAll()
    {
        var deliveries = await _deliveryService.GetAllAsync();
        return Ok(deliveries);
    }

    /// <summary>
    /// Get a delivery by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Delivery), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Delivery>> GetById(Guid id)
    {
        var delivery = await _deliveryService.GetByIdAsync(id);
        if (delivery == null)
            return NotFound();
        return Ok(delivery);
    }

    /// <summary>
    /// Create a new delivery
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Delivery), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Delivery>> Create([FromBody] Delivery delivery)
    {
        var created = await _deliveryService.CreateAsync(delivery);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Update a delivery
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Delivery), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Delivery>> Update(Guid id, [FromBody] Delivery delivery)
    {
        var updated = await _deliveryService.UpdateAsync(id, delivery);
        if (updated == null)
            return NotFound();
        return Ok(updated);
    }

    /// <summary>
    /// Delete a delivery
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _deliveryService.DeleteAsync(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Get deliveries by trip ID
    /// </summary>
    [HttpGet("trip/{tripId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<Delivery>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Delivery>>> GetByTrip(Guid tripId)
    {
        var deliveries = await _deliveryService.GetByTripIdAsync(tripId);
        return Ok(deliveries);
    }

    /// <summary>
    /// Get delivery by tracking number
    /// </summary>
    [HttpGet("track/{trackingNumber}")]
    [ProducesResponseType(typeof(Delivery), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Delivery>> GetByTracking(string trackingNumber)
    {
        var delivery = await _deliveryService.GetByTrackingNumberAsync(trackingNumber);
        if (delivery == null)
            return NotFound();
        return Ok(delivery);
    }

    /// <summary>
    /// Mark delivery as picked up
    /// </summary>
    [HttpPost("{id:guid}/pickup")]
    [ProducesResponseType(typeof(Delivery), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Delivery>> MarkPickedUp(Guid id)
    {
        var delivery = await _deliveryService.MarkAsPickedUpAsync(id);
        if (delivery == null)
            return NotFound();
        return Ok(delivery);
    }

    /// <summary>
    /// Mark delivery as delivered
    /// </summary>
    [HttpPost("{id:guid}/deliver")]
    [ProducesResponseType(typeof(Delivery), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Delivery>> MarkDelivered(Guid id)
    {
        var delivery = await _deliveryService.MarkAsDeliveredAsync(id);
        if (delivery == null)
            return NotFound();
        return Ok(delivery);
    }
}
