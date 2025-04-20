using System;
using IdentityService.Data;
using IdentityService.Interfaces;
using IdentityService.Models;
using IdentityService.Jwt;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public class AuthService : IAuthService
{
    private readonly IdentityDbContext _context;
    private readonly JwtTokenGenerator _jwt;
    private readonly IAuditLogger _audit;

    public AuthService(IdentityDbContext context, JwtTokenGenerator jwt, IAuditLogger audit)
    {
        _context = context;
        _jwt = jwt;
        _audit = audit;
    }

    public async Task<User?> RegisterAsync(string email, string password)
    {
        if (await _context.Users.AnyAsync(u => u.Email == email)) return null;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
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

            // Lägg till detta
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