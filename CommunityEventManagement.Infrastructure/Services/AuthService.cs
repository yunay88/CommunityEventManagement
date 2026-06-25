// FILE: CommunityEventManagement.Infrastructure/Services/AuthService.cs
// REPLACE ENTIRE FILE

using CommunityEventManagement.Domain.Entities;
using CommunityEventManagement.Domain.Exceptions;
using CommunityEventManagement.Domain.Interfaces.Services;
using CommunityEventManagement.Domain.Models;
using CommunityEventManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CommunityEventManagement.Infrastructure.Services
{
    /// <summary>
    /// Authentication service.
    /// Handles login validation and session state.
    ///
    /// Demonstrates:
    ///   - Authentication business logic
    ///   - Custom exception usage (AuthenticationException)
    ///   - try-catch-finally pattern
    ///   - Polymorphism: AppUser → Participant / Administrator
    ///   - Role-based access control
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthService> _logger;

        // In-memory session state for Blazor Server (Scoped per circuit)
        // In production this would use ASP.NET Core Identity + Cookies/JWT
        private AppUser? _currentUser;

        public AuthService(ApplicationDbContext context, ILogger<AuthService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> LoginAsync(LoginModel loginModel)
        {
            if (loginModel == null)
                throw new ArgumentNullException(nameof(loginModel));

            try
            {
                _logger.LogInformation("Login attempt for: {Email}", loginModel.Email);

                // Hash the input password the same way SeedData does
                // SeedData: Convert.ToBase64String(Encoding.UTF8.GetBytes(password))
                var inputHash = Convert.ToBase64String(
                    System.Text.Encoding.UTF8.GetBytes(loginModel.Password));

                // Query the Person hierarchy (TPH) – finds both Participants and Administrators
                // AppUser contains PasswordHash and Role
                var user = await _context.Set<Person>()
                    .OfType<AppUser>()
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == loginModel.Email.ToLower());

                if (user == null)
                {
                    _logger.LogWarning("Login failed – user not found: {Email}", loginModel.Email);
                    return false;
                }

                if (!user.IsActive)
                {
                    _logger.LogWarning("Login failed – account inactive: {Email}", loginModel.Email);
                    throw new AuthenticationException("Account is deactivated.");
                }

                // Verify password hash
                if (user.PasswordHash != inputHash)
                {
                    _logger.LogWarning("Login failed – invalid password: {Email}", loginModel.Email);
                    return false;
                }

                // Success – store session
                _currentUser = user;
                user.RecordLogin();
                await _context.SaveChangesAsync();

                _logger.LogInformation("Login successful: {Email} Role={Role}",
                    loginModel.Email, user.Role);

                return true;
            }
            catch (AuthenticationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", loginModel.Email);
                throw new AuthenticationException($"Login failed for {loginModel.Email}.");
            }
            finally
            {
                _logger.LogDebug("LoginAsync completed for {Email}", loginModel.Email);
            }
        }

        public Task LogoutAsync()
        {
            if (_currentUser != null)
            {
                _logger.LogInformation("User logged out: {Email}", _currentUser.Email);
            }
            _currentUser = null;
            return Task.CompletedTask;
        }

        public Task<bool> IsAuthenticatedAsync()
            => Task.FromResult(_currentUser != null);

        public Task<string?> GetCurrentUserEmailAsync()
            => Task.FromResult(_currentUser?.Email);

        public Task<string?> GetCurrentUserRoleAsync()
            => Task.FromResult(_currentUser?.Role.ToString());

        public Task<bool> IsInRoleAsync(string role)
            => Task.FromResult(
                _currentUser != null &&
                string.Equals(_currentUser.Role.ToString(), role,
                    StringComparison.OrdinalIgnoreCase));

        // --- Extra helpers for Blazor AuthStateService ---
        // These are not in IAuthService, but the concrete class exposes them
        // AuthStateService can cast to AuthService to get full user details.

        public Task<AppUser?> GetCurrentUserAsync()
            => Task.FromResult(_currentUser);
    }
}
