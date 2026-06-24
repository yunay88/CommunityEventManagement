namespace CommunityEventManagement.Domain.Enums
{
    /// <summary>
    /// Represents the lifecycle states of an event registration.
    /// A registration moves through these states over time.
    /// Pending → Confirmed → Cancelled
    /// Pending → Waitlisted → Confirmed
    /// </summary>
    public enum RegistrationStatus
    {
        Pending = 0,
        Confirmed = 1,
        Cancelled = 2,
        Waitlisted = 3
    }
}