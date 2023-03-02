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

    /// <summary>
    /// Creates a new instance of <see cref="SearchConfigurer{TDbContext}"/>.
    /// </summary>
    /// <param name="services">The service collection to register the searches as a service.</param>
    /// <param name="lifetime">The lifetime of the service.</param>
    public SearchConfigurer(
        IServiceCollection services,
        ServiceLifetime lifetime)
    {
        this.services = services;
        this.lifetime = lifetime;

        services.AddSearchesLinq();
        services.TryAddTransient<IPipelineFactory, PipelineFactory<TDbContext>>();
    }
    
    /// <inheritdoc />
    public ISearchConfigurer<TDbContext> Add<TEntity>() where TEntity : class
    {
        // adiciona search como serviço
        var searchType = typeof(ISearch<>).MakeGenericType(typeof(TEntity));
        var dbSearchType = typeof(ISearch<,>).MakeGenericType(typeof(TDbContext), typeof(TEntity));
        var searchImplType = typeof(InternalSearch<,>).MakeGenericType(typeof(TDbContext), typeof(TEntity));
        
        services.Add(ServiceDescriptor.Describe(
            dbSearchType,
            searchImplType,
            lifetime));

        services.Add(ServiceDescriptor.Describe(
            searchType,
            sp => sp.GetService(dbSearchType)!,
            lifetime));

        // adiciona all entities como serviço
        // TODO:


        // TODO: check if is really necessary the registration of the IQueryableProvider
        var queryableProviderType = typeof(IQueryableProvider<>).MakeGenericType(typeof(TEntity));
        var queryableProviderImplType = typeof(QueryableProvider<,>).MakeGenericType(typeof(TDbContext), typeof(TEntity));
        
        services.Add(ServiceDescriptor.Describe(
            queryableProviderType,
            queryableProviderImplType,
            lifetime));

        return this;
    }
}