using System.Linq.Expressions;
using RoyalCode.Searches.Abstractions;

namespace RoyalCode.Persistence.Searches.Abstractions.Base;

/// <summary>
/// The criteria for performing the search.
/// </summary>
public class SearchCriteria
{
    private List<SearchFilter>? filters;
    private List<ISorting>? sortings;

    /// <summary>
    /// Get all filters.
    /// </summary>
    public IEnumerable<SearchFilter> Filters => filters ?? Enumerable.Empty<SearchFilter>();

    /// <summary>
    /// Get all sortings.
    /// </summary>
    public IEnumerable<ISorting> Sortings => sortings ?? Enumerable.Empty<ISorting>();

    /// <summary>
    /// Information about the select expression.
    /// </summary>
    public SearchSelect? Select { get; private set; }
    
    /// <summary>
    /// <para>
    ///     Defines that the query will be paged and determines the number of items per page.
    /// </para>
    /// <para>
    ///     The default value is 10 items per page.
    /// </para>
    /// <para>
    ///     When zero (0) is entered, it will not be paged.
    /// </para>
    /// </summary>
    public int ItemsPerPage { get; set; } = Defaults.DefaultItemsPerPage;

    /// <summary>
    /// The number of the page to be searched.
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// <para>
    ///     Updates the last record count.
    /// </para>
    /// <para>
    ///     Used to not count the records again.
    /// </para>
    /// </summary>
    public int LastCount { get; set; }

    /// <summary>
    /// Whether to apply record counting.
    /// </summary>
    public bool UseCount { get; set; } = Defaults.DefaultUseCount;

    /// <summary>
    /// Adds a new filter to specify the search.
    /// </summary>
    /// <param name="modelType">The query model type.</param>
    /// <param name="filterType">The filter type.</param>
    /// <param name="filter">The filter instance.</param>
    public void AddFilter(Type modelType, Type filterType, object filter)
    {
        filters ??= new List<SearchFilter>();
        filters.Add(new SearchFilter(modelType, filterType, filter));
    }

    /// <summary>
    /// Add a sorting definition.
    /// </summary>
    /// <param name="sorting">The sorting definition.</param>
    public void AddSorting(ISorting sorting)
    {
        sortings ??= new List<ISorting>();
        sortings.Add(sorting);
    }

    /// <summary>
    /// Set the select expression.
    /// </summary>
    /// <param name="selectExpression">The select expression.</param>
    /// <typeparam name="TEntity">The query entity type.</typeparam>
    /// <typeparam name="TDto">The select type.</typeparam>
    /// <exception cref="ArgumentNullException">If expression is null.</exception>
    public void SetSelectExpression<TEntity, TDto>(Expression<Func<TEntity, TDto>> selectExpression)
    {
        if (selectExpression == null)
            throw new ArgumentNullException(nameof(selectExpression));

        Select = new SearchSelect(typeof(TEntity), typeof(TDto), selectExpression);
    }
    
    /// <summary>
    /// Default values for each new <see cref="SearchCriteria"/> created.
    /// </summary>
    public static class Defaults
    {
        /// <summary>
        /// The default value of <see cref="SearchCriteria.ItemsPerPage"/>.
        /// </summary>
        public static int DefaultItemsPerPage = 10;
        
        /// <summary>
        /// The default value of <see cref="SearchCriteria.UseCount"/>.
        /// </summary>
        public static bool DefaultUseCount = true;
    }
}