// FILE: CommunityEventManagement.Domain/Interfaces/Services/IParticipantService.cs
// REPLACE ENTIRE FILE

using CommunityEventManagement.Domain.Entities;

namespace CommunityEventManagement.Domain.Interfaces.Services
{
    public interface IParticipantService
    {
        Task<IEnumerable<Participant>> GetAllParticipantsAsync();
        Task<Participant?> GetParticipantByIdAsync(int id);
        Task<Participant?> GetParticipantWithRegistrationsAsync(int id);

        // Updated: now accepts password (plain text, service hashes it)
        // password parameter is optional for backward compatibility – defaults to "Password@1"
        Task<Participant> CreateParticipantAsync(
            string firstName, 
            string lastName, 
            string email, 
            string? phoneNumber,
            string password = "Password@1");

        Task UpdateParticipantAsync(int id, string firstName, string lastName, string email, string? phoneNumber);
        Task DeleteParticipantAsync(int id);
        Task<bool> EmailExistsAsync(string email);

        Task<Participant?> SearchParticipantByIdAsync(int participantId);
    }
}
