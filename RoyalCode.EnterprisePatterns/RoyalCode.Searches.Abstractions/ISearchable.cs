
namespace RoyalCode.Searches.Abstractions;

/// <summary>
/// <para>
///     Represents a search component for a persistence unit.
/// </para>
/// <para>
///     The search component is used to create a search for a specific entity type.
/// </para>
/// </summary>
public interface ISearchable
{
    /// <summary>
    /// <para>
    ///     Creates a new search for the entity.
    /// </para>
    /// <para>
    ///     There must be a search component for the persistence unit, otherwise an exception will be thrown.
    /// </para>
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <returns>A new instance of <see cref="ISearch{TEntity}"/>.</returns>
    /// <exception cref="InvalidOperationException">
    ///     If entity is not part of the persistence unit or there is no search component for it.
    /// </exception>
    ISearch<TEntity> CreateSearch<TEntity>() where TEntity : class;
}

