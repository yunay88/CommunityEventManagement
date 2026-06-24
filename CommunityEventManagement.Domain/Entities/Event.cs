using System.ComponentModel.DataAnnotations;
using CommunityEventManagement.Domain.Exceptions;
using CommunityEventManagement.Domain.Interfaces.Domain;
using CommunityEventManagement.Domain.Models;

namespace CommunityEventManagement.Domain.Entities
{
    /// <summary>
    /// Event entity — the core domain object of the system.
    /// Inheritance: Event → BaseEntity
    /// Implements: ISchedulable, IFilterable (multiple interfaces)
    /// 
    /// Demonstrates:
    ///   - Multiple interface implementation
    ///   - Method overloading: IsActive() has two versions
    ///   - Polymorphism: GetDisplayName() and GetSummary() override BaseEntity
    ///   - Encapsulation: private collections, controlled mutation methods
    ///   - Validation in constructor with custom exception
    /// </summary>
    public class Event : BaseEntity, ISchedulable, IFilterable
    {
        [Required(ErrorMessage = "Event name is required")]
        [StringLength(100, MinimumLength = 3,
            ErrorMessage = "Event name must be between 3 and 100 characters")]
        public string Name { get; private set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; private set; } = string.Empty;

        // ISchedulable properties
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        // Optional venue foreign key
        public int? VenueId { get; private set; }
        public Venue? Venue { get; private set; }

        // Navigation properties — encapsulated collections
        private readonly List<Registration> _registrations = new();
        public IReadOnlyCollection<Registration> Registrations => _registrations.AsReadOnly();
        public virtual ICollection<Registration> RegistrationsCollection
        {
            get => _registrations;
            set { _registrations.Clear(); if (value != null) _registrations.AddRange(value); }
        }

        private readonly List<EventActivity> _eventActivities = new();
        public IReadOnlyCollection<EventActivity> EventActivities => _eventActivities.AsReadOnly();
        public virtual ICollection<EventActivity> EventActivitiesCollection
        {
            get => _eventActivities;
            set { _eventActivities.Clear(); if (value != null) _eventActivities.AddRange(value); }
        }

        // EF Core constructor
        private Event() { }

        public Event(
            string name,
            string description,
            DateTime startDate,
            DateTime endDate,
            int? venueId = null)
        {
            ValidateDates(startDate, endDate);

            if (string.IsNullOrWhiteSpace(name))
                throw new EventValidationException("Name", "Event name cannot be empty.");

            Name = name.Trim();
            Description = description?.Trim() ?? string.Empty;
            StartDate = startDate;
            EndDate = endDate;
            VenueId = venueId;
        }

        // ISchedulable — METHOD OVERLOADING Version 1
        public bool IsActive() => StartDate <= DateTime.Now && EndDate >= DateTime.Now;

        // ISchedulable — METHOD OVERLOADING Version 2 (with reference date)
        public bool IsActive(DateTime referenceDate)
            => StartDate <= referenceDate && EndDate >= referenceDate;

        public TimeSpan GetDuration() => EndDate - StartDate;

        // Additional helper methods
        public bool IsUpcoming() => StartDate > DateTime.Now;
        public bool IsPast() => EndDate < DateTime.Now;

        public int GetConfirmedParticipantCount()
            => _registrations.Count(r => r.Status == Enums.RegistrationStatus.Confirmed);

        // IFilterable implementation — Events filter by date, venue, activity, search term
        public bool MatchesFilter(FilterCriteria criteria)
        {
            if (criteria.StartDate.HasValue && StartDate < criteria.StartDate.Value)
                return false;
            if (criteria.EndDate.HasValue && EndDate > criteria.EndDate.Value)
                return false;
            if (criteria.VenueId.HasValue && VenueId != criteria.VenueId.Value)
                return false;
            if (criteria.ActivityType.HasValue)
            {
                var hasActivity = _eventActivities.Any(
                    ea => ea.Activity?.Type == criteria.ActivityType.Value);
                if (!hasActivity) return false;
            }
            if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
            {
                var term = criteria.SearchTerm.ToLowerInvariant();
                if (!Name.ToLowerInvariant().Contains(term) &&
                    !Description.ToLowerInvariant().Contains(term))
                    return false;
            }
            return true;
        }

        // OVERRIDE abstract method — polymorphism
        public override string GetDisplayName() => Name;

        // OVERRIDE virtual method — polymorphism
        public override string GetSummary()
        {
            return $"Event: {Name} | " +
                   $"{StartDate:dd/MM/yyyy HH:mm} → {EndDate:dd/MM/yyyy HH:mm} | " +
                   $"Venue: {Venue?.Name ?? "TBD"} | " +
                   $"Participants: {GetConfirmedParticipantCount()}";
        }

        public void UpdateDetails(
            string name,
            string description,
            DateTime startDate,
            DateTime endDate,
            int? venueId)
        {
            ValidateDates(startDate, endDate);

            if (string.IsNullOrWhiteSpace(name))
                throw new EventValidationException("Name", "Event name cannot be empty.");

            Name = name.Trim();
            Description = description?.Trim() ?? string.Empty;
            StartDate = startDate;
            EndDate = endDate;
            VenueId = venueId;
            MarkAsUpdated();
        }

        private static void ValidateDates(DateTime startDate, DateTime endDate)
        {
            if (startDate >= endDate)
                throw new EventValidationException(
                    "Dates", "End date must be after start date.");
        }
    }
}