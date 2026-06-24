using System.ComponentModel.DataAnnotations;

namespace CommunityEventManagement.Domain.Entities
{
    /// <summary>
    /// Abstract Person class — second level of the inheritance hierarchy.
    /// Cannot be instantiated directly.
    /// 
    /// Inheritance chain: Person → BaseEntity
    /// Subclasses: Participant, Administrator, AppUser
    /// 
    /// Demonstrates:
    ///   - Multi-level inheritance (inherits BaseEntity)
    ///   - Abstract intermediate class
    ///   - Encapsulation with protected setters
    ///   - Method overloading (GetFullName has two versions)
    ///   - DataAnnotation validation attributes
    /// </summary>
    public abstract class Person : BaseEntity
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "First name must be between 2 and 50 characters")]
        public string FirstName { get; protected set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, MinimumLength = 2,
            ErrorMessage = "Last name must be between 2 and 50 characters")]
        public string LastName { get; protected set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; protected set; } = string.Empty;

        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string? PhoneNumber { get; protected set; }

        // Protected parameterless constructor for EF Core
        protected Person() { }

        // Protected parameterised constructor — only subclasses can call
        protected Person(string firstName, string lastName, string email, string? phoneNumber = null)
        {
            // Guard clauses — validate inputs
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name cannot be empty.", nameof(firstName));
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name cannot be empty.", nameof(lastName));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty.", nameof(email));

            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            Email = email.Trim().ToLowerInvariant();
            PhoneNumber = phoneNumber?.Trim();
        }

        // METHOD OVERLOADING — same name, different parameters
        // Version 1: simple full name
        public virtual string GetFullName() => $"{FirstName} {LastName}";

        // Version 2: optionally includes email (overloaded method)
        public virtual string GetFullName(bool includeEmail)
        {
            return includeEmail
                ? $"{FirstName} {LastName} <{Email}>"
                : GetFullName();
        }

        // Update contact details — encapsulates the update logic
        public void UpdateContactDetails(string email, string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty.", nameof(email));

            Email = email.Trim().ToLowerInvariant();
            PhoneNumber = phoneNumber?.Trim();
            MarkAsUpdated();
        }
    }
}