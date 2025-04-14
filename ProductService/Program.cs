using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Interfaces;
using ProductService.Services;

var builder = WebApplication.CreateBuilder(args);

// 👇 Lägg detta direkt efter CreateBuilder
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80); // Matchar EXPOSE 80 i Dockerfile
});

// =============================================
// Services – Dependency Injection
// =============================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext med InMemory-databas (byt till UseSqlServer vid behov)
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseInMemoryDatabase("ProductDb"));

// Registrera tjänster
builder.Services.AddScoped<IProductService, ProductService.Services.ProductService>();

var app = builder.Build();

// =============================================
// Middleware-pipeline
// =============================================

DbInitializer.Seed(app);

// Swagger & middleware – alltid aktiv
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();