using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RoyalCode.Persistence.Searches.Abstractions.Linq;
using RoyalCode.Persistence.Searches.Abstractions.Pipeline;
using RoyalCode.Searches.Abstractions;

namespace RoyalCode.Persistence.EntityFramework.Searches.Configurations;

/// <inheritdoc />
public class SearchConfigurer<TDbContext> : ISearchConfigurer<TDbContext>
    where TDbContext : DbContext
{
    private readonly IServiceCollection services;
    private readonly ServiceLifetime lifetime;
    private readonly Action<Type>? callback;

    /// <summary>
    /// Creates a new instance of <see cref="SearchConfigurer{TDbContext}"/>.
    /// </summary>
    /// <param name="services">The service collection to register the searches as a service.</param>
    /// <param name="lifetime">The lifetime of the service.</param>
    /// <param name="callback">The callback to be called after the search is registered.</param>
    public SearchConfigurer(
        IServiceCollection services,
        ServiceLifetime lifetime,
        Action<Type>? callback = null)
    {
        this.services = services;
        this.lifetime = lifetime;
        this.callback = callback;

        services.AddSearchesLinq();
        services.TryAddTransient<ISearchPipelineFactory, SearchPipelineFactory<TDbContext>>();
    }
    
    /// <inheritdoc />
    public ISearchConfigurer<TDbContext> Add<TEntity>() where TEntity : class
    {
        var searchType = typeof(ISearch<>).MakeGenericType(typeof(TEntity));
        var entitySearchImplType = typeof(Search<,>).MakeGenericType(typeof(TDbContext), typeof(TEntity));
        
        services.Add(ServiceDescriptor.Describe(
            searchType,
            entitySearchImplType,
            lifetime));

        var queryableProviderType = typeof(IQueryableProvider<>).MakeGenericType(typeof(TEntity));
        var queryableProviderImplType = typeof(QueryableProvider<,>).MakeGenericType(typeof(TDbContext), typeof(TEntity));
        
        services.Add(ServiceDescriptor.Describe(
            queryableProviderType,
            queryableProviderImplType,
            lifetime));

        callback?.Invoke(typeof(TEntity));

        return this;
    }
}