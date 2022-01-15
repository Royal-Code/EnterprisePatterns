
using System.Linq.Expressions;

namespace RoyalCode.Repositories.Abstractions.Searchable;

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

/// <summary>
/// <para>
///     Component to perform entity searches, being able to apply multiple filters,
///     sorting, define paging, include projections, and determine which data must be selected.
/// </para>
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface ISearch<TEntity>
    where TEntity : class
{
    /// <summary>
    /// <para>
    ///     Adds a filter object to the search.
    /// </para>
    /// <para>
    ///     The search engine must be able to apply this filter, otherwise an exception will be throwed.
    /// </para>
    /// </summary>
    /// <typeparam name="TFilter">The filter type.</typeparam>
    /// <param name="filter">The filter object.</param>
    /// <returns>The same instance for chaining calls.</returns>
    ISearch<TEntity> FilterBy<TFilter>(TFilter filter)
        where TFilter : class;

    ISearch<TEntity, TSelected> Select<TSelected>()
        where TSelected : class;

    ISearch<TEntity, TSelected> Select<TSelected>(Expression<Func<TEntity, TSelected>> select)
        where TSelected : class;
}

public interface ISearch<TEntity, TSelected>
{

}