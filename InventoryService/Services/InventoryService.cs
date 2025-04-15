using InventoryService.Data;
using InventoryService.Interfaces;
using InventoryService.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Services;

public class InventoryService : IInventoryService
{
    private readonly InventoryDbContext _context;

    public InventoryService(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryItem?> GetInventoryAsync(Guid productId)
    {
        return await _context.InventoryItems
            .FirstOrDefaultAsync(i => i.ProductId == productId);
    }

    public async Task<bool> UpdateInventoryAsync(UpdateInventoryRequest request)
    {
        var inventory = await _context.InventoryItems
            .FirstOrDefaultAsync(i => i.ProductId == request.ProductId);

        if (inventory == null)
        {
            inventory = new InventoryItem
            {
                Id = Guid.NewGuid(),
                ProductId = request.ProductId,
                Quantity = request.QuantityChange
            };

            _context.InventoryItems.Add(inventory);
        }
        else
        {
            inventory.Quantity += request.QuantityChange;
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
