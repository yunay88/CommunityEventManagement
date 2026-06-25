// FILE: CommunityEventManagement.Infrastructure/Services/ParticipantService.cs
// REPLACE ENTIRE CreateParticipantAsync method only - lines ~88-133
// Full file replacement for safety:

using CommunityEventManagement.Domain.Algorithms;
using CommunityEventManagement.Domain.Entities;
using CommunityEventManagement.Domain.Exceptions;
using CommunityEventManagement.Domain.Interfaces;
using CommunityEventManagement.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CommunityEventManagement.Infrastructure.Services
{
    /// <summary>
    /// Participant service — business logic for participant management.
    /// Demonstrates:
    ///   - Duplicate email validation (server-side)
    ///   - Binary Search for participant lookup
    ///   - try-catch-finally pattern
    /// </summary>
    public class ParticipantService : IParticipantService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ParticipantService> _logger;

        public ParticipantService(
            IUnitOfWork unitOfWork,
            ILogger<ParticipantService> logger)
        {
            _unitOfWork = unitOfWork
                ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Participant>> GetAllParticipantsAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all participants");
                return await _unitOfWork.Participants.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving participants");
                throw new CommunityEventException(
                    "Failed to retrieve participants.", "RETRIEVAL_ERROR", ex);
            }
            finally
            {
                _logger.LogDebug("GetAllParticipantsAsync completed");
            }
        }

        public async Task<Participant?> GetParticipantByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(
                    "Participant ID must be a positive number.", nameof(id));

            try
            {
                return await _unitOfWork.Participants.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving participant {Id}", id);
                throw new CommunityEventException(
                    $"Failed to retrieve participant {id}.", "RETRIEVAL_ERROR", ex);
            }
        }

        public async Task<Participant?> GetParticipantWithRegistrationsAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException(
                    "Participant ID must be a positive number.", nameof(id));

            try
            {
                return await _unitOfWork.Participants.GetWithRegistrationsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error retrieving participant with registrations {Id}", id);
                throw new CommunityEventException(
                    $"Failed to retrieve participant {id}.", "RETRIEVAL_ERROR", ex);
            }
        }

        public async Task<Participant> CreateParticipantAsync(
            string firstName,
            string lastName,
            string email,
            string? phoneNumber,
            string password = "Password@1")
        {
            try
            {
                _logger.LogInformation("Creating participant: {Email}", email);

                // Server-side duplicate email validation
                var emailExists = await _unitOfWork.Participants.EmailExistsAsync(email);
                if (emailExists)
                    throw new RegistrationException(
                        $"A participant with email '{email}' already exists.");

                // Hash password the same way SeedData does
                // SeedData.BCryptHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(password))
                var passwordHash = Convert.ToBase64String(
                    System.Text.Encoding.UTF8.GetBytes(password));

                var participant = new Participant(
                    firstName, lastName, email, passwordHash, phoneNumber);

                await _unitOfWork.Participants.AddAsync(participant);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation(
                    "Participant created. ID: {ParticipantId}", participant.Id);

                return participant;
            }
            catch (CommunityEventException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating participant: {Email}", email);
                throw new CommunityEventException(
                    "Failed to create participant.", "CREATE_ERROR", ex);
            }
            finally
            {
                _logger.LogDebug("CreateParticipantAsync completed");
            }
        }

        public async Task UpdateParticipantAsync(
            int id,
            string firstName,
            string lastName,
            string email,
            string? phoneNumber)
        {
            try
            {
                _logger.LogInformation("Updating participant {Id}", id);

                var participant = await _unitOfWork.Participants.GetByIdAsync(id)
                    ?? throw new ParticipantNotFoundException(id);

                // Check if new email belongs to a DIFFERENT participant
                var existingWithEmail = await _unitOfWork.Participants
                    .GetByEmailAsync(email);

                if (existingWithEmail != null && existingWithEmail.Id != id)
                    throw new RegistrationException(
                        $"Email '{email}' is already used by another participant.");

                participant.UpdateDetails(firstName, lastName, email, phoneNumber);

                await _unitOfWork.Participants.UpdateAsync(participant);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Participant {Id} updated successfully", id);
            }
            catch (CommunityEventException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating participant {Id}", id);
                throw new CommunityEventException(
                    $"Failed to update participant {id}.", "UPDATE_ERROR", ex);
            }
            finally
            {
                _logger.LogDebug("UpdateParticipantAsync completed for {Id}", id);
            }
        }

        public async Task DeleteParticipantAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting participant {Id}", id);

                var exists = await _unitOfWork.Participants.ExistsAsync(id);
                if (!exists)
                    throw new ParticipantNotFoundException(id);

                await _unitOfWork.Participants.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Participant {Id} deleted successfully", id);
            }
            catch (CommunityEventException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting participant {Id}", id);
                throw new CommunityEventException(
                    $"Failed to delete participant {id}.", "DELETE_ERROR", ex);
            }
            finally
            {
                _logger.LogDebug("DeleteParticipantAsync completed for {Id}", id);
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _unitOfWork.Participants.EmailExistsAsync(email);
        }

        /// <summary>
        /// Searches for a participant by ID using Binary Search.
        /// Demonstrates: algorithm usage in business logic layer.
        /// </summary>
        public async Task<Participant?> SearchParticipantByIdAsync(int participantId)
        {
            try
            {
                var all = (await _unitOfWork.Participants.GetAllAsync()).ToList();

                // Sort by ID — required for Binary Search
                SortAlgorithms.InsertionSort(all, p => p.Id);

                // Binary Search on sorted list
                int index = SearchAlgorithms.BinarySearch(all, participantId, p => p.Id);

                return index >= 0 ? all[index] : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error searching for participant {Id}", participantId);
                throw new CommunityEventException(
                    $"Failed to search for participant {participantId}.",
                    "SEARCH_ERROR", ex);
            }
        }
    }
}
