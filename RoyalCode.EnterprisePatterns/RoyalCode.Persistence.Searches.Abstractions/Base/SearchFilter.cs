namespace RoyalCode.Persistence.Searches.Abstractions.Base;

/// <summary>
/// Information about a filter to be applied to a query.
/// </summary>
/// <param name="modelType">The query model type.</param>
/// <param name="FilterType">The filter type.</param>
/// <param name="filter">The filter instance.</param>
public record SearchFilter(Type modelType, Type FilterType, object filter);