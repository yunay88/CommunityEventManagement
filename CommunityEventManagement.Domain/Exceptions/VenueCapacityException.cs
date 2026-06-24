namespace CommunityEventManagement.Domain.Exceptions
{
    /// <summary>
    /// Thrown when a venue capacity limit is exceeded or when
    /// an invalid capacity value is provided.
    /// Demonstrates: capacity/limit validation exception.
    /// </summary>
    public class VenueCapacityException : CommunityEventException
    {
        public int? VenueId { get; }
        public int? MaxCapacity { get; }

        public VenueCapacityException(string message)
            : base(message, "VENUE_CAPACITY_ERROR")
        {
        }

        public VenueCapacityException(int venueId, int maxCapacity)
            : base(
                $"Venue {venueId} has reached its maximum capacity of {maxCapacity}.",
                "VENUE_CAPACITY_EXCEEDED")
        {
            VenueId = venueId;
            MaxCapacity = maxCapacity;
        }
    }
}