using CommunityEventManagement.Domain.Entities;
using CommunityEventManagement.Domain.Exceptions;
using CommunityEventManagement.Domain.Interfaces;
using CommunityEventManagement.Domain.Interfaces.Services;
using CommunityEventManagement.Infrastructure.DataStructures;
using Microsoft.Extensions.Logging;

namespace CommunityEventManagement.Infrastructure.Services
{
    /// <summary>
    /// Registration service — most complex business logic.
    /// Handles the full registration workflow.
    ///
    /// Demonstrates:
    ///   - Transaction management (begin/commit/rollback)
    ///   - Multiple business rule validations
    ///   - Queue data structure for waitlisting
    ///   - Custom exceptions for each failure scenario
    ///   - try-catch-finally pattern
    /// </summary>
    public class RegistrationService : IRegistrationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RegistrationService> _logger;

        // Queue data structure — manages waiting lists per event
        private readonly Dictionary<int, RegistrationQueue> _waitingLists = new();

        public RegistrationService(
            IUnitOfWork unitOfWork,
            ILogger<RegistrationService> logger)
        {
            _unitOfWork = unitOfWork
                ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Registration>> GetAllRegistrationsAsync()
        {
            try
            {
                return await _unitOfWork.Registrations.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all registrations");
                throw new CommunityEventException(
                    "Failed to retrieve registrations.", "RETRIEVAL_ERROR", ex);
            }
            finally
            {
                _logger.LogDebug("GetAllRegistrationsAsync completed");
            }
        }

        public async Task<Registration?> GetRegistrationByIdAsync(int id)
        {
            try
            {
                return await _unitOfWork.Registrations.GetWithDetailsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving registration {Id}", id);
                throw new CommunityEventException(
                    $"Failed to retrieve registration {id}.", "RETRIEVAL_ERROR", ex);
            }
        }

        public async Task<IEnumerable<Registration>> GetParticipantRegistrationsAsync(
            int participantId)
        {
            try
            {
                return await _unitOfWork.Registrations.GetByParticipantAsync(participantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error retrieving registrations for participant {Id}", participantId);
                throw new CommunityEventException(
                    $"Failed to retrieve registrations for participant {participantId}.",
                    "RETRIEVAL_ERROR", ex);
            }
        }

        public async Task<IEnumerable<Registration>> GetEventRegistrationsAsync(int eventId)
        {
            try
            {
                return await _unitOfWork.Registrations.GetByEventAsync(eventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error retrieving registrations for event {Id}", eventId);
                throw new CommunityEventException(
                    $"Failed to retrieve registrations for event {eventId}.",
                    "RETRIEVAL_ERROR", ex);
            }
        }

        public async Task<Registration> RegisterParticipantAsync(
            int participantId,
            int eventId,
            string? notes = null)
        {
            _logger.LogInformation(
                "Registering participant {ParticipantId} for event {EventId}",
                participantId, eventId);

            // Begin transaction — all or nothing
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // ── Validation 1: Participant exists ──────────────────
                var participant = await _unitOfWork.Participants.GetByIdAsync(participantId)
                    ?? throw new ParticipantNotFoundException(participantId);

                // ── Validation 2: Event exists ────────────────────────
                var eventEntity = await _unitOfWork.Events.GetWithDetailsAsync(eventId)
                    ?? throw new EventNotFoundException(eventId);

                // ── Validation 3: Event is not in the past ────────────
                if (eventEntity.IsPast())
                    throw new RegistrationException(
                        $"Cannot register for past event '{eventEntity.Name}'.");

                // ── Validation 4: No duplicate registration ───────────
                var isDuplicate = await _unitOfWork.Registrations
                    .ExistsAsync(participantId, eventId);
                if (isDuplicate)
                    throw new DuplicateRegistrationException(participantId, eventId);

                // ── Validation 5: Venue capacity check ────────────────
                if (eventEntity.Venue != null &&
                    !eventEntity.Venue.HasAvailableSpace())
                {
                    // Add to waiting list queue instead
                    var registration = new Registration(eventId, participantId, notes);
                    registration.Waitlist();

                    await AddToWaitingListAsync(eventEntity, registration);

                    await _unitOfWork.Registrations.AddAsync(registration);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();

                    _logger.LogInformation(
                        "Participant {ParticipantId} added to waiting list for event {EventId}",
                        participantId, eventId);

                    return registration;
                }

                // ── Create confirmed registration ──────────────────────
                var confirmedRegistration = new Registration(eventId, participantId, notes);
                confirmedRegistration.Confirm();

                await _unitOfWork.Registrations.AddAsync(confirmedRegistration);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation(
                    "Registration confirmed. ID: {RegistrationId}",
                    confirmedRegistration.Id);

                return confirmedRegistration;
            }
            catch (CommunityEventException)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex,
                    "Unexpected error registering participant {ParticipantId} " +
                    "for event {EventId}", participantId, eventId);
                throw new CommunityEventException(
                    "An unexpected error occurred during registration.",
                    "REGISTRATION_ERROR", ex);
            }
            finally
            {
                _logger.LogDebug(
                    "RegisterParticipantAsync completed for participant {ParticipantId}",
                    participantId);
            }
        }

        public async Task CancelRegistrationAsync(int registrationId, string? reason = null)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Cancelling registration {Id}", registrationId);

                var registration = await _unitOfWork.Registrations
                    .GetWithDetailsAsync(registrationId)
                    ?? throw new RegistrationException(
                        $"Registration {registrationId} was not found.");

                // Throws RegistrationException if already cancelled
                registration.Cancel(reason);

                await _unitOfWork.Registrations.UpdateAsync(registration);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Process next person from waiting list
                await ProcessNextFromWaitingListAsync(registration.EventId);

                _logger.LogInformation("Registration {Id} cancelled successfully", registrationId);
            }
            catch (CommunityEventException)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error cancelling registration {Id}", registrationId);
                throw new CommunityEventException(
                    $"Failed to cancel registration {registrationId}.",
                    "CANCEL_ERROR", ex);
            }
            finally
            {
                _logger.LogDebug(
                    "CancelRegistrationAsync completed for {Id}", registrationId);
            }
        }

        public async Task ConfirmRegistrationAsync(int registrationId)
        {
            try
            {
                var registration = await _unitOfWork.Registrations
                    .GetByIdAsync(registrationId)
                    ?? throw new RegistrationException(
                        $"Registration {registrationId} was not found.");

                registration.Confirm();

                await _unitOfWork.Registrations.UpdateAsync(registration);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation(
                    "Registration {Id} confirmed successfully", registrationId);
            }
            catch (CommunityEventException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming registration {Id}", registrationId);
                throw new CommunityEventException(
                    $"Failed to confirm registration {registrationId}.",
                    "CONFIRM_ERROR", ex);
            }
        }

        public async Task<bool> IsParticipantRegisteredAsync(int participantId, int eventId)
        {
            try
            {
                return await _unitOfWork.Registrations
                    .ExistsAsync(participantId, eventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error checking registration for participant {ParticipantId} " +
                    "event {EventId}", participantId, eventId);
                return false;
            }
        }

        // ── Private helpers ───────────────────────────────────────────

        private Task AddToWaitingListAsync(Event eventEntity, Registration registration)
        {
            if (!_waitingLists.ContainsKey(eventEntity.Id))
            {
                _waitingLists[eventEntity.Id] = new RegistrationQueue(
                    eventEntity.Id, eventEntity.Name);
            }

            _waitingLists[eventEntity.Id].AddToWaitingList(registration);
            return Task.CompletedTask;
        }

        private async Task ProcessNextFromWaitingListAsync(int eventId)
        {
            if (!_waitingLists.ContainsKey(eventId)) return;

            var queue = _waitingLists[eventId];
            if (!queue.HasWaiting) return;

            var next = queue.ProcessNext();
            if (next != null)
            {
                next.Confirm();
                await _unitOfWork.Registrations.UpdateAsync(next);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation(
                    "Promoted participant {ParticipantId} from waiting list for event {EventId}",
                    next.ParticipantId, eventId);
            }
        }
    }
}