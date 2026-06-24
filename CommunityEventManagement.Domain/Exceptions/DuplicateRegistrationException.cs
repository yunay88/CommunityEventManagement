namespace CommunityEventManagement.Domain.Exceptions
{
    /// <summary>
    /// Thrown when a participant attempts to register for an event
    /// they are already registered for.
    /// Demonstrates: specific business rule exception.
    /// </summary>
    public class DuplicateRegistrationException : CommunityEventException
    {
        public int ParticipantId { get; }
        public int EventId { get; }

        public DuplicateRegistrationException(int participantId, int eventId)
            : base(
                $"Participant {participantId} is already registered for event {eventId}.",
                "DUPLICATE_REGISTRATION")
        {
            ParticipantId = participantId;
            EventId = eventId;
        }
    }
}