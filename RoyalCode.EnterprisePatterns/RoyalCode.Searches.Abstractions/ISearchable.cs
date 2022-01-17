
namespace RoyalCode.Searches.Abstractions;

/// <summary>
/// <para>
///     Interface to entity data services where searches can be applied.
/// </para>
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface ISearchable<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Creates a new search for the entity.
    /// </summary>
    /// <returns>A new instance of <see cref="ISearch{TEntity}"/>.</returns>
    ISearch<TEntity> CreateSearch();
}

