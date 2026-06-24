using CommunityEventManagement.Domain.Enums;

namespace CommunityEventManagement.Domain.Entities
{
    /// <summary>
    /// Administrator — concrete entity at level 4 of inheritance hierarchy.
    /// Represents a system administrator who manages events.
    /// 
    /// Inheritance chain: Administrator → AppUser → Person → BaseEntity
    /// 
    /// Demonstrates:
    ///   - Polymorphism: GetDisplayName() and GetFullName() both overridden
    ///     differently from Participant — SAME method, DIFFERENT behaviour
    ///   - 4-level inheritance chain
    /// </summary>
    public class Administrator : AppUser
    {
        public string AdminRole { get; private set; } = string.Empty;
        public string Department { get; private set; } = string.Empty;

        // EF Core constructor
        private Administrator() { }

        public Administrator(
            string firstName,
            string lastName,
            string email,
            string passwordHash,
            string adminRole,
            string department = "General")
            : base(firstName, lastName, email, passwordHash, UserRole.Admin)
        {
            AdminRole = adminRole;
            Department = department;
        }

        // OVERRIDE — different from Participant's GetDisplayName (polymorphism)
        public override string GetDisplayName() => $"{GetFullName()} [{AdminRole}]";

        // OVERRIDE GetFullName — different from Person's version (polymorphism)
        public override string GetFullName() => $"Admin: {FirstName} {LastName}";

        // OVERRIDE GetSummary — different from all parent implementations
        public override string GetSummary()
        {
            return $"Administrator: {FirstName} {LastName} | " +
                   $"Role: {AdminRole} | " +
                   $"Department: {Department} | " +
                   $"Last Login: {LastLoginDate?.ToString("dd/MM/yyyy") ?? "Never"}";
        }

        // OVERRIDE — admin can manage events
        public override bool CanManageEvents() => true;
        public override bool CanRegisterForEvents() => false;

        public void UpdateAdminDetails(string adminRole, string department)
        {
            AdminRole = adminRole;
            Department = department;
            MarkAsUpdated();
        }
    }
}