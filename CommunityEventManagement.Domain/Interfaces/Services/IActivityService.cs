using CommunityEventManagement.Domain.Entities;
using CommunityEventManagement.Domain.Enums;

namespace CommunityEventManagement.Domain.Interfaces.Services
{
    /// <summary>
    /// Service interface for activity business logic operations.
    /// </summary>
    public interface IActivityService
    {
        Task<IEnumerable<Activity>> GetAllActivitiesAsync();
        Task<Activity?> GetActivityByIdAsync(int id);
        Task<IEnumerable<Activity>> GetActivitiesByTypeAsync(ActivityType type);
        Task<Activity> CreateActivityAsync(string name, string description, ActivityType type, int durationMinutes);
        Task UpdateActivityAsync(int id, string name, string description, ActivityType type, int durationMinutes);
        Task DeleteActivityAsync(int id);
    }
}