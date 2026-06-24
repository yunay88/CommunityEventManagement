using CommunityEventManagement.Domain.Entities;
using CommunityEventManagement.Domain.Models;

namespace CommunityEventManagement.Domain.Interfaces.Services
{
    /// <summary>
    /// Service interface for event business logic operations.
    /// Demonstrates: interface segregation, service layer pattern.
    /// Implemented by: EventService
    /// </summary>
    public interface IEventService
    {
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<IEnumerable<Event>> GetUpcomingEventsAsync();
        Task<IEnumerable<Event>> FilterEventsAsync(FilterCriteria criteria);
        Task<Event?> GetEventByIdAsync(int id);
        Task<Event?> GetEventWithDetailsAsync(int id);
        Task<Event> CreateEventAsync(string name, string description, DateTime startDate, DateTime endDate, int? venueId);
        Task UpdateEventAsync(int id, string name, string description, DateTime startDate, DateTime endDate, int? venueId);
        Task DeleteEventAsync(int id);
        Task AddActivityToEventAsync(int eventId, int activityId, int order = 0);
        Task RemoveActivityFromEventAsync(int eventId, int activityId);
        Task<IEnumerable<Event>> GetEventsByVenueAsync(int venueId);
        Task<IEnumerable<Event>> GetEventsByParticipantAsync(int participantId);
    }
}