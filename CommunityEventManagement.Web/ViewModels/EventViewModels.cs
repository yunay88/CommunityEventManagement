using System.ComponentModel.DataAnnotations;
using CommunityEventManagement.Domain.Entities;
using CommunityEventManagement.Domain.Enums;

namespace CommunityEventManagement.Web.ViewModels
{
    /// <summary>
    /// Read-only display model for an Event.
    /// Used in lists and detail views.
    /// Demonstrates: static factory method (FromEntity).
    /// </summary>
    public class EventViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? VenueName { get; set; }
        public string? VenueLocation { get; set; }
        public int ParticipantCount { get; set; }
        public bool IsUpcoming { get; set; }
        public bool IsActive { get; set; }
        public bool IsPast { get; set; }
        public string DurationFormatted { get; set; } = string.Empty;
        public List<string> ActivityNames { get; set; } = new();
        public string StatusBadge => IsActive ? "Live" : IsUpcoming ? "Upcoming" : "Past";
        public string StatusColour => IsActive ? "success" : IsUpcoming ? "primary" : "secondary";

        /// <summary>
        /// Static factory method — maps Entity to ViewModel.
        /// Demonstrates: Factory-like static creation method.
        /// </summary>
        public static EventViewModel FromEntity(Event e)
        {
            return new EventViewModel
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                VenueName = e.Venue?.Name,
                VenueLocation = e.Venue?.GetLocation(),
                ParticipantCount = e.GetConfirmedParticipantCount(),
                IsUpcoming = e.IsUpcoming(),
                IsActive = e.IsActive(),
                IsPast = e.IsPast(),
                DurationFormatted = FormatDuration(e.GetDuration()),
                ActivityNames = e.EventActivities
                    .Select(ea => ea.Activity?.Name ?? string.Empty)
                    .Where(n => !string.IsNullOrEmpty(n))
                    .ToList()
            };
        }

        private static string FormatDuration(TimeSpan duration)
        {
            if (duration.TotalDays >= 1) return $"{(int)duration.TotalDays} day(s)";
            if (duration.TotalHours >= 1) return $"{(int)duration.TotalHours} hour(s)";
            return $"{(int)duration.TotalMinutes} minute(s)";
        }
    }

    /// <summary>
    /// Form model for creating and editing an Event.
    /// Has DataAnnotations for both client-side and server-side validation.
    /// </summary>
    public class CreateEventModel : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Event name is required")]
        [StringLength(100, MinimumLength = 3,
            ErrorMessage = "Event name must be between 3 and 100 characters")]
        [Display(Name = "Event Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Start date is required")]
        [Display(Name = "Start Date & Time")]
        public DateTime StartDate { get; set; } = DateTime.Now.AddDays(7);

        [Required(ErrorMessage = "End date is required")]
        [Display(Name = "End Date & Time")]
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(7).AddHours(3);

        [Display(Name = "Venue")]
        public int? VenueId { get; set; }

        [Display(Name = "Activities")]
        public List<int> SelectedActivityIds { get; set; } = new();

        /// <summary>
        /// IValidatableObject — custom cross-field validation.
        /// Demonstrates: server-side business rule validation.
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate <= StartDate)
            {
                yield return new ValidationResult(
                    "End date must be after start date.",
                    new[] { nameof(EndDate) });
            }

            if (StartDate < DateTime.Now.AddMinutes(-5))
            {
                yield return new ValidationResult(
                    "Start date cannot be in the past.",
                    new[] { nameof(StartDate) });
            }

            if ((EndDate - StartDate).TotalMinutes < 30)
            {
                yield return new ValidationResult(
                    "Event must be at least 30 minutes long.",
                    new[] { nameof(EndDate) });
            }
        }

        public static CreateEventModel FromEntity(Event e)
        {
            return new CreateEventModel
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                VenueId = e.VenueId,
                SelectedActivityIds = e.EventActivities
                    .Select(ea => ea.ActivityId)
                    .ToList()
            };
        }
    }

    /// <summary>
    /// Filter model for the event list page.
    /// </summary>
    public class EventFilterModel
    {
        [Display(Name = "Search")]
        public string? SearchTerm { get; set; }

        [Display(Name = "From Date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "To Date")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Venue")]
        public int? VenueId { get; set; }

        [Display(Name = "Activity Type")]
        public ActivityType? ActivityType { get; set; }

        public bool HasFilters =>
            !string.IsNullOrWhiteSpace(SearchTerm) ||
            StartDate.HasValue ||
            EndDate.HasValue ||
            VenueId.HasValue ||
            ActivityType.HasValue;

        public Domain.Models.FilterCriteria ToFilterCriteria()
        {
            return new Domain.Models.FilterCriteria
            {
                SearchTerm = SearchTerm,
                StartDate = StartDate,
                EndDate = EndDate,
                VenueId = VenueId,
                ActivityType = ActivityType
            };
        }
    }

    /// <summary>
    /// Detailed event view model including registrations and activities.
    /// </summary>
    public class EventDetailViewModel : EventViewModel
    {
        public List<RegistrationViewModel> Registrations { get; set; } = new();
        public List<ActivityViewModel> Activities { get; set; } = new();
        public int? VenueMaxCapacity { get; set; }
        public decimal VenueOccupancyRate { get; set; }

        public new static EventDetailViewModel FromEntity(Event e)
        {
            return new EventDetailViewModel
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                VenueName = e.Venue?.Name,
                VenueLocation = e.Venue?.GetLocation(),
                VenueMaxCapacity = e.Venue?.MaxCapacity,
                VenueOccupancyRate = e.Venue?.GetOccupancyRate() ?? 0,
                ParticipantCount = e.GetConfirmedParticipantCount(),
                IsUpcoming = e.IsUpcoming(),
                IsActive = e.IsActive(),
                IsPast = e.IsPast(),
                DurationFormatted = FormatDuration(e.GetDuration()),
                ActivityNames = e.EventActivities
                    .Select(ea => ea.Activity?.Name ?? string.Empty)
                    .Where(n => !string.IsNullOrEmpty(n))
                    .ToList(),
                Registrations = e.Registrations
                    .Select(RegistrationViewModel.FromEntity)
                    .ToList(),
                Activities = e.EventActivities
                    .Where(ea => ea.Activity != null)
                    .Select(ea => ActivityViewModel.FromEntity(ea.Activity!))
                    .ToList()
            };
        }

        private static string FormatDuration(TimeSpan duration)
        {
            if (duration.TotalDays >= 1) return $"{(int)duration.TotalDays} day(s)";
            if (duration.TotalHours >= 1) return $"{(int)duration.TotalHours} hour(s)";
            return $"{(int)duration.TotalMinutes} minute(s)";
        }
    }
}