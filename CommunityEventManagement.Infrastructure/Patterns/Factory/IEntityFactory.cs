namespace CommunityEventManagement.Infrastructure.Patterns.Factory
{
    /// <summary>
    /// Generic factory interface — Factory Pattern.
    /// Defines the contract for all entity factories.
    ///
    /// Demonstrates:
    ///   - Factory design pattern (Creational)
    ///   - Generic interface with type parameter
    ///   - Interface segregation — each factory has its own interface
    /// </summary>
    public interface IEntityFactory<TEntity, TRequest>
    {
        /// <summary>
        /// Creates a new entity from a request object.
        /// Validates inputs and returns a properly initialised entity.
        /// </summary>
        TEntity Create(TRequest request);

        /// <summary>
        /// Validates whether a request is valid before creation.
        /// Returns list of validation errors (empty if valid).
        /// </summary>
        IEnumerable<string> Validate(TRequest request);
    }
}