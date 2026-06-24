using CommunityEventManagement.Domain.Entities;
using CommunityEventManagement.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace CommunityEventManagement.Infrastructure.Patterns.Factory
{
    /// <summary>
    /// Request object for creating a Participant.
    /// </summary>
    public class CreateParticipantRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }

    /// <summary>
    /// Participant Factory — encapsulates Participant creation logic.
    ///
    /// Factory Pattern (Creational):
    ///   - Centralises all participant creation
    ///   - Handles password hashing
    ///   - Validates email format, phone format
    ///   - Ensures consistent participant initialisation
    ///
    /// Demonstrates:
    ///   - Factory pattern for a different entity (ParticipantFactory vs EventFactory)
    ///   - Input sanitisation
    ///   - Email validation using Regex
    /// </summary>
    public class ParticipantFactory : IEntityFactory<Participant, CreateParticipantRequest>
    {
        private static readonly Regex EmailRegex = new(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex PhoneRegex = new(
            @"^(\+44|0)[\d\s\-]{9,14}$",
            RegexOptions.Compiled);

        /// <summary>
        /// Creates a new Participant from a validated request.
        /// Handles password hashing internally.
        /// </summary>
        public Participant Create(CreateParticipantRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var errors = Validate(request).ToList();
            if (errors.Any())
                throw new RegistrationException(
                    $"Cannot create participant. Errors: {string.Join(", ", errors)}");

            // Hash the password — factory handles this concern
            var passwordHash = HashPassword(request.Password);

            return new Participant(
                request.FirstName.Trim(),
                request.LastName.Trim(),
                request.Email.Trim().ToLowerInvariant(),
                passwordHash,
                request.PhoneNumber?.Trim());
        }

        /// <summary>
        /// Validates a CreateParticipantRequest.
        /// </summary>
        public IEnumerable<string> Validate(CreateParticipantRequest request)
        {
            if (request == null)
            {
                yield return "Request cannot be null.";
                yield break;
            }

            if (string.IsNullOrWhiteSpace(request.FirstName))
                yield return "First name is required.";

            if (request.FirstName?.Length < 2)
                yield return "First name must be at least 2 characters.";

            if (string.IsNullOrWhiteSpace(request.LastName))
                yield return "Last name is required.";

            if (request.LastName?.Length < 2)
                yield return "Last name must be at least 2 characters.";

            if (string.IsNullOrWhiteSpace(request.Email))
                yield return "Email address is required.";
            else if (!EmailRegex.IsMatch(request.Email))
                yield return "Email address is not in a valid format.";

            if (string.IsNullOrWhiteSpace(request.Password))
                yield return "Password is required.";
            else if (request.Password.Length < 6)
                yield return "Password must be at least 6 characters.";

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber) &&
                !PhoneRegex.IsMatch(request.PhoneNumber))
                yield return "Phone number is not in a valid UK format.";
        }

        /// <summary>
        /// Creates a participant with a pre-hashed password.
        /// Overloaded version — demonstrates method overloading in factory.
        /// </summary>
        public Participant CreateWithHashedPassword(
            string firstName,
            string lastName,
            string email,
            string passwordHash,
            string? phoneNumber = null)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name required.", nameof(firstName));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email required.", nameof(email));

            return new Participant(
                firstName.Trim(),
                lastName.Trim(),
                email.Trim().ToLowerInvariant(),
                passwordHash,
                phoneNumber?.Trim());
        }

        // Simple password hashing — in production use BCrypt
        private static string HashPassword(string password)
        {
            return Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
}