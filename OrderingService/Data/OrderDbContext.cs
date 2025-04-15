using Microsoft.EntityFrameworkCore;
using OrderingService.Models;
using OrderingService.Models;

namespace OrderingService.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
}