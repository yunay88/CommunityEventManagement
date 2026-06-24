using CommunityEventManagement.Domain.Entities;
using CommunityEventManagement.Domain.Models;

namespace CommunityEventManagement.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Event-specific repository interface.
    /// Extends the generic IRepository with event-specific queries.
    /// Demonstrates: interface inheritance (IEventRepository extends IRepository).
    /// </summary>
    public interface IEventRepository : IRepository<Event>
    {
        /// <summary>Returns event with all related data included.</summary>
        Task<Event?> GetWithDetailsAsync(int id);

        /// <summary>Returns all events that have not yet started.</summary>
        Task<IEnumerable<Event>> GetUpcomingEventsAsync();

        /// <summary>Returns events matching the given filter criteria.</summary>
        Task<IEnumerable<Event>> FilterEventsAsync(FilterCriteria criteria);

        /// <summary>Returns all events at a specific venue.</summary>
        Task<IEnumerable<Event>> GetByVenueAsync(int venueId);

        /// <summary>Returns all events a participant is registered for.</summary>
        Task<IEnumerable<Event>> GetByParticipantAsync(int participantId);
    }
}