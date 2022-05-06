namespace RoyalCode.Persistence.Searches.Abstractions.Linq.Sorter;

/// <summary>
/// <para>
///     Manages and provides the query ordering handlers of a given data model.
/// </para>
/// </summary>
/// <typeparam name="TModel">The query source model type.</typeparam>
public interface IOrderByProvider<TModel>
    where TModel: class
{
    /// <summary>
    /// <para>
    ///     It tries to get an ordering handler configured for the data model,
    ///     according to a sort name that identifies how the query should be sorted.
    /// </para>
    /// <para>
    ///     May be returned null if no handler is configured for the order by identification, or throw an exception.
    /// </para>
    /// </summary>
    /// <param name="orderBy">A sort name that identifies how the query should be sorted.</param>
    /// <returns>An ordering handler or null if not exists.</returns>
    /// <exception cref="Exception">Optional, if no handler is configured for the order by identification.</exception>
    IOrderByHandler<TModel>? GetHandler(string orderBy);
    
    IOrderByHandler<TModel> GetDefaultHandler();
}