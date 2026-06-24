using CommunityEventManagement.Domain.Entities;

namespace CommunityEventManagement.Domain.Interfaces.Services
{
    /// <summary>
    /// Service interface for registration business logic operations.
    /// Handles the complex registration workflow including
    /// duplicate checking, capacity validation, and status management.
    /// </summary>
    public interface IRegistrationService
    {
        Task<IEnumerable<Registration>> GetAllRegistrationsAsync();
        Task<Registration?> GetRegistrationByIdAsync(int id);
        Task<IEnumerable<Registration>> GetParticipantRegistrationsAsync(int participantId);
        Task<IEnumerable<Registration>> GetEventRegistrationsAsync(int eventId);
        Task<Registration> RegisterParticipantAsync(int participantId, int eventId, string? notes = null);
        Task CancelRegistrationAsync(int registrationId, string? reason = null);
        Task ConfirmRegistrationAsync(int registrationId);
        Task<bool> IsParticipantRegisteredAsync(int participantId, int eventId);
    }
}