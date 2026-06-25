// FILE: CommunityEventManagement.Domain/Interfaces/Services/IAuthService.cs
// REPLACE ENTIRE FILE

using CommunityEventManagement.Domain.Entities;
using CommunityEventManagement.Domain.Models;

namespace CommunityEventManagement.Domain.Interfaces.Services
{
    /// <summary>
    /// Service interface for authentication operations.
    /// Handles login, logout, and session management.
    /// 
    /// Demonstrates:
    ///   - Interface segregation (auth concerns separated)
    ///   - Role-based access control contract
    /// </summary>
    public interface IAuthService
    {
        Task<bool> LoginAsync(LoginModel loginModel);
        Task LogoutAsync();
        Task<bool> IsAuthenticatedAsync();
        Task<string?> GetCurrentUserEmailAsync();
        Task<string?> GetCurrentUserRoleAsync();
        Task<bool> IsInRoleAsync(string role);

        // Extended for Blazor AuthState integration
        // Allows UI layer to get full polymorphic AppUser (Participant / Administrator)
        Task<AppUser?> GetCurrentUserAsync();
    }
}
