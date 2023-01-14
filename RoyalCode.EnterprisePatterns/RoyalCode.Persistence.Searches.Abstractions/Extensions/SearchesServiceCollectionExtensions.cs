using RoyalCode.Persistence.Searches.Abstractions.Linq.Filter;
using RoyalCode.Persistence.Searches.Abstractions.Linq.Selector;
using RoyalCode.Persistence.Searches.Abstractions.Linq.Sorter;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class SearchesServiceCollectionExtensions
{
    /// <summary>
    /// Adds the essentials services for the searches with linq.
    /// </summary>
    /// <param name="services">The services collection.</param>
    /// <returns>The same instance of the services collection.</returns>
    public static IServiceCollection AddSearchesLinq(this IServiceCollection services)
    {
        if (services.Any(d => d.ImplementationType == typeof(SpecifierFactory)))
            return services;

        services.AddSingleton<ISpecifierFactory, SpecifierFactory>();
        services.AddSingleton<IOrderByProvider, OrderByProvider>();
        services.AddSingleton<ISelectorFactory, SelectorFactory>();
        
        services.AddSingleton(SpecifiersMap.Instance);
        services.AddSingleton(OrderByHandlersMap.Instance);
        services.AddSingleton(SelectorsMap.Instance);

        services.AddSingleton<ISpecifierGenerator, DefaultSpecifierGenerator>();
        services.AddSingleton<IOrderByGenerator, DefaultOrderByGenerator>();
        services.AddSingleton<ISelectorGenerator, DefaultSelectorGenerator>();
        
        return services;
    }
}
