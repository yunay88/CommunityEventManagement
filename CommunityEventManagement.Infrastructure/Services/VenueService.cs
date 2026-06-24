using CommunityEventManagement.Domain.Entities;
using CommunityEventManagement.Domain.Exceptions;
using CommunityEventManagement.Domain.Interfaces;
using CommunityEventManagement.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CommunityEventManagement.Infrastructure.Services
{
    /// <summary>
    /// Venue service — business logic for venue management.
    /// Demonstrates:
    ///   - Capacity validation (business rule)
    ///   - try-catch-finally pattern
    /// </summary>
    public class VenueService : IVenueService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<VenueService> _logger;

        public VenueService(IUnitOfWork unitOfWork, ILogger<VenueService> logger)
        {
            _unitOfWork = unitOfWork
                ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Venue>> GetAllVenuesAsync()
        {
            try
            {
                return await _unitOfWork.Venues.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving venues");
                throw new CommunityEventException(
                    "Failed to retrieve venues.", "RETRIEVAL_ERROR", ex);
            }
            finally
            {
                _logger.LogDebug("GetAllVenuesAsync completed");
            }
        }

        public async Task<Venue?> GetVenueByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(
                    "Venue ID must be a positive number.", nameof(id));

            try
            {
                return await _unitOfWork.Venues.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving venue {Id}", id);
                throw new CommunityEventException(
                    $"Failed to retrieve venue {id}.", "RETRIEVAL_ERROR", ex);
            }
        }

        public async Task<Venue?> GetVenueWithEventsAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(
                    "Venue ID must be a positive number.", nameof(id));

            try
            {
                return await _unitOfWork.Venues.GetWithEventsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving venue with events {Id}", id);
                throw new CommunityEventException(
                    $"Failed to retrieve venue {id}.", "RETRIEVAL_ERROR", ex);
            }
        }

        public async Task<Venue> CreateVenueAsync(
            string name,
            string address,
            string city,
            int maxCapacity)
        {
            try
            {
                _logger.LogInformation("Creating venue: {VenueName}", name);

                // Capacity validation — Venue constructor also validates
                // but we validate here for clear error messages
                if (maxCapacity <= 0)
                    throw new VenueCapacityException(
                        "Venue capacity must be greater than zero.");

                var venue = new Venue(name, address, city, maxCapacity);

                await _unitOfWork.Venues.AddAsync(venue);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation(
                    "Venue created successfully. ID: {VenueId}", venue.Id);

                return venue;
            }
            catch (CommunityEventException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating venue: {VenueName}", name);
                throw new CommunityEventException(
                    $"Failed to create venue '{name}'.", "CREATE_ERROR", ex);
            }
            finally
            {
                _logger.LogDebug("CreateVenueAsync completed for: {VenueName}", name);
            }
        }

        public async Task UpdateVenueAsync(
            int id,
            string name,
            string address,
            string city,
            int maxCapacity)
        {
            try
            {
                _logger.LogInformation("Updating venue {Id}", id);

                var venue = await _unitOfWork.Venues.GetByIdAsync(id)
                    ?? throw new VenueNotFoundException(id);

                if (maxCapacity <= 0)
                    throw new VenueCapacityException(
                        "Venue capacity must be greater than zero.");

                venue.UpdateDetails(name, address, city, maxCapacity);

                await _unitOfWork.Venues.UpdateAsync(venue);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Venue {Id} updated successfully", id);
            }
            catch (CommunityEventException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating venue {Id}", id);
                throw new CommunityEventException(
                    $"Failed to update venue {id}.", "UPDATE_ERROR", ex);
            }
            finally
            {
                _logger.LogDebug("UpdateVenueAsync completed for {Id}", id);
            }
        }

        public async Task DeleteVenueAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting venue {Id}", id);

                var exists = await _unitOfWork.Venues.ExistsAsync(id);
                if (!exists)
                    throw new VenueNotFoundException(id);

                await _unitOfWork.Venues.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Venue {Id} deleted successfully", id);
            }
            catch (CommunityEventException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting venue {Id}", id);
                throw new CommunityEventException(
                    $"Failed to delete venue {id}.", "DELETE_ERROR", ex);
            }
            finally
            {
                _logger.LogDebug("DeleteVenueAsync completed for {Id}", id);
            }
        }
    }
}