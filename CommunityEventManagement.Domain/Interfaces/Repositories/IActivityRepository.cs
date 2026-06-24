using CommunityEventManagement.Domain.Entities;
using CommunityEventManagement.Domain.Enums;

namespace CommunityEventManagement.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Activity-specific repository interface.
    /// Extends generic IRepository with activity-specific queries.
    /// </summary>
    public interface IActivityRepository : IRepository<Activity>
    {
        /// <summary>Returns all activities of a specific type.</summary>
        Task<IEnumerable<Activity>> GetByTypeAsync(ActivityType type);

        /// <summary>Returns all activities associated with a specific event.</summary>
        Task<IEnumerable<Activity>> GetByEventAsync(int eventId);
    }
}