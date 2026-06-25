using CommunityEventManagement.Domain.Entities;

namespace CommunityEventManagement.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Participant-specific repository interface.
    /// Extends generic IRepository with participant-specific queries.
    /// </summary>
    public interface IParticipantRepository : IRepository<Participant>
    {
        /// <summary>Returns participant with all their registrations included.</summary>
        Task<Participant?> GetWithRegistrationsAsync(int id);

        /// <summary>Finds a participant by their email address.</summary>
        Task<Participant?> GetByEmailAsync(string email);

        /// <summary>Checks if an email address is already registered.</summary>
        Task<bool> EmailExistsAsync(string email);
        
        /// <summary>
        /// Gets any AppUser (Participant or Administrator) by email.
        /// Used for authentication - resolves TPH inheritance.
        /// </summary>
        Task<Entities.AppUser?> GetUserByEmailAsync(string email);

    }
}