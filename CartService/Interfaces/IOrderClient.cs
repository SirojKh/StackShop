using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CartService.Interfaces;

public interface IOrderClient
{
    Task<object?> CreateOrderAsync(Guid userId, List<OrderItemDto> items);
}

public class OrderItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}