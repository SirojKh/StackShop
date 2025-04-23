using Microsoft.EntityFrameworkCore;
using CartService.Data;
using CartService.Interfaces;
using CartService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CartDbContext>(options =>
    options.UseInMemoryDatabase("Carts"));

builder.Services.AddHttpClient<IAuditLogger, AuditHttpClient>(client =>
{
    client.BaseAddress = new Uri("http://audit:7010");
});

builder.Services.AddHttpClient<IOrderClient, OrderHttpClient>(client =>
{
    client.BaseAddress = new Uri("http://ordering:7003");
});

builder.Services.AddScoped<ICartService, CartService.Services.CartService>();

var app = builder.Build();

// Middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Kör på port 7004
app.Run("http://0.0.0.0:7004");