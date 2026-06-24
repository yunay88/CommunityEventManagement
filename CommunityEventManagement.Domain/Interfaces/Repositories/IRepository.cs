using System.Linq.Expressions;
using CommunityEventManagement.Domain.Entities;

namespace CommunityEventManagement.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Generic repository interface — Repository Pattern.
    /// Provides a standard set of data access operations for any entity.
    /// Demonstrates:
    ///   - Generic type parameter constrained to BaseEntity
    ///   - Repository design pattern
    ///   - Interface defining a contract for all repositories
    /// Implemented by: Repository(T) generic class
    /// </summary>
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    }
}