// FILE: CommunityEventManagement.Web/Services/AuthStateService.cs
// REPLACE ENTIRE FILE

using CommunityEventManagement.Domain.Interfaces.Services;
using CommunityEventManagement.Web.ViewModels;

namespace CommunityEventManagement.Web.Services
{
    /// <summary>
    /// Manages authentication state across Blazor components.
    /// Wraps IAuthService and provides a Blazor-friendly session ViewModel.
    /// 
    /// In Blazor Server, services are Scoped per circuit (per user connection).
    /// This means each user has their own AuthStateService instance.
    ///
    /// Demonstrates:
    ///   - Service layer in Blazor architecture
    ///   - Observer-like pattern using Action callbacks
    ///   - Encapsulation of auth state
    ///   - Facade over IAuthService for UI consumption
    /// </summary>
    public class AuthStateService
    {
        private readonly IAuthService _authService;
        private UserSessionViewModel _currentUser = new();

        /// <summary>
        /// Components subscribe to this to get notified of auth changes.
        /// Demonstrates: event/callback pattern in Blazor.
        /// </summary>
        public event Action? OnAuthStateChanged;

        public AuthStateService(IAuthService authService)
        {
            _authService = authService;
        }

        public UserSessionViewModel CurrentUser => _currentUser;
        public bool IsAuthenticated => _currentUser.IsAuthenticated;
        public bool IsAdmin => _currentUser.IsAdmin;
        public bool IsParticipant => _currentUser.IsParticipant;
        public int? UserId => _currentUser.UserId;

        /// <summary>
        /// Attempt login via IAuthService, then populate session ViewModel.
        /// Returns true on success.
        /// </summary>
        public async Task<bool> LoginAsync(string email, string password)
        {
            var loginModel = new CommunityEventManagement.Domain.Models.LoginModel
            {
                Email = email,
                Password = password
            };

            var success = await _authService.LoginAsync(loginModel);
            if (!success) return false;

            // Pull full user details from auth service (polymorphic AppUser)
            var user = await _authService.GetCurrentUserAsync();
            if (user == null) return false;

            _currentUser = new UserSessionViewModel
            {
                IsAuthenticated = true,
                UserId = user.Id,
                Email = user.Email,
                FullName = user.GetFullName(),
                Role = user.Role.ToString()
            };

            NotifyStateChanged();
            return true;
        }

        public async Task LogoutAsync()
        {
            await _authService.LogoutAsync();
            _currentUser = new UserSessionViewModel { IsAuthenticated = false };
            NotifyStateChanged();
        }

        // Kept for backward compatibility with existing code
        public void SetUser(string email, string fullName, string role)
        {
            _currentUser = new UserSessionViewModel
            {
                IsAuthenticated = true,
                Email = email,
                FullName = fullName,
                Role = role
            };
            NotifyStateChanged();
        }

        public void ClearUser()
        {
            _currentUser = new UserSessionViewModel { IsAuthenticated = false };
            NotifyStateChanged();
        }

        private void NotifyStateChanged()
        {
            OnAuthStateChanged?.Invoke();
        }
    }
}
