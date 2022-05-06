using System.Runtime.CompilerServices;
using RoyalCode.Persistence.Searches.Abstractions.Base;
using RoyalCode.Persistence.Searches.Abstractions.Linq;
using RoyalCode.Persistence.Searches.Abstractions.Linq.Filter;
using RoyalCode.Persistence.Searches.Abstractions.Pipeline;

namespace RoyalCode.Persistence.EntityFramework.Searches;

/// <summary>
/// <para>
///     A base implementation for <see cref="ISearchPipeline{TModel}"/> to share commons methods/operations.
/// </para>
/// </summary>
/// <typeparam name="TEntity">The query source model type.</typeparam>
public abstract class SearchPipelineBase<TEntity>
    where TEntity : class
{
    protected readonly IQueryableProvider<TEntity> queryableProvider;
    protected readonly ISpecifierFactory specifierFactory;
    protected readonly ISorter<TEntity> sorter;

    /// <summary>
    /// Creates a new search pipeline that receives the dependencies.
    /// </summary>
    /// <param name="queryableProvider">Provides the query.</param>
    /// <param name="specifierFactory">Provides the specifiers.</param>
    /// <param name="sorter">Sort the query.</param>
    protected SearchPipelineBase(
        IQueryableProvider<TEntity> queryableProvider,
        ISpecifierFactory specifierFactory,
        ISorter<TEntity> sorter)
    {
        this.queryableProvider = queryableProvider;
        this.specifierFactory = specifierFactory;
        this.sorter = sorter;
    }

    /// <summary>
    /// Creates the query, apply the filters and sort.
    /// </summary>
    /// <param name="criteria">Search criteria.</param>
    /// <returns>A prepared query.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected IQueryable<TEntity> PrepareQuery(SearchCriteria criteria)
    {
        var baseQuery = queryableProvider.GetQueryable();

        var queryFilters = criteria.Filters.Where(f => f.ModelType == typeof(TEntity)).ToList();
        if (queryFilters.Any())
        {
            var handler = new SpecifierHandler<TEntity>(specifierFactory, baseQuery);
            foreach (var searchFilter in queryFilters)
            {
                searchFilter.ApplyFilter(handler);
            }

            baseQuery = handler.Query;
        }

        var sortedQuery = criteria.Sortings.Any()
            ? sorter.OrderBy(baseQuery, criteria.Sortings)
            : criteria.Paginate
                ? sorter.DefaultOrderBy(baseQuery)
                : baseQuery;

        return sortedQuery;
    }

    /// <summary>
    /// Count the pages.
    /// </summary>
    /// <param name="count">The record count.</param>
    /// <param name="itemsPerPage">How many items is listed per page.</param>
    /// <returns>The total number of pages.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static int CountPages(int count, int itemsPerPage)
    {
        return count == 0 || itemsPerPage < 1
            ? 0
            : (int)Math.Floor((double)count / itemsPerPage);
    }
}