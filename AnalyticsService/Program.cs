using AnalyticsService.Data;
using AnalyticsService.Interfaces;
using AnalyticsService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AnalyticsService", Version = "v1" });
    c.UseInlineDefinitionsForEnums();
});



// Lägg till DbContext med InMemory
builder.Services.AddDbContext<AnalyticsDbContext>(opt =>
    opt.UseInMemoryDatabase("AnalyticsDb"));

builder.Services.AddScoped<IAnalyticsService, AnalyticsService.Services.AnalyticsService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();

app.Run("http://0.0.0.0:7009");