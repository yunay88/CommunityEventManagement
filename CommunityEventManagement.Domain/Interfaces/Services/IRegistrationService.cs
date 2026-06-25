// FILE: CommunityEventManagement.Domain/Interfaces/Services/IRegistrationService.cs
// REPLACE ENTIRE FILE
// Change: adds GetRegistrationsByParticipantAsync alias (forwards to GetParticipantRegistrationsAsync)
// This improves API clarity for the UI layer – "Registrations by Participant" vs "Participant Registrations"
// Demonstrates interface evolution / backward compatibility

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
        
        // Alias for UI clarity – same as GetParticipantRegistrationsAsync
        // Demonstrates: interface default implementation / semantic naming
        Task<IEnumerable<Registration>> GetRegistrationsByParticipantAsync(int participantId);
        
        Task<IEnumerable<Registration>> GetEventRegistrationsAsync(int eventId);
        
        /// <summary>
        /// Register a participant for an event.
        /// Parameter order: participantId, eventId (matches domain: Participant registers FOR Event)
        /// </summary>
        Task<Registration> RegisterParticipantAsync(int participantId, int eventId, string? notes = null);
        
        Task CancelRegistrationAsync(int registrationId, string? reason = null);
        Task ConfirmRegistrationAsync(int registrationId);
        Task<bool> IsParticipantRegisteredAsync(int participantId, int eventId);
    }
}
