using System;
using System.Threading.Tasks;
using CartService.Interfaces;
using CartService.Services;
using Microsoft.AspNetCore.Mvc;

namespace CartService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly IAuditLogger _auditLogger;
    private readonly IOrderClient _orderClient;

    public CartController(ICartService cartService, IAuditLogger auditLogger, IOrderClient orderClient)
    {
        _cartService = cartService;
        _auditLogger = auditLogger;
        _orderClient = orderClient;
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

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout(CheckoutRequest request)
    {
        var cart = await _cartService.GetCartAsync(request.UserId);
        if (cart == null || cart.Items.Count == 0)
            return BadRequest("Kundvagnen är tom.");

        // Logga audit event
        await _auditLogger.LogAsync("CartCheckedOut", request.UserId.ToString(),
            $"Kundvagn checkades ut med {cart.Items.Count} produkter.");

        // Skapa order via HTTP
        var orderItems = cart.Items.Select(i => new OrderItemDto
        {
            ProductId = i.ProductId,
            Quantity = i.Quantity
        }).ToList();

        var order = await _orderClient.CreateOrderAsync(request.UserId, orderItems);

        // Rensa kundvagnen efter checkout
        await _cartService.ClearCartAsync(request.UserId);

        // Returnera både cart + order
        return Ok(new
        {
            message = "Checkout klar.",
            cart = new
            {
                cart.UserId,
                Items = orderItems
            },
            order
        });
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

public class CheckoutRequest
{
    public Guid UserId { get; set; }
}
