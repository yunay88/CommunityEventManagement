namespace CommunityEventManagement.Domain.Exceptions
{
    /// <summary>
    /// Thrown when an event cannot be found by its identifier.
    /// Inherits from CommunityEventException — demonstrates inheritance chain.
    /// </summary>
    public class EventNotFoundException : CommunityEventException
    {
        public int EventId { get; }

        public EventNotFoundException(int eventId)
            : base($"Event with ID {eventId} was not found.", "EVENT_NOT_FOUND")
        {
            EventId = eventId;
        }

        public EventNotFoundException(string message)
            : base(message, "EVENT_NOT_FOUND")
        {
            EventId = 0;
        }
    }
}