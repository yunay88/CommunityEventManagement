using CommunityEventManagement.Domain.Enums;

namespace CommunityEventManagement.Domain.Models
{
    /// <summary>
    /// Encapsulates all possible filter options for querying events.
    /// Used by IFilterable interface and repository filter methods.
    /// Demonstrates: encapsulation of filter state in a single object.
    /// </summary>
    public class FilterCriteria
    {
        public string? SearchTerm { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? VenueId { get; set; }
        public ActivityType? ActivityType { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Computed property — true if any filter has been set.
        /// Demonstrates: computed property using expression body.
        /// </summary>
        public bool HasFilters =>
            StartDate.HasValue ||
            EndDate.HasValue ||
            VenueId.HasValue ||
            ActivityType.HasValue ||
            !string.IsNullOrWhiteSpace(SearchTerm);

        /// <summary>Returns a human-readable description of active filters.</summary>
        public string GetFilterSummary()
        {
            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
                parts.Add($"Search: '{SearchTerm}'");
            if (StartDate.HasValue)
                parts.Add($"From: {StartDate:dd/MM/yyyy}");
            if (EndDate.HasValue)
                parts.Add($"To: {EndDate:dd/MM/yyyy}");
            if (VenueId.HasValue)
                parts.Add($"Venue ID: {VenueId}");
            if (ActivityType.HasValue)
                parts.Add($"Activity: {ActivityType}");

            return parts.Count > 0
                ? string.Join(" | ", parts)
                : "No filters applied";
        }
    }
}