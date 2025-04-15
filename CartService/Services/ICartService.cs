using CartService.Models;

namespace CartService.Services;

public interface ICartService
{
    Task<Cart?> GetCartAsync(Guid userId);
    Task AddItemAsync(Guid userId, Guid productId, int quantity);
    Task RemoveItemAsync(Guid userId, Guid productId);
    Task ClearCartAsync(Guid userId);
}