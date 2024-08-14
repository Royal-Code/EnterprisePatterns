using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoyalCode.Repositories.EntityFramework.Configurations;
using RoyalCode.Searches.Persistence.EntityFramework.Configurations;

namespace RoyalCode.UnitOfWork.EntityFramework;

/// <summary>
/// <para>
///     Internal default implementation of <see cref="IUnitOfWorkBuilder{TDbContext}"/>.
/// </para>
/// </summary>
/// <typeparam name="TDbContext">The type of the <see cref="DbContext"/>.</typeparam>
internal sealed class UnitOfWorkBuilder<TDbContext> : IUnitOfWorkBuilder<TDbContext>
    where TDbContext : DbContext
{
    /// <summary>
    /// Creates a new builder.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The lifetime that will be used when register services.</param>
    public UnitOfWorkBuilder(
        IServiceCollection services, 
        ServiceLifetime lifetime)
    {
        Services = services;
        Lifetime = lifetime;
    }

    /// <inheritdoc />
    public IServiceCollection Services { get; }

    /// <inheritdoc />
    public ServiceLifetime Lifetime { get; }

    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureDbContextPool(Action<DbContextOptionsBuilder> configurer)
    {
        ArgumentNullException.ThrowIfNull(configurer);
        Services.AddDbContextPool<TDbContext>(configurer);
        return this;
    }
    
    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureDbContextPool(Action<IServiceProvider, DbContextOptionsBuilder> configurer)
    {
        ArgumentNullException.ThrowIfNull(configurer);
        Services.AddDbContextPool<TDbContext>(configurer);
        return this;
    }
    
    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureDbContext(Action<DbContextOptionsBuilder> configurer)
    {
        ArgumentNullException.ThrowIfNull(configurer);
        Services.AddDbContext<TDbContext>(configurer, Lifetime);
        return this;
    }
    
    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureDbContext(Action<IServiceProvider, DbContextOptionsBuilder> configurer)
    {
        ArgumentNullException.ThrowIfNull(configurer);
        Services.AddDbContext<TDbContext>(configurer, Lifetime);
        return this;
    }

    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureRepositories(Action<IRepositoriesBuilder<TDbContext>> configureAction)
    {
        ArgumentNullException.ThrowIfNull(configureAction);
        var repositoryConfigurer = new RepositoriesBuilder<TDbContext>(Services, Lifetime);
        configureAction(repositoryConfigurer);
        return this;
    }

    /// <inheritdoc />
    public IUnitOfWorkBuilder<TDbContext> ConfigureSearches(Action<ISearchConfigurations<TDbContext>> configureAction)
    {
        ArgumentNullException.ThrowIfNull(configureAction);

        var configurations = new SearchConfigurations<TDbContext>(Services);
        configureAction(configurations);
        return this;
    }
}