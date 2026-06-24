using CommunityEventManagement.Domain.Enums;

namespace CommunityEventManagement.Domain.Entities
{
    /// <summary>
    /// Abstract AppUser — third level of the inheritance hierarchy.
    /// Represents any authenticated user of the system.
    /// 
    /// Inheritance chain: AppUser → Person → BaseEntity
    /// Subclasses: Participant, Administrator
    /// 
    /// Demonstrates:
    ///   - Multi-level inheritance (3 levels deep before concrete classes)
    ///   - Authentication model in the inheritance chain
    ///   - Abstract class at level 3
    /// </summary>
    public abstract class AppUser : Person
    {
        public string PasswordHash { get; protected set; } = string.Empty;
        public UserRole Role { get; protected set; }
        public DateTime? LastLoginDate { get; protected set; }
        public bool IsActive { get; protected set; }

        // EF Core constructor
        protected AppUser() { }

        // Parameterised constructor — calls Person constructor (chain up)
        protected AppUser(
            string firstName,
            string lastName,
            string email,
            string passwordHash,
            UserRole role,
            string? phoneNumber = null)
            : base(firstName, lastName, email, phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash cannot be empty.", nameof(passwordHash));

            PasswordHash = passwordHash;
            Role = role;
            IsActive = true;
        }

        // Virtual method — subclasses can override
        public virtual bool CanManageEvents() => Role == UserRole.Admin;
        public virtual bool CanRegisterForEvents() => Role == UserRole.Participant;

        public void RecordLogin()
        {
            LastLoginDate = DateTime.UtcNow;
            MarkAsUpdated();
        }

        public void Deactivate()
        {
            IsActive = false;
            MarkAsUpdated();
        }

        public void Activate()
        {
            IsActive = true;
            MarkAsUpdated();
        }
    }
}