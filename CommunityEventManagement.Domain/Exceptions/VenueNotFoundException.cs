namespace CommunityEventManagement.Domain.Exceptions
{
    /// <summary>
    /// Thrown when a venue cannot be found by its identifier.
    /// </summary>
    public class VenueNotFoundException : CommunityEventException
    {
        public int VenueId { get; }

        public VenueNotFoundException(int venueId)
            : base($"Venue with ID {venueId} was not found.", "VENUE_NOT_FOUND")
        {
            VenueId = venueId;
        }

        public VenueNotFoundException(string message)
            : base(message, "VENUE_NOT_FOUND")
        {
            VenueId = 0;
        }
    }
}