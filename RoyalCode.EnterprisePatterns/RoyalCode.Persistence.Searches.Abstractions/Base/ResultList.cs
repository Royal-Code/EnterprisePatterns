using RoyalCode.Searches.Abstractions;

namespace RoyalCode.Persistence.Searches.Abstractions.Base;

/// <summary>
/// <para>
///     Default implementation of <see cref="IResultList{TModel}"/>.
/// </para>
/// </summary>
/// <typeparam name="TModel">The result model type.</typeparam>
public class ResultList<TModel> : IResultList<TModel>
{
    /// <inheritdoc />
    public int Page { get; init; }

    /// <inheritdoc />
    public int Count { get; init; }

    /// <inheritdoc />
    public int ItemsPerPage { get; init; }

    /// <inheritdoc />
    public int Pages { get; init; }

    /// <inheritdoc />
    public IEnumerable<ISorting> Sortings { get; init; }

    /// <inheritdoc />
    public Dictionary<string, object> Projections { get; init; }

    /// <inheritdoc />
    public IEnumerable<TModel> Items { get; init; }

    /// <inheritdoc />
    public T GetProjection<T>(string name, T? defaultValue = default)
    {
        throw new NotImplementedException();
    }
}