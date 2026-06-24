using CommunityEventManagement.Domain.Entities;

namespace CommunityEventManagement.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Venue-specific repository interface.
    /// Extends generic IRepository with venue-specific queries.
    /// </summary>
    public interface IVenueRepository : IRepository<Venue>
    {
        /// <summary>Returns venue with all its associated events.</summary>
        Task<Venue?> GetWithEventsAsync(int id);

        /// <summary>Returns venues that currently have available space.</summary>
        Task<IEnumerable<Venue>> GetAvailableVenuesAsync();
    }
}