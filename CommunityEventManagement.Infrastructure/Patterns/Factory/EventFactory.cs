using CommunityEventManagement.Domain.Entities;
using CommunityEventManagement.Domain.Exceptions;

namespace CommunityEventManagement.Infrastructure.Patterns.Factory
{
    /// <summary>
    /// Request object carrying all data needed to create an Event.
    /// Separates data from creation logic.
    /// </summary>
    public class CreateEventRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? VenueId { get; set; }
        public List<int> ActivityIds { get; set; } = new();
    }

    /// <summary>
    /// Event Factory — encapsulates Event creation logic.
    ///
    /// Factory Pattern (Creational):
    ///   - Client code calls EventFactory.Create() instead of new Event()
    ///   - Factory validates all inputs before creating
    ///   - Factory ensures Event is always in a valid state
    ///   - If requirements change, only the factory changes
    ///
    /// Demonstrates:
    ///   - Factory design pattern
    ///   - Separation of creation logic from business logic
    ///   - Validation before object creation
    ///   - Interface implementation
    /// </summary>
    public class EventFactory : IEntityFactory<Event, CreateEventRequest>
    {
        /// <summary>
        /// Creates a new Event from a validated request.
        /// Throws EventValidationException if request is invalid.
        /// </summary>
        public Event Create(CreateEventRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // Validate first — collect all errors
            var errors = Validate(request).ToList();
            if (errors.Any())
                throw new EventValidationException(
                    $"Cannot create event. Validation errors: {string.Join(", ", errors)}");

            // Create the event — constructor does final validation
            return new Event(
                request.Name.Trim(),
                request.Description.Trim(),
                request.StartDate,
                request.EndDate,
                request.VenueId);
        }

        /// <summary>
        /// Validates a CreateEventRequest.
        /// Returns empty list if valid, list of error messages if invalid.
        /// </summary>
        public IEnumerable<string> Validate(CreateEventRequest request)
        {
            if (request == null)
            {
                yield return "Request cannot be null.";
                yield break;
            }

            if (string.IsNullOrWhiteSpace(request.Name))
                yield return "Event name is required.";

            if (request.Name?.Length < 3)
                yield return "Event name must be at least 3 characters.";

            if (request.Name?.Length > 100)
                yield return "Event name cannot exceed 100 characters.";

            if (request.StartDate == default)
                yield return "Start date is required.";

            if (request.EndDate == default)
                yield return "End date is required.";

            if (request.StartDate >= request.EndDate)
                yield return "End date must be after start date.";

            if (request.StartDate < DateTime.Now.AddMinutes(-5))
                yield return "Start date cannot be in the past.";

            if (!string.IsNullOrWhiteSpace(request.Description) &&
                request.Description.Length > 500)
                yield return "Description cannot exceed 500 characters.";
        }

        /// <summary>
        /// Creates multiple events from a list of requests.
        /// Demonstrates: factory working with collections.
        /// </summary>
        public IEnumerable<Event> CreateMany(IEnumerable<CreateEventRequest> requests)
        {
            if (requests == null)
                throw new ArgumentNullException(nameof(requests));

            return requests.Select(Create).ToList();
        }

        /// <summary>
        /// Creates an event with default values for quick testing/demo.
        /// Demonstrates: factory method overloading.
        /// </summary>
        public Event CreateDefault(string name)
        {
            var request = new CreateEventRequest
            {
                Name = name,
                Description = "A community event",
                StartDate = DateTime.Now.AddDays(7),
                EndDate = DateTime.Now.AddDays(7).AddHours(3),
                VenueId = null
            };

            return Create(request);
        }
    }
}