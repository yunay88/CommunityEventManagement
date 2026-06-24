using System.ComponentModel.DataAnnotations;
using CommunityEventManagement.Domain.Enums;

namespace CommunityEventManagement.Domain.Entities
{
    /// <summary>
    /// Activity entity — a specific activity within an event.
    /// Examples: Workshop, Talk, Game, Performance.
    /// Inheritance: Activity → BaseEntity
    /// 
    /// Demonstrates:
    ///   - Encapsulation with private setters
    ///   - Computed methods (GetFormattedDuration)
    ///   - DataAnnotation validation
    /// </summary>
    public class Activity : BaseEntity
    {
        [Required(ErrorMessage = "Activity name is required")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Activity name must be between 2 and 100 characters")]
        public string Name { get; private set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; private set; } = string.Empty;

        public ActivityType Type { get; private set; }

        [Range(1, 480, ErrorMessage = "Duration must be between 1 and 480 minutes")]
        public int DurationMinutes { get; private set; }

        // Navigation property — events this activity belongs to
        public ICollection<EventActivity> EventActivities { get; private set; }
            = new List<EventActivity>();

        // EF Core constructor
        private Activity() { }

        public Activity(string name, string description, ActivityType type, int durationMinutes)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Activity name cannot be empty.", nameof(name));
            if (durationMinutes <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(durationMinutes), "Duration must be greater than zero.");

            Name = name.Trim();
            Description = description?.Trim() ?? string.Empty;
            Type = type;
            DurationMinutes = durationMinutes;
        }

        // OVERRIDE abstract method — polymorphism
        public override string GetDisplayName() => $"{Name} ({Type})";

        // OVERRIDE virtual method — polymorphism
        public override string GetSummary()
        {
            return $"Activity: {Name} | Type: {Type} | Duration: {GetFormattedDuration()}";
        }

        /// <summary>
        /// Converts duration in minutes to human-readable format.
        /// Demonstrates: computed string formatting method.
        /// </summary>
        public string GetFormattedDuration()
        {
            if (DurationMinutes < 60) return $"{DurationMinutes} min";
            var hours = DurationMinutes / 60;
            var minutes = DurationMinutes % 60;
            return minutes > 0 ? $"{hours}h {minutes}m" : $"{hours}h";
        }

        public void UpdateDetails(
            string name,
            string description,
            ActivityType type,
            int durationMinutes)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Activity name cannot be empty.", nameof(name));
            if (durationMinutes <= 0)
                throw new ArgumentOutOfRangeException(nameof(durationMinutes));

            Name = name.Trim();
            Description = description?.Trim() ?? string.Empty;
            Type = type;
            DurationMinutes = durationMinutes;
            MarkAsUpdated();
        }
    }
}