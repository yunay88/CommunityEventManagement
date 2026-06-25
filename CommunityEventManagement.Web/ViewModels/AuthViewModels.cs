// FILE: CommunityEventManagement.Web/ViewModels/AuthViewModels.cs
// REPLACE ENTIRE FILE

using System.ComponentModel.DataAnnotations;

namespace CommunityEventManagement.Web.ViewModels
{
    /// <summary>
    /// Login form ViewModel.
    /// Used by the Login Blazor component.
    /// </summary>
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; } = false;
    }

    /// <summary>
    /// Holds current authenticated user state.
    /// Used across Blazor components via AuthStateService.
    /// </summary>
    public class UserSessionViewModel
    {
        public bool IsAuthenticated { get; set; }
        public int? UserId { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Role { get; set; }
        public bool IsAdmin => Role == "Admin";
        public bool IsParticipant => Role == "Participant";
    }
}
