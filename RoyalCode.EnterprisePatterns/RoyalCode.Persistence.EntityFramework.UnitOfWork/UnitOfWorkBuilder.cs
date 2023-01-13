using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.Persistence.EntityFramework.Repositories.Configurations;
using RoyalCode.Persistence.EntityFramework.Searches.Configurations;

namespace RoyalCode.Persistence.EntityFramework.UnitOfWork;

/// <summary>
/// <para>
///     Internal default implementation of <see cref="IUnitOfWorkBuilder{TDbContext}"/>.
/// </para>
/// </summary>
/// <typeparam name="TDbContext">The type of the <see cref="DbContext"/>.</typeparam>
internal sealed class UnitOfWorkBuilder<TDbContext> : IUnitOfWorkBuilder<TDbContext>
    where TDbContext : DbContext
{
    private readonly IServiceCollection services;
    private readonly ServiceLifetime lifetime;
    private readonly Action<Type>? repositoryAddedCallback;
    private readonly Action<Type>? searchAddedCallback;

    /// <summary>
    /// Creates a new builder.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The lifetime that will be used when register services.</param>
    /// <param name="repositoryAddedCallback">The callback that will be called when a repository is added.</param>
    /// <param name="searchAddedCallback">The callback that will be called when a search is added.</param>
    public UnitOfWorkBuilder(
        IServiceCollection services, 
        ServiceLifetime lifetime, 
        Action<Type>? repositoryAddedCallback = null,
        Action<Type>? searchAddedCallback = null)
    {
        this.services = services;
        this.lifetime = lifetime;
        this.repositoryAddedCallback = repositoryAddedCallback;
        this.searchAddedCallback = searchAddedCallback;
    }

    /// <inheritdoc />
    public IServiceCollection Services => services;

    /// <inheritdoc />
    public ServiceLifetime Lifetime => lifetime;

    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureDbContextPool(Action<DbContextOptionsBuilder> configurer)
    {
        if (configurer is null)
            throw new ArgumentNullException(nameof(configurer));

        services.AddDbContextPool<TDbContext>(builder =>
        {
            builder.UseUnitOfWork();
            configurer(builder);
        });
        return this;
    }
    
    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureDbContextPool(Action<IServiceProvider, DbContextOptionsBuilder> configurer)
    {
        if (configurer is null)
            throw new ArgumentNullException(nameof(configurer));

        services.AddDbContextPool<TDbContext>((sp, builder) =>
        {
            builder.UseUnitOfWork();
            configurer(sp, builder);
        });
        return this;
    }
    
    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureDbContext(Action<DbContextOptionsBuilder> configurer)
    {
        if (configurer is null)
            throw new ArgumentNullException(nameof(configurer));

        services.AddDbContext<TDbContext>(builder =>
        {
            builder.UseUnitOfWork();
            configurer(builder);
        }, lifetime);
        return this;
    }
    
    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureDbContext(Action<IServiceProvider, DbContextOptionsBuilder> configurer)
    {
        if (configurer is null)
            throw new ArgumentNullException(nameof(configurer));

        services.AddDbContext<TDbContext>((sp, builder) =>
        {
            builder.UseUnitOfWork();
            configurer(sp, builder);
        }, lifetime);
        return this;
    }

    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureRepositories(Action<IRepositoryConfigurer<TDbContext>> configureAction)
    {
        if (configureAction is null)
            throw new ArgumentNullException(nameof(configureAction));

        var repositoryConfigurer = new RepositoryConfigurer<TDbContext>(services, lifetime, repositoryAddedCallback);
        configureAction(repositoryConfigurer);
        return this;
    }

    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureSearches(Action<ISearchConfigurer<TDbContext>> configureAction)
    {
        if (configureAction is null)
            throw new ArgumentNullException(nameof(configureAction));

        var searchConfigurer = new SearchConfigurer<TDbContext>(services, lifetime, searchAddedCallback);
        configureAction(searchConfigurer);
        return this;
    }
}