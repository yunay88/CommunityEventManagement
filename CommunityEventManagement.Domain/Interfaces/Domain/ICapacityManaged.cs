namespace CommunityEventManagement.Domain.Interfaces.Domain
{
    /// <summary>
    /// Defines capacity management behaviour for entities
    /// that have a maximum limit on participants or items.
    /// Implemented by: Venue
    /// Demonstrates: interface with computed behaviour methods.
    /// </summary>
    public interface ICapacityManaged
    {
        int MaxCapacity { get; }
        int CurrentCapacity { get; }

        /// <summary>Returns true if at least one space is available.</summary>
        bool HasAvailableSpace();

        /// <summary>
        /// Overloaded version — checks if a specific number of spaces are available.
        /// Demonstrates: method overloading via interface.
        /// </summary>
        bool HasAvailableSpace(int requiredSpaces);

        /// <summary>
        /// Returns the occupancy rate as a percentage (0-100).
        /// </summary>
        decimal GetOccupancyRate();
    }
}