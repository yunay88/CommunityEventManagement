namespace CommunityEventManagement.Domain.Exceptions
{
    /// <summary>
    /// Thrown when authentication fails — invalid credentials,
    /// expired session, or unauthorised access attempt.
    /// Demonstrates: multi-level exception hierarchy for auth.
    /// </summary>
    public class AuthenticationException : CommunityEventException
    {
        public AuthenticationException(string message)
            : base(message, "AUTHENTICATION_ERROR")
        {
        }
    }

    /// <summary>
    /// Thrown when an authenticated user attempts an action
    /// they do not have permission to perform.
    /// Inherits from AuthenticationException — demonstrates
    /// multi-level custom exception inheritance.
    /// </summary>
    public class UnauthorisedException : AuthenticationException
    {
        public string? RequiredRole { get; }

        public UnauthorisedException(string message)
            : base(message)
        {
        }

        public UnauthorisedException(string action, string requiredRole)
            : base($"You do not have permission to {action}. Required role: {requiredRole}.")
        {
            RequiredRole = requiredRole;
        }
    }
}