using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CartService.Data;
using CartService.Interfaces;
using CartService.Models;
using Microsoft.EntityFrameworkCore;

namespace CartService.Services;

public class CartService : ICartService
{
    private readonly CartDbContext _context;
    private readonly IAuditLogger _audit;
    private readonly IAnalyticsLogger _analytics;

    public CartService(CartDbContext context, IAuditLogger audit, IAnalyticsLogger analytics)
    {
        _context = context;
        _audit = audit;
        _analytics = analytics;
    }

    public async Task<Cart?> GetCartAsync(Guid userId)
    {
        return await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task AddItemAsync(Guid userId, Guid productId, int quantity)
    {
        var cart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            _context.Carts.Add(cart);
        }

        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            item.Quantity += quantity;
        }
        else
        {
            cart.Items.Add(new CartItem { ProductId = productId, Quantity = quantity });
        }

        await _context.SaveChangesAsync();

        await _audit.LogAsync("CartItemAdded", userId.ToString(), $"Produkt {productId} x{quantity} lades till.");
        await _analytics.LogAsync("CartItemAdded", userId.ToString(), JsonSerializer.Serialize(new
        {
            productId,
            quantity
        }));
    }

    public async Task RemoveItemAsync(Guid userId, Guid productId)
    {
        var cart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null) return;

        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            cart.Items.Remove(item);
            await _context.SaveChangesAsync();
        }
    }

    public async Task ClearCartAsync(Guid userId)
    {
        var cart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null) return;

        cart.Items.Clear();
        await _context.SaveChangesAsync();

        await _audit.LogAsync("CartCleared", userId.ToString(), "Kundvagn tömd.");
        await _analytics.LogAsync("CartCleared", userId.ToString(), null);
    }
}
