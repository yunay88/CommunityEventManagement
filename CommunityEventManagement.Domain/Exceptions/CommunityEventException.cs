namespace CommunityEventManagement.Domain.Exceptions
{
    /// <summary>
    /// Base custom exception for the Community Event Management System.
    /// All domain-specific exceptions inherit from this class.
    /// Demonstrates: custom exception hierarchy, inheritance from Exception.
    /// </summary>
    public class CommunityEventException : Exception
    {
        public string ErrorCode { get; }
        public DateTime OccurredAt { get; }

        // Default constructor
        public CommunityEventException()
            : base("A community event error occurred.")
        {
            ErrorCode = "GENERAL_ERROR";
            OccurredAt = DateTime.UtcNow;
        }

        // Constructor with message
        public CommunityEventException(string message)
            : base(message)
        {
            ErrorCode = "GENERAL_ERROR";
            OccurredAt = DateTime.UtcNow;
        }

        // Constructor with message and error code
        public CommunityEventException(string message, string errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
            OccurredAt = DateTime.UtcNow;
        }

        // Constructor with message, error code, and inner exception
        public CommunityEventException(string message, string errorCode, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
            OccurredAt = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"[{ErrorCode}] {Message} (Occurred at: {OccurredAt:yyyy-MM-dd HH:mm:ss} UTC)";
        }
    }
}