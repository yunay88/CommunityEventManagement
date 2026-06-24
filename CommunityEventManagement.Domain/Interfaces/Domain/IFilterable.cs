using CommunityEventManagement.Domain.Models;

namespace CommunityEventManagement.Domain.Interfaces.Domain
{
    /// <summary>
    /// Defines filtering behaviour for entities that can be
    /// searched and filtered by various criteria.
    /// Implemented by: Event, Participant
    /// Demonstrates: interface enabling polymorphic filtering.
    /// Each implementing class filters differently based on its own properties.
    /// </summary>
    public interface IFilterable
    {
        /// <summary>
        /// Determines whether this entity matches the given filter criteria.
        /// Each entity implements this differently — polymorphism in action.
        /// </summary>
        bool MatchesFilter(FilterCriteria criteria);
    }
}