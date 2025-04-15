using Microsoft.EntityFrameworkCore;
using OrderingService.Data;
using OrderingService.Interfaces;
using OrderingService.Models;
using OrderingService.Data;
using OrderingService.Interfaces;
using OrderingService.Models;

namespace OrderingService.Services;

public class OrderService : IOrderService
{
    private readonly OrderDbContext _context;

    public OrderService(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Order?> GetOrderByIdAsync(Guid id) =>
        await _context.Orders.FindAsync(id);

    public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId) =>
        await _context.Orders.Where(o => o.UserId == userId).ToListAsync();
}