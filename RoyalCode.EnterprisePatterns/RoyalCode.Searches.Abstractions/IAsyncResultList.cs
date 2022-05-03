namespace RoyalCode.Searches.Abstractions;

/// <summary>
/// Component interface for async listing the result of a search.
/// </summary>
/// <typeparam name="TModel"></typeparam>
public interface IAsyncResultList<TModel> : IResultList
{
    /// <summary>
    /// Async collection of the searched models.
    /// </summary>
    IAsyncEnumerable<TModel> Items { get; }
}