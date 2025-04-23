using System;
using IdentityService.Data;
using IdentityService.Interfaces;
using IdentityService.Models;
using IdentityService.Jwt;
using Microsoft.EntityFrameworkCore;

public class AuthService : IAuthService
{
    private readonly IdentityDbContext _context;
    private readonly JwtTokenGenerator _jwt;
    private readonly IAuditLogger _audit;
    private readonly IAnalyticsLogger _analytics;

    public AuthService(IdentityDbContext context, JwtTokenGenerator jwt, IAuditLogger audit, IAnalyticsLogger analytics)
    {
        _context = context;
        _jwt = jwt;
        _audit = audit;
        _analytics = analytics;
    }

    public async Task<User?> RegisterAsync(string email, string password)
    {
        if (await _context.Users.AnyAsync(u => u.Email == email))
        {
            await _audit.LogAsync("UserRegistrationFailed", null, $"Registrering misslyckades – e-post {email} existerar redan.");
            await _analytics.LogAsync("UserRegistrationFailed", null, $"E-post {email} är redan registrerad.");
            return null;
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        await _audit.LogAsync("UserRegistered", user.Id.ToString(), $"Ny användare registrerad: {email}");
        await _analytics.LogAsync("UserRegistered", user.Id.ToString(), $"Användare {email} skapades.");

        return user;
    }


    public async Task<string?> LoginAsync(string email, string password)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            var token = _jwt.GenerateToken(user);
            Console.WriteLine($"Token: {token}");

            await _audit.LogAsync("UserLoggedIn", user.Id.ToString(), $"Användare {user.Email} loggade in.");

            return token;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login error: {ex.Message}");
            await _audit.LogAsync("LoginError", null, $"Fel vid inloggning för {email}: {ex.Message}");
            throw;
        }
    }
}
