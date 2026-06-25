namespace CommunityEventManagement.Domain.Models
{
    /// <summary>
    /// Authentication result returned from IAuthService.
    /// Demonstrates: DTO pattern, encapsulation of auth outcome.
    /// </summary>
    public class AuthResultModel
    {
        public bool Success { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Role { get; set; }
        public string? ErrorMessage { get; set; }

        public static AuthResultModel Failed(string message) => new()
        {
            Success = false,
            ErrorMessage = message
        };

        public static AuthResultModel Succeeded(string email, string fullName, string role) => new()
        {
            Success = true,
            Email = email,
            FullName = fullName,
            Role = role
        };
    }
}
