using System;

namespace InventoryService.Models;

public class UpdateInventoryRequest
{
    public Guid ProductId { get; set; }
    public int QuantityChange { get; set; } // + för tillägg, - för minskning
}
