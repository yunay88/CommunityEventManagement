namespace CommunityEventManagement.Domain.Enums
{
    /// <summary>
    /// Defines the types of activities that can be part of a community event.
    /// Used by Activity entity and filtering criteria.
    /// </summary>
    public enum ActivityType
    {
        Workshop = 0,
        Talk = 1,
        Game = 2,
        Performance = 3,
        Exhibition = 4,
        Networking = 5,
        Other = 6
    }
}