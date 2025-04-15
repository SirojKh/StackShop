using EmailService.Models;
using EmailService.Services;

var builder = WebApplication.CreateBuilder(args);

// Lägg till SMTP-inställningar från appsettings.json
builder.Services.Configure<SmtpSettings>(
    builder.Configuration.GetSection("SmtpSettings"));

// Registrera EmailService
builder.Services.AddScoped<IEmailService, EmailService.Services.EmailService>();

// Swagger och controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();