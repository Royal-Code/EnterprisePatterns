using RoyalCode.Persistence.Searches.Abstractions.Linq.Sorter;
using RoyalCode.Searches.Abstractions;

namespace RoyalCode.Persistence.Searches.Abstractions.Linq;

/// <summary>
/// <para>
///     Interface of the component to apply the sort operation to a query.
/// </para>
/// <para>
///     It has a default implementation, <see cref="DefaultSorter{TModel}"/>,
///     which makes use of other abstract components to automate sorting.
/// </para>
/// </summary>
/// <typeparam name="TModel">The query model type.</typeparam>
public interface ISorter<TModel>
    where TModel: class
{
    /// <summary>
    /// <para>
    ///     Applies sorting to the <paramref name="query"/>
    ///     according to the definitions of <paramref name="sortings"/>.
    /// </para>
    /// </summary>
    /// <param name="query">The query to sort.</param>
    /// <param name="sortings">The soring definitions.</param>
    /// <returns>A ordered query.</returns>
    IQueryable<TModel> OrderBy(IQueryable<TModel> query, IEnumerable<ISorting> sortings);

    IQueryable<TModel> DefaultOrderBy(IQueryable<TModel> query);
}