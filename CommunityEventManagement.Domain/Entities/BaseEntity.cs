namespace CommunityEventManagement.Domain.Entities
{
    /// <summary>
    /// Abstract base entity — root of the entire inheritance hierarchy.
    /// ALL domain entities inherit from this class.
    /// 
    /// Demonstrates:
    ///   - Abstract class (cannot be instantiated directly)
    ///   - Encapsulation (private/protected setters)
    ///   - Abstract method (forces subclasses to implement GetDisplayName)
    ///   - Virtual method (subclasses CAN override GetSummary)
    ///   - Protected constructor (only subclasses can call it)
    /// </summary>
    public abstract class BaseEntity
    {
        // Encapsulation: Id cannot be set from outside the class hierarchy
        public int Id { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime? UpdatedAt { get; protected set; }
        public bool IsDeleted { get; protected set; }

        // Protected default constructor — only callable by subclasses
        // Required by Entity Framework Core for materialisation
        protected BaseEntity()
        {
            CreatedAt = DateTime.UtcNow;
            IsDeleted = false;
        }

        // ABSTRACT METHOD — every subclass MUST implement this
        // This is runtime polymorphism — different behaviour per subclass
        public abstract string GetDisplayName();

        // VIRTUAL METHOD — subclasses CAN override this
        // Provides a default implementation that subclasses may improve
        public virtual string GetSummary()
        {
            return $"[{GetType().Name}] ID:{Id} | {GetDisplayName()} | Created: {CreatedAt:dd/MM/yyyy}";
        }

        // Soft delete — marks as deleted without removing from database
        public void MarkAsDeleted()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsUpdated()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        // Override Equals for value-based comparison
        public override bool Equals(object? obj)
        {
            if (obj is not BaseEntity other) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false;
            return Id == other.Id && Id != 0;
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}