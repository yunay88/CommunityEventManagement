namespace CommunityEventManagement.Domain.Exceptions
{
    /// <summary>
    /// Thrown when an activity cannot be found by its identifier.
    /// </summary>
    public class ActivityNotFoundException : CommunityEventException
    {
        public int ActivityId { get; }

        public ActivityNotFoundException(int activityId)
            : base($"Activity with ID {activityId} was not found.", "ACTIVITY_NOT_FOUND")
        {
            ActivityId = activityId;
        }
    }
}