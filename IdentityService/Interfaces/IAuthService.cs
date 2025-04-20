using System.Threading.Tasks;
using IdentityService.Models;

namespace IdentityService.Interfaces;

public interface IAuthService
{
    Task<User?> RegisterAsync(string email, string password);
    Task<string?> LoginAsync(string email, string password);
}