namespace CommunityEventManagement.Domain.Enums
{
    /// <summary>
    /// Defines the roles available in the Community Event Management System.
    /// Used for role-based access control throughout the application.
    /// Admin role → full CRUD on all entities
    /// Participant role → register for events, view own registrations
    /// </summary>
    public enum UserRole
    {
        Admin = 0,
        Participant = 1
    }
}