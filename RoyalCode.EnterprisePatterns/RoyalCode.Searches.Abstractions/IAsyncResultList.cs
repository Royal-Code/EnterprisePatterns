namespace RoyalCode.Searches.Abstractions;

/// <summary>
/// Component interface for async listing the result of a search.
/// </summary>
/// <typeparam name="TModel">Type of data listed by the result.</typeparam>
public interface IAsyncResultList<out TModel> : IResultList
{
    /// <summary>
    /// Async collection of the searched models.
    /// </summary>
    IAsyncEnumerable<TModel> Items { get; }
    
    /// <summary>
    /// Gets a value from the projection if it exists and is of the type entered,
    /// or returns the default value if the value does not exist or the type is different.
    /// </summary>
    /// <typeparam name="T">Projection value type.</typeparam>
    /// <param name="name">Projection name.</param>
    /// <param name="defaultValue">Default value.</param>
    /// <returns>The projection value, or default value.</returns>
    T GetProjection<T>(string name, T? defaultValue = default);
}