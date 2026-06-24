using CommunityEventManagement.Domain.Exceptions;
using CommunityEventManagement.Domain.Interfaces;
using CommunityEventManagement.Domain.Interfaces.Services;
using CommunityEventManagement.Domain.Models;
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
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthService> _logger;

        // In-memory session state
        // In production this would use ASP.NET Core Identity + JWT
        private string? _currentUserEmail;
        private string? _currentUserRole;
        private bool _isAuthenticated;

        public AuthService(IUnitOfWork unitOfWork, ILogger<AuthService> logger)
        {
            _unitOfWork = unitOfWork
                ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> LoginAsync(LoginModel loginModel)
        {
            if (loginModel == null)
                throw new ArgumentNullException(nameof(loginModel));

            try
            {
                _logger.LogInformation(
                    "Login attempt for: {Email}", loginModel.Email);

                // Check administrators first
                var admin = await _unitOfWork.Participants
                    .GetByEmailAsync(loginModel.Email);

                // Check participants
                var participant = await _unitOfWork.Participants
                    .GetByEmailAsync(loginModel.Email);

                // Simple hash comparison
                var inputHash = Convert.ToBase64String(
                    System.Text.Encoding.UTF8.GetBytes(loginModel.Password));

                if (participant != null && participant.PasswordHash == inputHash)
                {
                    _isAuthenticated = true;
                    _currentUserEmail = participant.Email;
                    _currentUserRole = "Participant";

                    _logger.LogInformation(
                        "Participant login successful: {Email}", loginModel.Email);
                    return true;
                }

                _logger.LogWarning(
                    "Failed login attempt for: {Email}", loginModel.Email);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", loginModel.Email);
                throw new AuthenticationException(
                    $"Login failed for {loginModel.Email}.");
            }
            finally
            {
                _logger.LogDebug("LoginAsync completed for {Email}", loginModel.Email);
            }
        }

        public Task LogoutAsync()
        {
            _isAuthenticated = false;
            _currentUserEmail = null;
            _currentUserRole = null;

            _logger.LogInformation("User logged out");
            return Task.CompletedTask;
        }

        public Task<bool> IsAuthenticatedAsync()
            => Task.FromResult(_isAuthenticated);

        public Task<string?> GetCurrentUserEmailAsync()
            => Task.FromResult(_currentUserEmail);

        public Task<string?> GetCurrentUserRoleAsync()
            => Task.FromResult(_currentUserRole);

        public Task<bool> IsInRoleAsync(string role)
            => Task.FromResult(
                _isAuthenticated &&
                string.Equals(_currentUserRole, role,
                    StringComparison.OrdinalIgnoreCase));
    }
}