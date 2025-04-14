using ProductService.DTOs;
using ProductService.Models;

namespace ProductService.Interfaces;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(Guid id);
    Task<Product> CreateAsync(ProductCreateDto dto);
    Task<Product?> UpdateAsync(Guid id, ProductUpdateDto dto);
    Task<bool> DeleteAsync(Guid id);
}