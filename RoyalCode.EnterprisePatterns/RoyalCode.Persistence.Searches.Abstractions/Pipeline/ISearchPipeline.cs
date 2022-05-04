using RoyalCode.Persistence.Searches.Abstractions.Base;
using RoyalCode.Searches.Abstractions;

namespace RoyalCode.Persistence.Searches.Abstractions.Pipeline;

/// <summary>
/// <para>
///     A search pipeline for executing queries from the input criteria.
/// </para>
/// <para>
///     This component will perform the various steps necessary to execute the query.
/// </para>
/// </summary>
/// <typeparam name="TModel">The query model type.</typeparam>
public interface ISearchPipeline<TModel>
    where TModel : class
{
    /// <summary>
    /// Execute the search and it returns a list of results.
    /// </summary>
    /// <param name="criteria">The criteria for the search.</param>
    /// <returns>A list of results.</returns>
    IResultList<TModel> Execute(SearchCriteria criteria);
    
    /// <summary>
    /// Async execute the search and it returns a list of results.
    /// </summary>
    /// <param name="criteria">The criteria for the search.</param>
    /// <returns>A task of a list of results.</returns>
    Task<IResultList<TModel>> ExecuteAsync(SearchCriteria criteria);

    /// <summary>
    /// Async execute the search and it returns a list of results.
    /// </summary>
    /// <param name="criteria">The criteria for the search.</param>
    /// <returns>A task of an async list of results.</returns>
    Task<IAsyncResultList<TModel>> AsyncExecuteAsync(SearchCriteria criteria);
}