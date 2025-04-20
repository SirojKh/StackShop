using System;
using System.Threading.Tasks;
using InventoryService.Interfaces;
using InventoryService.Models;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet("{productId}")]
    public async Task<IActionResult> Get(Guid productId)
    {
        var inventory = await _inventoryService.GetInventoryAsync(productId);
        if (inventory == null)
            return NotFound();
        return Ok(inventory);
    }

    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] UpdateInventoryRequest request)
    {
        var success = await _inventoryService.UpdateInventoryAsync(request);
        if (!success)
            return BadRequest();
        return Ok();
    }
}
