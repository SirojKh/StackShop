using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Interfaces;
using NotificationService.Services;
using Shared.Contracts.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<INotificationService, NotificationService.Services.NotificationService>();

builder.Services.AddHttpClient<IEmailClient, EmailClient>(client =>
{
    client.BaseAddress = new Uri("http://email:8080");
});

builder.Services.AddHttpClient<IAuditLogger, AuditHttpClient>(client =>
{
    client.BaseAddress = new Uri("http://audit:7010");
});

builder.Services.AddHttpClient<IAnalyticsLogger, AnalyticsHttpClient>(client =>
{
    client.BaseAddress = new Uri("http://analytics:7009");
});

builder.Services.AddHostedService<OrderCreatedConsumer>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run("http://0.0.0.0:7006");