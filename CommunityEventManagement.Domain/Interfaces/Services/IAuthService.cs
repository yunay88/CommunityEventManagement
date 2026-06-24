using CommunityEventManagement.Domain.Models;

namespace CommunityEventManagement.Domain.Interfaces.Services
{
    /// <summary>
    /// Service interface for authentication operations.
    /// Handles login, logout, and session management.
    /// </summary>
    public interface IAuthService
    {
        Task<bool> LoginAsync(LoginModel loginModel);
        Task LogoutAsync();
        Task<bool> IsAuthenticatedAsync();
        Task<string?> GetCurrentUserEmailAsync();
        Task<string?> GetCurrentUserRoleAsync();
        Task<bool> IsInRoleAsync(string role);
    }
}