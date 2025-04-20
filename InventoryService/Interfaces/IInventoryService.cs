using System;
using System.Threading.Tasks;
using InventoryService.Models;

namespace InventoryService.Interfaces;

public interface IInventoryService
{
    Task<InventoryItem?> GetInventoryAsync(Guid productId);
    Task<bool> UpdateInventoryAsync(UpdateInventoryRequest request);
}
