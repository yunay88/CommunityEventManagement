using CommunityEventManagement.Domain.Entities;

namespace CommunityEventManagement.Domain.Interfaces.Services
{
    /// <summary>
    /// Service interface for participant business logic operations.
    /// </summary>
    public interface IParticipantService
    {
        Task<IEnumerable<Participant>> GetAllParticipantsAsync();
        Task<Participant?> GetParticipantByIdAsync(int id);
        Task<Participant?> GetParticipantWithRegistrationsAsync(int id);
        Task<Participant> CreateParticipantAsync(string firstName, string lastName, string email, string? phoneNumber);
        Task UpdateParticipantAsync(int id, string firstName, string lastName, string email, string? phoneNumber);
        Task DeleteParticipantAsync(int id);
        Task<bool> EmailExistsAsync(string email);
    }
}