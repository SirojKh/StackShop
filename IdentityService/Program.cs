using IdentityService.Data;
using IdentityService.Interfaces;
using IdentityService.Jwt;
using IdentityService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// JWT-inställningar
var jwtSettings = new JwtSettings
{
    Key = builder.Configuration["JwtSettings:Key"] ?? throw new Exception("JWT key missing!"),
    Issuer = builder.Configuration["JwtSettings:Issuer"] ?? "StackShop.Identity",
    Audience = builder.Configuration["JwtSettings:Audience"] ?? "StackShop.Users",
    ExpiryMinutes = int.TryParse(builder.Configuration["JwtSettings:ExpiryMinutes"], out var minutes) ? minutes : 60
};

builder.Services.AddSingleton(jwtSettings);
builder.Services.AddSingleton<JwtTokenGenerator>();

// Databas
builder.Services.AddDbContext<IdentityDbContext>(opt =>
    opt.UseInMemoryDatabase("IdentityDb"));

builder.Services.AddScoped<IAuthService, AuthService>();

// HTTP-klienter för Audit + Analytics
builder.Services.AddHttpClient<IAuditLogger, AuditHttpClient>(client =>
{
    client.BaseAddress = new Uri("http://audit:7010");
});

builder.Services.AddHttpClient<IAnalyticsLogger, AnalyticsHttpClient>(client =>
{
    client.BaseAddress = new Uri("http://analytics:7009");
});

// Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference{
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run("http://0.0.0.0:7005");
