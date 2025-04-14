using ProductService.Models;

namespace ProductService.Data;

// Data/DbInitializer.cs
public static class DbInitializer
{
    public static void Seed(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ProductDbContext>();

        if (!context.Products.Any())
        {
            context.Products.AddRange(
                new Product { Id = Guid.NewGuid(), Name = "Laptop", Price = 12999, Description = "Gaming laptop", Category = "Electronics", ImageUrl = "https://..." },
                new Product { Id = Guid.NewGuid(), Name = "Phone", Price = 7999, Description = "Smartphone", Category = "Electronics", ImageUrl = "https://..." }
            );
            context.SaveChanges();
        }
    }
}
