namespace RoyalCode.Persistence.Searches.Abstractions.Linq;

/// <summary>
/// Component to provide a <see cref="IQueryable{T}"/> for an entity.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IQueryableProvider<out TEntity>
    where TEntity: class
{
    /// <summary>
    /// Get a new queryable for the entity.
    /// </summary>
    /// <returns>An <see cref="IQueryable{T}"/> instance.</returns>
    IQueryable<TEntity> GetQueryable();
}