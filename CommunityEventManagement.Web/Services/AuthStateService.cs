using CommunityEventManagement.Web.ViewModels;

namespace CommunityEventManagement.Web.Services
{
    /// <summary>
    /// Manages authentication state across Blazor components.
    /// Uses Action callbacks to notify components of state changes.
    ///
    /// In Blazor Server, services are Scoped per circuit (per user connection).
    /// This means each user has their own AuthStateService instance.
    ///
    /// Demonstrates:
    ///   - Service layer in Blazor architecture
    ///   - Observer-like pattern using Action callbacks
    ///   - Encapsulation of auth state
    /// </summary>
    public class AuthStateService
    {
        private UserSessionViewModel _currentUser = new();

        /// <summary>
        /// Components subscribe to this to get notified of auth changes.
        /// Demonstrates: event/callback pattern in Blazor.
        /// </summary>
        public event Action? OnAuthStateChanged;

        public UserSessionViewModel CurrentUser => _currentUser;
        public bool IsAuthenticated => _currentUser.IsAuthenticated;
        public bool IsAdmin => _currentUser.IsAdmin;

        public void SetUser(string email, string fullName, string role)
        {
            _currentUser = new UserSessionViewModel
            {
                IsAuthenticated = true,
                Email = email,
                FullName = fullName,
                Role = role
            };

            // Notify all subscribed components
            NotifyStateChanged();
        }

        public void ClearUser()
        {
            _currentUser = new UserSessionViewModel
            {
                IsAuthenticated = false
            };

            NotifyStateChanged();
        }

        private void NotifyStateChanged()
        {
            OnAuthStateChanged?.Invoke();
        }
    }
}