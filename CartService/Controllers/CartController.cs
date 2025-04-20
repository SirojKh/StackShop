using System;
using System.Threading.Tasks;
using CartService.Services;
using Microsoft.AspNetCore.Mvc;

namespace CartService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetCart(Guid userId)
    {
        var cart = await _cartService.GetCartAsync(userId);
        if (cart == null) return NotFound();
        return Ok(cart);
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddItem(AddCartItemRequest request)
    {
        await _cartService.AddItemAsync(request.UserId, request.ProductId, request.Quantity);
        return Ok();
    }

    [HttpPost("remove")]
    public async Task<IActionResult> RemoveItem(RemoveCartItemRequest request)
    {
        await _cartService.RemoveItemAsync(request.UserId, request.ProductId);
        return Ok();
    }

    [HttpPost("clear")]
    public async Task<IActionResult> ClearCart(ClearCartRequest request)
    {
        await _cartService.ClearCartAsync(request.UserId);
        return Ok();
    }
}

public class AddCartItemRequest
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

public class RemoveCartItemRequest
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
}

public class ClearCartRequest
{
    public Guid UserId { get; set; }
}