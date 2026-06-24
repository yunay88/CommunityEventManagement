namespace CommunityEventManagement.Domain.Interfaces.Domain
{
    /// <summary>
    /// Defines location behaviour for entities that have
    /// a physical address or location.
    /// Implemented by: Venue
    /// Demonstrates: single-responsibility interface.
    /// </summary>
    public interface ILocatable
    {
        string Name { get; }
        string Address { get; }
        string City { get; }

        /// <summary>
        /// Returns a formatted human-readable location string.
        /// </summary>
        string GetLocation();
    }
}