using CommunityEventManagement.Domain.Entities;
using CommunityEventManagement.Domain.Exceptions;
using CommunityEventManagement.Domain.Interfaces.Services;
using CommunityEventManagement.Infrastructure.Patterns.Factory;
using Microsoft.Extensions.Logging;

namespace CommunityEventManagement.Infrastructure.Patterns.Facade
{
    /// <summary>
    /// Result object returned by facade operations.
    /// Carries the created entity and any messages.
    /// </summary>
    public class FacadeResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();

        public static FacadeResult<T> Ok(T data, string message = "")
            => new() { Success = true, Data = data, Message = message };

        public static FacadeResult<T> Fail(string error)
            => new() { Success = false, Errors = new List<string> { error } };

        public static FacadeResult<T> Fail(List<string> errors)
            => new() { Success = false, Errors = errors };
    }

    /// <summary>
    /// Event Management Facade — Facade design pattern (Structural).
    ///
    /// Provides a SIMPLIFIED interface to the complex subsystem of:
    ///   - EventService
    ///   - ActivityService
    ///   - VenueService
    ///   - RegistrationService
    ///   - EventFactory
    ///   - ParticipantFactory
    ///
    /// Blazor components interact with this Facade instead of
    /// multiple services directly — reduces coupling and complexity.
    ///
    /// Demonstrates:
    ///   - Facade design pattern (Structural)
    ///   - Dependency injection of multiple services
    ///   - Simplified API over complex subsystem
    ///   - Coordinated multi-service operations
    /// </summary>
    public class EventManagementFacade
    {
        private readonly IEventService _eventService;
        private readonly IActivityService _activityService;
        private readonly IVenueService _venueService;
        private readonly IRegistrationService _registrationService;
        private readonly IParticipantService _participantService;
        private readonly EventFactory _eventFactory;
        private readonly ParticipantFactory _participantFactory;
        private readonly ILogger<EventManagementFacade> _logger;

        // Constructor injection — Facade receives all subsystem dependencies
        public EventManagementFacade(
            IEventService eventService,
            IActivityService activityService,
            IVenueService venueService,
            IRegistrationService registrationService,
            IParticipantService participantService,
            EventFactory eventFactory,
            ParticipantFactory participantFactory,
            ILogger<EventManagementFacade> logger)
        {
            _eventService = eventService;
            _activityService = activityService;
            _venueService = venueService;
            _registrationService = registrationService;
            _participantService = participantService;
            _eventFactory = eventFactory;
            _participantFactory = participantFactory;
            _logger = logger;
        }

        /// <summary>
        /// Creates an event and optionally assigns activities.
        /// Facade simplifies what would be multiple service calls.
        ///
        /// Without Facade: component calls EventService + ActivityService separately
        /// With Facade: one call handles everything
        /// </summary>
        public async Task<FacadeResult<Event>> CreateEventWithActivitiesAsync(
            CreateEventRequest request)
        {
            try
            {
                _logger.LogInformation(
                    "Facade: Creating event with activities. Name: {Name}", request.Name);

                // Step 1: Validate using Factory
                var validationErrors = _eventFactory.Validate(request).ToList();
                if (validationErrors.Any())
                    return FacadeResult<Event>.Fail(validationErrors);

                // Step 2: Create the event via service
                var eventEntity = await _eventService.CreateEventAsync(
                    request.Name,
                    request.Description,
                    request.StartDate,
                    request.EndDate,
                    request.VenueId);

                // Step 3: Add activities if provided
                if (request.ActivityIds.Any())
                {
                    foreach (var activityId in request.ActivityIds)
                    {
                        try
                        {
                            await _eventService.AddActivityToEventAsync(
                                eventEntity.Id, activityId);
                        }
                        catch (Exception ex)
                        {
                            // Log but don't fail the whole operation
                            _logger.LogWarning(ex,
                                "Could not add activity {ActivityId} to event {EventId}",
                                activityId, eventEntity.Id);
                        }
                    }
                }

                _logger.LogInformation(
                    "Facade: Event created successfully. ID: {EventId}", eventEntity.Id);

                return FacadeResult<Event>.Ok(eventEntity,
                    $"Event '{eventEntity.Name}' created successfully.");
            }
            catch (CommunityEventException ex)
            {
                _logger.LogError(ex, "Facade: Domain error creating event");
                return FacadeResult<Event>.Fail(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Facade: Unexpected error creating event");
                return FacadeResult<Event>.Fail(
                    "An unexpected error occurred while creating the event.");
            }
        }

        /// <summary>
        /// Registers a participant for an event.
        /// Handles all validation, duplicate checking, capacity in one call.
        /// </summary>
        public async Task<FacadeResult<Registration>> RegisterParticipantForEventAsync(
            int participantId,
            int eventId,
            string? notes = null)
        {
            try
            {
                _logger.LogInformation(
                    "Facade: Registering participant {ParticipantId} for event {EventId}",
                    participantId, eventId);

                var registration = await _registrationService
                    .RegisterParticipantAsync(participantId, eventId, notes);

                var message = registration.IsConfirmed()
                    ? "Registration confirmed successfully!"
                    : "Added to the waiting list.";

                return FacadeResult<Registration>.Ok(registration, message);
            }
            catch (DuplicateRegistrationException ex)
            {
                return FacadeResult<Registration>.Fail(ex.Message);
            }
            catch (RegistrationException ex)
            {
                return FacadeResult<Registration>.Fail(ex.Message);
            }
            catch (VenueCapacityException ex)
            {
                return FacadeResult<Registration>.Fail(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Facade: Error registering participant");
                return FacadeResult<Registration>.Fail(
                    "An unexpected error occurred during registration.");
            }
        }

        /// <summary>
        /// Creates a new participant account.
        /// Uses ParticipantFactory for creation + validates duplicate email.
        /// </summary>
        public async Task<FacadeResult<Participant>> CreateParticipantAsync(
            CreateParticipantRequest request)
        {
            try
            {
                _logger.LogInformation(
                    "Facade: Creating participant. Email: {Email}", request.Email);

                // Validate via factory
                var errors = _participantFactory.Validate(request).ToList();
                if (errors.Any())
                    return FacadeResult<Participant>.Fail(errors);

                // Check duplicate email
                var emailExists = await _participantService.EmailExistsAsync(request.Email);
                if (emailExists)
                    return FacadeResult<Participant>.Fail(
                        $"A participant with email '{request.Email}' already exists.");

                // Create via service
                var participant = await _participantService.CreateParticipantAsync(
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.PhoneNumber);

                return FacadeResult<Participant>.Ok(participant,
                    $"Participant '{participant.GetFullName()}' created successfully.");
            }
            catch (CommunityEventException ex)
            {
                return FacadeResult<Participant>.Fail(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Facade: Error creating participant");
                return FacadeResult<Participant>.Fail(
                    "An unexpected error occurred while creating the participant.");
            }
        }

        /// <summary>
        /// Gets a complete event dashboard summary.
        /// Aggregates data from multiple services in one call.
        /// This is the Facade pattern at its clearest —
        /// ONE call replaces FOUR separate service calls.
        /// </summary>
        public async Task<EventDashboardSummary> GetDashboardSummaryAsync()
        {
            try
            {
                var upcomingEvents = await _eventService.GetUpcomingEventsAsync();
                var allParticipants = await _participantService.GetAllParticipantsAsync();
                var allVenues = await _venueService.GetAllVenuesAsync();
                var allRegistrations = await _registrationService.GetAllRegistrationsAsync();

                return new EventDashboardSummary
                {
                    TotalUpcomingEvents = upcomingEvents.Count(),
                    TotalParticipants = allParticipants.Count(),
                    TotalVenues = allVenues.Count(),
                    TotalRegistrations = allRegistrations.Count(),
                    RecentEvents = upcomingEvents.Take(3).ToList(),
                    GeneratedAt = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Facade: Error generating dashboard summary");
                return new EventDashboardSummary { GeneratedAt = DateTime.Now };
            }
        }
    }

    /// <summary>
    /// Dashboard summary data object.
    /// Aggregated by the Facade from multiple services.
    /// </summary>
    public class EventDashboardSummary
    {
        public int TotalUpcomingEvents { get; set; }
        public int TotalParticipants { get; set; }
        public int TotalVenues { get; set; }
        public int TotalRegistrations { get; set; }
        public List<Event> RecentEvents { get; set; } = new();
        public DateTime GeneratedAt { get; set; }
    }
}