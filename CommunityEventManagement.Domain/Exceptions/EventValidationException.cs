namespace CommunityEventManagement.Domain.Exceptions
{
    /// <summary>
    /// Thrown when event data fails business rule validation.
    /// Examples: end date before start date, event name too short.
    /// Demonstrates: business rule validation exception.
    /// </summary>
    public class EventValidationException : CommunityEventException
    {
        public string? FieldName { get; }

        public EventValidationException(string message)
            : base(message, "EVENT_VALIDATION_ERROR")
        {
        }

        public EventValidationException(string fieldName, string message)
            : base(message, "EVENT_VALIDATION_ERROR")
        {
            FieldName = fieldName;
        }
    }
}