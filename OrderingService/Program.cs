using Microsoft.EntityFrameworkCore;
using OrderingService.Data;
using OrderingService.Interfaces;
using OrderingService.Services;
using OrderingService.Data;
using OrderingService.Interfaces;
using OrderingService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseInMemoryDatabase("OrderDb"));

builder.Services.AddScoped<IOrderService, OrderingService.Services.OrderService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run("http://0.0.0.0:7003");