namespace CommunityEventManagement.Domain.Exceptions
{
    /// <summary>
    /// Thrown when a participant cannot be found by their identifier.
    /// </summary>
    public class ParticipantNotFoundException : CommunityEventException
    {
        public int ParticipantId { get; }

        public ParticipantNotFoundException(int participantId)
            : base($"Participant with ID {participantId} was not found.", "PARTICIPANT_NOT_FOUND")
        {
            ParticipantId = participantId;
        }

        public ParticipantNotFoundException(string message)
            : base(message, "PARTICIPANT_NOT_FOUND")
        {
            ParticipantId = 0;
        }
    }
}