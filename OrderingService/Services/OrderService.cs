using Microsoft.EntityFrameworkCore;
using OrderingService.Interfaces;
using OrderingService.Models;
using Shared.Contracts;
using Shared.Contracts.Events;

namespace OrderingService.Services;

public class OrderService : IOrderService
{
    private readonly OrderDbContext _context;
    private readonly IRabbitMqPublisher _publisher;
    private readonly ILogger<OrderService> _logger;
    private readonly IAuditLogger _audit;

    public OrderService(OrderDbContext context, IRabbitMqPublisher publisher, ILogger<OrderService> logger, IAuditLogger audit)
    {
        _context = context;
        _publisher = publisher;
        _logger = logger;
        _audit = audit;
    }


    public async Task<Order> CreateOrderAsync(Order order)
    {
        try
        {
            _logger.LogInformation("Skapar order för användare: {UserId}", order.UserId);

            order.Id = order.Id == Guid.Empty ? Guid.NewGuid() : order.Id;
            order.CreatedAt = DateTime.UtcNow;

            foreach (var item in order.Items)
            {
                item.OrderId = order.Id;
                item.Order = null;
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var eventMessage = new OrderCreatedEvent
            {
                OrderId = order.Id,
                UserId = order.UserId,
                CreatedAt = order.CreatedAt,
                UserEmail = "sirojkhamid@outlook.com",
                Items = order.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };

            _publisher.Publish(eventMessage, "order-created-queue");

            _logger.LogInformation("Order skapad och skickad till queue med ID: {OrderId}", order.Id);

            //Logga till AuditService
            await _audit.LogAsync("OrderCreated", order.UserId.ToString(), $"Order {order.Id} skapad.");

            return order;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ett fel inträffade vid skapande av order.");

            //Logga även fel till AuditService
            await _audit.LogAsync("OrderError", order.UserId.ToString(), $"Fel vid order: {ex.Message}");

            throw;
        }
    }




    public async Task<Order?> GetOrderByIdAsync(Guid id) =>
        await _context.Orders.FindAsync(id);

    public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId) =>
        await _context.Orders.Where(o => o.UserId == userId).ToListAsync();
}