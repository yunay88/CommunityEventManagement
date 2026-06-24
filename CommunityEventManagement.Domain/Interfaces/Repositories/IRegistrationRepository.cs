using CommunityEventManagement.Domain.Entities;

namespace CommunityEventManagement.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Registration-specific repository interface.
    /// Extends generic IRepository with registration-specific queries.
    /// </summary>
    public interface IRegistrationRepository : IRepository<Registration>
    {
        /// <summary>Returns all registrations for a specific participant.</summary>
        Task<IEnumerable<Registration>> GetByParticipantAsync(int participantId);

        /// <summary>Returns all registrations for a specific event.</summary>
        Task<IEnumerable<Registration>> GetByEventAsync(int eventId);

        /// <summary>Returns registration with full participant and event details.</summary>
        Task<Registration?> GetWithDetailsAsync(int id);

        /// <summary>
        /// Checks if a participant is already registered for an event.
        /// Used for duplicate registration validation.
        /// </summary>
        Task<bool> ExistsAsync(int participantId, int eventId);
    }
}