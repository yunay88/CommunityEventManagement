using CommunityEventManagement.Domain.Entities;

namespace CommunityEventManagement.Domain.Interfaces.Services
{
    /// <summary>
    /// Service interface for venue business logic operations.
    /// </summary>
    public interface IVenueService
    {
        Task<IEnumerable<Venue>> GetAllVenuesAsync();
        Task<Venue?> GetVenueByIdAsync(int id);
        Task<Venue?> GetVenueWithEventsAsync(int id);
        Task<Venue> CreateVenueAsync(string name, string address, string city, int maxCapacity);
        Task UpdateVenueAsync(int id, string name, string address, string city, int maxCapacity);
        Task DeleteVenueAsync(int id);
    }
}