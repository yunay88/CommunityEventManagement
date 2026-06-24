using CommunityEventManagement.Domain.Algorithms;
using CommunityEventManagement.Domain.Entities;
using CommunityEventManagement.Domain.Enums;
using CommunityEventManagement.Domain.Exceptions;
using CommunityEventManagement.Domain.Interfaces;
using CommunityEventManagement.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CommunityEventManagement.Infrastructure.Services
{
    /// <summary>
    /// Activity service — business logic for activity management.
    /// Demonstrates:
    ///   - Bubble Sort algorithm usage
    ///   - try-catch-finally pattern
    /// </summary>
    public class ActivityService : IActivityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ActivityService> _logger;

        public ActivityService(IUnitOfWork unitOfWork, ILogger<ActivityService> logger)
        {
            _unitOfWork = unitOfWork
                ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Activity>> GetAllActivitiesAsync()
        {
            try
            {
                return await _unitOfWork.Activities.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving activities");
                throw new CommunityEventException(
                    "Failed to retrieve activities.", "RETRIEVAL_ERROR", ex);
            }
            finally
            {
                _logger.LogDebug("GetAllActivitiesAsync completed");
            }
        }

        public async Task<Activity?> GetActivityByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(
                    "Activity ID must be a positive number.", nameof(id));

            try
            {
                return await _unitOfWork.Activities.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving activity {Id}", id);
                throw new CommunityEventException(
                    $"Failed to retrieve activity {id}.", "RETRIEVAL_ERROR", ex);
            }
        }

        public async Task<IEnumerable<Activity>> GetActivitiesByTypeAsync(ActivityType type)
        {
            try
            {
                return await _unitOfWork.Activities.GetByTypeAsync(type);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving activities by type {Type}", type);
                throw new CommunityEventException(
                    $"Failed to retrieve activities of type {type}.",
                    "RETRIEVAL_ERROR", ex);
            }
        }

        public async Task<Activity> CreateActivityAsync(
            string name,
            string description,
            ActivityType type,
            int durationMinutes)
        {
            try
            {
                _logger.LogInformation("Creating activity: {Name}", name);

                var activity = new Activity(name, description, type, durationMinutes);

                await _unitOfWork.Activities.AddAsync(activity);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation(
                    "Activity created successfully. ID: {Id}", activity.Id);

                return activity;
            }
            catch (CommunityEventException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating activity: {Name}", name);
                throw new CommunityEventException(
                    $"Failed to create activity '{name}'.", "CREATE_ERROR", ex);
            }
            finally
            {
                _logger.LogDebug("CreateActivityAsync completed for: {Name}", name);
            }
        }

        public async Task UpdateActivityAsync(
            int id,
            string name,
            string description,
            ActivityType type,
            int durationMinutes)
        {
            try
            {
                _logger.LogInformation("Updating activity {Id}", id);

                var activity = await _unitOfWork.Activities.GetByIdAsync(id)
                    ?? throw new ActivityNotFoundException(id);

                activity.UpdateDetails(name, description, type, durationMinutes);

                await _unitOfWork.Activities.UpdateAsync(activity);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Activity {Id} updated successfully", id);
            }
            catch (CommunityEventException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating activity {Id}", id);
                throw new CommunityEventException(
                    $"Failed to update activity {id}.", "UPDATE_ERROR", ex);
            }
            finally
            {
                _logger.LogDebug("UpdateActivityAsync completed for {Id}", id);
            }
        }

        public async Task DeleteActivityAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting activity {Id}", id);

                var exists = await _unitOfWork.Activities.ExistsAsync(id);
                if (!exists)
                    throw new ActivityNotFoundException(id);

                await _unitOfWork.Activities.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Activity {Id} deleted successfully", id);
            }
            catch (CommunityEventException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting activity {Id}", id);
                throw new CommunityEventException(
                    $"Failed to delete activity {id}.", "DELETE_ERROR", ex);
            }
            finally
            {
                _logger.LogDebug("DeleteActivityAsync completed for {Id}", id);
            }
        }

        /// <summary>
        /// Returns activities sorted by name using Bubble Sort.
        /// Demonstrates: Bubble Sort algorithm in service layer.
        /// </summary>
        public async Task<IEnumerable<Activity>> GetActivitiesSortedByNameAsync()
        {
            try
            {
                var activities = (await _unitOfWork.Activities.GetAllAsync()).ToList();
                SortAlgorithms.BubbleSort(activities, a => a.Name);
                return activities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sorted activities");
                throw new CommunityEventException(
                    "Failed to retrieve sorted activities.", "RETRIEVAL_ERROR", ex);
            }
        }
    }
}