namespace CommunityEventManagement.Domain.Exceptions
{
    /// <summary>
    /// Thrown when a registration operation violates a business rule.
    /// Examples: registering for a past event, confirming a cancelled registration.
    /// </summary>
    public class RegistrationException : CommunityEventException
    {
        public RegistrationException(string message)
            : base(message, "REGISTRATION_ERROR")
        {
        }

        public RegistrationException(string message, Exception innerException)
            : base(message, "REGISTRATION_ERROR", innerException)
        {
        }
    }
}