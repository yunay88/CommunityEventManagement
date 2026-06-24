namespace CommunityEventManagement.Domain.Entities
{
    /// <summary>
    /// EventActivity — junction entity for many-to-many between Event and Activity.
    /// Does NOT inherit from BaseEntity (no Id needed — composite key).
    /// 
    /// Demonstrates:
    ///   - Junction/bridge entity pattern
    ///   - Extra data on a many-to-many relationship (OrderInEvent, AddedAt)
    /// </summary>
    public class EventActivity
    {
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;

        public int ActivityId { get; set; }
        public Activity Activity { get; set; } = null!;

        public int OrderInEvent { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}